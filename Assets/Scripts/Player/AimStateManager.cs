using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AimStateManager : MonoBehaviour
{
    public AxisState xAxis, yAxis;
    [SerializeField] private Transform camFollowPos;

    [Header("Smooth Settings")]
    [SerializeField] private float smoothSpeed = 10f; // Velocidad de suavizado

    private float currentXRotation;
    private float currentYRotation;

    // Start is called before the first frame update
    void Start()
    {
        currentXRotation = transform.eulerAngles.y;
        currentYRotation = camFollowPos.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        // Suavizar la rotación en el eje Y (vertical)
        currentYRotation = Mathf.LerpAngle(currentYRotation, yAxis.Value, Time.deltaTime * smoothSpeed);
        camFollowPos.localEulerAngles = new Vector3(currentYRotation, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);

        // Suavizar la rotación en el eje X (horizontal)
        currentXRotation = Mathf.LerpAngle(currentXRotation, xAxis.Value, Time.deltaTime * smoothSpeed);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentXRotation, transform.eulerAngles.z);
    }
}
