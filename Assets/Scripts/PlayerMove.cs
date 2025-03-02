using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController ch_Controller;
    private Animator animator;
    private float gravity = -9.8f;

    [Header("Movement")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float stickToGroundSpeed = -3f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpRayDistance = 3f;

    private Vector3 playerVelocity;
    private float verticalVelocity;
    private bool isJumping;
    private bool endJump = true;

    private bool isCrouched = false;
    
    [Header("Dash")]
    private bool isDashing = false;
    private float dashTime;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashSpeed = 7f;
    [SerializeField] private float dashEndSpeed = 1f;
    private Vector3 dashDirection;

    //Variables de números enteros
    private static readonly int ZSpeed = Animator.StringToHash("zSpeed");
    private static readonly int XSpeed = Animator.StringToHash("xSpeed");
    private static readonly int Crouched = Animator.StringToHash("crouched");

    void Start()
    {
        ch_Controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (isDashing) HandleDash();
        else
        {
            UpdatePlayerVelocity();
            Jump();
            ApplyVelocity();
            if (Input.GetKeyDown(KeyCode.C)) StartDash();
        }
    }
    void ApplyVelocity()
    {
        Vector3 totalVelocity = playerVelocity + verticalVelocity * Vector3.up;
         
            ch_Controller.Move(totalVelocity * Time.deltaTime); 
        
    }

    void UpdatePlayerVelocity() 
    {
        //se recogen los input
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 vectorInput = new Vector3(xInput,0, zInput);
        if(vectorInput.sqrMagnitude > 1) 
        {
            vectorInput.Normalize();
        }

        //Interpolar para caminar y correr.
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) && !isCrouched ? runSpeed : normalSpeed; //Si presiona Shift y no está agachado se usa la velocidad para correr sino usa la velociad normal.
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5); //Suaviza el cambio de velocidad gradualmente.
        
        Vector3 localPlayerVelocity = new Vector3(xInput * currentSpeed, 0, zInput * currentSpeed);
        playerVelocity = transform.TransformVector(localPlayerVelocity);
        Debug.Log(localPlayerVelocity);

        //Llamar a las animaciones pasándole la velocidad de movimiento en cada eje.
        animator.SetFloat(ZSpeed, localPlayerVelocity.z);
        animator.SetFloat(XSpeed, localPlayerVelocity.x);
    }

    void Jump()
    {
        if (Input.GetAxisRaw("Jump") > 0.5f && ch_Controller.isGrounded && !isJumping)
        {
            isJumping = true;
            verticalVelocity = jumpForce; // Aplicamos la fuerza de salto
            StartCoroutine(JumpCoroutine()); // Iniciamos la corrutina de animación
        }

        if (verticalVelocity < 0 && ch_Controller.isGrounded)
        {
            Debug.Log("Fin del salto");
            isJumping = false;
            verticalVelocity = stickToGroundSpeed; // Asegura que el personaje se mantenga pegado al suelo
        }

        verticalVelocity += gravity * Time.deltaTime; // Aplica la gravedad al personaje
    }

    IEnumerator JumpCoroutine()
    {
        animator.SetInteger("jump", 1); // Activa la animación de salto
        yield return new WaitForSeconds(0.2f); // Espera un poco para sincronizar con la animación

        // Espera hasta que el personaje deje de estar en el aire
        while (!ch_Controller.isGrounded)
        {
            yield return null;
        }

        animator.SetInteger("jump", 2); // Desactiva la animación al aterrizar
    }


    void StartDash()
    {
        isDashing = true; // Activamos el estado de dash
        dashTime = 0; // Reiniciamos el temporizador del dash

        // Si el jugador se estaba moviendo, usamos su dirección normalizada.
        // Si no se estaba moviendo, usamos transform.forward como dirección por defecto.
        dashDirection = playerVelocity.sqrMagnitude > 0 ? playerVelocity.normalized : transform.forward;
    }

    void HandleDash()
    {
        dashTime += Time.deltaTime; // Aumentamos el tiempo transcurrido en el dash
        ch_Controller.Move(dashDirection * dashSpeed * Time.deltaTime); // Movemos al jugador en la dirección del dash
        if (dashTime >= dashDuration) isDashing = false; // Terminamos el dash cuando se cumple el tiempo
    }
}
