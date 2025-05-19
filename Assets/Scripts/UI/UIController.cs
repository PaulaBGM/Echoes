using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject playerHUD;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject fullCirclePistol;
    [SerializeField] private GameObject fullCircleRifle;
    [SerializeField] private GameObject fullCirclePistolSelect; //Por si solo se recoge este arma
    [SerializeField] private GameObject fullCircleRifleSelect; //Por si solo se recoge este arma
    [SerializeField] private GameObject middleCircleRifle;
    [SerializeField] private GameObject middleCirclePistol;
    [SerializeField] private GameObject victoryMenu;

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
        Invoke("GoToMainMenu", 2f);
    }

    public void OpenVictoryMenu() 
    {
        StartCoroutine(ShowVictoryMenu());
        
    }

    private IEnumerator ShowGameOverMenu()
    {
        // Aquí se puede agregar un retraso o animación si es necesario
        yield return new WaitForSeconds(2f); // Por ejemplo, 2 segundos
        playerHUD.SetActive(false);
        gameOverMenu.SetActive(true);
    }
    private IEnumerator ShowVictoryMenu()
    {
        // Aquí se puede agregar un retraso o animación si es necesario
        yield return new WaitForSeconds(1f); // Por ejemplo, 2 segundos
        playerHUD.SetActive(false);
        victoryMenu.SetActive(true);
        Invoke("GoToMainMenu", 3f);
    }
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        // Implementa la lógica para cerrar el juego
        Application.Quit();
    }

    public void UpdateWeaponUI(WeaponType activeWeapon, bool hasPistol, bool hasRifle)
    {
        bool hasBothWeapons = hasPistol && hasRifle;

        // Apaga todo al inicio
        fullCirclePistol.SetActive(false);
        fullCircleRifle.SetActive(false);
        fullCirclePistolSelect.SetActive(false);
        fullCircleRifleSelect.SetActive(false);
        middleCirclePistol.SetActive(false);
        middleCircleRifle.SetActive(false);

        if (!hasBothWeapons)
        {
            // Solo una arma: mostrar UI simple
            if (activeWeapon == WeaponType.Short)
            {
                fullCirclePistolSelect.SetActive(true);
            }
            else if (activeWeapon == WeaponType.Long)
            {
                fullCircleRifleSelect.SetActive(true);
            }
            return;
        }

        // Dos armas: mostrar UI completa
        switch (activeWeapon)
        {
            case WeaponType.Short:
                fullCircleRifle.SetActive(true);       // Rifle está guardado
                middleCirclePistol.SetActive(true);    // Pistola está activa
                break;

            case WeaponType.Long:
                fullCirclePistol.SetActive(true);      // Pistola está guardada
                middleCircleRifle.SetActive(true);     // Rifle está activo
                break;
        }
    }
}
