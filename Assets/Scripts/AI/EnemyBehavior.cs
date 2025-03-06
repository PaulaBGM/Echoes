using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState 
{
    Wandering,
    FollowingPlayer,
    FollowingPath,
    Attack,
    Jump,
}
public class EnemyBehavior : MonoBehaviour
{
    [Header("Current States")]
    [SerializeField] private EnemyState state;
    [field: SerializeField] public Transform target { get; private set; }
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private float detectionDistance;
    [SerializeField] private AIBase[] aiStates;
    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        aiStates = GetComponents<AIBase>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        capsuleCollider.radius = detectionDistance;
    }

    
    void Update()
    {
        if (state == EnemyState.Wandering)
        {
            UpdateWandering();
        }
        else if (state == EnemyState.FollowingPlayer)
        {
            UpdateFollowingPlayer();
        }
        else if (state == EnemyState.FollowingPath)
        {
            UpdateFollowingPath();
        }
        else if (state == EnemyState.Attack)
        {
            UpdateAttack();
        }
        else if (state == EnemyState.Jump) 
        { 
            UpdateJump();
        }
    }
    private void UpdateWandering() 
    {
        if (!PlayerIsOnRange(detectionDistance)) return;
        ChangeState(EnemyState.FollowingPlayer);

        animator.SetFloat("speed",agent.speed);
    }

    private void UpdateFollowingPlayer() 
    {
        if (!PlayerIsOnRange(detectionDistance)) return;
        ChangeState(EnemyState.FollowingPlayer);
    }

    private void UpdateFollowingPath()
    {
        if (!PlayerIsOnRange(detectionDistance)) return;
        ChangeState(EnemyState.FollowingPlayer);
    }
    private void UpdateAttack()
    {
        if (!PlayerIsOnRange(detectionDistance)) return;
        ChangeState(EnemyState.FollowingPlayer);
    }
    private void UpdateJump() 
    {
        if (!PlayerIsOnRange(detectionDistance)) return;
        ChangeState(EnemyState.FollowingPlayer);
    }

    private void ChangeState(EnemyState newState) 
    { 
        state = newState;

        for (int i = 0; i < aiStates.Length; i++) {
            aiStates[i].enabled = i == (int)state; 
        }
    }

    private bool PlayerIsOnRange (float range) 
    {
        var sqrDistance = (target.position - transform.position).sqrMagnitude;
        return sqrDistance <= Mathf.Pow(range, 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ITargeteable>() == null) return;
        target = other.transform;
    }

}
