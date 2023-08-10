using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GAController : MonoBehaviour
{
    public Spawner spawner;
    public GameObject creatures;
    
    //public GameObject energy;
    List<GameObject> creaturesList;
    public float fitness;
    public int numParents = 10;
    public int numChildren = 10;
    public DataLogger dataLogger;

    
    IEnumerator Start()
    {
        // waitTime = 10.0f; // Cambia esto por la cantidad de tiempo que quieras esperar entre cada registro
        //StartCoroutine(LogSpeedsCoroutine(waitTime));

        spawner.InitializePopulation();
        creaturesList = spawner.creatures_list;

        foreach (GameObject creature in creaturesList)
        {
            Movement_final movement = creature.GetComponent<Movement_final>();
            FieldOfView FOV = creature.GetComponent<FieldOfView>();
            Spawner spawn = creature.GetComponent<Spawner>(); 

            movement.movementSpeed = Random.Range(2.0f, 10.0f);

            FOV.radius = Random.Range(10, 20);
            FOV.angle = Random.Range(50, 260);

            spawner.unitSize = Random.Range(1,6);
        }

        while (true)
        {
            // Esperar un tiempo para que las criaturas acumulen aptitud
            yield return new WaitForSeconds(10f);

            // Ahora las criaturas han tenido tiempo para comer y sobrevivir, podemos seleccionar a los padres
            List<GameObject> parents = SelectParents(creaturesList, numParents);
            foreach (GameObject parent in parents)
            { 
                parent.GetComponent<Renderer>().material.color = Color.yellow;
            }
            CrossoverAndSpawn(parents);
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

    public float GetHighestFitness()
    {
        float highestFitness = 0;

        foreach (GameObject creature in creaturesList)
        {
            // Verifica si la criatura todavía existe
            if (creature != null)
            {
                EnergyManager energy = creature.GetComponent<EnergyManager>();

                if (energy.fitness > highestFitness)
                {
                    highestFitness = energy.fitness;
                }
            }
        }

        return highestFitness;
    }


    List<GameObject> SelectParents(List<GameObject> population, int numParents)
    {
        List<GameObject> parents = new List<GameObject>();

        // Calcular la aptitud total de la población
        float totalFitness = 0;
        foreach (GameObject creature in population)
        {
            totalFitness += creature.GetComponent<EnergyManager>().fitness;
        }

        // Realizar la selección de ruleta
        for (int i = 0; i < numParents; i++)
        {
            float randomValue = Random.value * totalFitness;
            float runningTotal = 0;

            foreach (GameObject creature in population)
            {
                runningTotal += creature.GetComponent<EnergyManager>().fitness;
                if (runningTotal > randomValue)
                {
                    parents.Add(creature);
                    break;
                }
            }
        }

        return parents;
    }
    void CrossoverAndSpawn(List<GameObject> parents)
    {
        
        
        for (int i = 0; i < numChildren; i += 2)
        {
            // Obtén los padres
            GameObject parent1 = parents[i];
            GameObject parent2 = parents[i + 1];

            // Crea un nuevo hijo
            //GameObject child = Instantiate(creaturePrefab);
            Vector3 spawnPositions = new Vector3(Random.Range(-200, 200), 1, Random.Range(-200,200));
            GameObject child = Instantiate(creatures, spawnPositions, Quaternion.identity);

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

            float mutationRate = 0.01f; // Esto podría ser un parámetro ajustable en tu simulación
            if (Random.value < mutationRate)
            {
                // Cambia un poco el valor de los genes del hijo
                childMovement.movementSpeed += Random.Range(-0.1f, 0.1f);
                childFOV.radius += Random.Range(-1f, 1f);
                childFOV.angle += Random.Range(-5f, 5f);

                // Asegúrate de que los genes siguen dentro de sus rangos válidos
                childMovement.movementSpeed = Mathf.Clamp(childMovement.movementSpeed, 2.0f, 10.0f);
                childFOV.radius = Mathf.Clamp(childFOV.radius, 10, 20);
                childFOV.angle = Mathf.Clamp(childFOV.angle, 50, 260);
            }

            // Añade el hijo a la lista de criaturas
            creaturesList.Add(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float maxFit = GetHighestFitness();
        //print(maxFit);
        /*float time = Time.time;
        int agents = GameObject.FindGameObjectsWithTag("Player").Length;

        Movement_final movement = creatures.GetComponent<Movement_final>();
        float speed = movement.movementSpeed;
        dataLogger.LogData(agents, speed);*/



    }
    private int generationIndex = 0;

    public void LogAllCreatureSpeeds()
    {
        creaturesList = spawner.creatures_list;

        foreach (GameObject creature in creaturesList)
        {
            string creatureID = creature.GetInstanceID().ToString();
            float speed = creature.GetComponent<Movement_final>().movementSpeed; // Obtén la velocidad de la criatura
            dataLogger.LogData(creatureID, speed, generationIndex);
        }
        generationIndex++;


    }
    /*IEnumerator LogSpeedsCoroutine(float waitTime)
    {
        while (true)
        {
            LogAllCreatureSpeeds();
            yield return new WaitForSeconds(waitTime);
        }
    }*/

}
