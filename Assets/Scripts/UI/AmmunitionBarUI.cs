using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmunitionBarUI : MonoBehaviour
{
    [SerializeField] private Image ammunitionBar;   // La imagen de la barra de vida.

    public void UpdateAmmunitionBar(PlayerBehavior character)
    {
        var currentAmmunition = character.CurrentAmmunition;
        var maxAmmunition = character.MaxAmmunition;
        var ammunitionPercent = Mathf.Clamp(currentAmmunition / maxAmmunition, 0, 1);
        ammunitionBar.fillAmount = ammunitionPercent;
    }
}
