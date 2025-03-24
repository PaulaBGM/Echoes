using System.Collections;
using UnityEngine;

public class AIWandering : AIBase
{
    [SerializeField] private float wanderRadius;
    [SerializeField] private float waitTime = 2f; // Tiempo de espera antes de moverse
    private Vector3 initialPosition;
    private Vector3 randomEndPoint;
    private bool isWaiting = false;

    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position;
        StartCoroutine(WaitBeforeMoving());
        SetNewRandomPoint();
    }

    void Update()
    {
        if (!agent.enabled || isWaiting || enemyBehavior.IsDead) return;
        
    }

    private IEnumerator WaitBeforeMoving()
    {
        isWaiting = true;
        anim.SetTrigger("Wandering"); // Mantener la animación de "Wandering"
        yield return new WaitForSeconds(waitTime); // Esperar unos segundos
        SetNewRandomPoint();
        isWaiting = false;
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
