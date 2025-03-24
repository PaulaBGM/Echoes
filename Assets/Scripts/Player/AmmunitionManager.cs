using UnityEngine;
using UnityEngine.TextCore.Text;

public class AmmunitionManager : MonoBehaviour
{
    [SerializeField] private int maxAmmunition = 100;  // Munición máxima
    [SerializeField] private AmmunitionBarUI ammunitionBar;
    private PlayerBehavior character;
    private int currentAmmunition;

    public int CurrentAmmunition { get => currentAmmunition; }
    public int MaxAmmunition { get => maxAmmunition; }

    // Inicializamos las municiones al inicio
    private void Start()
    {
        currentAmmunition = 0;
        ammunitionBar.UpdateAmmunitionBar(this);
    }

    private void Update()
    {
        Debug.Log("CURRENT AMMUNITION: " + currentAmmunition);
    }

    // Método para restar munición
    public bool UseAmmo(int amount)
    {
        if (currentAmmunition >= amount)
        {
            currentAmmunition -= amount;
            ammunitionBar.UpdateAmmunitionBar(this);
            return true;  // Se pudo disparar
        }

        return false;  // No hay suficiente munición
    }

    // Método para recargar munición
    public void RechargeAmmo(int amount)
    {
        currentAmmunition += amount;

        ammunitionBar.UpdateAmmunitionBar(this);

        if (currentAmmunition > maxAmmunition)
        {
            currentAmmunition = maxAmmunition;  // No exceder el máximo
        }
    }
}

