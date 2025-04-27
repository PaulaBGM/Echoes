using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavBarController : MonoBehaviour
{
    public static NavBarController Instance { get; private set; }

    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private RawImage compassImage;
    [SerializeField] private Transform player;

    private List<QuestMarker> questMarkers = new List<QuestMarker>();

    private float maxDistance = 200f;
    private float compassUnit;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        compassUnit = compassImage.rectTransform.rect.width / 360f; // Relación entre el tamaño del compás y 360 grados
    }

    private void Update()
    {
        // Actualizamos la orientación del compás para que se mueva con el jugador y la cámara
        compassImage.uvRect = new Rect(player.localEulerAngles.y / 360f, 0, 1f, 1f);

        // Actualizamos cada marcador en el compás
        foreach (QuestMarker marker in questMarkers)
        {
            if (marker.image == null)
                continue;

            // Actualizamos la posición del marcador en el compás
            marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);

            // Calculamos la distancia entre el jugador y el marcador para ajustar la escala
            Vector2 playerPos2D = new Vector2(player.transform.position.x, player.transform.position.z);
            Vector2 markerPos2D = marker.position;

            float dstSqr = (playerPos2D - markerPos2D).sqrMagnitude;
            float maxDistanceSqr = maxDistance * maxDistance;

            float scale = 0f;

            if (dstSqr < maxDistanceSqr)
            {
                scale = 1f - Mathf.Sqrt(dstSqr) / maxDistance;
            }

            marker.image.rectTransform.localScale = Vector3.one * scale;
        }
    }

    public void AddQuestMarker(QuestMarker marker)
    {
        if (questMarkers.Contains(marker))
            return; // Ya está registrado

        // Instanciamos un nuevo marcador y lo añadimos a la lista
        GameObject newMarker = Instantiate(iconPrefab, compassImage.transform);
        marker.image = newMarker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        questMarkers.Add(marker);
    }

    public void RemoveQuestMarker(QuestMarker marker)
    {
        if (questMarkers.Contains(marker))
        {
            if (marker.image != null)
            {
                Destroy(marker.image.gameObject); // Destruir el icono en el minimapa
            }
            questMarkers.Remove(marker);
        }
    }

    private Vector2 GetPosOnCompass(QuestMarker marker)
    {
        // Obtener las posiciones en 2D (sin la componente Y)
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 markerPos = marker.position;

        // Calcular la dirección desde el jugador hacia el marcador
        Vector2 direction = markerPos - playerPos;

        // Calcular el ángulo entre la dirección hacia el marcador y la dirección del jugador (hacia adelante)
        // Necesitamos obtener el ángulo de la rotación global del jugador en relación al mundo
        float angle = Vector2.SignedAngle(Vector2.up, direction);

        // Obtener la rotación global del jugador en el espacio (incluyendo la rotación de la cámara)
        float adjustedAngle = angle - player.eulerAngles.y;

        // Normalizamos el ángulo para que esté entre -180 y 180 grados
        adjustedAngle = Mathf.Repeat(adjustedAngle + 180f, 360f) - 180f;

        // Normalizamos el ángulo para que se mueva entre -1 y 1
        float normalizedPosX = adjustedAngle / 180f;

        // Convertimos la posición normalizada a píxeles según el tamaño del compás
        float posX = normalizedPosX * (compassImage.rectTransform.rect.width / 2f);

        // Ajustamos la posición en la barra del compás
        return new Vector2(posX, 0f);
    }
}
