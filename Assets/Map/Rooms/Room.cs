using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door
{
    public int x;
    public int z;

    public Door(int x, int z) 
    { 
        this.x = x/2;
        this.z = z/2;
    }
}
public class Room : MonoBehaviour
{
    [SerializeField] public int roomWidth;
    [SerializeField] public int roomHeight;
    [SerializeField] List<Transform> doorList;
    [SerializeField] public List<Transform> floorList;
    [SerializeField] public Transform spawnPoint;
    public List<Door> doors = new List<Door>();
    public List<Door> floors = new List<Door>();

    public void UpdateDoors()
    {
        foreach (Transform door in doorList)
        {
            doors.Add(new Door((int)door.position.x, (int)door.position.z));
        }
    }

    public void UpdateFloors()
    {
        foreach (Transform flor in floorList)
        {
            floors.Add(new Door((int)flor.position.x, (int)flor.position.z));
        }
    }
}
