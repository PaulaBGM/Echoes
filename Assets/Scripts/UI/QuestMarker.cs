using UnityEngine;
using UnityEngine.UI;

public class QuestMarker : MonoBehaviour
{
    public Sprite icon;
    public Image image;

    public Vector2 position => new Vector2(transform.position.x, transform.position.z);

    public Sprite GetIcon()
    {
        return icon;
    }

    // Usamos Start en lugar de OnEnable
    private void Start()
    {
        if (NavBarController.Instance != null)
        {
            NavBarController.Instance.AddQuestMarker(this);
        }
    }

    private void OnDisable()
    {
        if (NavBarController.Instance != null)
        {
            NavBarController.Instance.RemoveQuestMarker(this);
        }
    }
}
