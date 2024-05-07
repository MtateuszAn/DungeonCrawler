using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomsManager : MonoBehaviour
{
    [SerializeField] List<Room> rooms;
    [SerializeField] List<Room> spawnRooms;
    [SerializeField] GameObject filler;
    [SerializeField] GameObject coriddor;
    MapGrid mapGrid;
    EnemieSpawnScript enemieSpawnScript;
    [SerializeField] private int numberOfRooms;
    private int bigestSize=1;

    

    void Start()
    {
        enemieSpawnScript = GetComponent<EnemieSpawnScript>();
        mapGrid = GetComponent<MapGrid>();
        mapGrid.InitializeMap();

        foreach (Room room in rooms)
        {
            if(bigestSize<room.roomHeight)
                bigestSize = room.roomHeight;
            if(bigestSize < room.roomWidth)
                bigestSize = room.roomWidth;
        }
        bool isSpawn = false;
        while(!isSpawn)
        {
            isSpawn=mapGrid.GenerateRoom(Random.Range(5, mapGrid.gridWidth-bigestSize), Random.Range(3, 4), false, spawnRooms[Random.Range(0, spawnRooms.Count)]);
            //Debug.Log("SpawnProba");
        }

        for (int x = 0; x < numberOfRooms; x++)
        {
            mapGrid.GenerateRoom(Random.Range(3, mapGrid.gridWidth - bigestSize), Random.Range(3, mapGrid.gridHeight - bigestSize), false, rooms[Random.Range(0, rooms.Count)]);
        }

        mapGrid.UpdateGridFlorsDors();
        UpdateDoors();
        mapGrid.BuildCoridors(coriddor);
        mapGrid.FillRest(filler);
        mapGrid.initializeSpawnPoints();
        mapGrid.SpawnPlayer();
        mapGrid.CreateNavMesh();
        enemieSpawnScript.Spawn();

    }

    private void UpdateDoors()
    {
        foreach(Room rom in mapGrid.rooms)
        {
            foreach(Transform en in rom.doorList)
            {
                en.gameObject.GetComponent<Entrance>().map = mapGrid;
            }
        }
    }

}
