using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Timer : MonoBehaviour
{
    float timer;
    int dia = 0;
    int cicle;
    bool diaSumado = false; // Agregar esta variable
    public GameObject player;

    public TextMeshProUGUI days;
    public TextMeshProUGUI vTime;
    public TextMeshProUGUI secs;
    public TextMeshProUGUI nAgents;

    public float customTimeScale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        days.text = "DIES: " + dia;
        
        // calcular_estat();
        cicles();
        vTime.text = "VELOCITAT TEMPS:  " + customTimeScale + "x";
        Time.timeScale = customTimeScale;

        int agents = GameObject.FindGameObjectsWithTag("Player").Length;
        nAgents.text = "Nº Agents: " + agents.ToString();


    }

    void cicles()
    {
        
        timer += Time.deltaTime;
        cicle = (int)(timer % 1000);

        cicle += 1;
        secs.text = "Segons: " + cicle;
        
        if (cicle == 1000 && !diaSumado) // Modificar esta condición
        {
            dia += 1;
            diaSumado = true; // Establecer la variable a true cuando se suma un día          
        }

        // Reiniciar la variable diaSumado cuando cicle no sea 1000
        if (cicle != 1000)
        {
            diaSumado = false;
        }
    }
    public void IncrementTime()
    {
        customTimeScale += 0.5f;
    }
    public void DecreaseTime()
    {
        customTimeScale -= 0.5f;
    }
    public void StopTime()
    {
        customTimeScale = 0.0f;
    }


}
