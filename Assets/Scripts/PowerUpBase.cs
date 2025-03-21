using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PowerUpBase : MonoBehaviour
{
    protected virtual void ApplyEffect(GameObject player) { }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ApplyEffect(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
