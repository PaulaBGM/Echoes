using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyState
{
    Wandering,
    FollowingPlayer,
    FollowingPath,
    Attack,
    Jump,
}
[Serializable, RequireComponent(typeof(SphereCollider))]
public class EnemyBehavior : MonoBehaviour
{
    [Header("Current State")]
    [SerializeField] private EnemyState state;
    [field: SerializeField] public Transform target { get; private set; }
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private float detectionDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private AIBase[] aiStates;
    [SerializeField] private float timeBetweenAttacks = 2.5f;
    [SerializeField] private float strongAttackChance = 0.3f;
    [SerializeField] private float wanderingTime = 5f; // Tiempo en segundos antes de cambiar de estado
    private float currentWanderingTime = 0f; // Tiempo transcurrido en estado Wandering
    private float runSpeed = 5f;
    private float normalSpeed = 3f;
    private float idleSpeed = 0f;
    private NavMeshAgent agent;
    private Animator animator;
    private float timeNextAttack;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        aiStates = GetComponents<AIBase>();
        sphereCollider = GetComponent<SphereCollider>();
        animator = GetComponent<Animator>();
        sphereCollider.radius = detectionDistance;
        state = EnemyState.Wandering;
        
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
        if (!PlayerIsOnRange(detectionDistance))
        {
            agent.speed = idleSpeed;
            // Contamos el tiempo que lleva vagando
            currentWanderingTime += Time.deltaTime;

            // Si supera el tiempo definido, cambia a FollowingPath
            if (currentWanderingTime >= wanderingTime)
            {
                agent.speed = normalSpeed;
                ChangeState(EnemyState.FollowingPath);
                currentWanderingTime = 0f; // Reseteamos el contador
            }

            return;
        }

        ChangeState(EnemyState.FollowingPlayer);
        currentWanderingTime = 0f;
        
        animator.SetFloat("zSpeed", agent.speed);
    }

    private void UpdateFollowingPath()
    {
        if (!PlayerIsOnRange(detectionDistance)) return;
        if (PlayerIsOnRange(attackDistance))
        {
            ChangeState(EnemyState.Attack);
        }
        else
        {
            agent.ResetPath();
            ChangeState(EnemyState.FollowingPlayer);
        }
        
        animator.SetFloat("zSpeed", agent.speed);
    }

    private void UpdateFollowingPlayer()
    {
        agent.speed = runSpeed;
        if (PlayerIsOnRange(detectionDistance)) return;
        ChangeState(EnemyState.Wandering);
    }
    private void UpdateAttack()
    {
        if (!PlayerIsOnRange(attackDistance))
        {
            ChangeState(EnemyState.FollowingPlayer);
        }
        else
        {
            transform.LookAt(target.position);
            timeNextAttack -= Time.deltaTime;
            if (timeNextAttack <= 0)
            {
                if (Random.Range(0.0f, 1.0f) < strongAttackChance)
                {
                    //animacion strong attack
                }
                else
                {
                    //animacion normal attack
                }
                timeNextAttack = timeBetweenAttacks;
            }
        }
    }
    private void UpdateJump()
    {
        if (!PlayerIsOnRange(detectionDistance)) return;
        ChangeState(EnemyState.FollowingPlayer);
    }

    private void ChangeState(EnemyState newState)
    {
        state = newState;

        for (int i = 0; i < aiStates.Length; i++)
        {
            aiStates[i].enabled = i == (int)state;
        }
    }

    private bool PlayerIsOnRange(float range)
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