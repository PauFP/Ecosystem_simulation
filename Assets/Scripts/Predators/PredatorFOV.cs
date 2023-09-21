using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorFOV : MonoBehaviour
{
    public Transform detectedPlayer = null;
    public Transform detectedPredator = null;


    public int radius;
    [Range(0, 360)]
    public float angle;

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
            playerFieldOfViewCheck();
        }
    }

    
    private void playerFieldOfViewCheck()
    {
        //if (isChasing) return;

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, playerMask);
        List<Collider> filteredRangeChecks = new List<Collider>(rangeChecks);

        filteredRangeChecks.RemoveAll(collider => collider.transform == this.transform);

        if (filteredRangeChecks.Count != 0)
        {

            Transform target = filteredRangeChecks[0].transform;

            if (target.tag == "Player")
            {
                detectedPlayer = target;
                canSeePlayer = true;
                detectedPredator = null;
            }
            


            //Debug.Log("Players detected: " + filteredRangeChecks.Count);


            Vector3 directionToTarget = (target.position - transform.position).normalized;



            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {

                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    //Debug.Log("Player seen: " + target.name);
                    canSeePlayer = true;

                }

                else
                {
                    //Debug.Log("Player obstructed: " + target.name);
                    canSeePlayer = false;

                }

            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;

    }


}



