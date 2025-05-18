using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckInteraction : MonoBehaviour
{
    private bool playerInRange;
    public bool interactionStarted;
    public GameObject dialogue;

    [SerializeField] private GameObject player;
    [SerializeField] private float waitTime;
    [SerializeField] private GameObject interactionMark;
    [SerializeField] private KeyCode interactButton;

    [SerializeField] private string[] dialogueLines;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private Text dialogueText;
    public UIController uicontroller;
    public bool Nyx;
    private int dialogueIndex;
    [SerializeField] Image sprite;
    public Sprite spriteNyx;

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        // Si el jugador está en rango y presiona la tecla de interacción
        if (playerInRange && Input.GetKeyDown(interactButton))
        {
            if (!interactionStarted)
            {
                StartInteraction();  // Comienza la interacción
            }
            else if (dialogueText.text == dialogueLines[dialogueIndex])
            {
                ContinueDialogue();  // Continúa el diálogo
            }
            else
            {
                StopAllCoroutines();  // Detiene cualquier Coroutine en ejecución
                dialogueText.text = dialogueLines[dialogueIndex];  // Muestra la línea completa
            }
        }
    }

    // Inicia la interacción mostrando el diálogo
    private void StartInteraction()
    {
        player.GetComponent<PlayerBehavior>().isInDialogue = true;  // Activa el estado de diálogo en el jugador
        interactionStarted = true;
        dialoguePanel.SetActive(true);  // Muestra el panel de diálogo
        playerHUD.SetActive(false);
        interactionMark.SetActive(false);  // Oculta la marca de interacción
        dialogueIndex = 0;  // Empieza desde la primera línea del diálogo
        Time.timeScale = 0f;  // Pausa el juego
        StartCoroutine(BlockMovement());  // Muestra el diálogo letra por letra
    }

    // Muestra las letras de la línea de diálogo
    private IEnumerator BlockMovement()
    {
        dialogueText.text = string.Empty;

        foreach (char characters in dialogueLines[dialogueIndex])
        {
            dialogueText.text += characters;
            yield return new WaitForSecondsRealtime(waitTime);  // Espera entre letras
        }
    }

    // Continúa al siguiente diálogo o finaliza la interacción
    private void ContinueDialogue()
    {
        dialogueIndex++;  // Avanza al siguiente índice

        if (dialogueIndex < dialogueLines.Length)
        {
            StartCoroutine(BlockMovement());  // Si hay más diálogo, continúa
        }
        else
        {
            interactionStarted = false;
            dialoguePanel.SetActive(false);  // Desactiva el panel
            playerHUD.SetActive(true);
            interactionMark.SetActive(true);  // Muestra la marca de interacción
            player.GetComponent<PlayerBehavior>().isInDialogue = false;  // Termina el estado de diálogo
            Time.timeScale = 1f;  // Reactiva el tiempo del juego
            Destroy(dialogue);

            if (Nyx)
            {
                uicontroller.OpenVictoryMenu();
            }
            
        }
    }

    // Detecta cuando el jugador entra o sale del área de interacción
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactionMark.SetActive(true);
            if (Nyx) 
            {
                sprite.sprite = spriteNyx;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionMark.SetActive(false);
            player.GetComponent<PlayerBehavior>().isInDialogue = false;

            
        }
    }
}
