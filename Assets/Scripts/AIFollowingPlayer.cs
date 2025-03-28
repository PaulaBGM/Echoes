using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIFollowingPlayer : AIBase
{
    void Update()
    {
        agent.SetDestination(enemyBehavior.target.position);
    }
}
