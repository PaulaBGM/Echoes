using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemyVision : MonoBehaviour
{
    [SerializeField] private Transform enemyHead;

    private SphereCollider colliderDetection;
    private bool playerInRadio = false;
    private bool raycastHitChest = false;

    private Transform transformPlayer;
    private Transform chestPlayer;
    private float angle;
   
    

    void Start()
    {
        colliderDetection = GetComponent<SphereCollider>();
    }

    void Update()
    {
        CheckVisionCone();
        Debug.Log(CheckVisionToPlayer());
        CheckRaycast();
        Debug.Log("jugador en radio: " + playerInRadio);
        Debug.Log("raycast: " + raycastHitChest);
    }

    public bool CheckVisionToPlayer()
    {
        return angle <= 30f && playerInRadio && raycastHitChest;
    }
    public bool CheckAttackDistance(float attackDistance)
    {
        if (transformPlayer == null) return false; // Evita el error si no hay jugador detectado
        var sqrDistance = (transformPlayer.position - transform.position).sqrMagnitude;
        return sqrDistance <= Mathf.Pow(attackDistance, 2);
    }
    private void CheckVisionCone()
    {
        if (transformPlayer == null) return; // Evita errores si el jugador no ha sido detectado aún

        Vector3 directionToPlayer = (transformPlayer.position - transform.position).normalized;
        angle = Vector3.Angle(transform.forward, directionToPlayer);
    }

    private void CheckRaycast()
    {
        if (transformPlayer == null || chestPlayer == null) return;

        Vector3 origin = enemyHead.position;
        Vector3 destiny = chestPlayer.position;
        Vector3 direction = (destiny - origin).normalized;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit))
        {
            Debug.DrawLine(origin, hit.point, Color.red, 0.1f);

            if (hit.collider.CompareTag("Player"))
            {
                raycastHitChest = true;
            }
            else
            {
                raycastHitChest = false;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRadio = true;
            transformPlayer = other.transform;
            chestPlayer = other.GetComponent<PlayerBehavior>().chestBone;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRadio = false;
            transformPlayer = null;
            chestPlayer = null;
        }
    }
}

