using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : BaseHealth, ITargeteable
{
    protected CharacterController ch_Controller;
    private PlayerMove playerMove;
    [SerializeField] private WeaponController longWeaponController, shortWeaponController;
    [SerializeField] private float delayOpenGameOver;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private AmmunitionBarUI ammunitionBarUI;
    private GetWeapon getWeapon;
    public Transform chestBone;

    // Se añade un delegado para notificar al UI
    public delegate void OnGameOverHandler();
    public static event OnGameOverHandler OnGameOver;

    [SerializeField] private int maxAmmunition;
    private int currentAmmunition;
    public float CurrentAmmunition { get => currentAmmunition; }
    public float MaxAmmunition { get => maxAmmunition; }

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

    /*private void ShootAnimation()
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
    }*/

    protected override void Die()
    {
        Debug.Log("MUERTA");
        animator.SetBool("isDeath", true);

        // Dispara el evento de "Game Over" para que la UI lo gestione
        OnGameOver?.Invoke();
    }

    public void AmunnitionRecharge(int amount)
    {
        if (currentAmmunition <= 0) return;

        currentAmmunition += amount;
        ammunitionBarUI.UpdateAmmunitionBar(this);

        if (currentAmmunition > maxAmmunition)
        {
            currentAmmunition = maxAmmunition;
        }
    }
}
