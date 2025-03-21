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
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float normalSpeed = 3f;
    private NavMeshAgent agent;
    private Animator animator;
    private float timeNextAttack;
    private AIEnemyVision checkVision;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        aiStates = GetComponents<AIBase>();
        sphereCollider = GetComponent<SphereCollider>();
        animator = GetComponent<Animator>();
        checkVision = GetComponent<AIEnemyVision>();

        // Set initial state
        ChangeState(EnemyState.FollowingPath);
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
            agent.speed = runSpeed;
        }
        else if (state == EnemyState.FollowingPath)
        {
            UpdateFollowingPath();
            agent.speed = normalSpeed;
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
        if (checkVision.CheckVisionToPlayer())
        {
            ChangeState(EnemyState.FollowingPlayer);
        }
        animator.SetFloat("speed", agent.speed);
    }

    private void UpdateFollowingPlayer()
    {
        if (!checkVision.CheckVisionToPlayer()) return;
        // Si está a la distancia de ataque, cambia al estado de ataque.
        if (checkVision.CheckAttackDistance(attackDistance))
        {
            ChangeState(EnemyState.Attack);
        }

        // Si el jugador ya no es visible, pasa al estado de FollowingPath.
        if (checkVision.CheckVisionToPlayer())
        {
            Debug.Log("Jugador a la vista: RESET PATH");
            agent.ResetPath();
            ChangeState(EnemyState.FollowingPath);
        }
       

    }

    private void UpdateFollowingPath()
    {
        // Si el jugador es visible, pasa al estado de FollowingPlayer.
        if (!checkVision.CheckVisionToPlayer()) return;
        ChangeState(EnemyState.FollowingPlayer);
        
    }

    private void UpdateAttack()
    {
        if (!checkVision.CheckAttackDistance(attackDistance))
        {
            ChangeState(EnemyState.FollowingPlayer); // Si está fuera del rango de ataque, sigue al jugador.
        }
        else
        {
            // Ataque al jugador.
            transform.LookAt(target.position);
            timeNextAttack -= Time.deltaTime;

            if (timeNextAttack <= 0)
            {
                if (Random.Range(0.0f, 1.0f) < strongAttackChance)
                {
                    // Animación de ataque fuerte.
                }
                else
                {
                    // Animación de ataque normal.
                }
                timeNextAttack = timeBetweenAttacks;
            }
        }
    }

    private void UpdateJump()
    {
        // Aquí podrías implementar el comportamiento para saltar, si es necesario.
        if (checkVision.CheckVisionToPlayer())
        {
            ChangeState(EnemyState.FollowingPlayer);
        }
    }

    public void ChangeState(EnemyState newState)
    {
        state = newState;

        // Activa el comportamiento correspondiente al nuevo estado.
        for (int i = 0; i < aiStates.Length; i++)
        {
            aiStates[i].enabled = i == (int)state;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ITargeteable>() == null) return;
        target = other.transform;
    }
}

