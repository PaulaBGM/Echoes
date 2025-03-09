using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Behavior : MonoBehaviour, ITargeteable
{
    protected CharacterController ch_Controller;
    protected Animator animator;

    private bool shortWeapon = true;

    void Start()
    {
        ch_Controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if(shortWeapon)
        {
            animator.SetBool("longWeapon", shortWeapon);
        }
    }
}
