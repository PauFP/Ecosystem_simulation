using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public GameObject playerPrefab;

    public static float velocity;

    public int agentsCount = 100;
    public int initFoodCount;
    public int spawnFoodCount;
    public float minX = -10.0f;
    public float maxX = 10.0f;
    public float minZ = -10.0f;
    public float maxZ = 10.0f;

    public int unitSize;
    public float SpawnTime;

    public static List<Vector3> foodPositions = new List<Vector3>();
    public static List<Vector3> playerPositions = new List<Vector3>();

    public List<GameObject> creatures_list = new List<GameObject>();

    void Awake()
    {
        StartCoroutine(SpawnFood());
        InitializePopulation();
        for (int i = 0; i < initFoodCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(minX, maxX), 1, Random.Range(minZ, maxZ));
            GameObject food = Instantiate(foodPrefab, position, Quaternion.identity);
            foodPositions.Add(position);
        }

    
    }

    public void InitializePopulation()
    {
        for (int i = 0; i < agentsCount; i++)
        {
            print("nuit_size " + unitSize);
            float last_unit_size = unitSize / 2;
            print("lalalal " + last_unit_size);

            Vector3 position = new Vector3(Random.Range(minX, maxX), last_unit_size, Random.Range(minZ, maxZ));

            GameObject player = Instantiate(playerPrefab, position, Quaternion.Euler(0, Random.Range(-360, 360), 0));

            player.transform.localScale = new Vector3(unitSize, unitSize, unitSize);

            playerPositions.Add(position);
            creatures_list.Add(player);
        }
    }

    IEnumerator SpawnFood()
    {
        while (true)
        {
            // Espera un cierto tiempo antes de generar comida
            yield return new WaitForSeconds(SpawnTime);

            // Generar piezas de comida
            for (int i = 0; i < spawnFoodCount; i++)
            {
                Vector3 position = new Vector3(Random.Range(minX, maxX), 1, Random.Range(minZ, maxZ));
                GameObject food = Instantiate(foodPrefab, position, Quaternion.identity);
                foodPositions.Add(position);
            }
        }
    }
    void Update()
    {
        // Limpiar la lista antes de actualizarla
        playerPositions.Clear();

        // Recorrer la lista de criaturas y obtener sus posiciones actuales
        foreach (GameObject player in creatures_list)
        {
            playerPositions.Add(player.transform.position);
        }
    }

}
