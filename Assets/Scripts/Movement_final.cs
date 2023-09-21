using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_final : MonoBehaviour
{

    public float turnSpeed = 10f;
    public int movementSpeed;
    // Update is called once per frame
    Quaternion qTo;
    public FieldOfView fov;
    public int EatenFood;
    Quaternion rotGoal;


    private void Start()
    {


        fov = GetComponent<FieldOfView>();

        InvokeRepeating("ChangeRotation", 1f, Random.Range(1, 5));


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
    private void Update()
    {

        /*if (!fov.canSeePlayer && !fov.canSeeFood)
        {
            Movement();
        }
        else if (fov.canSeeFood)
        {
            MoveToFood();

        }*/
        if (fov.isBeingChased && fov.detectedPredator)
        {
            // Obtener la dirección de huida
            Vector3 fleeDirection = (transform.position - fov.detectedPredator.position).normalized;

            fleeDirection.y = 0;
            // Mover al jugador en la dirección de huida
            transform.position += fleeDirection * movementSpeed * Time.deltaTime;
        }
        else
        {
            if (!fov.canSeeFood)
            {
                Movement();
            }
            else if (fov.canSeeFood)
            {
                MoveToFood();
            }
        }
        


    }


    void MoveToFood()
    {
        bool canSeeFood = false;
        foreach (Vector3 pos in Spawner.foodPositions)
        {

            Vector3 directionToPos = (pos - fov.transform.position).normalized;
            if (Vector3.Angle(fov.transform.forward, directionToPos) < fov.angle / 2)
            {
                if (Vector3.Distance(fov.transform.position, pos) <= fov.radius)
                {
                    if (directionToPos != Vector3.zero)
                    {
                        rotGoal = Quaternion.LookRotation(directionToPos, Vector3.up);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnSpeed);
                    }
                    transform.position = Vector3.MoveTowards(transform.position, pos, movementSpeed * Time.deltaTime);
                    canSeeFood = true;
                    break; // salir del bucle si se detecta al jugador
                }
            }
        }
        fov.canSeeFood = canSeeFood; // establecer la variable canSeePlayer en el objeto fov
        if (canSeeFood == false)
        {
            Movement();
        }
    }
    public void Movement()
    {

        transform.rotation = Quaternion.Slerp(transform.rotation, qTo, turnSpeed);

        transform.position += transform.forward * movementSpeed * Time.deltaTime;



    }
    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Food"){
            Destroy(collision.gameObject);
            Spawner.foodPositions.Remove(collision.gameObject.transform.position);
            //Debug.Log(Spawner.foodPositions.Count);
            EatenFood += 1;
            //fov.position_list.Remove(collision.gameObject.transform.position);
            //fov.foodList.Remove(collision.gameObject);
        }
    }*/


}