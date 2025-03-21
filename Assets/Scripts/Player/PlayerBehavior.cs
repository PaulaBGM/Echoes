using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : BaseHealth, ITargeteable
{
    protected CharacterController ch_Controller;
    private PlayerMove playerMove;
    [SerializeField] private WeaponController longWeaponController, shortWeaponController;
    private GetWeapon getWeapon;
    public Transform chestBone;

    protected override void Start()
    {
        base.Start();

        playerMove = GetComponent<PlayerMove>();
        getWeapon = GetComponent<GetWeapon>();
    }

    private void Update()
    {
        //ShootAnimation();

        if(getWeapon.hasPistol)
        {
            playerMove.canLongIddle = false;
            animator.SetBool("hasPistol", getWeapon.hasPistol);
        }
        else
        {
            playerMove.canLongIddle = true;
            animator.SetBool("hasPistol", getWeapon.hasPistol);
        }
    }

    private void ShootAnimation()
    {
        if (longWeaponController.isShooting)
        {
            animator.SetBool("shootingLongWeapon", true);
        }
        else if (shortWeaponController.isShooting)
        {
            animator.SetBool("shootingPistol", true);
        }
        else
        {
            animator.SetBool("shootingPistol", false);
            animator.SetBool("shootingLongWeapon", false);
        }
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }
}
