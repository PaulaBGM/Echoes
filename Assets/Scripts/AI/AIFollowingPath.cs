using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIFollowingPath : AIBase
{
    [SerializeField] private Transform[] wayPoints;
    private int currentPathIndex = 0;
    private int direction = 1;
    protected override void Start()
    {
        base.Start();

        GoToNextPoint();
    }

    private void Update()
    {
        if (agent.remainingDistance < stopDistance && !agent.pathPending) 
        {
            GoToNextPoint();
        }
    }

    private void GoToNextPoint() 
    {
        agent.SetDestination(wayPoints[currentPathIndex].position);
        currentPathIndex += direction;
        if (currentPathIndex >= wayPoints.Length) { 
            direction = -1;
            currentPathIndex = wayPoints.Length-2; 
        }
        else if (currentPathIndex < 0) 
        { 
          direction = 1;  
          currentPathIndex = 1;
        }

    }
}
