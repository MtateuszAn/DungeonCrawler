
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class GridElement
{
    public bool isTaken;
    public bool isDoor;
    public Room room;
    public bool isCoridor;
    //grid center world coordinates
    public int x;
    public int z;

    public GridElement(int x, int z)
    {
        this.x = x; 
        this.z = z;
        isTaken = false;
    }
    public GridElement(double x, double z)
    {
        this.x = (int)x;
        this.z = (int)z;
        isTaken = false;
    }

    public Vector3 GetGridWP()
    {
        return new Vector3(x,0,z);
    }

    public Vector2Int GetGridV2I()
    {
        return new Vector2Int(x, z);
    }

}
public class MapGrid : MonoBehaviour
{
    //[SerializeField] public static MapGrid instance;
    [SerializeField] GameObject playerPref;
    [SerializeField] GameObject door;
    [SerializeField] GameObject coridor;
    [SerializeField] GameObject room;
    [SerializeField] bool seeAll;
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> enemySpawnPoints = new List<Transform>();
    //Grid variables
    int ges = 2;//GridEmementSize "ges"
    [SerializeField] public int gridHeight = 20;
    [SerializeField] public int gridWidth = 20;

    public GridElement[,] grid;

    public List<Room> rooms = new List<Room>();
    public List<GridElement> dorrs = new List<GridElement>();

    NavMeshSurface navMesh;

    //Coridor PF
    IDictionary<GridElement, GridElement> nodeParents = new Dictionary<GridElement, GridElement>();


    public void CreateNavMesh()
    {
        navMesh = GetComponent<NavMeshSurface>();
        navMesh.BuildNavMesh();
        
    }

    public void initializeSpawnPoints()
    {
        foreach (Room room in rooms)
        {
            if (room.spawnPoint != null)
            {
                spawnPoints.Add(room.spawnPoint);
            }
            if (room.enemySpawnPoits != null)
            {
                foreach(var spawn in room.enemySpawnPoits)
                    enemySpawnPoints.Add(spawn);
            }
        }
    }
    public void SpawnPlayer()
    {
        
        if (spawnPoints.Count > 0)
        {
            playerPref.transform.position = spawnPoints[0].transform.position;
            //Instantiate(playerPref, SpawnPoints[0].transform.position, gameObject.transform.rotation);
        }
        else
        {
            playerPref.transform.position = new Vector3(4, 0, 6);
            //Instantiate(playerPref, rooms[0].transform.position + new Vector3(4, 0, 6), gameObject.transform.rotation);
        }

    }
    public void InitializeMap()
    {
        grid = new GridElement[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                grid[x, z] = new GridElement(x * ges, z * ges);
            }
        }
    }
    public void FillRest(GameObject filler)
    {
        foreach (var item in grid)
        {
            if (!item.isTaken == true)
            {
                Instantiate(filler, item.GetGridWP(), gameObject.transform.rotation);
            }
            if (item.isDoor == true)
            {
                //Instantiate(Door, item.GetGridWP(), gameObject.transform.rotation);
            }
            if (item.isCoridor == true)
            {
                // Instantiate(Coridor, item.GetGridWP(), gameObject.transform.rotation);
            }
            if (item.isTaken == true && !item.isCoridor == true && !item.isDoor == true)
            {
                // Instantiate(Room, item.GetGridWP(), gameObject.transform.rotation);
            }
        }
    }

    public bool GenerateRoom(int x, int z, bool rotate, Room room)
    {
        if (!rotate)
        {
            if (ValidateRoom(x, z, room.roomWidth, room.roomHeight))
            {
                rooms.Add(Instantiate(room.gameObject, grid[x, z].GetGridWP(), gameObject.transform.rotation).GetComponent<Room>());
                for (int i = x; i < x + room.roomWidth; i++)
                {
                    for (int j = z; j < z + room.roomHeight; j++)
                    {
                        grid[i, j].isTaken = true;
                    }

                }
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            if (ValidateRoom(x - room.roomHeight + 1, z, room.roomHeight, room.roomWidth))
            {
                GameObject instantiatedRoom = Instantiate(room.gameObject, grid[x, z].GetGridWP(), Quaternion.identity);
                instantiatedRoom.transform.Rotate(0f, -90f, 0f);
                rooms.Add(instantiatedRoom.GetComponent<Room>());

                for (int i = x; i > x - room.roomHeight; i--)
                {
                    for (int j = z; j < z + room.roomWidth; j++)
                    {
                        grid[i, j].isTaken = true;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }

        }
    }
    private bool ValidateRoom(int x1, int z1, int x2, int z2)
    {
        for (int i = x1 - 1; i < x1 + x2 + 1; i++)
        {
            for (int j = z1 - 1; j < z1 + z2 + 1; j++)
            {
                if (grid[i, j].isTaken)
                {
                    return false;
                }

            }
        }
        return true;
    }

    public void UpdateGridFlorsDors()
    {
        foreach (GridElement element in grid)
            element.isTaken = false;

        foreach (Room room in rooms)
        {
            room.UpdateDoors();
            room.UpdateFloors();
            foreach (Door dor in room.doors)
            {
                dorrs.Add(grid[dor.x, dor.z]);
                grid[dor.x, dor.z].isDoor = true;
                grid[dor.x, dor.z].isTaken = true;
                grid[dor.x, dor.z].room = room;
            }
            foreach (Door flor in room.floors)
            {
                grid[flor.x, flor.z].isDoor = false;
                grid[flor.x, flor.z].isTaken = true;
                grid[flor.x, flor.z].room = room;
            }
        }
    }
    private GridElement GetGrid(Vector3 node)
    {
        if (node.x/2 >= 0 && node.x / 2 < grid.GetLength(0) && node.y / 2 >= 0 && node.y/2 < grid.GetLength(1))
        {
            return grid[(int)(node.x/2), (int)(node.y/2)];
        }
        else
        {
            Debug.LogError("Próba dostêpu do elementu poza granicami tablicy. x:" + node.x + " z: " + node.y);
            return null;
        }
    }

    List<Edge> edgesWithoutSuper = new List<Edge>();
    List<Edge> minimumSpanningTree = new List<Edge>();

    private void Update()
    {
        if (!seeAll)
            foreach (var edge in minimumSpanningTree)
            {
                Debug.DrawLine(new Vector3(edge.Start.x, 1, edge.Start.z), new Vector3(edge.End.x, 1, edge.End.z));
            }
        if (seeAll)
            foreach (var edge in edgesWithoutSuper)
            {
                Debug.DrawLine(edge.Start.GetGridWP(), edge.End.GetGridWP());
            }
    }
    public void BuildCoridors(GameObject coridor)
    {
        edgesWithoutSuper = FindPath.BowyerWatsonDelaunayTriangulation(dorrs);
        minimumSpanningTree = FindPath.MSTreePrim(edgesWithoutSuper);

        Queue<Edge> edgeQueue = new Queue<Edge>();
        //usuwanie sciezek z dzwi do dzwi tego samego pokoju
        foreach (var edge in minimumSpanningTree)
        {
            if (edge.Start.room == edge.End.room)
            {
                edgeQueue.Enqueue(edge);
            }
        }
        foreach(var edge in edgeQueue)
        {
            minimumSpanningTree.Remove(edge);
        }
        //tworzenie korytarzy dla tych samych pokoji
        foreach (var edge in minimumSpanningTree)
        {
            
            if (!MakeCoridors(edge.Start, edge.End))
            {
                Debug.Log("porazka");
            }
        }
        foreach (var item in grid)
        {
            if (item.isCoridor == true)
            {
                Instantiate(coridor, item.GetGridWP(), gameObject.transform.rotation).GetComponent<Coridor>().map=this;
            }
        }
    }
    
    bool MakeCoridors(GridElement gridStart, GridElement gridEnd)
    {
        GridElement curr = FindShortestPathBFS(gridStart, gridEnd);
        List<GridElement> path = new List<GridElement>();
        if (curr == gridStart)
        {
            return false;
        }
        while (curr != gridStart)
        {
            path.Add(curr);
            curr = nodeParents[curr];
        }
        nodeParents.Clear();
        foreach (GridElement node in path)
        {
            if (!grid[node.x/2, node.z/2].isTaken)
            {
                grid[node.x / 2, node.z/2].isTaken = true;
                grid[node.x / 2, node.z/2].isCoridor = true;
            }

        }
        
        return true;
    }

    public GridElement FindShortestPathBFS(GridElement start, GridElement end)
    {
        nodeParents.Clear();
        if (start.isDoor)
        {
            Queue<GridElement> queue = new Queue<GridElement>();
            HashSet<GridElement> visited = new HashSet<GridElement>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                GridElement curent = queue.Dequeue();

                if (curent == end)
                {
                    //Debug.Log("jest droga");
                    return curent;
                }

                IList<GridElement> nodes = GetWalkableNodes(curent, start, end);

                foreach (GridElement node in nodes)
                {
                    if (!visited.Contains(node))
                    {
                        nodeParents.TryAdd(node, curent);
                            visited.Add(node);
                            queue.Enqueue(node);
                    }

                }

            }
            Debug.Log("nie ma drogi");
            return start;

        }
        Debug.Log("nie jest dzwiami");
        return start;
    }
    IList<GridElement> GetWalkableNodes(GridElement curr, GridElement start, GridElement end)
    {

        IList<GridElement> walkableNodes = new List<GridElement>();

        IList<GridElement> possibleNodes = new List<GridElement>();

        //tu cos popraw
        if ((curr.z - 2) /2 > 1)
        {
            //dol
            possibleNodes.Add(grid[curr.x / 2, (curr.z - 2) / 2]);
        }
        if ((curr.z+2) / 2 < gridHeight - 1)
        {
            //gora
            possibleNodes.Add(grid[curr.x / 2, (curr.z + 2) / 2]);
        }
        if ((curr.x-2) /2 > 1)
        {
            //lewo
            possibleNodes.Add(grid[(curr.x - 2) / 2, curr.z / 2]);
        }
        if ((curr.x+2) /2 < gridWidth - 1)
        {
            //prawo
            possibleNodes.Add(grid[(curr.x + 2) / 2, curr.z / 2]);
        }
        foreach (GridElement node in possibleNodes)
        {
            if (((!node.isTaken || node.isCoridor || node == end)))
            {
                walkableNodes.Add(node);
            }

        }

        return walkableNodes;
    }
}
