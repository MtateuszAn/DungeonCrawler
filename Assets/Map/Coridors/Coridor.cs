using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coridor : MonoBehaviour
{
    [SerializeField] GameObject wall;
    public MapGrid map;

    Quaternion spawnRotation90 = Quaternion.Euler(0, 90, 0);
    Quaternion spawnRotationM90 = Quaternion.Euler(0, -90, 0);
    Quaternion spawnRotation180 = Quaternion.Euler(0, 180, 0);
    int x;
    int z;

    private void Start()
    {

        x = (int)transform.position.x / 2;
        z = (int)transform.position.z / 2;

        //gora
        if (!map.grid[x, z + 1].isTaken)
            Instantiate(wall, transform.position + new Vector3(0, 0, 1), transform.rotation);
        //dol
        if (!map.grid[x, z - 1].isTaken)
            Instantiate(wall, transform.position + new Vector3(0, 0, -1), spawnRotation180);
        //prawo
        if (!map.grid[x + 1, z].isTaken)
            Instantiate(wall, transform.position + new Vector3(1, 0, 0), spawnRotation90);
        //lewo
        if (!map.grid[x - 1, z].isTaken)
            Instantiate(wall, transform.position + new Vector3(-1, 0, 0), spawnRotationM90);

    }

}
