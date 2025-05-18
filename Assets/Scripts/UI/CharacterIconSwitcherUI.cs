using UnityEngine;
using UnityEngine.UI;

public class CharacterIconSwitcherUI : MonoBehaviour
{
    [Header("Referencias")]
    public Image iconImage;             // Imagen del personaje (UI)
    public GameObject characterObject;  // Objeto del que se leerá el tag

    [Header("Sprites")]
    public Sprite sprite;            // Sprite para tag "Nyx"

    void Update()
    {

        // Verifica que el objeto no sea nulo
        if (iconImage != null && characterObject != null)
        {
                iconImage.sprite = sprite;
        }
    }
}
