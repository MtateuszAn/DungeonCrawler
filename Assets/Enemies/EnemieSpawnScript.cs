using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieSpawnScript : MonoBehaviour
{
    [SerializeField] List<GameObject> listOfEnemies = new List<GameObject>();
    [SerializeField] int numberOfEnemies;
    MapGrid mapGrid;

    public void Spawn()
    {
        mapGrid=GetComponent<MapGrid>();
        if (listOfEnemies.Count == 0)
        {
            Debug.LogError("Lista przeciwnik�w jest pusta!");
            return;
        }

        if (mapGrid.enemySpawnPoints.Count == 0)
        {
            Debug.LogError("Lista punkt�w spawnu przeciwnik�w jest pusta!");
            return;
        }

        if (numberOfEnemies <= 0)
        {
            Debug.LogError("Nieprawid�owa liczba przeciwnik�w do zespawnowania!");
            return;
        }

        if (mapGrid.enemySpawnPoints.Count < numberOfEnemies)
        {
            Debug.LogError("Zbyt ma�o punkt�w spawnu przeciwnik�w, aby zaspawnowa� wszystkich!");
            return;
        }

        List<Transform> availableSpawnPoints = new List<Transform>(mapGrid.enemySpawnPoints);

        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (availableSpawnPoints.Count == 0)
            {
                Debug.LogError("Zabrak�o punkt�w spawnu dla pozosta�ych przeciwnik�w!");
                break;
            }

            int randomEnemyIndex = Random.Range(0, listOfEnemies.Count);
            int randomSpawnPointIndex = Random.Range(0, availableSpawnPoints.Count);

            GameObject enemy = Instantiate(listOfEnemies[randomEnemyIndex], availableSpawnPoints[randomSpawnPointIndex].position, Quaternion.identity);
            availableSpawnPoints.RemoveAt(randomSpawnPointIndex);
        }
    }
}
