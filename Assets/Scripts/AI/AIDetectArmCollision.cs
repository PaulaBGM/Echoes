using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDetectArmCollision : MonoBehaviour
{
    [SerializeField] private float damage = 15f;

    private AIAttack enemyAttack;

    void Start()
    {
        enemyAttack = GetComponentInParent<AIAttack>();   
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && enemyAttack.isAttacking)
        {
            var damageable = other.gameObject.GetComponentInParent<IDamagable>();
            damageable.ApplyDamage(damage);
        }
    }
}
