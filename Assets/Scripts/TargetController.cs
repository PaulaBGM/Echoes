using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private GameObject redTarget;
    [SerializeField] private GameObject greenTarget;

    public bool withMovement = false;

    [SerializeField] private float moveSpeed = 1f; // Velocidad del movimiento
    [SerializeField] private float moveDistance = 3f; // Distancia total de movimiento
    private Vector3 startPosition;

    [SerializeField] private bool verticalMovement = false;
    [SerializeField] private bool frontMovement = false;

    private void Awake()
    {
        redTarget.SetActive(true);
        greenTarget.SetActive(false);

    }

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (withMovement)
        {
            Move();
        }
    }

    public void ChangeColor()
    {
        redTarget.SetActive(false);
        greenTarget.SetActive(true);
    }

    private void Move()
    {
        float move = Mathf.PingPong(Time.time * moveSpeed, moveDistance);

        if (!verticalMovement && !frontMovement)
        {
            // Movimiento de ida y vuelta horizontal (X)
            transform.position = startPosition + new Vector3(move, 0, 0);
        }
        else if(frontMovement)
        {
            transform.position = startPosition + new Vector3(0, 0, move);
        }
        else if (verticalMovement)
        {
            transform.position = startPosition + new Vector3(0, move, 0);
        }
    }
}
