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
    [SerializeField] private float endJumpRaycastDistance = 2f;

    [Header("Crouched")]
    [SerializeField] private float crouchSpeed = 1f;
    [SerializeField] private float standHeight = 2f; // Altura de la cápsula cuando está de pie.
    [SerializeField] private float crouchHeight = 0.8f; // Altura de la cápsula cuando está agachado.
    [SerializeField] private float crouchCenter = 0.4f; // Centro de la cápsula cuando está agachado.
    [SerializeField] private float standCenter = 0.5f; // Centro de la cápsula cuando está de pie.

    [Header("LongIdle")]
    [SerializeField] private float longIdleTime = 15f;
    private float longIdleTimer = 0f;

    private Vector3 playerVelocity;
    private float verticalVelocity;
    private bool isJumping;
    private bool waitingForJumpAnim = false;
    private bool endJump = true;

    private bool isCrouched = false;
    private bool tryingToStand = false;

    [Header("Dash")]
    private bool isDashing = false;
    private float dashTime;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashSpeed = 7f;
    [SerializeField] private float dashEndSpeed = 1f;
    private Vector3 dashDirection;

    //Variables de números enteros
    private static readonly int Jump = Animator.StringToHash("jump");
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
        Debug.Log("Grounded: " + ch_Controller.isGrounded);
        Debug.Log("Jumping: " + isJumping);


        if (isDashing)
        {
            HandleDash();
            return;
        }

        UpdatePlayerVelocity();
        DoJump();
        ApplyVelocity();

        HandleCrouch();

        if (tryingToStand)
        {
            TryStandUp();
        }

        if (Input.GetKeyDown(KeyCode.C)) StartDash();

        LongIdle(); // Llamada a la función después de manejar el movimiento

        verticalVelocity = stickToGroundSpeed;
    }

    void LongIdle()
    {
        // Si el jugador está en movimiento, reiniciamos el temporizador
        if (playerVelocity.sqrMagnitude > 0.01f || isJumping || isDashing || isCrouched)
        {
            longIdleTimer = 0f;
            animator.SetBool("longIdle", false);
            animator.SetBool("movement", true);
            return;
        }

        // Si el jugador no se ha movido, aumentamos el temporizador
        longIdleTimer += Time.deltaTime;

        if (longIdleTimer > longIdleTime)
        {
            animator.SetBool("longIdle", true);
            animator.SetBool("movement", false);
        }
    }

    void ApplyVelocity()
    {
        Vector3 totalVelocity = playerVelocity + verticalVelocity * Vector3.up;
         
        ch_Controller.Move(totalVelocity * Time.deltaTime); 
    }

    void HandleCrouch()
    {
        // Si se mantiene presionada la tecla LeftControl, se agacha
        if (Input.GetKey(KeyCode.LeftControl) && !isCrouched)
        {
            StartCrouch();
        }
        // Levantarse si se suelta la tecla
        else if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouched)
        {
            tryingToStand = true;
        }
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

        if (isCrouched)
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            //Interpolar para caminar y correr.
            float targetSpeed = Input.GetKey(KeyCode.LeftShift) && !isCrouched ? runSpeed : normalSpeed; //Si presiona Shift y no está agachado se usa la velocidad para correr sino usa la velociad normal.
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5); //Suaviza el cambio de velocidad gradualmente.
        }

        Vector3 localPlayerVelocity = new Vector3(xInput * currentSpeed, 0, zInput * currentSpeed);
        playerVelocity = transform.TransformVector(localPlayerVelocity);

        //Llamar a las animaciones pasándole la velocidad de movimiento en cada eje.
        animator.SetFloat(ZSpeed, localPlayerVelocity.z);
        animator.SetFloat(XSpeed, localPlayerVelocity.x);
    }

    void DoJump()
    {
        // Iniciar salto
        if (Input.GetAxisRaw("Jump") > 0.5f && ch_Controller.isGrounded && !isJumping && !waitingForJumpAnim)
        {
            isJumping = true;
            animator.SetInteger(Jump, 1); // Activa la animación de salto
            StartCoroutine(JumpCoroutine()); // Iniciamos la corrutina de animación
        }

        // Aplicar gravedad cuando el personaje está en el aire
        if (!ch_Controller.isGrounded && isJumping)
        {
            verticalVelocity += gravity * Time.deltaTime; // Aplica la gravedad al personaje
        }

        // Si el personaje está en el suelo y no está saltando, devolverlo al suelo
        if (verticalVelocity < 0 && ch_Controller.isGrounded && endJump && !waitingForJumpAnim)
        {
            Debug.Log("Fin del salto");
            isJumping = false;
            verticalVelocity = stickToGroundSpeed; // Asegura que el personaje se mantenga pegado al suelo
        }

        // Verificar si el personaje aún está en el aire y hacer el Raycast para detectar el aterrizaje
        if (isJumping && !endJump && verticalVelocity < 0 && !waitingForJumpAnim)
        {
            Debug.DrawRay(transform.position, Vector3.down * endJumpRaycastDistance, Color.red);

            // Si el Raycast detecta el suelo, terminar el salto
            if (Physics.Raycast(transform.position, Vector3.down, endJumpRaycastDistance))
            {
                endJump = true;
            }
        }

        // Control de isGrounded solo para el aterrizaje
        if (ch_Controller.isGrounded && !isJumping && !endJump)
        {
            // Si el personaje ya aterrizó y no estaba saltando, aplicar el "stick to ground" (pegado al suelo)
            verticalVelocity = stickToGroundSpeed;
        }
    }

    IEnumerator JumpCoroutine()
    {
        waitingForJumpAnim = true;
        yield return new WaitForSeconds(0.3f); // Espera un poco para sincronizar con la animación
        verticalVelocity = jumpForce; // Aplicamos la fuerza de salto
        StartCoroutine(EndJumpCoroutine());
    }

    IEnumerator EndJumpCoroutine()
    {
        yield return new WaitForSeconds(1.5f); // Espera un poco para sincronizar con la animación
        waitingForJumpAnim = false;
        animator.SetInteger(Jump, 0); // Desactiva la animación de salto
    }


    void StartCrouch()
    {
        isCrouched = true;
        ch_Controller.height = crouchHeight;
        ch_Controller.center = new Vector3(0, crouchCenter, 0);
        animator.SetInteger(Crouched, 1);
        tryingToStand = false;
    }

    void TryStandUp()
    {
        if (CanStandUp())
        {
            StandUp();
            tryingToStand = false;
        }
    }

    private bool CanStandUp()
    {
        RaycastHit hitInfo;

        return !Physics.SphereCast(transform.position + ch_Controller.center, ch_Controller.radius, Vector3.up, out hitInfo, 2f);
    }

    void StandUp()
    {
        ch_Controller.height = standHeight;
        ch_Controller.center = new Vector3(0, standCenter, 0);
        animator.SetInteger(Crouched, 2);
        isCrouched = false;
        StartCoroutine(ResetCrouchState());
    }

    IEnumerator ResetCrouchState()
    {
        yield return new WaitForSeconds(1.5f);
        animator.SetInteger(Crouched, 0);
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
        animator.SetFloat(ZSpeed, dashSpeed);
        ch_Controller.Move(dashDirection * dashSpeed * Time.deltaTime); // Movemos al jugador en la dirección del dash
        if (dashTime >= dashDuration) isDashing = false; // Terminamos el dash cuando se cumple el tiempo
    }
}
