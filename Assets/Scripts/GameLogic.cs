using PaintTower.Canvas;
using PaintTower.ARCore;
using PaintTower.Enum;
using PaintTower.Network;
using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using PaintTower.Abstract;
using Assets.Scripts.Network;
using PaintTower.Painting;

namespace PaintTower.Scripts {

    /// <summary>
    /// Classe contém a lógica do jogo
    /// </summary>
    public class GameLogic : MonoBehaviour {
        /// <summary>
        /// Controladora de Network
        /// </summary>
        [Header("Componentes")]
        public CustomNetworkManager networkManager;

        /// <summary>
        /// Controladora do AR
        /// </summary>
        public ARController AR;

        /// <summary>
        /// Controladora do Camera
        /// </summary>
        public GameObject Camera;

        /// <summary>
        /// HUD do Menu
        /// </summary>
        [Header("HUD")]
        public HUD MenuHUD;

        /// <summary>
        /// HUD do Aviso de Host via Cloud
        /// </summary>
        public HUD HostingWarningHUD;

        /// <summary>
        /// HUD do Aviso do convidado via Cloud
        /// </summary>
        public HUD GuestResolvingWarningHUD;

        /// <summary>
        /// Interface Antes da Partida do Host
        /// </summary>
        public HUD HostBeforeMatchTooltip;

        /// <summary>
        /// Interface Antes da Partida do Guest
        /// </summary>
        public HUD GuestBeforeMatchTooltip;

        /// <summary>
        /// Interface Antes da Partida do Guest ao estar sozinho
        /// </summary>
        public HUD GuestAsHostBeforeMatchTooltip;

        /// <summary>
        /// Interface do Lobby
        /// </summary>
        public HUD LobbyMatch;

        /// <summary>
        /// Interface do Play
        /// </summary>
        public HUD Play;

        /// <summary>
        /// Interface do Play
        /// </summary>
        public HUD GameOver;

        /// <summary>
        /// HUD de Loading
        /// </summary>
        public HUD LockScreenHUD;

        /// <summary>
        /// Configurações de Ambiente
        /// </summary>
        [Header("Configurações")]
        public GlobalEnvironment globalEnvironment;

        /// <summary>
        /// Tempo necessário para o usuário confirmar a saída
        /// </summary>
        public float MaxLobbyTime;

        /// <summary>
        /// Tempo necessário para o usuário confirmar a saída
        /// </summary>
        public float MaxMatchTime;

        /// <summary>
        /// Tempo necessário para o usuário confirmar a saída
        /// </summary>
        public float TimeToConfirmExit;

        /// <summary>
        /// GameState Atual
        /// </summary>
        public GameState CurrentGameState { get; private set; }

        /// <summary>
        /// Modo de Aplicação Atual
        /// </summary>
        public ApplicationMode CurrentApplicationMode { get; set; } = ApplicationMode.Ready;

        /// <summary>
        /// Indica se o Player está jogando sozinho
        /// </summary>
        public bool PlayerIsAlone { get; private set; } = false;

        /// <summary>
        /// Sala Atual da Partida
        /// </summary>
        public string RoomNumber { get; set; } = "";

        /// <summary>
        /// Armazena a Instância atual da GameLogic
        /// </summary>
        private static GameLogic m_instance;

        /// <summary>
        /// Informa se a aplicação está em processo de saída
        /// otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        /// <summary>
        /// Informa se a aplicação está em processo de saída
        /// otherwise false.
        /// </summary>
        private bool m_UserForceQuitting = false;

        /// <summary>
        /// Informa se a aplicação está em processo de saída
        /// otherwise false.
        /// </summary>
        private float m_UserForceQuittingTime = 0;


        /// <summary>
        /// Instância da GameLogic
        /// </summary>
        public static GameLogic Instance {
            get {
                if (m_instance == null) {
                    m_instance = GameObject.FindObjectOfType<GameLogic>();
                    DontDestroyOnLoad(m_instance.gameObject);
                }

                return m_instance;
            }
        }

        /// <summary>
        /// Método Start() da Unity
        /// </summary>
        void Start() {
            networkManager.OnClientConnected += _OnConnectedToServer;
            networkManager.OnClientDisconnected += _OnDisconnectedFromServer;
            SetGameState(GameState.MENU);

            networkManager.StartMatchMaker();
        }

        // <summary>
        /// Método Update() da Unity
        /// </summary>
        void Update() {

            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (m_UserForceQuitting) {
                    DoQuit();
                }
                else {
                    m_UserForceQuitting = true;
                    ShowAndroidToastMessage("Pressione novamente para sair");
                }
            }

            if (m_UserForceQuitting) {
                m_UserForceQuittingTime += Time.deltaTime;
            }

            if (m_UserForceQuittingTime > TimeToConfirmExit) {
                m_UserForceQuitting = false;
                m_UserForceQuittingTime = 0f;
            }

        }

        /// <summary>
        /// Envia uma mensagem para o Usuário atravês do Toast
        /// </summary>
        /// <param name="message">Corpo da Mensagem.</param>
        public void ShowAndroidToastMessage(string message) {
#if UNITY_ANDROID
            try {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject unityActivity =
                    unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                if (unityActivity != null) {
                    AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                    unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                        AndroidJavaObject toastObject =
                            toastClass.CallStatic<AndroidJavaObject>(
                                "makeText", unityActivity, message, 0);
                        toastObject.Call("show");
                    }));
                }

            }
            catch (Exception e) {
                Debug.Log(message);
            }

#endif
#if !UNITY_ANDROID
            Debug.Log(message);
#endif
        }

        /// <summary>
        /// Define o GameState Atual do Jogo
        /// </summary>
        /// <param name="gamestate">Tipo de GameState</param>
        public void SetGameState(GameState gamestate) {
            switch (gamestate) {
                case GameState.MENU: {
                    MenuHUD.InitializeHUD();
                    globalEnvironment.SetEnvironmentMusic(gamestate, true);
                    PerformReset();
                    break;
                }
                case GameState.GUEST_WARNING: {
                    LockScreenHUD.UnloadHUD();
                    GuestResolvingWarningHUD.InitializeHUD();
                    CurrentApplicationMode = ApplicationMode.Resolving;
                    AR.ActivateARView(true);
                    AR.ResetTimeSinceStart();
                    break;
                }
                case GameState.HOST_WARNING: {
                    LockScreenHUD.UnloadHUD();
                    HostingWarningHUD.InitializeHUD();
                    CurrentApplicationMode = ApplicationMode.Hosting;
                    AR.ActivateARView(true);
                    AR.ResetTimeSinceStart();
                    break;
                }
                case GameState.HOST_PREPARATION: {
                    HostBeforeMatchTooltip.InitializeHUD();
                    break;
                }
                case GameState.GUEST_PREPARATION: {
                    GuestBeforeMatchTooltip.InitializeHUD();
                    break;
                }
                case GameState.GUEST_ALONE_PREPARATION: {
                    PlayerIsAlone = true;
                    GuestAsHostBeforeMatchTooltip.InitializeHUD();
                    break;
                }
                case GameState.LOBBY: {
                    LobbyMatch.InitializeHUD();
                    break;
                }
                case GameState.PLAY: {
                    Colors playerColor = GameObject.Find("LocalPlayer").GetComponent<MatchState>().playerColor;
                    (GameOver as GameOver).objectsToPaint = GameObject.FindObjectsOfType(typeof(PaintCalculation)) as PaintCalculation[];

                    switch (playerColor) {
                        case Colors.RED: Camera.GetComponent<ClickScript>().ColorProjectile = new Color(0.98f, 0.51f, 0.2f); break;
                        case Colors.YELLOW: Camera.GetComponent<ClickScript>().ColorProjectile = new Color(1, 0.85f, 0.28f); break;
                        case Colors.GREEN: Camera.GetComponent<ClickScript>().ColorProjectile = new Color(0.54f, 0.88f, 0.38f); break;
                        case Colors.BLUE: Camera.GetComponent<ClickScript>().ColorProjectile = new Color(0.21f, 0.73f, 0.95f); break;
                    }

                    Play.InitializeHUD();
                    break;
                }
                case GameState.GAMEOVER: {
                    GameOver.InitializeHUD();
                    break;
                }
            }

            CurrentGameState = gamestate;
        }

        /// <summary>
        /// Função executada quando conectado ao Servidor
        /// </summary>
        private void _OnConnectedToServer() {
            if (CurrentApplicationMode == ApplicationMode.Hosting) {
                //NetworkUIController.ShowDebugMessage(
                //    "Find a plane, tap to create a Cloud Anchor.");
            }
            else if (CurrentApplicationMode == ApplicationMode.Resolving) {
                //NetworkUIController.ShowDebugMessage(
                //    "Look at the same scene as the hosting phone.");
            }
            else {
                //_ReturnToLobbyWithReason(
                //    "Connected to server with neither Hosting nor Resolving" +
                //    "mode. Please start the app again.");
            }
        }

        /// <summary>
        ///Função executada quando desconectado do Servidor
        /// </summary>
        private void _OnDisconnectedFromServer() {
            //_ReturnToLobbyWithReason("Network session disconnected! " +
            //    "Please start the app again and try another room.");
        }

        /// <summary>
        /// Executado quando o jogo entra em modo de Host
        /// </summary>
        public void OnEnterHostingModeClick() {
            if (CurrentApplicationMode == ApplicationMode.Hosting) {
                PerformReset();
            }
            //Informa a GameLogic que aplicação está rodando em Modo de Host
            CurrentApplicationMode = ApplicationMode.Hosting;
        }

        /// <summary>
        /// Executado quando o jogo entra em modo de Guest
        /// </summary>
        public void OnEnterResolvingModeClick() {
            if (CurrentApplicationMode == ApplicationMode.Resolving) {
                PerformReset();
            }

            //Informa a GameLogic que aplicação está rodando em Modo de Guest
            CurrentApplicationMode = ApplicationMode.Resolving;
        }


        /// <summary>
        /// Reseta a Aplicação
        /// </summary>
        private void PerformReset() {
            CurrentApplicationMode = ApplicationMode.Ready;
            PlayerIsAlone = false;
            AR.PerformReset();
        }

        /// <summary>
        /// Ciclo de Vida da Aplicação
        /// </summary>
        private void _UpdateApplicationLifecycle() {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape)) {
                Application.Quit();
            }

            var sleepTimeout = SleepTimeout.NeverSleep;
            if (ARSession.state != ARSessionState.SessionTracking) {
                sleepTimeout = SleepTimeout.SystemSetting;
            }

            Screen.sleepTimeout = sleepTimeout;

            if (m_IsQuitting) {
                return;
            }

            if (ARSession.state == ARSessionState.Unsupported) {
                _QuitWithReason("Aparelho não compatível com o ARCore");
            }
        }

        /// <summary>
        /// Sai da Aplicação Mostrando um toast
        /// </summary>
        /// <param name="reason">A Razão da saída</param>
        private void _QuitWithReason(string reason) {
            if (m_IsQuitting) {
                return;
            }

            ShowAndroidToastMessage(reason);
            m_IsQuitting = true;
            Invoke("DoQuit", 5.0f);
        }

        /// <summary>
        /// Sai da Aplicação
        /// </summary>
        public void DoQuit() {
            Application.Quit();
        }
    }
}