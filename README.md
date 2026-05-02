# 🎮 Unity 6 — Player Movement + Security Camera System

Scripts prontos para Unity 6.3 LTS (6000.3.10f1) usando o **novo Input System**.

---

## 📁 Scripts

| Arquivo | Descrição |
|---|---|
| `PlayerController.cs` | Movimento WASD, corrida (Shift) e pulo (Espaço) |
| `CameraLook.cs` | Câmera em primeira pessoa com mouse. Tecla **V** solta/trava o cursor |
| `SecurityCameraSystem.cs` | Sistema de câmeras de segurança. Tecla **B** abre o menu, **1/2/3/4** troca de câmera, **ESC** fecha |

---

## 🗂️ Hierarquia da Cena

```
SampleScene
├── GameObject                  ← Script SecurityCameraSystem.cs aqui
│   ├── local1                  ← Modelo 3D (parede, poste, estrutura...)
│   │   └── Camera1             ← Camera component aqui (câmera de segurança 1)
│   ├── local2
│   │   └── Camera2
│   ├── local3
│   │   └── Camera3
│   └── local4
│       └── Camera4
│
└── personagem                  ← Scripts PlayerController.cs aqui
    ├── CharacterController     ← Componente do Unity (adicionado automaticamente)
    └── Camera                  ← Scripts CameraLook.cs aqui (câmera do jogador)
```

---

## ⚙️ Setup — Passo a Passo

### 1. Requisitos

- Unity **6.3 LTS (6000.3.10f1)**
- Package **Input System** instalado  
  `Window → Package Manager → Input System → Install`
- Em `Edit → Project Settings → Player → Active Input Handling` selecione **Input System Package (New)**

---

### 2. PlayerController.cs

1. Coloque o script no GameObject do **personagem**
2. O `CharacterController` é adicionado automaticamente pelo `[RequireComponent]`
3. No Inspector, ajuste os valores se quiser:

| Campo | Padrão | Descrição |
|---|---|---|
| Velocidade | `5` | Velocidade de caminhada |
| Velocidade Corrida | `10` | Velocidade ao segurar Shift |
| Força Pulo | `6` | Altura do pulo |
| Gravidade | `-25` | Intensidade da queda |

> ✅ **Não precisa configurar Layer** — a detecção de chão funciona automaticamente com qualquer Collider.

---

### 3. CameraLook.cs

1. Coloque o script na **Camera** filha do personagem
2. Posicione a câmera em `Y = 1.7` (altura dos olhos), `X = 0`, `Z = 0`
3. Nenhuma referência precisa ser arrastada — o script detecta o pai (personagem) automaticamente

| Campo | Padrão | Descrição |
|---|---|---|
| Sensibilidade X | `0.15` | Velocidade horizontal do mouse |
| Sensibilidade Y | `0.15` | Velocidade vertical do mouse |
| Limite Min Y | `-80` | Máximo para olhar para baixo |
| Limite Max Y | `80` | Máximo para olhar para cima |

**Controles:**
- **Mouse** → move a câmera
- **V** → solta/trava o cursor

---

### 4. SecurityCameraSystem.cs

1. Crie um **GameObject vazio** na cena (ex: `GameObject`)
2. Coloque o script `SecurityCameraSystem.cs` nele
3. Crie os locais das câmeras — cada `local` pode ser um modelo 3D (parede, poste, canto de sala...)
4. Dentro de cada `local`, crie um GameObject com o componente **Camera** (`Camera1`, `Camera2`...)
5. No Inspector do `GameObject`, preencha os campos:

| Campo | O que arrastar |
|---|---|
| **Cameras → Element 0** | `Camera1` (filha do `local1`) |
| **Cameras → Element 1** | `Camera2` (filha do `local2`) |
| **Cameras → Element 2** | `Camera3` (filha do `local3`) |
| **Cameras → Element 3** | `Camera4` (filha do `local4`) |
| **Camera Principal** | `Camera` (filha do `personagem`) |
| **Nomes Cameras** | Nomes dos locais (ex: "Entrada", "Corredor", "Sala", "Quintal") |

> ⚠️ Em cada `Camera1/2/3/4`, desative o componente **Audio Listener** — só o personagem precisa ter um ativo.

**Parâmetros de rotação:**

| Campo | Padrão | Descrição |
|---|---|---|
| Velocidade Rotacao | `30` | Graus por segundo na patrulha |
| Limite Rotacao | `60` | Máximo de graus para cada lado |
| Rotacao Automatica | ✅ | Câmera faz patrulha automática |

**Controles:**
- **B** → abre o menu de seleção
- **1 / 2 / 3 / 4** → seleciona a câmera (enquanto o menu está aberto)
- **B** (de novo) → volta ao menu para trocar de câmera
- **ESC** → fecha o sistema e volta para o personagem

---

## 🎮 Controles Resumidos

| Tecla | Ação |
|---|---|
| `W A S D` | Mover personagem |
| `Shift Esquerdo` | Correr |
| `Espaço` | Pular |
| `Mouse` | Mover câmera |
| `V` | Soltar / travar cursor |
| `B` | Abrir sistema de câmeras |
| `1` `2` `3` `4` | Selecionar câmera de segurança |
| `ESC` | Fechar sistema de câmeras |

---

## 📦 Dependências

- [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest)  
  Instale via `Window → Package Manager`

---

## 📝 Licença

Livre para uso pessoal e comercial. Créditos são bem-vindos mas não obrigatórios.
