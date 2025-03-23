using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private string sceneName;

    private void OnEnable()
    {
        // Nos suscribimos al evento de Game Over
        PlayerBehavior.OnGameOver += OpenGameOverMenu;
    }

    private void OnDisable()
    {
        // Desuscripción del evento
        PlayerBehavior.OnGameOver -= OpenGameOverMenu;
    }

    private void Start()
    {
        gameOverMenu.SetActive(false);
        playerHUD.SetActive(true);
    }

    // Método que maneja la activación del menú Game Over
    private void OpenGameOverMenu()
    {
        StartCoroutine(ShowGameOverMenu());
    }

    private IEnumerator ShowGameOverMenu()
    {
        // Aquí se puede agregar un retraso o animación si es necesario
        yield return new WaitForSeconds(2f); // Por ejemplo, 2 segundos
        playerHUD.SetActive(false);
        gameOverMenu.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToMainMenu()
    {
        // Implementa la lógica para volver al menú principal
    }

    public void QuitGame()
    {
        // Implementa la lógica para cerrar el juego
        Application.Quit();
    }
}
