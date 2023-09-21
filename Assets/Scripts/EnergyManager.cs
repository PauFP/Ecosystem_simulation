using System.Collections.Generic;
using System.Drawing;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

public class EnergyManager : MonoBehaviour
{
    private float energyReductionInterval = 10.0f; // reduce energy every 1 second
    private float timeSinceLastReduction = 0.0f;


    public float reproductionEnergy = 50f; // La energía necesaria para reproducirse
    //private float reproductionRate = 10; // La tasa de reproducción en segundos
    public float lastReproductionTime;

    public int eatedFood = 0;

    public FieldOfView fieldOfView;

    float energy = 100f; // La energía inicial del agente

    public float energyCostPerMeter = 1f; // El costo de energía por metro recorrido

    private Vector3 previousPosition; // Almacenar la posición anterior del agente
    private float distanceTraveled; // La distancia total recorrida por el agente

    public float birthTime;
    public float lifeTime;

    public float fitness;

    public Spawner spawner;
    void Start()
    {
        fieldOfView = GetComponent<FieldOfView>(); // Inicializar la referencia en Start

        previousPosition = transform.position; // Guardar la posición inicial del agente

        //CreatureGenes creatureGenes = GetComponent<CreatureGenes>();
        Movement_final movement = GetComponent<Movement_final>();

        //transform.localScale = Vector3.one * creatureGenes.sizeGene;

        FieldOfView fov = GetComponent<FieldOfView>();
        //fov.radius = creatureGenes.fovRadiusGene;
        //fov.angle = creatureGenes.fovAngleGene;
        spawner = FindObjectOfType<Spawner>();

    }
    void Update()
    {
        Movement_final movement = GetComponent<Movement_final>();
        FieldOfView fov = GetComponent<FieldOfView>();

        float distance = Vector3.Distance(transform.position, previousPosition); // Calcular la distancia recorrida en el último fotograma
        distanceTraveled += distance;

        // Si la distancia es mayor que un cierto umbral, ajustarla
        if (distance >= 10)
        {
            distance = 0f; // o algún otro valor que consideres adecuado
        }
        //float time = distance; ; // Calcular el tiempo que tardó en recorrer esa distancia
        //float time = Time.deltaTime/ Time.timeScale;

        // Acumular la distancia total recorrida
        //energy -= time * energyCostPerMeter; // Reducir la energía en función del 

        //print(energy);


        timeSinceLastReduction += Time.deltaTime;

        if (timeSinceLastReduction >= energyReductionInterval)
        {
            energy -= distance * energyCostPerMeter * movement.movementSpeed;
            // Reduce energy based on vision radius and other factors
            energy -= (fov.radius + (fov.angle / 180.0f * 5));
            // or any other calculation you have
            // Reset the timer


            timeSinceLastReduction = 0.0f;
        }

        // -= distance * (energyCostPerMeter / Movement_final.movementSpeed);

        /* // Calcular la distancia recorrida en el último fotograma
        distanceTraveled += distance; // Acumular la distancia total recorrida

        energy -= distance * energyCostPerMeter;*/ // Reducir la energía en función de la distancia recorrida

        previousPosition = transform.position; // Actualizar la posición anterior para el siguiente fotograma
        //print(energy);
        if (energy <= 0)
        {
            lifeTime = Time.time - birthTime;
            spawner.players_list.Remove(gameObject);
            Destroy(gameObject);
            
            //controller.creatureList.remove(gameObject);
            //Debug.Log("lifetime: " + lifeTime);
        }
        
        /*if (eatedFood >= 3 && Time.time - lastReproductionTime >= reproductionRate)
        {
            Reproduce();
            eatedFood -= 2;
        }*/



    }
    void OnCollisionEnter(Collision collision)
    {
        

        // Comprobar si el objeto colisionado tiene la etiqueta "Comida"
        if (collision.collider.CompareTag("Food"))
        {
            // Detectar la colisión y realizar acciones aquí
            //Debug.Log("Colisión detectada entre: " + gameObject.name + " y " + collision.collider.gameObject.name);
            Destroy(collision.gameObject);
            Spawner.foodPositions.Remove(collision.gameObject.transform.position);
            energy += 10;
            
            energy = Mathf.Clamp(energy, 0, 100);
            eatedFood += 1;
            CalculateFitness();
        }

        

        /*if (collision.collider.CompareTag("Player"))
        {
            if (energy >= reproductionEnergy && Time.time - lastReproductionTime >= reproductionRate)
            {
                Reproduce();
                
                
            }

            Quaternion qto = Quaternion.Euler(new Vector3(0, 180, 0));
            transform.rotation = Quaternion.Slerp(transform.rotation, qto, 1f);
            print("No");
        }*/

    }
   

    public void CalculateFitness()
    {
        fitness = lifeTime * 0.70f + eatedFood * 0.25f;
    }
}
