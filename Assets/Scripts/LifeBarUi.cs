using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LifeBarUi : MonoBehaviour
{
    [SerializeField] private Image lifeBar;   // La imagen de la barra de vida.
    private Transform cameraTransform;        // Para acceder a la posición de la cámara.
    private Canvas canvas;                    // Para acceder al Canvas.

    void Start()
    {
        // Obtener la referencia al Canvas
        canvas = GetComponent<Canvas>();
        cameraTransform = Camera.main?.transform;  // Asignar la cámara principal.
    }

    void Update()
    {
        // Solo si la barra de vida es parte del Canvas en el espacio mundial (World Space)
        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            Vector3 directionToCamera = cameraTransform.position - transform.position;

            // Asegurarse de que la dirección esté nivelada en el plano XY
            directionToCamera.y = 0; // Elimina la rotación sobre el eje Y para evitar que se incline

            // Solo actualizar la rotación si la dirección es válida
            if (directionToCamera.sqrMagnitude > 0.01f)
            {
                // Ajustamos la rotación de la barra para que mire hacia la cámara
                transform.rotation = Quaternion.LookRotation(directionToCamera);
            }
        }
    }

    public void UpdateLifeBar(BaseHealth character)
    {
        var currentLife = character.CurrentLife;
        var maxLife = character.MaxLife;
        var lifePercent = Mathf.Clamp(currentLife / maxLife, 0, 1);
        lifeBar.fillAmount = lifePercent;
    }
}
