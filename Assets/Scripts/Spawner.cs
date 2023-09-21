using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public GameObject playerPrefab;
    public GameObject predatorPrefab;

    public static float velocity;

    public int agentsCount;
    public int predatorsCount;
    public int initFoodCount;
    public int spawnFoodCount;
    public float minX = -10.0f;
    public float maxX = 10.0f;
    public float minZ = -10.0f;
    public float maxZ = 10.0f;

    public float unitSize;
    public float SpawnTime;

    public static List<Vector3> foodPositions = new List<Vector3>();
    public static List<Vector3> playerPositions = new List<Vector3>();
    public static List<Vector3> predatorPositions = new List<Vector3>();

    public List<GameObject> predators_list = new List<GameObject>();
    public List<GameObject> players_list = new List<GameObject>();

    void Awake()
    {
        StartCoroutine(SpawnFood());
        InitializePopulation();
        InitializePredators();

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
            //print("Unit_size " + unitSize);

            Vector3 position = new Vector3(Random.Range(minX, maxX), /*1*/ 1.7f, Random.Range(minZ, maxZ));
            GameObject player = Instantiate(playerPrefab, position, Quaternion.Euler(0, Random.Range(-360, 360), 0));

            //player.transform.localScale = new Vector3(unitSize, unitSize, unitSize);

            playerPositions.Add(position);
            players_list.Add(player);
            

        }
    }
    public void InitializePredators()
    {
        //float predatorUnitSize = 4;

        for (int i = 0; i < predatorsCount; i++)
        {

            Vector3 position = new Vector3(Random.Range(minX, maxX), 2, Random.Range(minZ, maxZ));
            GameObject predator = Instantiate(predatorPrefab, position, Quaternion.Euler(0, Random.Range(-360, 360), 0));

            //predator.transform.localScale = new Vector3(predatorUnitSize, predatorUnitSize, predatorUnitSize);

            predatorPositions.Add(position);            
            predators_list.Add(predator);


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
                Vector3 position = new Vector3(Random.Range(minX, maxX), -5.9f, Random.Range(minZ, maxZ));
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
        for (int i = 0; i < players_list.Count; i++)
        {
            if (players_list[i] != null)
            {
                playerPositions.Add(players_list[i].transform.position);
            }
            else
            {
                // Si el objeto es nulo, elimínalo de la lista
                players_list.RemoveAt(i);
                i--;  // Disminuir el índice ya que hemos eliminado un elemento de la lista
            }
        }
        // Limpiar la lista antes de actualizarla
        playerPositions.Clear();

        // Recorrer la lista de criaturas y obtener sus posiciones actuales
        foreach (GameObject player in players_list)
        {
            playerPositions.Add(player.transform.position);
        }



        predatorPositions.Clear();

        // Recorrer la lista de criaturas y obtener sus posiciones actuales
        for (int i = 0; i < predators_list.Count; i++)
        {
            if (predators_list[i] != null)
            {
                predatorPositions.Add(predators_list[i].transform.position);
            }
            else
            {
                // Si el objeto es nulo, elimínalo de la lista
                predators_list.RemoveAt(i);
                i--;  // Disminuir el índice ya que hemos eliminado un elemento de la lista
            }
        }
        // Limpiar la lista antes de actualizarla
        predatorPositions.Clear();

        // Recorrer la lista de criaturas y obtener sus posiciones actuales
        foreach (GameObject predator in predators_list)
        {
            predatorPositions.Add(predator.transform.position);
        }


        
    }

}
