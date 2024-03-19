
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        return new Vector2Int(x/2, z/2);
    }

}
public class MapGrid : MonoBehaviour
{
    public static MapGrid instance;
    [SerializeField] GameObject playerPref;
    [SerializeField] bool seeAll;
    public List<Transform> SpawnPoints = new List<Transform>();
    //Grid variables
    int ges = 2;//GridEmementSize "ges"
    [SerializeField] public int gridHeight = 20;
    [SerializeField] public int gridWidth = 20;

    public GridElement[,] grid ;

    List<Room> rooms = new List<Room>();
    public List<GridElement> dorrs = new List<GridElement>();

    //Coridor PF
    IDictionary<Vector2Int, Vector2Int> nodeParents = new Dictionary<Vector2Int, Vector2Int>();

    public UnityEvent e_EndEvent = new UnityEvent();
    //dsadsadasdasfasdasd

    private void Start()
    {
        instance = this;
    }
    public void SpawnPlayer()
    {
        foreach (Room room in rooms)
        {
            if (room.spawnPoint != null)
            {
                SpawnPoints.Add(room.spawnPoint);
            }
        }
        if(SpawnPoints.Count > 0)
        {
            playerPref.transform.position = SpawnPoints[0].transform.position;
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
        }
    }
    

    public bool GenerateRoom(int x, int z,bool rotate, Room room)
    {
        if(!rotate)
        {
            if(ValidateRoom(x,z, room.roomWidth, room.roomHeight))
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
            if(ValidateRoom(x - room.roomHeight+1, z, room.roomHeight, room.roomWidth))
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
        for (int i = x1-1; i < x1 + x2+1; i++)
        {
            for (int j = z1-1; j < z1 + z2+1; j++)
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
            element.isTaken= false;

        foreach (Room room in rooms)
        {
            room.UpdateDoors();
            room.UpdateFloors();
            foreach (Door dor in room.doors)
            {
                dorrs.Add(grid[dor.x, dor.z]);
                grid[dor.x,dor.z].isDoor = true;
                grid[dor.x,dor.z].isTaken = true;
                grid[dor.x,dor.z].room = room;
            }
            foreach (Door flor in room.floors)
            {
                grid[flor.x, flor.z].isDoor = true;
                grid[flor.x, flor.z].isTaken = true;
                grid[flor.x, flor.z].room = room;
            }
        }
    }
    private GridElement GetGrid(Vector2Int node)
    {
        return grid[node.x, node.y];
    }
    List<Edge> edgesWithoutSuper = new List<Edge>();
    List<Edge> minimumSpanningTree = new List<Edge>();
    public void BuildCoridors2(GameObject coridor)
    {
        edgesWithoutSuper = FindPath.BowyerWatsonDelaunayTriangulation();
        //Debug.Log("liczba scian w trojkatach" + edgesWithoutSuper.Count *3);
        minimumSpanningTree = FindPath.MSTreePrim(edgesWithoutSuper);
        Queue<Edge> edgeQueue = new Queue<Edge>();
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
        //Debug.Log("liczba scian" + minimumSpanningTree.Count);

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
                Instantiate(coridor, item.GetGridWP(), gameObject.transform.rotation);
            }
        }
    }
    private void Update()
    {
        /*if (seeAll) 
                foreach (Edge edge in edgesWithoutSuper)
                {
                    Debug.DrawLine(new Vector3(edge.Start.x, 2, edge.Start.z), new Vector3(edge.End.x, 2, edge.End.z));
                    Debug.Log("robie linie");
                }
        else
            foreach (Edge edge in minimumSpanningTree)
            {
                Debug.DrawLine(new Vector3(edge.Start.x, 2, edge.Start.z), new Vector3(edge.End.x, 2, edge.End.z));
                Debug.Log("robie linie");
            }*/
    }
    public void BuildCoridors(GameObject coridor)
    {
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
                Instantiate(coridor, item.GetGridWP(), gameObject.transform.rotation);
            }
        }
    }
    bool MakeCoridors(GridElement gridStart, GridElement gridEnd)
    {
        Vector2Int curr = FindShortestPathBFS(gridStart, gridEnd);
        List<Vector2Int> path = new List<Vector2Int>();
        if (curr == gridStart.GetGridV2I())
        {
            return false;
        }
        while (curr != gridStart.GetGridV2I())
        {
            path.Add(curr);
            curr = nodeParents[curr];
        }

        foreach (Vector2Int node in path)
        {
            if (!grid[node.x, node.y].isTaken)
            {
                grid[node.x, node.y].isTaken = true;
                grid[node.x, node.y].isCoridor = true;
            }

        }
        nodeParents.Clear();
        return true;
    }

    public Vector2Int FindShortestPathBFS(GridElement start, GridElement end)
    {
        nodeParents.Clear();
        if (start.isDoor)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            queue.Enqueue(start.GetGridV2I());

            while (queue.Count > 0)
            {
                Vector2Int curent = queue.Dequeue();

                if (GetGrid(curent) == end)
                {
                    //Debug.Log("jest droga");
                    return curent;
                }

                IList<Vector2Int> nodes = GetWalkableNodes(curent, start, end);

                foreach (Vector2Int node in nodes)
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
            return start.GetGridV2I();

        }
        Debug.Log("nie jest dzwiami");
        return start.GetGridV2I();
    }
    IList<Vector2Int> GetWalkableNodes(Vector2Int curr, GridElement start, GridElement end)
    {

        IList<Vector2Int> walkableNodes = new List<Vector2Int>();

        IList<Vector2Int> possibleNodes = new List<Vector2Int>() {
            new Vector2Int (curr.x , curr.y+1),
            new Vector2Int (curr.x+1 , curr.y),
            new Vector2Int (curr.x, curr.y-1),
            new Vector2Int (curr.x-1, curr.y),
        };

        foreach (Vector2Int node in possibleNodes)
        {
            if ((node.x > 1 && node.x < gridWidth - 1) && (node.y > 1 && node.y < gridHeight - 1) && ((!GetGrid(node).isTaken || GetGrid(node).isCoridor || GetGrid(node) == end)))
            {
                walkableNodes.Add(node);
            }

        }

        return walkableNodes;
    }
    /*
    bool MakeCoridors2(GridElement gridDor)
    {
        Vector2Int curr = FindShortestPathBFS(gridDor);
        List<Vector2Int> path = new List<Vector2Int>();
        if (curr == gridDor.GetGridV2I())
        {
            Debug.Log("znaleziono to samo miejsce");
            return false;
        }
        while (curr != gridDor.GetGridV2I())
        {
            path.Add(curr);
            curr = nodeParents[curr];
        }

        foreach (Vector2Int node in path)
        {
            if (!grid[node.x, node.y].isTaken)
            {
                grid[node.x, node.y].isTaken = true;
                grid[node.x, node.y].isCoridor = true;
            }

        }
        nodeParents.Clear();
        return true;
    }
    //Returns the goalPosition if a solution is found.
    //Returns the startPosition if no solution is found.
    public Vector2Int FindShortestPathBFS2(GridElement start)
    {

        if (start.isDoor)
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            queue.Enqueue(start.GetGridV2I());

            while (queue.Count > 0) 
            {
                Vector2Int curent = queue.Dequeue();

                if ((GetGrid(curent).isDoor && curent!=start.GetGridV2I()) || GetGrid(curent).isCoridor)
                {
                    Debug.Log("jest droga");
                    return curent;
                }

                IList<Vector2Int> nodes = GetWalkableNodes(curent, start);

                foreach(Vector2Int node in nodes)
                {
                    if (!visited.Contains(node))
                    {
                        visited.Add(node);

                        nodeParents.Add(node, curent);

                        queue.Enqueue(node);
                    }
                }
                
            }
            Debug.Log("nie ma drogi");
            return start.GetGridV2I();

        }
        Debug.Log("nie jest dzwiami");
        return start.GetGridV2I();
    }
    */

}
