using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour {

    /// <summary>
    /// HUD Inicial
    /// </summary>
    public BeforeStartHUD beforeStartHUD;

    public GameObject TestObject;

    /// <summary>
    /// Indica se é permitido inicializar a seleção do ponto a partir do rastreamento do ARCore
    /// </summary>
    [HideInInspector]
    public bool enableSelectOnTrackingPlanes = false;

    /// <summary>
    /// Armazena a Instância atual da GameLogic
    /// </summary>
    private static GameLogic _instance;

    /// <summary>
    /// Contém todos os BlockBuilds cadastrados na cena
    /// </summary>
    private static List<BlockBuild> blockBuilds = new List<BlockBuild>();

    /// <summary>
    /// Instância da GameLogic
    /// </summary>
    public static GameLogic Instance
    {
        get
        {
            if (_instance == null)
            {
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
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
            {
                Destroy(this.gameObject);
            }
        }
    }

    /// <summary>
    /// Registra um Block Build
    /// </summary>
    /// <param name="bb">Block Build Script</param>
    public void RegisterBlockBuilds(BlockBuild bb) {
        blockBuilds.Add(bb);
    }

    /// <summary>
    /// Desregistra um BlockBuild
    /// </summary>
    /// <param name="bb">Block Build Script</param>
    public void UnregisterBlockBuilds(BlockBuild bb) {
        blockBuilds.Remove(bb);
    }

    /// <summary>
    /// Avança para um determinado game state
    /// </summary>
    /// <param name="mode">Nome de Referência do GameState</param>
    public void SetGameMode(string mode) {
        switch (mode) {
            case "BeforeStart": {
                beforeStartHUD.InitializeHUD();
                break;
            }
            case "Basic": {
                //Inicializa o Block Build
                foreach (BlockBuild bb in blockBuilds)
                    bb.Create();
                break;
            }
        }
    }

    void Start() {
        SetGameMode("BeforeStart");
    }

    private void Update() {
        _UpdateApplicationLifecycle();
    }

    /// <summary>
    /// Checa e Atuliza se o ciclo dá aplicação está OK
    /// </summary>
    private void _UpdateApplicationLifecycle() {
        //Sai da Aplicação se estiver OK
        if (Input.GetKey(KeyCode.Escape)) {
            _DoQuit();
        }

        //Se a tela para não dormir enquanto está rastreando
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
            _ShowAndroidToastMessage("É preciso dar permissões da câmera para iniciar esta aplicação!");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError()) {
            _ShowAndroidToastMessage(
                "Não foi possível usar os recursos do Ar Core. Tente novamente");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void _DoQuit() {
        Application.Quit();
    }

    /// <summary>
    /// Envia uma mensagem para o Usuário atravês do Toast
    /// </summary>
    /// <param name="message">Corpo da Mensagem.</param>
    private void _ShowAndroidToastMessage(string message) {
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



}
