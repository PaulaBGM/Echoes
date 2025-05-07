using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AimStateManager : MonoBehaviour
{
    private PlayerBehavior playerBehavior;
    public AxisState xAxis, yAxis;
    public Transform camFollowPos;

    [Header("Smooth Settings")]
    [SerializeField] private float smoothSpeed = 10f; // Velocidad de suavizado

    private float currentXRotation;
    private float currentYRotation;

    // Start is called before the first frame update
    void Start()
    {
        playerBehavior = GetComponent<PlayerBehavior>();

        currentXRotation = transform.eulerAngles.y;
        currentYRotation = camFollowPos.localEulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBehavior.IsDead) return;

        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (playerBehavior.IsDead) return;

        SmoothRotate(camFollowPos, xAxis, yAxis);
    }

    public void ShootCamera() 
    {
        yAxis.m_MinValue = -11f;
        yAxis.m_MaxValue = 3f;
    }

    public void NormalCamera()
    {
        yAxis.m_MinValue = -15f;
        yAxis.m_MaxValue = 15f;
    }

    private void SmoothRotate(Transform location, AxisState x, AxisState y)
    {
        // Obtener los valores interpolados suavemente
        float targetYRotation = Mathf.LerpAngle(currentYRotation, y.Value, Time.deltaTime * smoothSpeed);
        float targetXRotation = Mathf.LerpAngle(currentXRotation, x.Value, Time.deltaTime * smoothSpeed);

        // Clampear el valor interpolado para evitar sobrepasar los límites
        targetYRotation = Mathf.Clamp(targetYRotation, yAxis.m_MinValue, yAxis.m_MaxValue);

        // Aplicar la rotación suavizada
        currentYRotation = targetYRotation;
        location.localEulerAngles = new Vector3(currentYRotation, location.localEulerAngles.y, location.localEulerAngles.z);

        currentXRotation = targetXRotation;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentXRotation, transform.eulerAngles.z);
    }
}
