using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public Transform detectedPlayer = null;
    public Transform detectedPredator = null;


    public int radius;
    [Range(0, 360)]
    public int angle;

    public GameObject playerRef;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public LayerMask playerMask;
    public LayerMask predatorMask;

    public bool canSeeFood;
    public bool canSeePlayer;
    public bool canSeePredator;

    //private float timeSinceLastSeen = 0f;
    public bool isChasing = false;

    public bool isBeingChased = false;
    private void Start()
    {

        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
            //playerFieldOfViewCheck();
            predatorFieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {


        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;


            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {

                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeeFood = true;
                }

                else
                {
                    canSeeFood = false;

                }
            }
            else
                canSeeFood = false;
        }
        else if (canSeeFood)
            canSeeFood = false;



    }
    private void playerFieldOfViewCheck()
    {
        if (isChasing) return;

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, playerMask);
        List<Collider> filteredRangeChecks = new List<Collider>(rangeChecks);

        filteredRangeChecks.RemoveAll(collider => collider.transform == this.transform);

        if (filteredRangeChecks.Count != 0)
        {

            Transform target = filteredRangeChecks[0].transform;

            if (target.tag == "Player")
            {
                detectedPlayer = target;
                canSeePlayer = false;
                detectedPredator = null;
            }
            if (target.tag == "Predator")
            {
                detectedPredator = target;
            }



            Debug.Log("Players detected: " + filteredRangeChecks.Count);


            Vector3 directionToTarget = (target.position - transform.position).normalized;



            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {

                float distanceToTarget = Vector3.Distance(transform.position, target.position);


                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    Debug.Log("Player seen: " + target.name);
                    canSeePlayer = true;

                }

                else
                {
                    Debug.Log("Player obstructed: " + target.name);
                    canSeePlayer = false;

                }

            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;

    }

    private void predatorFieldOfViewCheck()
    {

        if (canSeePlayer) return;

        detectedPredator = null;


        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, predatorMask);
        List<Collider> filteredRangeChecks = new List<Collider>(rangeChecks);

        filteredRangeChecks.RemoveAll(collider => collider.transform == this.transform);

        if (filteredRangeChecks.Count != 0)
        {

            Transform target = filteredRangeChecks[0].transform;
            detectedPredator = target;
            //Debug.Log("Players detected: " + filteredRangeChecks.Count);


            Vector3 directionToTarget = (target.position - transform.position).normalized;



            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {

                float distanceToTarget = Vector3.Distance(transform.position, target.position);


                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    //Debug.Log("Player seen: " + target.name);
                    canSeePredator = true;
                    isBeingChased = true;

                }

                else
                {
                    Debug.Log("Player obstructed: " + target.name);
                    canSeePredator = false;
                    isBeingChased = false;


                }

            }
            else
            {
                canSeePredator = false;
                detectedPredator = null;
                isBeingChased = false;

            }


        }
        else if (canSeePredator)
        {
            canSeePredator = false;
            isBeingChased = false;
        }
          
    }
}



