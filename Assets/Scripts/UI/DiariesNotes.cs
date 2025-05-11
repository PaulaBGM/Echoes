using UnityEngine;

public class DiariesNotes : MonoBehaviour
{
    [SerializeField] private GameObject playerUi;
    [SerializeField] private GameObject noteUi;
    [SerializeField] private GameObject fButtonImage;

    private bool isPlayerInRange = false;
    private bool isNoteOpen = false;

    void Start()
    {
        noteUi.SetActive(false);
        fButtonImage.SetActive(false);

        Time.timeScale = 1f; // Asegúrate de que el juego comience sin pausa
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            ToggleNote();
        }
    }

    private void ToggleNote()
    {
        isNoteOpen = !isNoteOpen;
        noteUi.SetActive(isNoteOpen);
        playerUi.SetActive(!isNoteOpen);

        Time.timeScale = isNoteOpen ? 0f : 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            fButtonImage.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            fButtonImage.SetActive(false);

            // Cerrar la nota si el jugador se aleja
            if (isNoteOpen)
            {
                isNoteOpen = false;
                noteUi.SetActive(false);
                playerUi.SetActive(true);
                Time.timeScale = 1f;
            }
        }
    }
}