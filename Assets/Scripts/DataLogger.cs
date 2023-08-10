using System.IO;
using System.Text;
using UnityEditor.PackageManager;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    private string filePath;

    private void Start()
    {
        filePath = "D:/Desktop/data.csv";

        // Escribir la cabecera del archivo
        string header = "agents,speed\n";
        File.WriteAllText(filePath, header);
    }

    public void aLogData(int agents, float speed)
    {
        StringBuilder line = new StringBuilder();
        line.Append(agents/*.ToString("F3"*/); // Tiempo con tres decimales
        line.Append(",");
        line.Append(speed.ToString("F3")); // Velocidad con tres decimales
        line.Append("\n");

        File.AppendAllText(filePath, line.ToString());
    }
    public void LogData(string creatureID, float speed, int generation)
    {
        string path = "D:/Desktop/creatureSpeeds.csv";
        StringBuilder linea = new StringBuilder();
        linea.Append("generation"); // Tiempo con tres decimales
        linea.Append(",");
        linea.Append("creatureID"); // Velocidad con tres decimales
        linea.Append(",");
        linea.Append("speed"); // Velocidad con tres 
        linea.Append("\n");
        File.AppendAllText(filePath, linea.ToString());

        // Use append mode ('true') so that data is just added to the existing file
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            string line = string.Format("{0},{1},{2}", generation, creatureID, Mathf.RoundToInt(speed));

            writer.WriteLine(line);
        }
    }
}
