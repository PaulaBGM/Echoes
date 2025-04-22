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
        if (!verticalMovement)
        {
            // Movimiento de ida y vuelta horizontal (X)
            float x = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
            transform.position = startPosition + new Vector3(x, 0, 0);
        }
        else
        {
            float y = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
            transform.position = startPosition + new Vector3(0, y, 0);
        }
    }
}
