using System;
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
public class EnemyBehavior : BaseHealth
{
    [Header("Current State")]
    [SerializeField] private EnemyState state;
    [SerializeField] private GameObject colliderShoot; //Collider que detecta los disparos
    [field: SerializeField] public Transform target { get; private set; }
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private float detectionDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private AIBase[] aiStates;
    
    [SerializeField] private float wanderingTime = 5f; // Tiempo en segundos antes de cambiar de estado
    private float currentWanderingTime = 0f; // Tiempo transcurrido en estado Wandering
    private float runSpeed = 5f;
    private float normalSpeed = 3f;
    private float idleSpeed = 0f;
    private NavMeshAgent agent;

    private AIEnemyVision enemyVision;
    private AIAttack enemyAttack;
    private bool killPlayer;

    protected override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        aiStates = GetComponents<AIBase>();
        sphereCollider = GetComponent<SphereCollider>();
        animator = GetComponent<Animator>();
        enemyVision = GetComponent<AIEnemyVision>();
        enemyAttack = GetComponentInChildren<AIAttack>();
        sphereCollider.radius = detectionDistance;
        state = EnemyState.Wandering;
        colliderShoot.SetActive(true);
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
        if (!enemyVision.CheckVisionToPlayer())
        {
            agent.speed = idleSpeed;
            currentWanderingTime += Time.deltaTime;

            if (currentWanderingTime >= wanderingTime)
            {
                agent.speed = normalSpeed;
                ChangeState(EnemyState.FollowingPath);
                currentWanderingTime = 0f;
            }

            return;
        }

        if (enemyVision.CheckAttackDistance(attackDistance))
        {
            ChangeState(EnemyState.Attack);
        }

        ChangeState(EnemyState.FollowingPlayer);
        currentWanderingTime = 0f;

        animator.SetFloat("zSpeed", agent.speed);
    }

    private void UpdateFollowingPath()
    {
        if (!enemyVision.CheckVisionToPlayer()) return;
        if (enemyVision.CheckAttackDistance(attackDistance))
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
        animator.SetFloat("zSpeed", agent.speed);
        if (!enemyVision.CheckVisionToPlayer())
        {
            ChangeState(EnemyState.Wandering);
        }

        if (enemyVision.CheckAttackDistance(attackDistance))
        {
            ChangeState(EnemyState.Attack);
        }
    }

    private void UpdateAttack()
    {
        if (!enemyVision.CheckAttackDistance(attackDistance))
        {
            agent.isStopped = false; // Permite que el enemigo vuelva a moverse
            ChangeState(EnemyState.FollowingPlayer);
        }
        else
        {
            agent.isStopped = true; // Detiene el movimiento al atacar
            agent.velocity = Vector3.zero;
            animator.SetFloat("zSpeed", 0); // Evita que se active la animación de caminar/correr
            enemyAttack.AttackTimer(target, animator);
        }
    }

    private void UpdateJump()
    {
        if (!enemyVision.CheckVisionToPlayer()) return;
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

    protected override void Die()
    {
        isDead = true;
        animator.SetTrigger("Dying");
        agent.speed = 0;  // Establece la velocidad a 0
        agent.isStopped = true;  // Detén el agente
        colliderShoot.SetActive(false);
    }

    public void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ITargeteable>() == null) return;
        target = other.transform;

        if(other.GetComponent<PlayerBehavior>().IsDead)
        {
            killPlayer = true;
            animator.SetBool("KillPlayer", true);
        }
    }
}
