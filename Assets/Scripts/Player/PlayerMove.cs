using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController ch_Controller;
    private PlayerBehavior playerBehavior;
    private GetWeapon getWeapon;
    private float gravity = -9.8f;

    [Header("Movement")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float currentSpeed = 0f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float underWaterSpeed = 1.5f;
    [SerializeField] private float stickToGroundSpeed = -3f;

    [Header("Jump")]
    private float jumpTimer = 0f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float endJumpAnimTime = 1.5f;
    private float startJumpAnimTime;
    [SerializeField] private float timeBetweenJump = 0.5f;
    [SerializeField] private float initialJumpAnimTime;

    [Header("Slide")]
    [SerializeField] private AnimationCurve slideSlowCurve;
    [SerializeField] private float slideSlope = 4f;
    [SerializeField] private float slideSpeed = 3f;
    [SerializeField] private float maxSlideVelocity = 6f;
    [SerializeField] private float slideDownTime = 3f;

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
    private bool walking = false;
    private bool waitingForJumpAnim = false;
    private bool endJump = true;
    public bool canLongIddle;

    private bool isCrouched = false;
    private bool tryingToStand = false;

    [Header("Dash")]
    private bool isDashing = false;
    private float dashTime;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashSpeed = 7f;
    [SerializeField] private float dashEndSpeed = 1f;
    private Vector3 dashDirection;

    private Vector3 slideVelocity;
    private float slidenTime = 0f;
    private float slideVelocityFactor = 1f;
    private bool sliding = false;
    private bool isInWater = false;

    //Variables de números enteros
    private static readonly int Jump = Animator.StringToHash("jump");
    private static readonly int ZSpeed = Animator.StringToHash("zSpeed");
    private static readonly int XSpeed = Animator.StringToHash("xSpeed");
    private static readonly int Crouched = Animator.StringToHash("crouched");

    [SerializeField] private LayerMask ceilingLayer; // Crea un LayerMask en el inspector solo para techo/obstáculos

    private void Start()
    {
        ch_Controller = GetComponent<CharacterController>();
        playerBehavior = GetComponent<PlayerBehavior>();
        getWeapon = GetComponent<GetWeapon>();
    }

    private  void Update()
    {
        if (playerBehavior.IsDead) return;

        if (isDashing)
        {
            HandleDash();
            return;
        }

        if (walking)
        {
            startJumpAnimTime = 0;
        }
        else
        {
            startJumpAnimTime = initialJumpAnimTime;
        }

        UpdatePlayerVelocity();
        DoJump();
        UpdateSlideVelocity();
        ApplyVelocity();

        if(!getWeapon.hasPistol || !getWeapon.hasLargeWeapon)
        {
            HandleCrouch();
        }

        if (tryingToStand)
        {
            TryStandUp();
        }

        if (Input.GetKeyDown(KeyCode.C)) StartDash();

        LongIdle(); // Llamada a la función después de manejar el movimiento
    }

    private void LongIdle()
    {
        if (!canLongIddle) return;
        // Si el jugador está en movimiento, reiniciamos el temporizador
        if (playerVelocity.sqrMagnitude > 0.01f || isJumping || isDashing || isCrouched)
        {
            longIdleTimer = 0f;
            playerBehavior.Animator.SetBool("longIdle", false);
            playerBehavior.Animator.SetBool("movement", true);
            return;
        }

        // Si el jugador no se ha movido, aumentamos el temporizador
        longIdleTimer += Time.deltaTime;

        if (longIdleTimer > longIdleTime)
        {
            playerBehavior.Animator.SetBool("longIdle", true);
            playerBehavior.Animator.SetBool("movement", false);
        }
    }

    private void ApplyVelocity()
    {
        Vector3 horizontalVelocity = playerVelocity + slideVelocity;

        if (!isInWater)
        {
            horizontalVelocity *= slideVelocityFactor;
        }

        Vector3 totalVelocity = horizontalVelocity + Vector3.up * verticalVelocity;
        ch_Controller.Move(totalVelocity * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!isCrouched && !tryingToStand)
            {
                StartCrouch();
            }
            else if (isCrouched && !tryingToStand)
            {
                tryingToStand = true;
            }
        }
    }

    private void UpdatePlayerVelocity()
    {
        // Se recogen los inputs
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        Vector3 vectorInput = new Vector3(xInput, 0, zInput);

        // Determinar si el jugador está en movimiento
        walking = vectorInput.sqrMagnitude > 0.01f;

        // Normalizamos el vectorInput si su magnitud es mayor que 1
        if (vectorInput.sqrMagnitude > 1)
        {
            vectorInput.Normalize();
        }

        // Actualizamos la velocidad actual según el estado de agachado o corriendo
        if (isCrouched)
        {
            currentSpeed = crouchSpeed; // Si está agachado, se usa la velocidad de agachado
            walking = false; // Si está agachado, no se está corriendo
        }
        else if (isInWater)
        {
            currentSpeed = underWaterSpeed;
        }
        else
        {
            // Si se pulsa Shift, asignamos la velocidad de correr
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = normalSpeed; // Si no se pulsa Shift, se corre con velocidad normal
            }
        }

        // Calculamos la velocidad en función de los inputs
        Vector3 localPlayerVelocity = new Vector3(xInput * currentSpeed, 0, zInput * currentSpeed);
        playerVelocity = transform.TransformVector(localPlayerVelocity); // Convertimos la velocidad local a la global

        if (isInWater && !playerBehavior.Animator.GetBool("inWater"))
        {
            playerBehavior.Animator.SetBool("inWater", true);
        }
        else if (!isInWater && playerBehavior.Animator.GetBool("inWater"))
        {
            playerBehavior.Animator.SetBool("inWater", false);
        }

        // Llamamos a las animaciones pasándole la velocidad de movimiento en cada eje
        playerBehavior.Animator.SetFloat(ZSpeed, localPlayerVelocity.z);
        playerBehavior.Animator.SetFloat(XSpeed, localPlayerVelocity.x);
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
            playerBehavior.Animator.SetInteger(Jump, 1); // Activa la animación de salto
            StartCoroutine(JumpCoroutine()); // Iniciamos la corrutina de animación
        }

        // Control de isGrounded solo para el aterrizaje
        if (ch_Controller.isGrounded && !endJump && !isJumping)
        {
            endJump = true;
            jumpTimer = 0;
            playerBehavior.Animator.SetInteger(Jump, 0);
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
        playerBehavior.Animator.SetInteger(Jump, 2); // Activa la animación de salto
        isJumping = false;
        waitingForJumpAnim = false;
    }

    void UpdateSlideVelocity()
    {
        RaycastHit hitInfo;

        // Origen del raycast levemente elevado para evitar colisión dentro del suelo
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        // Raycast hacia abajo para detectar el suelo bajo el personaje
        bool hit = Physics.Raycast(rayOrigin, Vector3.down, out hitInfo, ch_Controller.height / 2 + 0.5f);

        if (ch_Controller.isGrounded && hit)
        {
            // Calcula el ángulo entre la normal del suelo y el vector hacia arriba
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);

            // Si el ángulo es mayor que el umbral definido como pendiente resbaladiza
            if (angle > slideSlope)
            {
                // Si no estaba deslizando, reseteamos parámetros
                if (!sliding)
                {
                    sliding = true;
                    slidenTime = 0f;
                    slideVelocity = Vector3.zero;
                }

                // Calculamos la dirección de deslizamiento sobre la pendiente
                Vector3 slideDir = Vector3.ProjectOnPlane(Vector3.down, hitInfo.normal).normalized;

                // Velocidad objetivo del deslizamiento
                Vector3 targetSlide = slideDir * slideSpeed;

                // Limita la velocidad máxima de deslizamiento
                targetSlide = Vector3.ClampMagnitude(targetSlide, maxSlideVelocity);

                // Si recién empezó el deslizamiento, asigna directamente la velocidad
                if (slidenTime < 0.1f)
                {
                    slideVelocity = targetSlide;
                }
                else
                {
                    // Suaviza la transición hacia la velocidad objetivo
                    slideVelocity = Vector3.Lerp(slideVelocity, targetSlide, Time.deltaTime * 5f);
                }
            }
            else
            {
                // Si ya no hay pendiente pronunciada pero antes había deslizamiento
                if (sliding)
                {
                    // Suaviza la detención del deslizamiento
                    slideVelocity = Vector3.Lerp(slideVelocity, Vector3.zero, Time.deltaTime * 10f);

                    // Si la velocidad es muy baja, detiene el deslizamiento
                    if (slideVelocity.magnitude < 0.1f)
                    {
                        sliding = false;
                        slidenTime = 0f;
                        slideVelocity = Vector3.zero;
                    }
                }
            }
        }
        else
        {
            // No hay suelo o no está en el suelo (en el aire)
            sliding = false;
            slidenTime = 0f;
            slideVelocity = Vector3.zero;
        }

        // Si está deslizando, actualiza el factor de desaceleración según la curva
        if (sliding)
        {
            slidenTime += Time.deltaTime;

            float t = Mathf.Clamp01(slidenTime / slideDownTime);

            // Aplica curva con un mínimo inicial para evitar frenado
            slideVelocityFactor = Mathf.Max(0.4f, slideSlowCurve.Evaluate(t));
        }
        else
        {
            // Si no está deslizando, suavemente vuelve el factor a 1
            slideVelocityFactor = Mathf.Lerp(slideVelocityFactor, 1f, Time.deltaTime * 10f);
        }
    }

    private void StartCrouch()
    {
        isCrouched = true;
        ch_Controller.height = crouchHeight;
        ch_Controller.center = new Vector3(0, crouchCenter, 0);
        playerBehavior.Animator.SetInteger(Crouched, 1);
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
        float checkDistance = standHeight - crouchHeight; // altura adicional necesaria

        Vector3 start = transform.position + Vector3.up * crouchHeight;
        // Solo colisiona con objetos en "ceilingLayer", no con armas ni el propio jugador
        return !Physics.SphereCast(start, ch_Controller.radius, Vector3.up, out hitInfo, checkDistance, ceilingLayer); ;
    }

    private void StandUp()
    {
        ch_Controller.height = standHeight;
        ch_Controller.center = new Vector3(0, standCenter, 0);
        playerBehavior.Animator.SetInteger(Crouched, 2);
        isCrouched = false;
    }

    private IEnumerator ResetCrouchState()
    {
        yield return new WaitForSeconds(endCrouchAnimTime);
        playerBehavior.Animator.SetInteger(Crouched, 0);
    }

    private void StartDash()
    {
        if (isInWater) return; // No permite dash bajo el agua

        isDashing = true; // Activamos el estado de dash
        dashTime = 0; // Reiniciamos el temporizador del dash

        // Si el jugador se estaba moviendo, usamos su dirección normalizada.
        // Si no se estaba moviendo, usamos transform.forward como dirección por defecto.
        dashDirection = playerVelocity.sqrMagnitude > 0 ? playerVelocity.normalized : transform.forward;
    }

    private void HandleDash()
    {
        dashTime += Time.deltaTime; // Aumentamos el tiempo transcurrido en el dash
        playerBehavior.Animator.SetFloat(ZSpeed, dashSpeed);
        ch_Controller.Move(dashDirection * dashSpeed * Time.deltaTime); // Movemos al jugador en la dirección del dash
        if (dashTime >= dashDuration) isDashing = false; // Terminamos el dash cuando se cumple el tiempo
    }

    public void SetUnderwaterSpeed(bool inWater)
    {
        isInWater = inWater;
    }
}
