using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Behavior : MonoBehaviour, ITargeteable
{
    protected CharacterController ch_Controller;
    protected Animator animator;

    [SerializeField] private float maxLife = 100f;
    private float currentLife = 0f;
    protected bool isDead = false;

    void Start()
    {
        ch_Controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if(isDead) return;

        if (currentLife < 0) isDead = true;
    }
}
