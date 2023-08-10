using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Movement_final : MonoBehaviour
{

    public enum AgentState
    {
        Wander,
        ChaseFood,
        ChaseAgent
    }

    public AgentState currentState = AgentState.Wander;

    public float turnSpeed = 0.01f;
    public float movementSpeed;
    Quaternion qTo;
    public FieldOfView fov;
    public int EatenFood;
    Quaternion rotGoal;

    private Vector3? currentTarget = null; // Usamos un Vector3 nullable para representar el objetivo

    private void Start()
    {
        fov = GetComponent<FieldOfView>();
        InvokeRepeating("ChangeRotation", 1f, Random.Range(1, 5));
    }

    void ChangeRotation()
    {
        qTo = Quaternion.Euler(new Vector3(0, Random.Range(-180, 180), 0));
    }

    private void Update()
    {
        switch (currentState)
        {
            case AgentState.Wander:
                if (fov.canSeePlayer)
                {
                    Debug.Log(gameObject.name + " ha detectado a otro agente.");

                    ChangeState(AgentState.ChaseAgent);
                }
                else if (fov.canSeeFood)
                {
                    ChangeState(AgentState.ChaseFood);
                }
                else
                {
                    Movement();
                }
                break;

            case AgentState.ChaseAgent:
                if (!fov.canSeePlayer || (currentTarget.HasValue && Vector3.Distance(transform.position, currentTarget.Value) <= 0.1f))
                {
                    // Has alcanzado al agente o ya no lo ves. 
                    currentTarget = null;
                    ChangeState(AgentState.Wander);
                }
                else
                {
                    MoveToPlayer();
                }
                break;

            case AgentState.ChaseFood:
                if (!fov.canSeeFood || (currentTarget.HasValue && Vector3.Distance(transform.position, currentTarget.Value) <= 0.1f))
                {
                    // Has alcanzado la comida o ya no la ves.
                    currentTarget = null;
                    ChangeState(AgentState.Wander);
                }
                else
                {
                    MoveToFood();
                }
                break;
        }
    }

    void ChangeState(AgentState newState)
    {
        if (currentState != newState)
        {
            if (newState == AgentState.Wander)
            {
                InvokeRepeating("ChangeRotation", 1f, Random.Range(1, 5));
            }
            else
            {
                print("Cancelado");
                CancelInvoke("ChangeRotation");
            }

            currentState = newState;
        }
    }

    void Movement()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, qTo, turnSpeed);
        transform.position += transform.forward * movementSpeed * Time.deltaTime;
    }

    void MoveToPlayer()
    {
        // Nota: Aquí hemos movido la lógica de detección del jugador fuera del bloque condicional,
        // para que siempre estemos comprobando y actualizando la posición del jugador.
        foreach (Vector3 pos in Spawner.playerPositions)
        {
            Vector3 directionToPos = (pos - fov.transform.position).normalized;
            if (Vector3.Angle(fov.transform.forward, directionToPos) < fov.angle / 2)
            {
                float mod_radius = fov.radius + 5;
                if (Vector3.Distance(fov.transform.position, pos) <= mod_radius)
                {
                    currentTarget = pos; // Actualiza el objetivo
                    break;
                }
            }
        }

        if (currentTarget.HasValue)
        {
            GoTowards(currentTarget.Value);
        }
    }


    void MoveToFood()
    {
        if (currentTarget.HasValue)
        {
            GoTowards(currentTarget.Value);
            return;
        }

        foreach (Vector3 pos in Spawner.foodPositions)
        {
            Vector3 directionToPos = (pos - fov.transform.position).normalized;
            if (Vector3.Angle(fov.transform.forward, directionToPos) < fov.angle / 2)
            {
                float mod_radius = fov.radius + 2;
                if (Vector3.Distance(fov.transform.position, pos) <= mod_radius)
                {
                    currentTarget = pos; // Fija el objetivo
                    GoTowards(pos);
                    break;
                }
            }
        }
    }
    /*void GoTowards(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        if (directionToTarget != Vector3.zero)
        {
            rotGoal = Quaternion.LookRotation(directionToTarget, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnSpeed);
        }
        transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
    }*/

    void GoTowards(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;

        if (directionToTarget!= Vector3.zero)
        {
            rotGoal = Quaternion.LookRotation(directionToTarget, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotGoal, turnSpeed);
        }
        transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);

    }

}

