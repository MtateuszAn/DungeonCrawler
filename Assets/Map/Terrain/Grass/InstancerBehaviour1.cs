using System.Collections.Generic;
using UnityEngine;



public class InstancerBehaviourB : MonoBehaviour
{
    [SerializeField] private int instances=0;
    [SerializeField] private int instancesPerTenSquareMeters=1;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    private List<List<Matrix4x4>> batches = new List<List<Matrix4x4>>();

    
    private List<Vector3> positions = new List<Vector3>();


    private Vector2 terrainMax;
    private Vector2 terrainMin;

    private void Start()
    {
        CalculateTerrainSize();
        CalculateGrassPositionsOnMesh();
        CreateBathes();
    }

    // Funkcja do pobierania szerokoœci i d³ugoœci modelu
    private void CalculateTerrainSize()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Vector3 size = renderer.bounds.size;
            terrainMax.x = size.x/2;
            terrainMin.x = - size.x/2;
            terrainMax.y = size.z/2;
            terrainMin.y = - size.z/2;

            Debug.Log($"Szerokoœæ terenu: {size.x}, D³ugoœæ terenu: {size.z}");
        }
        else
        {
            Debug.LogWarning("Brak komponentu MeshRenderer na obiekcie!");
        }
    }
    private void CalculateGrassPositionsOnMesh()
    {
        instances = 0;
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices; // Pobieramy wierzcho³ki
        int[] triangles = mesh.triangles; // Pobieramy trójk¹ty
        Color[] colors = mesh.colors; // Pobieramy kolory wierzcho³ków (jeœli istniej¹)

        for (int i = (int)terrainMin.x; i < (int)terrainMax.x; i += 10)
        {
            for (int j = (int)terrainMin.y; j < (int)terrainMax.y; j += 10)
            {
                for (int inst = 0; inst < (int)instancesPerTenSquareMeters; inst += 1)
                {
                    Debug.Log("TRAWA");
                    Vector3 positionXZ = new Vector3(
                        gameObject.transform.position.x + i + (Random.Range(0, 1000) / 100f),
                        0,
                        gameObject.transform.position.z + j + (Random.Range(0, 1000) / 100f)
                    );
                    instances++;
                }
            }
        }
    }
    #region TryGrid
    private struct GridCell
    {
        public List<int> triangleIndices; // Indeksy trójk¹tów znajduj¹cych siê w komórce
    }

    private GridCell[,] grid;
    private int gridSize = 10; // Rozmiar komórki siatki, np. 10 jednostek
    private int gridWidth, gridHeight;

    private void CreateGrid(Vector3 terrainMin, Vector3 terrainMax, Vector3[] vertices, int[] triangles)
    {
        gridWidth = Mathf.CeilToInt((terrainMax.x - terrainMin.x) / gridSize);
        gridHeight = Mathf.CeilToInt((terrainMax.z - terrainMin.z) / gridSize);

        grid = new GridCell[gridWidth, gridHeight];

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            // ZnajdŸ wspó³rzêdne komórek, do których nale¿y trójk¹t
            int minX = Mathf.FloorToInt(Mathf.Min(v0.x, v1.x, v2.x) / gridSize);
            int maxX = Mathf.FloorToInt(Mathf.Max(v0.x, v1.x, v2.x) / gridSize);
            int minZ = Mathf.FloorToInt(Mathf.Min(v0.z, v1.z, v2.z) / gridSize);
            int maxZ = Mathf.FloorToInt(Mathf.Max(v0.z, v1.z, v2.z) / gridSize);

            for (int x = minX; x <= maxX; x++)
            {
                for (int z = minZ; z <= maxZ; z++)
                {
                    if (x >= 0 && x < gridWidth && z >= 0 && z < gridHeight)
                    {
                        if (grid[x, z].triangleIndices == null)
                        {
                            grid[x, z].triangleIndices = new List<int>();
                        }

                        // Dodaj indeksy trójk¹ta do komórki
                        grid[x, z].triangleIndices.Add(i);
                    }
                }
            }
        }
    }

    // Funkcja do znajdowania interpolowanej wysokoœci na podstawie siatki z u¿yciem gridu
    private Vector3? FindHeightFromMeshWithGrid(Vector3 positionXZ, Vector3[] vertices, int[] triangles)
    {
        int gridX = Mathf.FloorToInt(positionXZ.x / gridSize);
        int gridZ = Mathf.FloorToInt(positionXZ.z / gridSize);

        if (gridX >= 0 && gridX < gridWidth && gridZ >= 0 && gridZ < gridHeight)
        {
            GridCell cell = grid[gridX, gridZ];

            if (cell.triangleIndices != null)
            {
                foreach (int i in cell.triangleIndices)
                {
                    Vector3 v0 = vertices[triangles[i]];
                    Vector3 v1 = vertices[triangles[i + 1]];
                    Vector3 v2 = vertices[triangles[i + 2]];

                    if (IsPointInTriangleXZ(positionXZ, v0, v1, v2))
                    {
                        float y = BarycentricInterpolation(positionXZ, v0, v1, v2);
                        return new Vector3(positionXZ.x, y, positionXZ.z);
                    }
                }
            }
        }

        return null; // Jeœli nie znaleziono trójk¹ta dla punktu
    }

    // Funkcja rozmieszczania trawy, teraz u¿ywaj¹ca gridu
    private void CalculateGrassPositionsWithGrid()
    {
        instances = 0;
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices; // Pobieramy wierzcho³ki
        int[] triangles = mesh.triangles; // Pobieramy trójk¹ty
        Color[] colors = mesh.colors; // Pobieramy kolory wierzcho³ków (jeœli istniej¹)

        CreateGrid(terrainMin, terrainMax, vertices, triangles); // Tworzymy grid

        for (int i = (int)terrainMin.x; i < (int)terrainMax.x; i += 10)
        {
            for (int j = (int)terrainMin.y; j < (int)terrainMax.y; j += 10)
            {
                for (int inst = 0; inst < (int)instancesPerTenSquareMeters; inst += 1)
                {
                    Vector3 positionXZ = new Vector3(
                        gameObject.transform.position.x + i + (Random.Range(0, 1000) / 100f),
                        0,
                        gameObject.transform.position.z + j + (Random.Range(0, 1000) / 100f)
                    );

                    Vector3? interpolatedPosition = FindHeightFromMeshWithGrid(positionXZ, vertices, triangles);

                    if (interpolatedPosition.HasValue)
                    {
                        bool canPlaceGrass = CheckVertexColors(triangles, colors, interpolatedPosition.Value);

                        if (canPlaceGrass)
                        {
                            positions.Add(interpolatedPosition.Value);
                            instances++;
                        }
                    }
                }
            }
        }
    }
    #endregion
    #region ODLMETOD
    private void CalculateGrassPositions()
    {
        instances = 0;
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices; // Pobieramy wierzcho³ki
        int[] triangles = mesh.triangles; // Pobieramy trójk¹ty
        Color[] colors = mesh.colors; // Pobieramy kolory wierzcho³ków (jeœli istniej¹)

        for (int i = (int)terrainMin.x; i < (int)terrainMax.x; i += 10)
        {
            for (int j = (int)terrainMin.y; j < (int)terrainMax.y; j += 10)
            {
                for (int inst = 0; inst < (int)instancesPerTenSquareMeters; inst += 1)
                {
                    Vector3 positionXZ = new Vector3(
                        gameObject.transform.position.x + i + (Random.Range(0, 1000) / 100f),
                        0,
                        gameObject.transform.position.z + j + (Random.Range(0, 1000) / 100f)
                    );

                    Vector3? interpolatedPosition = FindHeightFromMesh(positionXZ, vertices, triangles);

                    if (interpolatedPosition.HasValue)
                    {
                        // Sprawdzamy kolor wierzcho³ków trójk¹ta
                        bool canPlaceGrass = CheckVertexColors(triangles, colors, interpolatedPosition.Value);

                        if (canPlaceGrass)
                        {
                            positions.Add(interpolatedPosition.Value);
                            instances++;
                        }
                    }
                }
            }
        }
    }

    // Funkcja do znajdowania interpolowanej wysokoœci na podstawie siatki
    private Vector3? FindHeightFromMesh(Vector3 positionXZ, Vector3[] vertices, int[] triangles)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            // Sprawdzamy, czy punkt XZ znajduje siê w trójk¹cie
            if (IsPointInTriangleXZ(positionXZ, v0, v1, v2))
            {
                // Interpolujemy wysokoœæ w obrêbie trójk¹ta
                float y = BarycentricInterpolation(positionXZ, v0, v1, v2);
                return new Vector3(positionXZ.x, y, positionXZ.z);
            }
        }

        return null; // Jeœli nie znaleziono trójk¹ta dla punktu
    }

    // Funkcja do interpolacji wysokoœci barycentrycznie
    private float BarycentricInterpolation(Vector3 positionXZ, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // Algorytm barycentryczny do interpolacji wysokoœci
        Vector3 p = new Vector3(positionXZ.x, 0, positionXZ.z);
        Vector3 a = new Vector3(v0.x, 0, v0.z);
        Vector3 b = new Vector3(v1.x, 0, v1.z);
        Vector3 c = new Vector3(v2.x, 0, v2.z);

        Vector3 v0v1 = b - a;
        Vector3 v0v2 = c - a;
        Vector3 v0p = p - a;

        float d00 = Vector3.Dot(v0v1, v0v1);
        float d01 = Vector3.Dot(v0v1, v0v2);
        float d11 = Vector3.Dot(v0v2, v0v2);
        float d20 = Vector3.Dot(v0p, v0v1);
        float d21 = Vector3.Dot(v0p, v0v2);

        float denom = d00 * d11 - d01 * d01;
        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;

        return u * v0.y + v * v1.y + w * v2.y;
    }

    // Funkcja do sprawdzania, czy punkt XZ znajduje siê w trójk¹cie XZ
    private bool IsPointInTriangleXZ(Vector3 p, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        float dX = p.x - v2.x;
        float dZ = p.z - v2.z;
        float dX21 = v2.x - v1.x;
        float dZ12 = v1.z - v2.z;
        float D = dZ12 * (v0.x - v2.x) + dX21 * (v0.z - v2.z);
        float s = dZ12 * dX + dX21 * dZ;
        float t = (v0.z - v2.z) * dX + (v2.x - v0.x) * dZ;

        if (D < 0) return s <= 0 && t <= 0 && s + t >= D;
        return s >= 0 && t >= 0 && s + t <= D;
    }

    // Funkcja do sprawdzania kolorów wierzcho³ków trójk¹ta
    private bool CheckVertexColors(int[] triangles, Color[] colors, Vector3 position)
    {
        // Sprawdzamy kolory wierzcho³ków trójk¹ta
        if (colors.Length == 0) return true; // Jeœli brak danych o kolorach, akceptujemy

        Color c0 = colors[triangles[0]];
        Color c1 = colors[triangles[1]];
        Color c2 = colors[triangles[2]];

        // Mo¿esz dodaæ interpolacjê kolorów, ale dla uproszczenia sprawdzamy tylko, czy któryœ z wierzcho³ków nie ma czerwonego < 100
        return (c0.r * 255 < 100 || c1.r * 255 < 100 || c2.r * 255 < 100);
    }
    #endregion

    private void CreateBathes()
    {
        int addedMatrices = 0;

        batches.Add(new List<Matrix4x4>());

        for (int i = 0; i < instances; i++)
        {
            if (addedMatrices < 1000)
            {
                batches[batches.Count - 1].Add(
                    Matrix4x4.TRS(
                    positions[i],
                    Quaternion.identity,
                    Vector3.one));
                addedMatrices++;
            }
            else
            {
                batches.Add(new List<Matrix4x4>());
                addedMatrices = 0;
            }

        }
    }

    private void RenderBatches()
    {
        foreach (var batch in batches)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(mesh, i, material, batch);
            }
        }
    }

    private void Update()
    {
        RenderBatches();
    }
}
