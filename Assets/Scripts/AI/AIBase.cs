using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class AIBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected EnemyBehavior enemyBehavior;
    [SerializeField] protected float stopDistance = 0.1f;
    protected Animator anim;
  
    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.stoppingDistance = stopDistance;
    }

   
    void Update()
    {
        
    }
}
