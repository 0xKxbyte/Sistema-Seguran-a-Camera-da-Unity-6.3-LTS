using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movimento + Pulo + Gravidade — funciona sem precisar configurar Layer.
/// Requer: CharacterController no mesmo GameObject.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float velocidade = 5f;
    [SerializeField] private float velocidadeCorrida = 10f;

    [Header("Pulo e Gravidade")]
    [SerializeField] private float forcaPulo = 6f;
    [SerializeField] private float gravidade = -25f;

    // Componentes
    private CharacterController controller;

    // Input
    private InputAction moverAction;
    private InputAction correrAction;
    private InputAction pularAction;

    // Estado
    private float velocidadeY = 0f;

    // -------------------------------------------------------

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        moverAction = new InputAction("Mover");
        moverAction.AddCompositeBinding("2DVector")
            .With("Up",    "<Keyboard>/w")
            .With("Down",  "<Keyboard>/s")
            .With("Left",  "<Keyboard>/a")
            .With("Right", "<Keyboard>/d")
            .With("Up",    "<Keyboard>/upArrow")
            .With("Down",  "<Keyboard>/downArrow")
            .With("Left",  "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        correrAction = new InputAction("Correr", binding: "<Keyboard>/leftShift");
        pularAction  = new InputAction("Pular",  binding: "<Keyboard>/space");
    }

    private void OnEnable()
    {
        moverAction.Enable();
        correrAction.Enable();
        pularAction.Enable();
    }

    private void OnDisable()
    {
        moverAction.Disable();
        correrAction.Disable();
        pularAction.Disable();
    }

    private void OnDestroy()
    {
        moverAction.Dispose();
        correrAction.Dispose();
        pularAction.Dispose();
    }

    // -------------------------------------------------------

    private void Update()
    {
        bool noChao = EstaNoChao();

        // Reseta velocidade vertical ao pousar
        if (noChao && velocidadeY < 0f)
            velocidadeY = -4f;

        // ---------- Movimento horizontal ----------
        Vector2 input = moverAction.ReadValue<Vector2>();
        Vector3 direcao = transform.right * input.x + transform.forward * input.y;
        if (direcao.magnitude > 1f) direcao.Normalize();

        float vel = correrAction.IsPressed() ? velocidadeCorrida : velocidade;
        controller.Move(direcao * vel * Time.deltaTime);

        // ---------- Pulo ----------
        if (pularAction.WasPressedThisFrame() && noChao)
            velocidadeY = Mathf.Sqrt(forcaPulo * -2f * gravidade);

        // ---------- Gravidade ----------
        velocidadeY += gravidade * Time.deltaTime;
        controller.Move(Vector3.up * velocidadeY * Time.deltaTime);
    }

    // -------------------------------------------------------

    /// <summary>
    /// Detecta o chão usando a flag do CharacterController +
    /// um SphereCast de segurança. Não precisa de Layer configurada.
    /// </summary>
    private bool EstaNoChao()
    {
        // isGrounded nativo do CharacterController
        if (controller.isGrounded) return true;

        // SphereCast extra para casos onde isGrounded falha em rampas/bordas
        float raio   = controller.radius * 0.9f;
        float altura = controller.height / 2f - raio + 0.05f;
        Vector3 origem = transform.position + Vector3.up * altura;

        // Ignora o próprio collider do personagem
        return Physics.SphereCast(
            origem,
            raio,
            Vector3.down,
            out _,
            0.2f,
            ~LayerMask.GetMask("Ignore Raycast") // detecta tudo exceto "Ignore Raycast"
        );
    }
}