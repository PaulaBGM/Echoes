using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmmunitionBarUI : MonoBehaviour
{
    [SerializeField] private Image ammunitionBar;   // La imagen de la barra de munición
    [SerializeField] private Text percentageText;    // El texto donde se mostrará el porcentaje
    [SerializeField] private float smoothSpeed = 5f;  // Velocidad de transición

    private Coroutine smoothUpdateCoroutine;

    public void UpdateAmmunitionBar(AmmunitionManager ammunition)
    {
        // Calcular el porcentaje de munición
        float targetFillAmount = Mathf.Clamp01((float)ammunition.CurrentAmmunition / ammunition.MaxAmmunition);

        // Actualizar la barra de munición con suavizado
        if (smoothUpdateCoroutine != null)
        {
            StopCoroutine(smoothUpdateCoroutine);
        }

        smoothUpdateCoroutine = StartCoroutine(SmoothFillChange(targetFillAmount));

        // Actualizar el texto del porcentaje
        UpdatePercentageText(ammunition);
    }

    private IEnumerator SmoothFillChange(float targetFillAmount)
    {
        float startFill = ammunitionBar.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < 1f / smoothSpeed)
        {
            elapsedTime += Time.deltaTime;
            ammunitionBar.fillAmount = Mathf.Lerp(startFill, targetFillAmount, elapsedTime * smoothSpeed);
            yield return null;
        }

        ammunitionBar.fillAmount = targetFillAmount; // Asegurar que llega al valor exacto
    }

    // Actualiza el texto del porcentaje de balas
    private void UpdatePercentageText(AmmunitionManager ammunition)
    {
        // Calculamos el porcentaje de munición restante
        float percentage = (float)ammunition.CurrentAmmunition / ammunition.MaxAmmunition * 100f;

        // Actualizar el texto con el porcentaje (redondeado a un número entero)
        percentageText.text = Mathf.RoundToInt(percentage).ToString();
    }
}
