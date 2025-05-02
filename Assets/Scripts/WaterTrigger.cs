using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    private bool isInWater;
    private PlayerMove playerMove;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("ENTRA PLAYER");

            isInWater = true;

            playerMove = other.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.SetUnderwaterSpeed(isInWater); // crea un método público en PlayerMove
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isInWater = false;

            if (playerMove != null)
            {
                playerMove.SetUnderwaterSpeed(isInWater); // crea un método público en PlayerMove
            }
        }
    }
}