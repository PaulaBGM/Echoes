using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Player_Behavior
{
    private float gravity = -9.8f;

    [Header("Movement")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float stickToGroundSpeed = -3f;

    [Header("Jump")]
    private float jumpTimer = 0f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float endJumpAnimTime = 1.5f;
    private float startJumpAnimTime;
    [SerializeField] private float timeBetweenJump = 0.5f;
    [SerializeField] private float initialJumpAnimTime;

    [Header("Crouched")]
    [SerializeField] private float crouchSpeed = 1f;
    [SerializeField] private float standHeight = 2f; // Altura de la cápsula cuando está de pie.
    [SerializeField] private float crouchHeight = 0.8f; // Altura de la cápsula cuando está agachado.
    [SerializeField] private float crouchCenter = 0.4f; // Centro de la cápsula cuando está agachado.
    [SerializeField] private float standCenter = 0.5f; // Centro de la cápsula cuando está de pie.
    [SerializeField] private float endCrouchAnimTime = 1.5f;

    [Header("LongIdle")]
    [SerializeField] private float longIdleTime = 15f;
    private float longIdleTimer = 0f;

    private Vector3 playerVelocity;
    private float verticalVelocity;
    private bool isJumping;
    private bool running = false;
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

    protected override void Update()
    {
        base.Update();

        if (isDead) return;

        if (isDashing)
        {
            HandleDash();
            return;
        }

        if (running)
        {
            startJumpAnimTime = 0;
        }
        else
        {
            startJumpAnimTime = initialJumpAnimTime;
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
    }

    private void LongIdle()
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

    private void ApplyVelocity()
    {
        Vector3 totalVelocity = playerVelocity + verticalVelocity * Vector3.up;
         
        ch_Controller.Move(totalVelocity * Time.deltaTime); 
    }

    private void HandleCrouch()
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

    private void UpdatePlayerVelocity()
    {
        // Se recogen los inputs
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 vectorInput = new Vector3(xInput, 0, zInput);

        // Normalizamos el vectorInput si su magnitud es mayor que 1
        if (vectorInput.sqrMagnitude > 1)
        {
            vectorInput.Normalize();
        }

        // Actualizamos la velocidad actual según el estado de agachado o corriendo
        if (isCrouched)
        {
            currentSpeed = crouchSpeed; // Si está agachado, se usa la velocidad de agachado
            running = false; // Si está agachado, no se está corriendo
        }
        else
        {
            // Si se pulsa Shift, asignamos la velocidad de correr
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = runSpeed;
                running = true; // Se establece running a true si estamos corriendo
            }
            else
            {
                currentSpeed = normalSpeed; // Si no se pulsa Shift, se corre con velocidad normal
                running = false; // No estamos corriendo
            }
        }

        // Calculamos la velocidad en función de los inputs
        Vector3 localPlayerVelocity = new Vector3(xInput * currentSpeed, 0, zInput * currentSpeed);
        playerVelocity = transform.TransformVector(localPlayerVelocity); // Convertimos la velocidad local a la global

        // Llamamos a las animaciones pasándole la velocidad de movimiento en cada eje
        animator.SetFloat(ZSpeed, localPlayerVelocity.z);
        animator.SetFloat(XSpeed, localPlayerVelocity.x);
    }

    private void DoJump()
    {
        jumpTimer += Time.deltaTime;
        // Aplicar gravedad cuando el personaje está en el aire
        if (!ch_Controller.isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime; // Aplica la gravedad al personaje
        }

        // Iniciar salto
        if (Input.GetAxisRaw("Jump") > 0.5f && ch_Controller.isGrounded && !isJumping && !waitingForJumpAnim && endJump && jumpTimer > timeBetweenJump)
        {
            isJumping = true;
            waitingForJumpAnim = true;
            animator.SetInteger(Jump, 1); // Activa la animación de salto
            StartCoroutine(JumpCoroutine()); // Iniciamos la corrutina de animación
        }

        // Control de isGrounded solo para el aterrizaje
        if (ch_Controller.isGrounded && !endJump && !isJumping)
        {
            endJump = true;
            jumpTimer = 0;
            animator.SetInteger(Jump, 0);
            verticalVelocity = stickToGroundSpeed;
        }
    }

    private IEnumerator JumpCoroutine()
    {
        yield return new WaitForSeconds(startJumpAnimTime); // Espera un poco para sincronizar con la animación
        verticalVelocity = jumpForce; // Aplicamos la fuerza de salto
        endJump = false;
        StartCoroutine(EndJumpCoroutine());
    }

    private IEnumerator EndJumpCoroutine()
    {
        yield return new WaitForSeconds(endJumpAnimTime); // Espera un poco para sincronizar con la animación
        animator.SetInteger(Jump, 2); // Activa la animación de salto
        isJumping = false;
        waitingForJumpAnim = false;
    }

    private void StartCrouch()
    {
        isCrouched = true;
        ch_Controller.height = crouchHeight;
        ch_Controller.center = new Vector3(0, crouchCenter, 0);
        animator.SetInteger(Crouched, 1);
        tryingToStand = false;
    }

    private void TryStandUp()
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

    private void StandUp()
    {
        ch_Controller.height = standHeight;
        ch_Controller.center = new Vector3(0, standCenter, 0);
        animator.SetInteger(Crouched, 2);
        isCrouched = false;
        StartCoroutine(ResetCrouchState());
    }

    private IEnumerator ResetCrouchState()
    {
        yield return new WaitForSeconds(endCrouchAnimTime);
        animator.SetInteger(Crouched, 0);
    }

    private void StartDash()
    {
        isDashing = true; // Activamos el estado de dash
        dashTime = 0; // Reiniciamos el temporizador del dash

        // Si el jugador se estaba moviendo, usamos su dirección normalizada.
        // Si no se estaba moviendo, usamos transform.forward como dirección por defecto.
        dashDirection = playerVelocity.sqrMagnitude > 0 ? playerVelocity.normalized : transform.forward;
    }

    private void HandleDash()
    {
        dashTime += Time.deltaTime; // Aumentamos el tiempo transcurrido en el dash
        animator.SetFloat(ZSpeed, dashSpeed);
        ch_Controller.Move(dashDirection * dashSpeed * Time.deltaTime); // Movemos al jugador en la dirección del dash
        if (dashTime >= dashDuration) isDashing = false; // Terminamos el dash cuando se cumple el tiempo
    }
}
