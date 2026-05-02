using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Câmera em primeira pessoa que gira o corpo do personagem horizontalmente.
/// Pressione V para soltar/travar o cursor.
///
/// HIERARQUIA NECESSÁRIA:
///
///   Personagem          <-- PlayerController aqui
///   └── Main Camera     <-- CameraLook aqui (filho direto do personagem)
///
/// Posicione a câmera em Y = ~1.7 (altura dos olhos).
/// Deixe o campo "Personagem" VAZIO — o script pega o pai automaticamente.
/// </summary>
public class CameraLook : MonoBehaviour
{
    [Header("Sensibilidade")]
    [SerializeField] private float sensibilidadeX = 0.15f;
    [SerializeField] private float sensibilidadeY = 0.15f;

    [Header("Limite Vertical (graus)")]
    [SerializeField] private float limiteMinY = -80f;
    [SerializeField] private float limiteMaxY =  80f;

    // Input
    private InputAction olharAction;
    private InputAction toggleCursorAction;

    // Referência ao corpo do personagem (pai da câmera)
    private Transform corpo;

    // Rotação acumulada
    private float rotacaoVertical = 0f;

    // Estado do cursor
    private bool cursorLivre = false;

    // -------------------------------------------------------

    private void Awake()
    {
        // O corpo é o pai da câmera (o GameObject do personagem)
        corpo = transform.parent;

        if (corpo == null)
            Debug.LogError("[CameraLook] A câmera precisa ser filha do personagem!");

        olharAction = new InputAction("Olhar", binding: "<Mouse>/delta");

        toggleCursorAction = new InputAction("ToggleCursor", binding: "<Keyboard>/v");
        toggleCursorAction.performed += _ => AlternarCursor();
    }

    private void Start()
    {
        TrvarCursor();
        // Começa sem inclinação vertical
        rotacaoVertical = 0f;
    }

    private void OnEnable()
    {
        olharAction.Enable();
        toggleCursorAction.Enable();
    }

    private void OnDisable()
    {
        olharAction.Disable();
        toggleCursorAction.Disable();
    }

    private void OnDestroy()
    {
        olharAction.Dispose();
        toggleCursorAction.Dispose();
    }

    // -------------------------------------------------------

    private void LateUpdate()
    {
        if (cursorLivre) return;

        Vector2 delta = olharAction.ReadValue<Vector2>();

        // --- Vertical: só inclina a câmera ---
        rotacaoVertical -= delta.y * sensibilidadeY;
        rotacaoVertical  = Mathf.Clamp(rotacaoVertical, limiteMinY, limiteMaxY);
        transform.localRotation = Quaternion.Euler(rotacaoVertical, 0f, 0f);

        // --- Horizontal: gira o CORPO do personagem ---
        // Assim o W sempre vai para onde a câmera aponta
        if (corpo != null)
            corpo.Rotate(Vector3.up * delta.x * sensibilidadeX);
    }

    // -------------------------------------------------------

    private void AlternarCursor()
    {
        if (cursorLivre) TrvarCursor();
        else             LiberarCursor();
    }

    private void TrvarCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        cursorLivre      = false;
    }

    private void LiberarCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
        cursorLivre      = true;
    }
}