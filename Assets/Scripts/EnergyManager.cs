using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

public class EnergyManager : MonoBehaviour
{
    public float reproductionEnergy = 50f; // La energ�a necesaria para reproducirse
    //private float reproductionRate = 10; // La tasa de reproducci�n en segundos
    public float lastReproductionTime;

    public int eatedFood = 0;

    public FieldOfView fieldOfView;

    float energy = 100f; // La energ�a inicial del agente

    public float energyCostPerMeter = 1f; // El costo de energ�a por metro recorrido

    private Vector3 previousPosition; // Almacenar la posici�n anterior del agente
    private float distanceTraveled; // La distancia total recorrida por el agente

    public float birthTime;
    public float lifeTime;

    public float fitness;

    public Spawner spawner;
    void Start()
    {
        fieldOfView = GetComponent<FieldOfView>(); // Inicializar la referencia en Start

        previousPosition = transform.position; // Guardar la posici�n inicial del agente

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

        float distance = Vector3.Distance(transform.position, previousPosition); // Calcular la distancia recorrida en el �ltimo fotograma
        distanceTraveled += distance;
        
        //float time = distance; ; // Calcular el tiempo que tard� en recorrer esa distancia
        //float time = Time.deltaTime/ Time.timeScale;

         // Acumular la distancia total recorrida
        //energy -= time * energyCostPerMeter; // Reducir la energ�a en funci�n del 

        energy -= distance * (energyCostPerMeter / movement.movementSpeed);
        // -= distance * (energyCostPerMeter / Movement_final.movementSpeed);

        /* // Calcular la distancia recorrida en el �ltimo fotograma
        distanceTraveled += distance; // Acumular la distancia total recorrida

        energy -= distance * energyCostPerMeter;*/ // Reducir la energ�a en funci�n de la distancia recorrida

        previousPosition = transform.position; // Actualizar la posici�n anterior para el siguiente fotograma
        //print(energy);
        if (energy <= 0)
        {
            lifeTime = Time.time - birthTime;
            spawner.creatures_list.Remove(gameObject);
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
            // Detectar la colisi�n y realizar acciones aqu�
            //Debug.Log("Colisi�n detectada entre: " + gameObject.name + " y " + collision.collider.gameObject.name);
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
    void Reproduce()
{

        // Crear un nuevo agente en la misma posici�n
        GameObject newAgent = Instantiate(gameObject, transform.position, Quaternion.identity);
        Renderer renderer = newAgent.GetComponent<Renderer>();
        Color newColor = Color.blue;
        renderer.material.color = newColor;


        // Dividir la energ�a con el nuevo agente
        EnergyManager newAgentEnergy = newAgent.GetComponent<EnergyManager>();
        newAgentEnergy.energy = 20;
        energy /= 2;
        newAgentEnergy.eatedFood = 0;
        birthTime = Time.time;
        lifeTime = Time.time - birthTime;
        
        // Actualizar el �ltimo tiempo de reproducci�n
        lastReproductionTime = Time.time;

        /*
        CreatureGenes parentGenes = GetComponent<CreatureGenes>();
        CreatureGenes offspringGenes = newAgent.GetComponent<CreatureGenes>();


        offspringGenes.movementSpeedGene = (parentGenes.movementSpeedGene + parentGenes.movementSpeedGene) / 2f + Random.Range(-0.5f, 0.5f);
        // Puedes ajustar la tasa de mutaci�n modificando los valores de Random.Range()
        offspringGenes.sizeGene = (parentGenes.sizeGene + parentGenes.sizeGene) / 2f + Random.Range(-0.1f, 0.1f);
        offspringGenes.fovRadiusGene = (parentGenes.fovRadiusGene + parentGenes.fovRadiusGene) / 2f + Random.Range(-2f, 2f);
        offspringGenes.fovAngleGene = (parentGenes.fovAngleGene + parentGenes.fovAngleGene) / 2f + Random.Range(-10f, 10f);


        // Asegurarse de que los genes no sean menores de los valores m�nimos permitidos
        offspringGenes.movementSpeedGene = Mathf.Max(offspringGenes.movementSpeedGene, 1f);
        offspringGenes.sizeGene = Mathf.Max(offspringGenes.sizeGene, 1f);
        offspringGenes.fovRadiusGene = Mathf.Max(offspringGenes.fovRadiusGene, 1f);
        offspringGenes.fovAngleGene = Mathf.Clamp(offspringGenes.fovAngleGene, 10f, 360f);*/

    }

    public void CalculateFitness()
    {
        fitness = lifeTime * 0.15f + eatedFood * 0.75f;
    }
}
