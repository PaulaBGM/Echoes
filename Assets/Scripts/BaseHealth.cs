using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHealth: MonoBehaviour, IDamagable
{
    protected Animator animator;

    [SerializeField] private LifeBarUi lifeBarUI;
    [SerializeField] private float maxLife = 100f;
    [SerializeField] private float damageCooldown;
    private float currentLife = 0f;
    protected bool isDead = false;
    private bool canTakeDamage;

    public Animator Animator { get => animator; }
    public bool IsDead { get => isDead; }
    public float CurrentLife { get => currentLife; }
    public float MaxLife { get => maxLife; }

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        currentLife = maxLife;
        canTakeDamage = true;
    }

    public void ApplyDamage(float damage)
    {
        Debug.Log($"Recibiendo daño: {damage}");
        Debug.Log($"Vida antes de daño: {currentLife}");

        if (currentLife <= 0 && !canTakeDamage)
        {
            Debug.Log("No se puede recibir daño en este momento.");
            return;
        }

        currentLife -= damage;
        Debug.Log($"Vida después del daño: {currentLife}");
        lifeBarUI.UpdateLifeBar(this);

        if (currentLife <= 0)
        {
            Debug.Log("Enemigo muerto.");
            Die();
        }
        else
        {
            StartCoroutine(DamageCoolDown());
        }
    }

    public void Heal(float heal) //Cura según el valor del objeto recogido.
    {
        if (currentLife <= 0) return;

        currentLife += heal;
        lifeBarUI.UpdateLifeBar(this);

        if (currentLife>maxLife)
        {
            currentLife = maxLife;
        }
    }

    private IEnumerator DamageCoolDown() //Corrutina que impide volver a recibir daño durante unos segundos.
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canTakeDamage = true;
    }

    protected abstract void Die(); //Se llama desde el script del jugador o enemigo.
}
