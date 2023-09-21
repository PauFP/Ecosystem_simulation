using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(PredatorFOV))]

public class PredatorFOVEditor : Editor
{
    private void OnSceneGUI()
    {
        PredatorFOV pfov = (PredatorFOV)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(pfov.transform.position, Vector3.up, Vector3.forward, 360, pfov.radius);

        Vector3 pviewAngle01 = DirectionFromAngle(pfov.transform.eulerAngles.y, -pfov.angle / 2);
        Vector3 pviewAngle02 = DirectionFromAngle(pfov.transform.eulerAngles.y, pfov.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(pfov.transform.position, pfov.transform.position + pviewAngle01 * pfov.radius);
        Handles.DrawLine(pfov.transform.position, pfov.transform.position + pviewAngle02 * pfov.radius);

        foreach (Vector3 pos in Spawner.playerPositions)
        {
            Vector3 directionToPos = (pos - pfov.transform.position).normalized;
            if (Vector3.Angle(pfov.transform.forward, directionToPos) < pfov.angle / 2)
            {
                if (Vector3.Distance(pfov.transform.position, pos) <= pfov.radius)
                {
                    Handles.color = Color.green;
                    Handles.DrawLine(pfov.transform.position, pos);


                }
            }
        }
        foreach (Vector3 pos in Spawner.predatorPositions)
        {
            Vector3 directionToPos = (pos - pfov.transform.position).normalized;
            if (Vector3.Angle(pfov.transform.forward, directionToPos) < pfov.angle / 2)
            {
                if (Vector3.Distance(pfov.transform.position, pos) <= pfov.radius)
                {
                    Handles.color = Color.green;
                    Handles.DrawLine(pfov.transform.position, pos);


                }
            }
        }


        /*foreach (Vector3 pos in Spawner.playerPositions)
        {

            Vector3 directionToPos = (pos - pfov.transform.position).normalized;
            if (Vector3.Angle(fov.transform.forward, directionToPos) < pfov.angle / 2)
            {
                if (Vector3.Distance(pfov.transform.position, pos) <= fov.radius)
                {
                    Handles.color = Color.green;
                    Handles.DrawLine(pfov.transform.position, pos);

                }

            }


        }*/
    }


    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
