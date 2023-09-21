using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

public class PredatorEM : MonoBehaviour
{
    private float energyReductionInterval = 10.0f; // reduce energy every 1 second
    private float timeSinceLastReduction = 0.0f;


    public float reproductionEnergy = 50f; // La energía necesaria para reproducirse
    //private float reproductionRate = 10; // La tasa de reproducción en segundos
    public float lastReproductionTime;

    public int eatedPlayer = 0;

    public PredatorFOV fov;
    public PredatorBehaviour movement;

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
        fov = GetComponent<PredatorFOV>(); // Inicializar la referencia en Start
        movement = GetComponent<PredatorBehaviour>();


        previousPosition = transform.position; // Guardar la posición inicial del agente

        //CreatureGenes creatureGenes = GetComponent<CreatureGenes>();

        //transform.localScale = Vector3.one * creatureGenes.sizeGene;

        //fov.radius = creatureGenes.fovRadiusGene;
        //fov.angle = creatureGenes.fovAngleGene;

        

        spawner = FindObjectOfType<Spawner>();

    }
    void Update()
    {
        

        float distance = Vector3.Distance(transform.position, previousPosition); // Calcular la distancia recorrida en el último fotograma

        // Si la distancia es mayor que un cierto umbral, ajustarla
        if (distance >= 10)
        {
            distance = 0f; // o algún otro valor que consideres adecuado
        }

        distanceTraveled += distance;

        previousPosition = transform.position; // Actualizar la posición anterior para el siguiente fotograma



        timeSinceLastReduction += Time.deltaTime;

        if (timeSinceLastReduction >= energyReductionInterval)
        {
            // Reduce energy based on vision radius and other factors
            energy -= distance * energyCostPerMeter * movement.PredatorSpeed;
            energy -= (fov.radius + (fov.angle / 180.0f * 5));
            // or any other calculation you have
            // Reset the timer
            timeSinceLastReduction = 0.0f;

        }

        //print(energy);
        if (energy <= 0)
        {
            lifeTime = Time.time - birthTime;
            spawner.predators_list.Remove(gameObject);
            Destroy(gameObject);

           
        }


    }
    void OnCollisionEnter(Collision collision)
    {


        // Comprobar si el objeto colisionado tiene la etiqueta "Comida"
        if (collision.collider.CompareTag("Player"))
        {
            
            Destroy(collision.gameObject);
            Spawner.playerPositions.Remove(collision.gameObject.transform.position);
            //Atribucio +20 energia per ser jugador
            energy += 40;

            energy = Mathf.Clamp(energy, 0, 100);
            eatedPlayer += 1;
            CalculateFitness();
            fov.isChasing = false;  // Una vez que hay "contacto", dejamos de 

        }

    }

    public void CalculateFitness()
    {
        fitness = lifeTime * 0.260f + eatedPlayer * 0.70f;
    }
}
