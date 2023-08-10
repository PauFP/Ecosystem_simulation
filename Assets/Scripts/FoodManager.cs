using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public static List<Vector3> foodPositions = new List<Vector3>();
    public int numFood = 100;
    public GameObject foodPrefab;
    public float spawnRadius = 10f;

    void Start()
    {
        // Spawn initial food
        for (int i = 0; i < numFood; i++)
        {
            Vector3 spawnPosition = GetRandomPosition();
            Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
            foodPositions.Add(spawnPosition);
        }
    }

    void Update()
    {
        // Check for eaten food and remove from list
        foreach (GameObject foodObject in GameObject.FindGameObjectsWithTag("Food"))
        {
            if (!foodPositions.Contains(foodObject.transform.position))
            {
                foodPositions.Remove(foodObject.transform.position);
            }
        }

        // Respawn food if necessary
        int numFoodMissing = numFood - foodPositions.Count;
        if (numFoodMissing > 0)
        {
            for (int i = 0; i < numFoodMissing; i++)
            {
                Vector3 spawnPosition = GetRandomPosition();
                Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
                foodPositions.Add(spawnPosition);
            }
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection += transform.position;
        UnityEngine.AI.NavMeshHit navHit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out navHit, spawnRadius, -1);
        return navHit.position;
    }
}
