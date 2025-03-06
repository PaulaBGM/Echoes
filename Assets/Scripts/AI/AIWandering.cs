using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWandering : AIBase
{
    [SerializeField] private float wanderRadius;
    private Vector3 initialPosition;
    private Vector3 randomEndPoint;

    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position;  
        SetNewRandomPoint();
    }

    void Update()
    {
        if (!agent.enabled) return;
        if (agent.remainingDistance < stopDistance && !agent.pathPending) 
        {
            SetNewRandomPoint();
        }
    }
    private void SetNewRandomPoint() 
    {
        randomEndPoint = initialPosition + Random.insideUnitSphere * wanderRadius;
        randomEndPoint.y = 0;
        agent.SetDestination(randomEndPoint);
    }
    private void OnDrawGizmos()
    {
        if (agent != null)
        {
            Gizmos.DrawWireSphere(randomEndPoint, wanderRadius);
            Gizmos.DrawWireSphere(agent.destination, wanderRadius);
        }
    }
}
