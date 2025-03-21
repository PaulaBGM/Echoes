using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerUp : PowerUpBase
{
    [SerializeField] private int healthAmount = 10;

    [Header("Oscillation Settings")]
    [SerializeField] private float oscillationAmplitude = 0.5f; // Cuánto se moverá hacia arriba y hacia abajo.
    [SerializeField] private float oscillationSpeed = 1f; // La velocidad de la oscilación.

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 30f; // Velocidad de rotación.

    private Vector3 initialPosition;


    private void Start()
    {
        // Guardamos la posición inicial del objeto.
        initialPosition = transform.position;
    }

    private void Update()
    {
        OscilationMovement();
    }

    private void OscilationMovement()
    {
        // Oscilación hacia arriba y abajo usando la función Mathf.Sin
        float yOffset = Mathf.Sin(Time.time * oscillationSpeed) * oscillationAmplitude;
        transform.position = new Vector3(initialPosition.x, initialPosition.y + yOffset, initialPosition.z);

        // Rotación del objeto en el eje Y (puedes cambiarlo al eje que prefieras)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    protected override void ApplyEffect(GameObject player)
    {
        PlayerBehavior playerHealth = player.GetComponent<PlayerBehavior>();

        if (playerHealth != null)
        {
            playerHealth.Heal(healthAmount);
        }
    }
}
