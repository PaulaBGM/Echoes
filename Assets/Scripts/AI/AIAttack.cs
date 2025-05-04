using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAttack : AIBase
{
    [SerializeField] private Transform[] repositionPoints;
    [SerializeField] private float timeBetweenAttacks = 2.5f;
    [SerializeField] private float strongAttackChance = 0.3f;

    public bool isAttacking = false;
    private float timeNextAttack;

    protected override void Start()
    {
        base.Start();
        timeNextAttack = timeBetweenAttacks;
    }

    void Update()
    {
        
    }

    public void AttackTimer(Transform target, Animator animator)
    {
        anim = animator;
        transform.LookAt(target.position);
        timeNextAttack -= Time.deltaTime;
        if (timeNextAttack <= 0)
        {
            isAttacking = true;

            if (Random.Range(0.0f, 1.0f) < strongAttackChance)
            {
                // Animación de ataque fuerte
            }
            else
            {
                animator.SetBool("NormalAttack", true);
            }
            timeNextAttack = timeBetweenAttacks;
        }
    }

    // Método llamado por el evento de la animación al final
    public void ResetAttack()
    {
        anim.SetBool("NormalAttack", false);
        isAttacking = false;
        //RepositionForAttack();
    }

    /*private void RepositionForAttack()
    {
        if (!isAttacking)
        {
            // Obtener el NavMeshAgent
            NavMeshAgent agent = GetComponent<NavMeshAgent>();

            // Asegurarse de que el agente no está parado
            agent.isStopped = false;

            // Verificar que tengamos puntos de reposición
            if (repositionPoints.Length > 0)
            {
                // Elegir un punto de reposición aleatorio
                Transform randomRepositionPoint = repositionPoints[Random.Range(0, repositionPoints.Length)];

                // Mover al enemigo hacia el punto de reposición
                agent.SetDestination(randomRepositionPoint.position);
            }
            else
            {
                // Si no hay puntos de reposición, mover a una posición predeterminada (puedes ajustar esto)
                agent.SetDestination(transform.position + transform.forward * 5f); // Ejemplo simple
            }
        }
    }*/
}
