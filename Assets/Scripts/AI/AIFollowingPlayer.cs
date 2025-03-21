using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIFollowingPlayer : AIBase
{
    
    protected override void Start()
    {
        base.Start();
       
    }
    void Update()
    {
        agent.SetDestination(enemyBehavior.target.position);
    }
}
