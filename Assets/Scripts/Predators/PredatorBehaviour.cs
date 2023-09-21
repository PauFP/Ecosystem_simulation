using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PredatorBehaviour : MonoBehaviour
{
    public PredatorFOV fov;
    
    public int PredatorSpeed;
    public float turnSpeed = 10f;

    Quaternion rotGoal;
    Quaternion qTo;



    // Start is called before the first frame update
    void Start()
    {
        fov = GetComponent<PredatorFOV>();
        InvokeRepeating("ChangeRotation", 1f, Random.Range(1, 5));

    }
   
    // Update is called once per frame
    void Update()
    {
        if (fov.canSeePlayer && fov.detectedPlayer)
        {
            fov.isChasing = true;  // Establecer que estamos persiguiendo a un jugador

            StopRandomRotation();

            Vector3 prey_pos = new Vector3(fov.detectedPlayer.position.x, transform.position.y, fov.detectedPlayer.position.z);

            Vector3 directionToPos = (prey_pos - transform.position).normalized;

            rotGoal = Quaternion.LookRotation(directionToPos, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnSpeed);

            transform.position = Vector3.MoveTowards(transform.position, prey_pos, PredatorSpeed * Time.deltaTime);
        }
        else if (!fov.detectedPredator)
        {
            StartRandomRotation();
            Movement();

        }
        


    }
    public void Movement()
    {

        transform.rotation = Quaternion.Slerp(transform.rotation, qTo, turnSpeed);

        transform.position += transform.forward * PredatorSpeed * Time.deltaTime;



    }

    
    public void ChangeRotation()
    {
        qTo = Quaternion.Euler(new Vector3(0, Random.Range(-180, 180), 0));
    }

    public void StopRandomRotation()
    {
        CancelInvoke("ChangeRotation");
    }

    public void StartRandomRotation()
    {
        if (!IsInvoking("ChangeRotation"))
        {
            InvokeRepeating("ChangeRotation", 1f, Random.Range(1, 5));
        }
    }
}
