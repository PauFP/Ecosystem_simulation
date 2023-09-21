using JetBrains.Annotations;
//using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal.VR;
using UnityEngine;

using UnityEngine.EventSystems;

public class GAController : MonoBehaviour
{
    private float recordInterval = 10.0f; // Registrar datos cada 1 segundo
    private float nextRecordTime = 0.0f;

    public Spawner spawner;
    
    public GameObject Players;
    public GameObject Predators;
    
    //public GameObject energy;
    List<GameObject> PlayersList;
    List<GameObject> PredatorsList;
    [Space(10)]
    public float fitness;
    public DataLogger dataLogger;

    [Header("Children and Parent Settings")]
    public int numParents = 10;
    public int numChildren = 10;

    [Header("Mutation Settings")]
    public float PreyMutationRate = 0.01f;
    public float PredatorMutationRate = 0.01f;

    [Header("Reproduction Settings")]
    public float playerReproductionRate = 10f;  // tasa para las presas
    public float predatorReproductionRate = 15f;  // tasa para los depredadores

    [Header("Player Velocity Settings")]
    public int minVelocityPlayer = 2;
    public int  maxVelocityPlayer = 10;
    [Header("Predator Velocity Settings")]
    public int minVelocityPredator = 2;
    public int maxVelocityPredator = 10;
    
    [Header("Player FOV Settings")]
    public int minRadiusPlayer = 10;
    public int maxRadiusPlayer = 20;
    [Space(5)]
    public int minAnglePlayer = 50;
    public int maxAnglePlayer = 320;
    [Header("Predator FOV Settings")]
    public int minRadiusPredator = 10;
    public int maxRadiusPredator = 20;
    [Space(5)]
    public int minAnglePredator = 50;
    public int maxAnglePredator = 320;
    [Space(5)]
    public int minLimitSpeed;
    public int maxLimitSpeed;
    [Space(1)]
    public int minLimitRadius;
    public int maxLimitRadius;
    [Space(1)]
    public int minLimitAngle;
    public int maxLimitAngle;



    void Start()
    {
        // waitTime = 10.0f; // Cambia esto por la cantidad de tiempo que quieras esperar entre cada registro
        //StartCoroutine(LogSpeedsCoroutine(waitTime));

        //spawner.InitializePopulation();
        PlayersList = spawner.players_list;
        PredatorsList = spawner.predators_list;

        foreach (GameObject player in PlayersList)
        {
            Movement_final movement = player.GetComponent<Movement_final>();
            FieldOfView FOV = player.GetComponent<FieldOfView>();

            movement.movementSpeed = Random.Range(minVelocityPlayer, maxVelocityPlayer);

            FOV.radius = Random.Range(minRadiusPlayer, maxRadiusPlayer);
            FOV.angle = Random.Range(minAnglePlayer, maxAnglePlayer);

            spawner.unitSize = Random.Range(1, 6);
        }

        foreach (GameObject predator in PredatorsList)
        {
            PredatorBehaviour pBehaviour = predator.GetComponent<PredatorBehaviour>();
            PredatorFOV pFOV = predator.GetComponent<PredatorFOV>();

            //Spawner spawn = predator.GetComponent<Spawner>();

            pBehaviour.PredatorSpeed = Random.Range(minVelocityPredator, maxVelocityPredator);
            pFOV.radius = Random.Range(minRadiusPredator, maxRadiusPredator);
            pFOV.angle = Random.Range(minAnglePredator, maxAnglePredator);

            spawner.unitSize = Random.Range(1, 6);
        }
        StartCoroutine(PlayerReproductionCycle());
        StartCoroutine(PredatorReproductionCycle());
    }

    IEnumerator PlayerReproductionCycle()
    {
        while (true)
        {
            Debug.Log("Inicio de un nuevo ciclo de reproducción de presas");

            yield return new WaitForSeconds(playerReproductionRate);

            // Esperar un tiempo para que las criaturas acumulen aptitud
            // Ahora las criaturas han tenido tiempo para comer y sobrevivir, podemos seleccionar a los padres
            List<GameObject> PlayerParents = SelectParents(PlayersList, numParents, "Player");
            Debug.Log("Número de padres jugadores seleccionados: " + PlayerParents.Count);  // <-- Añade esta línea aquí

            foreach (GameObject parent in PlayerParents)
            {
                parent.GetComponent<Renderer>().material.color = Color.yellow;
            }
            CrossoverAndSpawn(PlayerParents, Players);
        }
    }

    IEnumerator PredatorReproductionCycle()
    {
        while (true)
        {
            Debug.Log("Inicio de un nuevo ciclo de reproducción de depredadores");

            yield return new WaitForSeconds(predatorReproductionRate);


            List<GameObject> PredatorParents = SelectParents(PredatorsList, numParents, "Predator");
            foreach (GameObject parent in PredatorParents)
            {
                parent.GetComponent<Renderer>().material.color = Color.cyan;
            }
            CrossoverAndSpawn(PredatorParents, Predators);
        }
    }
    /*void Fitness()
    {
        foreach (GameObject creatures in creaturesList)
        {
          
            EnergyManager energy = creatures.GetComponent<EnergyManager>();

            fitness = energy.lifeTime * 0.5f + energy.eatedFood * 0.5f;
            
        }
        
    }*/

    public float GetHighestFitness(List<GameObject> AgentList, string type)
    {
        float highestFitness = 0;

        foreach (GameObject creature in AgentList)
        {
            // Verifica si la criatura todavía existe
            if (creature != null)
            {
                if (type == "Player")
                {
                    EnergyManager energy = creature.GetComponent<EnergyManager>();

                    if (energy.fitness > highestFitness)
                    {
                        highestFitness = energy.fitness;
                    }
                }
                if (type == "Predator")
                {
                    PredatorEM energy = creature.GetComponent<PredatorEM>();

                    if (energy.fitness > highestFitness)
                    {
                        highestFitness = energy.fitness;
                    }
                }

            }
        }

        return highestFitness;

    }

   

    //ACORDARSE DE PONER TIPO ANTES DE LLAMAR FUNCION
    List<GameObject> SelectParents(List<GameObject> population, int numParents, string type)
    {
        if (numParents > population.Count)
        {
            numParents = population.Count;
        }
        List<GameObject> parents = new List<GameObject>();

        // Calcular la aptitud total de la población
        float totalFitness = 0;
        foreach (GameObject creature in population)
        {
            if (type == "Player")
            {
                totalFitness += creature.GetComponent<EnergyManager>().fitness;
            }
            if (type == "Predator")
            {
                totalFitness += creature.GetComponent<PredatorEM>().fitness;
            }
        }

        // Realizar la selección de ruleta
        for (int i = 0; i < numParents; i++)
        {
            float randomValue = UnityEngine.Random.value * totalFitness;
            float runningTotal = 0;

            foreach (GameObject creature in population)
            {
                if (type == "Player")
                {
                    runningTotal += creature.GetComponent<EnergyManager>().fitness;
                    if (runningTotal > randomValue)
                    {
                        parents.Add(creature);
                        break;
                    }
                }
                if (type == "Predator")
                {
                    runningTotal += creature.GetComponent<PredatorEM>().fitness;
                    if (runningTotal > randomValue)
                    {
                        parents.Add(creature);
                        break;
                    }
                }

            }
        }

        return parents;

    }
    void CrossoverAndSpawn(List<GameObject> parents, GameObject Agents)
    {
        //int originalNumChildren = 10; // Establece esto en el valor que desees al inicio de tu script

        Debug.Log("Inicio de CrossoverAndSpawn");
        //numChildren = originalNumChildren;

        /*if (numChildren > parents.Count)
        {
            numChildren = parents.Count;
        }*/

        Debug.Log("Antes del bucle for. numChildren: " + numChildren);


        for (int i = 0; i < numChildren; i += 2)
        {
            // Obtén los padres
            Debug.Log("Comenzando el proceso de cruce");
            // Selecciona pares de manera aleatòria
            GameObject parent1 = parents[Random.Range(0, parents.Count)];
            GameObject parent2 = parents[Random.Range(0, parents.Count)];

            //GameObject parent1 = parents[i];
            //GameObject parent2;

            // Si hay un segundo padre, úsalo; de lo contrario, simplemente clona al primer padre
            if (i + 1 < parents.Count)
            {
                parent2 = parents[i + 1];
            }
            else
            {
                parent2 = parent1; // Esto resultará en una clonación en lugar de un cruce
            }

            Debug.Log("Parent 1: " + parent1.name);
            Debug.Log("Parent 2: " + parent2.name);

            // Crea un nuevo hijo
            //GameObject child = Instantiate(creaturePrefab);
            Vector3 spawnPositions = new Vector3(Random.Range(-200, 200), parent1.transform.localScale.y/2, Random.Range(-200,200));
            GameObject child = Instantiate(Agents, spawnPositions, Quaternion.identity);
            if (child.tag == "Predator")
            {
                child.transform.localScale = new Vector3(4, 4, 4);
            }
            Debug.Log("Hijo creado con éxito");

            if (Agents.tag == "Player")
            {
                // Obtiene los componentes de los padres
                Movement_final movement1 = parent1.GetComponent<Movement_final>();
                Movement_final movement2 = parent2.GetComponent<Movement_final>();
                FieldOfView FOV1 = parent1.GetComponent<FieldOfView>();
                FieldOfView FOV2 = parent2.GetComponent<FieldOfView>();

                // Obtiene los componentes del hijo
                Movement_final childMovement = child.GetComponent<Movement_final>();
                FieldOfView childFOV = child.GetComponent<FieldOfView>();


                // Cruza los genes de los padres para generar los genes del hijo
                childMovement.movementSpeed = Random.value < 0.5 ? movement1.movementSpeed : movement2.movementSpeed;
                childFOV.radius = Random.value < 0.5 ? FOV1.radius : FOV2.radius;
                childFOV.angle = Random.value < 0.5 ? FOV1.angle : FOV2.angle;
                child.GetComponent<Renderer>().material.color = Color.green;

                //float mutationRate = 0.01f; // Esto podría ser un parámetro ajustable en tu simulación
                if (Random.value < PreyMutationRate)
                {
                    // Cambia un poco el valor de los genes del hijo
                    //childMovement.movementSpeed += Random.Range(-0.1f, 0.1f);
                    childMovement.movementSpeed += Random.Range(-1, 1);
                    //POR SI ACASO CANVIAR A childFOV.radius += Random.Range(-1f, 1f);
                    childFOV.radius += Random.Range(-1, 1);
                    childFOV.angle += Random.Range(-5, 5);

                    // Asegúrate de que los genes siguen dentro de sus rangos válidos
                    childMovement.movementSpeed = Mathf.Clamp(childMovement.movementSpeed, minLimitSpeed, maxLimitSpeed);
                    childFOV.radius = Mathf.Clamp(childFOV.radius, minLimitRadius, maxLimitRadius);
                    childFOV.angle = Mathf.Clamp(childFOV.angle, minLimitAngle, maxLimitAngle);
                }

                // Añade el hijo a la lista de criaturas
                PlayersList.Add(child);
            }
            if (Agents.tag == "Predator")
            {
                // Obtiene los componentes de los padres
                PredatorBehaviour movement1 = parent1.GetComponent<PredatorBehaviour>();
                PredatorBehaviour movement2 = parent2.GetComponent<PredatorBehaviour>();
                PredatorFOV FOV1 = parent1.GetComponent<PredatorFOV>();
                PredatorFOV FOV2 = parent2.GetComponent<PredatorFOV>();

                // Obtiene los componentes del hijo
                PredatorBehaviour childMovement = child.GetComponent<PredatorBehaviour>();
                PredatorFOV childFOV = child.GetComponent<PredatorFOV>();


                // Cruza los genes de los padres para generar los genes del hijo
                childMovement.PredatorSpeed = Random.value < 0.5 ? movement1.PredatorSpeed : movement2.PredatorSpeed;
                childFOV.radius = Random.value < 0.5 ? FOV1.radius : FOV2.radius;
                childFOV.angle = Random.value < 0.5 ? FOV1.angle : FOV2.angle;
                child.GetComponent<Renderer>().material.color = Color.magenta;

                if (Random.value < PredatorMutationRate)
                {
                    // Cambia un poco el valor de los genes del hijo
                    childMovement.PredatorSpeed += Random.Range(-1, 1);
                    childFOV.radius += Random.Range(-1, 1);
                    childFOV.angle += Random.Range(-5, 5);

                    // Asegúrate de que los genes siguen dentro de sus rangos válidos
                    childMovement.PredatorSpeed = Mathf.Clamp(childMovement.PredatorSpeed, minLimitSpeed, maxLimitSpeed);
                    childFOV.radius = Mathf.Clamp(childFOV.radius, minLimitRadius, maxLimitRadius);
                    childFOV.angle = Mathf.Clamp(childFOV.angle, minLimitAngle, maxLimitAngle);
                }

                // Añade el hijo a la lista de criaturas
                PredatorsList.Add(child);
            }
            Debug.Log("Fin del proceso de reproducción");
        }
    }

    // Update is called once per frame
    void Update()
    {
        float maxPlayerFit = GetHighestFitness(PlayersList, "Player");
        float maxPredatorFit = GetHighestFitness(PredatorsList, "Predator");
        
        if (Time.time >= nextRecordTime)
        {
            float currentTime = Time.time;
            int playerCount = PlayersList.Count;
            int predatorCount = PredatorsList.Count;


            dataLogger.LogPopulationData(currentTime, playerCount, predatorCount, "D:/Desktop/TrData/PopulationData.csv");

            //dataLogger.LogSpeedData(PlayersList, Time.time, "D:/Desktop/TrData/PlayersSpeedData.csv", "Player");
            //dataLogger.LogSpeedData(PredatorsList, Time.time, "D:/Desktop/TrData/PredatorsSpeedData.csv", "Predator");
            dataLogger.LogSpeedData(PlayersList, "D:/Desktop/TrData/PlayersSpeedData.csv", "Player");
            dataLogger.LogSpeedData(PredatorsList, "D:/Desktop/TrData/PredatorsSpeedData.csv", "Predator");
            



            dataLogger.LogRadiusData(PlayersList,"D:/Desktop/TrData/PlayersRadiusData.csv", "Player");
            dataLogger.LogRadiusData(PredatorsList, "D:/Desktop/TrData/PredatorsRadiusData.csv", "Predator");
            
            dataLogger.LogAngleData(PlayersList, "D:/Desktop/TrData/PlayersAngleData.csv", "Player");
            dataLogger.LogAngleData(PredatorsList, "D:/Desktop/TrData/PredatorsAngleData.csv", "Predator");
            nextRecordTime = Time.time + recordInterval;
        }
        //print(maxFit);
        /*float time = Time.time;
        int agents = GameObject.FindGameObjectsWithTag("Player").Length;

        Movement_final movement = creatures.GetComponent<Movement_final>();
        float speed = movement.movementSpeed;
        dataLogger.LogData(agents, speed);*/



    }

    /*public void LogAllCreatureSpeeds()
    {
        PlayersList = spawner.players_list;

        foreach (GameObject creature in PlayersList)
        {
            string creatureID = creature.GetInstanceID().ToString();
            float speed = creature.GetComponent<Movement_final>().movementSpeed; // Obtén la velocidad de la criatura
            dataLogger.LogData(creatureID, speed, generationIndex);
        }
        generationIndex++;


    }
    IEnumerator LogSpeedsCoroutine(float waitTime)
    {
        while (true)
        {
            LogAllCreatureSpeeds();
            yield return new WaitForSeconds(waitTime);
        }
    }*/

}
