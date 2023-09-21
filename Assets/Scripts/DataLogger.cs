using System;
using System.Text;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class DataLogger : MonoBehaviour
{
    private string filePathPopulation = "D:/Desktop/populationData.csv";
    private string filePathSpeed = "D:/Desktop/speedData.csv";
    private Dictionary<string, StreamWriter> openFiles = new Dictionary<string, StreamWriter>();

    private void InitializeFile(string path, string header)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, header);
        }

        if (!openFiles.ContainsKey(path))
        {
            var streamWriter = new StreamWriter(path, true);
            openFiles[path] = streamWriter;
        }
    }

    private void OnApplicationQuit()
    {
        // Asegurarse de cerrar todos los archivos al salir de la aplicación
        foreach (var writer in openFiles.Values)
        {
            writer.Close();
        }
    }

    public void LogPopulationData(float time, int playerCount, int predatorCount, string ubi)
    {
        StringBuilder line = new StringBuilder();

        line.Append(time.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
        line.Append(",");
        line.Append(playerCount);
        line.Append(",");
        line.Append(predatorCount);
        line.Append("\n");

        try
        {
            // Comprobar e inicializar el archivo si es necesario
            if (!openFiles.ContainsKey(ubi))
            {
                InitializeFile(ubi, "time,preys,predators\n");
            }
            openFiles[ubi].Write(line.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to append to {ubi}. Error: {e.Message}");
        }
    }


    public void LogSpeedData(List<GameObject> list, string ubi, string type)
    {
        StringBuilder line = new StringBuilder();

        if (type == "Player")
        {
            foreach (var player in list)
            {
                var movement = player.GetComponent<Movement_final>();
                if (movement != null)
                {
                    line.Clear();
                    line.Append(Time.time.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append(",");
                    line.Append(movement.movementSpeed.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append("\n");

                    if (!openFiles.ContainsKey(ubi))
                    {
                        InitializeFile(ubi, "time,speed\n");
                    }
                    openFiles[ubi].Write(line.ToString());
                }
            }
        }
        else if (type == "Predator")
        {
            foreach (var player in list)
            {
                var movement = player.GetComponent<PredatorBehaviour>();
                if (movement != null)
                {
                    line.Clear();
                    line.Append(Time.time.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append(",");
                    line.Append(movement.PredatorSpeed.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append("\n");

                    if (!openFiles.ContainsKey(ubi))
                    {
                        InitializeFile(ubi, "time,speed\n");
                    }
                    openFiles[ubi].Write(line.ToString());
                }
            }
        }
    }

    public void LogRadiusData(List<GameObject> players, string ubi, string type)
    {
        StringBuilder line = new StringBuilder();

        if (type == "Player")
        {
            foreach (var player in players)
            {
                var fov = player.GetComponent<FieldOfView>();
                if (fov != null)
                {
                    line.Clear();
                    line.Append(Time.time.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append(",");
                    line.Append(fov.radius.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append("\n");

                    if (!openFiles.ContainsKey(ubi))
                    {
                        InitializeFile(ubi, "time,radius\n");
                    }
                    openFiles[ubi].Write(line.ToString());
                }
            }
        }
        else if (type == "Predator")
        {
            foreach (var player in players)
            {
                var fov = player.GetComponent<PredatorFOV>();
                if (fov != null)
                {
                    line.Clear();
                    line.Append(Time.time.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append(",");
                    line.Append(fov.radius.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append("\n");

                    if (!openFiles.ContainsKey(ubi))
                    {
                        InitializeFile(ubi, "time,radius\n");
                    }
                    openFiles[ubi].Write(line.ToString());
                }
            }
        }
    }

    public void LogAngleData(List<GameObject> players, string ubi, string type)
    {
        StringBuilder line = new StringBuilder();

        if (type == "Player")
        {
            foreach (var player in players)
            {
                var fov = player.GetComponent<FieldOfView>();
                if (fov != null)
                {
                    line.Clear();
                    line.Append(Time.time.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append(",");
                    line.Append(fov.angle.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append("\n");

                    if (!openFiles.ContainsKey(ubi))
                    {
                        InitializeFile(ubi, "time,angle\n");
                    }
                    openFiles[ubi].Write(line.ToString());
                }
            }
        }
        else if (type == "Predator")
        {
            foreach (var player in players)
            {
                var fov = player.GetComponent<PredatorFOV>();
                if (fov != null)
                {
                    line.Clear();
                    line.Append(Time.time.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append(",");
                    line.Append(fov.angle.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
                    line.Append("\n");

                    if (!openFiles.ContainsKey(ubi))
                    {
                        InitializeFile(ubi, "time,angle\n");
                    }
                    openFiles[ubi].Write(line.ToString());
                }
            }
        }
    }

}
