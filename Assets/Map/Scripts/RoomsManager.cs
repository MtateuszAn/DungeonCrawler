using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomsManager : MonoBehaviour
{
    [SerializeField] int seed = 0;
    [SerializeField] List<Room> rooms;
    [SerializeField] List<Room> spawnRooms;
    [SerializeField] GameObject coriddor;
    MapGrid mapGrid;
    EnemieSpawnScript enemieSpawnScript;
    [SerializeField] private int numberOfRooms;
    private int bigestSize=1;

    

    void Start()
    {
        if (seed == 0)
        {
            seed = Random.Range(0,int.MaxValue);
            Debug.Log("Seed: " + seed);
        }
        Random.seed = seed;
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
        //Place Spawn Room
        bool isSpawn = false;
        while(!isSpawn)
        {
            isSpawn=mapGrid.GenerateRoom(Random.Range(5, mapGrid.gridWidth-bigestSize), Random.Range(3, 4), false, spawnRooms[Random.Range(0, spawnRooms.Count)]);
            //Debug.Log("SpawnProba");
        }

        //place rooms
        int failedAttempts = 0;
        for (int x = 0; x < numberOfRooms; x++)
        {
            Room room = rooms[Random.Range(0, rooms.Count)];
            int gridW = Random.Range(3, mapGrid.gridWidth - bigestSize);
            int gridH = Random.Range(3, mapGrid.gridHeight - bigestSize);
            bool success = false;
            do
            {
                success = mapGrid.GenerateRoom(gridW, gridH, false, room);
                failedAttempts++;
                if (failedAttempts >= 10000)
                {
                    Debug.LogError("Failed to generate room");
                    failedAttempts = 0;
                    break;
                }
                gridW++;
                if (gridW >= mapGrid.gridWidth - bigestSize)
                {
                    gridW = 3;
                    gridH++;
                }
                if (gridH >= mapGrid.gridHeight - bigestSize) gridH = 3;
            } while (success != true);
        }

        mapGrid.UpdateGridFlorsDors();
        UpdateDoors();
        mapGrid.BuildCoridors(coriddor);
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
