using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.radius);

        foreach (Vector3 pos in Spawner.foodPositions)
        {
            Vector3 directionToPos = (pos - fov.transform.position).normalized;
            if (Vector3.Angle(fov.transform.forward, directionToPos) < fov.angle / 2)
            {
                if (Vector3.Distance(fov.transform.position, pos) <= fov.radius)
                {
                    Handles.color = Color.green;
                    Handles.DrawLine(fov.transform.position, pos);

                }
            }
        }
       

        foreach (Vector3 pos in Spawner.playerPositions)
        {
            
            Vector3 directionToPos = (pos - fov.transform.position).normalized;
            if (Vector3.Angle(fov.transform.forward, directionToPos) < fov.angle / 2)
            {
                if (Vector3.Distance(fov.transform.position, pos) <= fov.radius)
                {
                    Handles.color = Color.green;
                    Handles.DrawLine(fov.transform.position, pos);

                }
               
            }
            
            
        }


    }


    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
