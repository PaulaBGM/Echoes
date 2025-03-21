using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LifeBarUi : MonoBehaviour
{
    [SerializeField] private Image lifeBar;
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main?.transform;
    }

    void Update()
    {
        transform.LookAt(cameraTransform);
    }

    public void UpdateLifeBar(BaseHealth character)
    {
        var currentLife = character.CurrentLife;
        var maxLife = character.MaxLife;
        var lifePercent = Mathf.Clamp(currentLife / maxLife, 0, 1);
        lifeBar.fillAmount = lifePercent;
    }
}
