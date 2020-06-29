using GoogleARCore;
using GoogleARCore.Examples.CloudAnchors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;

/// <summary>
/// Enumerates modes the example application can be in.
/// </summary>
public enum ApplicationMode {
    /// <summary>
    /// Enume mode that indicate the example application is ready to host or resolve.
    /// </summary>
    Ready,

    /// <summary>
    /// Enume mode that indicate the example application is hosting cloud anchors.
    /// </summary>
    Hosting,

    /// <summary>
    /// Enume mode that indicate the example application is resolving cloud anchors.
    /// </summary>
    Resolving,
}

public class GameLogic : MonoBehaviour {

    /// <summary>
    /// HUD Inicial do Menu
    /// </summary>
    public MainMenuHUD MainMenuHUD;

    /// <summary>
    /// HUD de Inicio do Host
    /// </summary>
    public WarningHUD beforeHostingHUD;  

    /// HUD de Inicio do Host
    /// </summary>
    public WarningHUD beforeGuestEnterHUD;

    /// <summary>
    /// Template padrão da Cidade
    /// </summary>
    public GameObject BasicCityTemplate;

    /// <summary>
    /// Controlador do ARCore
    /// </summary>
    public ARController AR;

    /// <summary>
    /// Controladora de Network do ARCore
    /// </summary>
    public CloudAnchorsNetworkManager NetworkManager;

    /// <summary>
    /// The current cloud anchor mode.
    /// </summary>
    private ApplicationMode m_CurrentMode = ApplicationMode.Ready;

    /// <summary>
    /// Informa se é possível selecionar um plano para exibição
    /// </summary>
    private bool isSelectTrackingPlanAvaible = false;

    /// <summary>
    /// Informa o atual GameState
    /// </summary>
    private string currentGameState = "";
    public string GameState { get { return currentGameState; } } 
    public bool CanSeePlane {
        get {
            //return currentGameState == "BeforeStart";
            return true;
        }
    }

    public ApplicationMode ApplicationMode { get { return m_CurrentMode; } }

    /// <summary>
    /// Indica se é permitido inicializar a seleção do ponto a partir do rastreamento do ARCore
    /// </summary>
    public bool SelectOnTrackingPlanes {
        get {
            return isSelectTrackingPlanAvaible;
        }
        set {
            isSelectTrackingPlanAvaible = value;
            OnTrackingStatusChange(value);
        }
    }

    public string LobbyNumber { get; set; } = "";

    /// <summary>
    /// Armazena a Instância atual da GameLogic
    /// </summary>
    private static GameLogic _instance;

    /// <summary>
    /// Contém todos os BlockBuilds cadastrados na cena
    /// </summary>
    private static List<BuildGenerator> blockBuilds = new List<BuildGenerator>();

    /// <summary>
    /// Instância da GameLogic
    /// </summary>
    public static GameLogic Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<GameLogic>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    /// <summary>
    /// Flag usada para indicar que a aplicação está em processo de encerramento
    /// </summary>
    private bool m_IsQuitting = false;

    /// <summary>
    /// Ao acordar
    /// </summary>
    void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            if (this != _instance) {
                Destroy(this.gameObject);
            }
        }
    }

    /// <summary>
    /// Registra um Block Build
    /// </summary>
    /// <param name="bb">Block Build Script</param>
    public void RegisterBlockBuilds(BuildGenerator bb) {
        blockBuilds.Add(bb);
    }

    /// <summary>
    /// Desregistra um BlockBuild
    /// </summary>
    /// <param name="bb">Block Build Script</param>
    public void UnregisterBlockBuilds(BuildGenerator bb) {
        blockBuilds.Remove(bb);
    }

    /// <summary>
    /// Avança para um determinado game state
    /// </summary>
    /// <param name="mode">Nome de Referência do GameState</param>
    public void SetGameState(string mode) {
        switch (mode) {
            case "Menu": {
                MainMenuHUD.InitializeHUD();
                break;
            }
            case "BeforeHosting": {
                beforeHostingHUD.InitializeHUD();
                break;
            }
            case "BeforeGuestEnter": {
                beforeGuestEnterHUD.InitializeHUD();
                break;
            }
            case "Basic": {
                //Coloca o template da Cidade
                GameObject basic = Instantiate(BasicCityTemplate, Vector3.zero, Quaternion.identity);
                basic.transform.SetParent(AR.GlobalAR.transform, false);

                //Inicializa o Block Build
                foreach (BuildGenerator bb in blockBuilds)
                    bb.Create();
                break;
            }
        }

        currentGameState = mode;
    }

    void Start() {

        NetworkManager.OnClientConnected += _OnConnectedToServer;
        NetworkManager.OnClientDisconnected += _OnDisconnectedFromServer;

        NetworkManager.StartMatchMaker();
        NetworkManager.matchMaker.ListMatches(
            startPageNumber: 0,
            resultPageSize: 5,
            matchNameFilter: string.Empty,
            filterOutPrivateMatchesFromResults: false,
            eloScoreTarget: 0,
            requestDomain: 0,
            callback: _OnMatchList);

        SetGameState("Menu");
    }

    private void Update() {
        _UpdateApplicationLifecycle();
    }

    /// <summary>
    /// S
    /// </summary>
    /// <param name="value"></param>
    private void OnTrackingStatusChange(bool value) {

        //Se estou no gameState de Inicio
        if (currentGameState == "BeforeHosting") {
            //Se acabou de ficar falso, significa que o usuário selecionou o local
            if (!value) {
                SetGameState("Basic");
            }
        }
    }

    /// <summary>
    /// Checa e Atuliza se o ciclo dá aplicação está OK
    /// </summary>
    private void _UpdateApplicationLifecycle() {
        //Sai da Aplicação se estiver OK
        if (Input.GetKey(KeyCode.Escape)) {
            _DoQuit();
        }

        //Enquanto a tela estiver rastreando, o sistema não pode dormir
        if (Session.Status != SessionStatus.Tracking) {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
        else {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting) {
            return;
        }

        // Verifica as permissões, se estiver tudo OK, perde para Sair
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted) {
            ShowAndroidToastMessage("É preciso dar permissões da câmera para iniciar esta aplicação!");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError()) {
            ShowAndroidToastMessage(
                "Não foi possível usar os recursos do Ar Core. Tente novamente");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Fecha a aplicação
    /// </summary>
    private void _DoQuit() {
        Application.Quit();
    }

    /// <summary>
    /// Envia uma mensagem para o Usuário atravês do Toast
    /// </summary>
    /// <param name="message">Corpo da Mensagem.</param>
    public void ShowAndroidToastMessage(string message) {
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

    /// <summary>
    /// Handles user intent to enter a mode where they can place an anchor to host or to exit
    /// this mode if already in it.
    /// </summary>
    public void OnEnterHostingModeClick() {
        if (m_CurrentMode == ApplicationMode.Hosting) {
            m_CurrentMode = ApplicationMode.Ready;
            AR.ResetStatus();
            Debug.Log("Reset ApplicationMode from Hosting to Ready.");
        }

        m_CurrentMode = ApplicationMode.Hosting;
    }

    /// <summary>
    /// Handles a user intent to enter a mode where they can input an anchor to be resolved or
    /// exit this mode if already in it.
    /// </summary>
    public void OnEnterResolvingModeClick() {
        if (m_CurrentMode == ApplicationMode.Resolving) {
            m_CurrentMode = ApplicationMode.Ready;
            AR.ResetStatus();
            Debug.Log("Reset ApplicationMode from Resolving to Ready.");
        }

        m_CurrentMode = ApplicationMode.Resolving;
    }
    /// <summary>
    /// Callback that happens when the client successfully connected to the server.
    /// </summary>
    private void _OnConnectedToServer() {
        if (m_CurrentMode == ApplicationMode.Hosting) {
            ShowAndroidToastMessage("Encontre um Plano");
        }
        else if (m_CurrentMode == ApplicationMode.Resolving) {
            ShowAndroidToastMessage("Visualize um Plano");
        }
        else {
            //_ReturnToLobbyWithReason(
            //    "Connected to server with neither Hosting nor Resolving" +
            //    "mode. Please start the app again.");
        }
    }

    /// <summary>
    /// Callback that happens when the client disconnected from the server.
    /// </summary>
    private void _OnDisconnectedFromServer() {
        //_ReturnToLobbyWithReason("Network session disconnected! " +
        //    "Please start the app again and try another room.");
    }

    /// <summary>
    /// Callback that happens when a <see cref="NetworkMatch.ListMatches"/> request has been
    /// processed on the server.
    /// </summary>
    /// <param name="success">Indicates if the request succeeded.</param>
    /// <param name="extendedInfo">A text description for the error if success is false.</param>
    /// <param name="matches">A list of matches corresponding to the filters set in the initial
    /// list request.</param>
#pragma warning disable 618
    private void _OnMatchList(
        bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
#pragma warning restore 618
        {
        if (!success) {
            ShowAndroidToastMessage("Could not list matches: " + extendedInfo);
            return;
        }

        NetworkManager.OnMatchList(success, extendedInfo, matches);

        if (NetworkManager.matches != null) {
            //// Reset all buttons in the pool.
            //foreach (GameObject button in m_JoinRoomButtonsPool) {
            //    button.SetActive(false);
            //    button.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            //    button.GetComponentInChildren<Text>().text = string.Empty;
            //}

            //NoPreviousRoomsText.gameObject.SetActive(m_Manager.matches.Count == 0);

            List<MatchInfoSnapshot > matchInfoSnapshots = new List< MatchInfoSnapshot>();
           
        // Add buttons for each existing match.
        int i = 0;
#pragma warning disable 618
            foreach (var match in NetworkManager.matches)
#pragma warning restore 618
                {
                if (i >= 5) {
                    break;
                }

                matchInfoSnapshots.Add(match);
                
            }

            MainMenuHUD.CreateLobbyList(matchInfoSnapshots);
        }
    }

}
