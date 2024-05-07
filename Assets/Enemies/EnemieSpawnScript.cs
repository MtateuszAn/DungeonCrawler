using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieSpawnScript : MonoBehaviour
{
    [SerializeField] List<GameObject> listOfEnemies = new List<GameObject>();
    [SerializeField] int numberOfEnemies;
    [SerializeField] bool allAvailableSpawnPoints;
    MapGrid mapGrid;

    public void Spawn()
    {
        mapGrid=GetComponent<MapGrid>();
        if (listOfEnemies.Count == 0)
        {
            Debug.LogError("Lista przeciwników jest pusta!");
            return;
        }

        if (mapGrid.enemySpawnPoints.Count == 0)
        {
            Debug.LogError("Lista punktów spawnu przeciwników jest pusta!");
            return;
        }

        if (numberOfEnemies <= 0)
        {
            Debug.LogError("Nieprawid³owa liczba przeciwników do zespawnowania!");
            return;
        }

        if (mapGrid.enemySpawnPoints.Count < numberOfEnemies)
        {
            Debug.LogError("Zbyt ma³o punktów spawnu przeciwników, aby zaspawnowaæ wszystkich!");
            return;
        }

        List<Transform> availableSpawnPoints = new List<Transform>(mapGrid.enemySpawnPoints);
        if(!allAvailableSpawnPoints)
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                if (availableSpawnPoints.Count == 0)
                {
                    Debug.LogError("Zabrak³o punktów spawnu dla pozosta³ych przeciwników!");
                    break;
                }

                int randomEnemyIndex = Random.Range(0, listOfEnemies.Count);
                int randomSpawnPointIndex = Random.Range(0, availableSpawnPoints.Count);

                GameObject enemy = Instantiate(listOfEnemies[randomEnemyIndex], availableSpawnPoints[randomSpawnPointIndex].position, Quaternion.identity);
                availableSpawnPoints.RemoveAt(randomSpawnPointIndex);
            }
        }
        else
        {
            for (int i = 0; i < availableSpawnPoints.Count; i++)
            {
                int randomEnemyIndex = Random.Range(0, listOfEnemies.Count);

                GameObject enemy = Instantiate(listOfEnemies[randomEnemyIndex], availableSpawnPoints[i].position, Quaternion.identity);
                availableSpawnPoints.RemoveAt(i);
            }
        }
        
    }
}
