using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Sistema de Câmeras de Segurança.
/// 
/// - Aperte B para abrir/fechar o painel de seleção
/// - Aperte B + 1/2/3/4 para trocar de câmera
/// - As câmeras giram automaticamente (patrulha) ou você pode
///   configurá-las para girar apenas manualmente pelo Inspector
///
/// SETUP:
///   1. Crie 4 GameObjects vazios na cena (ex: "Camera1", "Camera2"...)
///      e posicione-os onde quiser as câmeras de segurança.
///   2. Adicione uma Camera component em cada um (ou como filho).
///   3. Arraste-os nos campos Cameras[] deste script.
///   4. Coloque este script num GameObject vazio chamado "CameraSystem".
///   5. A câmera principal do jogador arraste em "Camera Principal".
/// </summary>
public class SecurityCameraSystem : MonoBehaviour
{
    [Header("Câmeras de Segurança (4 slots)")]
    [SerializeField] private Camera[] cameras = new Camera[4];
    [SerializeField] private string[] nomesCameras = { "Local 1", "Local 2", "Local 3", "Local 4" };

    [Header("Câmera Principal do Jogador")]
    [SerializeField] private Camera cameraPrincipal;

    [Header("Rotação das Câmeras")]
    [SerializeField] private float velocidadeRotacao = 30f;   // graus por segundo (patrulha)
    [SerializeField] private float limiteRotacao = 60f;        // graus para cada lado
    [SerializeField] private bool rotacaoAutomatica = true;    // patrulha automática

    [Header("UI — Canvas (criado automaticamente se vazio)")]
    [SerializeField] private Canvas canvasUI;

    // Input
    private InputAction abrirAction;
    private InputAction cam1Action;
    private InputAction cam2Action;
    private InputAction cam3Action;
    private InputAction cam4Action;
    private InputAction fecharAction;

    // Estado
    private bool sistemaAberto = false;
    private int cameraAtiva = -1; // -1 = nenhuma

    // Rotações iniciais de cada câmera
    private float[] rotacoesIniciais;
    private float[] rotacoesAtuais;
    private float[] direcoesPasCameras; // +1 ou -1

    // UI
    private GameObject painelSelecao;
    private GameObject painelCamera;
    private Text textoNomeCamera;
    private Text textoInstrucoes;

    // -------------------------------------------------------

    private void Awake()
    {
        // Salva rotações iniciais
        rotacoesIniciais  = new float[4];
        rotacoesAtuais    = new float[4];
        direcoesPasCameras = new float[4];

        for (int i = 0; i < 4; i++)
        {
            if (cameras[i] != null)
            {
                rotacoesIniciais[i] = cameras[i].transform.eulerAngles.y;
                rotacoesAtuais[i]   = 0f;
                direcoesPasCameras[i] = 1f;

                // Desativa todas as câmeras de segurança no início
                cameras[i].gameObject.SetActive(false);
            }
        }

        ConfigurarInput();
        CriarUI();
    }

    private void ConfigurarInput()
    {
        abrirAction  = new InputAction("AbrirCameras", binding: "<Keyboard>/b");
        cam1Action   = new InputAction("Cam1", binding: "<Keyboard>/1");
        cam2Action   = new InputAction("Cam2", binding: "<Keyboard>/2");
        cam3Action   = new InputAction("Cam3", binding: "<Keyboard>/3");
        cam4Action   = new InputAction("Cam4", binding: "<Keyboard>/4");
        fecharAction = new InputAction("Fechar", binding: "<Keyboard>/escape");

        abrirAction.performed  += _ => ToggleSistema();
        cam1Action.performed   += _ => { if (sistemaAberto) SelecionarCamera(0); };
        cam2Action.performed   += _ => { if (sistemaAberto) SelecionarCamera(1); };
        cam3Action.performed   += _ => { if (sistemaAberto) SelecionarCamera(2); };
        cam4Action.performed   += _ => { if (sistemaAberto) SelecionarCamera(3); };
        fecharAction.performed += _ => { if (sistemaAberto) FecharSistema(); };
    }

    private void OnEnable()
    {
        abrirAction.Enable();
        cam1Action.Enable();
        cam2Action.Enable();
        cam3Action.Enable();
        cam4Action.Enable();
        fecharAction.Enable();
    }

    private void OnDisable()
    {
        abrirAction.Disable();
        cam1Action.Disable();
        cam2Action.Disable();
        cam3Action.Disable();
        cam4Action.Disable();
        fecharAction.Disable();
    }

    private void OnDestroy()
    {
        abrirAction.Dispose();
        cam1Action.Dispose();
        cam2Action.Dispose();
        cam3Action.Dispose();
        cam4Action.Dispose();
        fecharAction.Dispose();
    }

    // -------------------------------------------------------

    private void Update()
    {
        if (!sistemaAberto) return;

        // Rotação automática (patrulha) das câmeras
        for (int i = 0; i < 4; i++)
        {
            if (cameras[i] == null) continue;
            if (!rotacaoAutomatica && i != cameraAtiva) continue;

            rotacoesAtuais[i] += velocidadeRotacao * direcoesPasCameras[i] * Time.deltaTime;

            // Inverte direção ao atingir o limite
            if (rotacoesAtuais[i] >= limiteRotacao)
            {
                rotacoesAtuais[i] = limiteRotacao;
                direcoesPasCameras[i] = -1f;
            }
            else if (rotacoesAtuais[i] <= -limiteRotacao)
            {
                rotacoesAtuais[i] = -limiteRotacao;
                direcoesPasCameras[i] = 1f;
            }

            // Aplica rotação apenas no eixo Y
            cameras[i].transform.rotation = Quaternion.Euler(
                cameras[i].transform.eulerAngles.x,
                rotacoesIniciais[i] + rotacoesAtuais[i],
                0f
            );
        }
    }

    // -------------------------------------------------------

    private void ToggleSistema()
    {
        if (sistemaAberto) FecharSistema();
        else               AbrirSistema();
    }

    private void AbrirSistema()
    {
        sistemaAberto = true;
        painelSelecao.SetActive(true);
        painelCamera.SetActive(false);

        // Libera cursor para interagir com o menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        // Atualiza instrução
        textoInstrucoes.text = "Pressione 1, 2, 3 ou 4 para selecionar a câmera\nESC para fechar";
    }

    private void FecharSistema()
    {
        sistemaAberto = false;
        painelSelecao.SetActive(false);
        painelCamera.SetActive(false);

        // Desativa câmera ativa
        if (cameraAtiva >= 0 && cameras[cameraAtiva] != null)
        {
            cameras[cameraAtiva].gameObject.SetActive(false);
            if (cameraPrincipal != null) cameraPrincipal.gameObject.SetActive(true);
        }

        cameraAtiva = -1;

        // Trava cursor de volta
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void SelecionarCamera(int indice)
    {
        if (indice < 0 || indice >= 4 || cameras[indice] == null)
        {
            Debug.LogWarning($"[SecurityCameraSystem] Câmera {indice + 1} não configurada!");
            return;
        }

        // Desativa câmera anterior
        if (cameraAtiva >= 0 && cameras[cameraAtiva] != null)
            cameras[cameraAtiva].gameObject.SetActive(false);

        // Ativa nova câmera
        cameraAtiva = indice;
        cameras[indice].gameObject.SetActive(true);

        // Desativa câmera do jogador enquanto vê a câmera de segurança
        if (cameraPrincipal != null) cameraPrincipal.gameObject.SetActive(false);

        // Atualiza UI
        painelSelecao.SetActive(false);
        painelCamera.SetActive(true);
        textoNomeCamera.text = nomesCameras[indice];
        textoInstrucoes.text = $"CAM {indice + 1} — {nomesCameras[indice]}\nB = trocar câmera  |  ESC = sair";
    }

    // -------------------------------------------------------
    // Cria a UI em código (sem precisar de Canvas na cena)

    private void CriarUI()
    {
        if (canvasUI == null)
        {
            GameObject canvasGO = new GameObject("SecurityCameraCanvas");
            canvasUI = canvasGO.AddComponent<Canvas>();
            canvasUI.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasUI.sortingOrder = 99;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // ---------- Painel de seleção ----------
        painelSelecao = CriarPainel("PainelSelecao", new Color(0, 0, 0, 0.85f));
        painelSelecao.SetActive(false);

        // Título
        CriarTexto(painelSelecao.transform, "SISTEMA DE CÂMERAS", 28, new Vector2(0, 80),
            new Vector2(500, 50), Color.green, FontStyle.Bold);

        // Botões de câmera
        string[] labels = { "[1] Local 1 / CAM 1", "[2] Local 2 / CAM 2",
                             "[3] Local 3 / CAM 3", "[4] Local 4 / CAM 4" };
        for (int i = 0; i < 4; i++)
            CriarTexto(painelSelecao.transform, labels[i], 20,
                new Vector2(0, 20 - i * 40), new Vector2(400, 35), Color.white);

        textoInstrucoes = CriarTexto(painelSelecao.transform, "", 16,
            new Vector2(0, -120), new Vector2(500, 40), new Color(0.7f, 0.7f, 0.7f));

        // ---------- Painel de câmera ativa ----------
        painelCamera = new GameObject("PainelCameraAtiva");
        painelCamera.transform.SetParent(canvasUI.transform, false);
        painelCamera.SetActive(false);

        // Borda verde estilo câmera de segurança
        CriarBorda();

        // Nome da câmera (canto superior esquerdo)
        textoNomeCamera = CriarTexto(painelCamera.transform, "CAM 1", 18,
            new Vector2(-Screen.width / 2f + 120, Screen.height / 2f - 30),
            new Vector2(200, 30), Color.green, FontStyle.Bold);

        // REC piscando (canto superior direito)
        CriarTexto(painelCamera.transform, "● REC", 16,
            new Vector2(Screen.width / 2f - 60, Screen.height / 2f - 30),
            new Vector2(100, 30), Color.red, FontStyle.Bold);
    }

    private GameObject CriarPainel(string nome, Color cor)
    {
        GameObject go = new GameObject(nome);
        go.transform.SetParent(canvasUI.transform, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(520, 320);
        rt.anchoredPosition = Vector2.zero;
        Image img = go.AddComponent<Image>();
        img.color = cor;
        return go;
    }

    private Text CriarTexto(Transform pai, string conteudo, int tamanho,
        Vector2 pos, Vector2 tamanhoRect, Color cor, FontStyle estilo = FontStyle.Normal)
    {
        GameObject go = new GameObject("Texto_" + conteudo.Substring(0, Mathf.Min(10, conteudo.Length)));
        go.transform.SetParent(pai, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = tamanhoRect;
        rt.anchoredPosition = pos;
        Text txt = go.AddComponent<Text>();
        txt.text = conteudo;
        txt.fontSize = tamanho;
        txt.color = cor;
        txt.fontStyle = estilo;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return txt;
    }

    private void CriarBorda()
    {
        // Cria 4 bordas verdes finas ao redor da tela
        float espessura = 3f;
        Color verde = new Color(0f, 1f, 0f, 0.6f);
        string[] nomes = { "BordaCima", "BordaBaixo", "BordaEsq", "BordaDir" };
        Vector2[] tamanhos = {
            new Vector2(Screen.width, espessura),
            new Vector2(Screen.width, espessura),
            new Vector2(espessura, Screen.height),
            new Vector2(espessura, Screen.height)
        };
        Vector2[] posicoes = {
            new Vector2(0,  Screen.height / 2f - espessura / 2f),
            new Vector2(0, -Screen.height / 2f + espessura / 2f),
            new Vector2(-Screen.width / 2f + espessura / 2f, 0),
            new Vector2( Screen.width / 2f - espessura / 2f, 0)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject borda = new GameObject(nomes[i]);
            borda.transform.SetParent(painelCamera.transform, false);
            RectTransform rt = borda.AddComponent<RectTransform>();
            rt.sizeDelta      = tamanhos[i];
            rt.anchoredPosition = posicoes[i];
            Image img = borda.AddComponent<Image>();
            img.color = verde;
        }
    }
}