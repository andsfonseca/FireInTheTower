
using Assets.Scripts.Network;
using PaintTower.Canvas;
using PaintTower.Enum;
using PaintTower.Network;
using PaintTower.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace PaintTower.ARCore {
    /// <summary>
    /// Controladora do ARCore
    /// </summary>
    public class ARController : MonoBehaviour {
        [Header("AR Foundation")]

        /// <summary>
        /// AR Session Origin Ativa na cena
        /// </summary>
        public ARSessionOrigin SessionOrigin;

        /// <summary>
        /// Sessão do Ar Core
        /// </summary>
        public GameObject SessionCore;

        /// <summary>
        /// Extensões do AR
        /// </summary>
        public GameObject ARExtentions;

        /// <summary>
        /// Controladora de Ancoras do ARCore
        /// </summary>
        public ARAnchorManager AnchorManager;

        /// <summary>
        /// Controladora de Planos do ARCore
        /// </summary>
        public ARPlaneManager ARPlaneManager;

        /// <summary>
        /// Controladora de Raycast do ARCore
        /// </summary>
        public ARRaycastManager RaycastManager;

        /// <summary>
        /// A controladora de Network
        /// </summary>
        private CustomNetworkManager m_NetworkManager;

        /// <summary>
        /// The key name used in PlayerPrefs which indicates whether
        /// the start info has displayed at least one time.
        /// </summary>
        private const string k_HasDisplayedStartInfoKey = "HasDisplayedStartInfo";

        /// <summary>
        /// The time between room starts up and ARCore session starts resolving.
        /// </summary>
        private const float k_ResolvingPrepareTime = 3.0f;

        /// <summary>
        /// Record the time since the room started. If it passed the resolving prepare time,
        /// applications in resolving mode start resolving the anchor.
        /// </summary>
        private float m_TimeSinceStart = 0.0f;

        /// <summary>
        /// Indicates whether passes the resolving prepare time.
        /// </summary>
        private bool m_PassedResolvingPreparedTime = false;

        /// <summary>
        /// Indicates whether the Anchor was already instantiated.
        /// </summary>
        private bool m_AnchorAlreadyInstantiated = false;

        /// <summary>
        /// Indicates whether the Cloud Anchor finished hosting.
        /// </summary>
        private bool m_AnchorFinishedHosting = false;

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error,
        /// otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        /// <summary>
        /// The world origin transform for this session.
        /// </summary>
        private Transform m_WorldOrigin = null;

        /// <summary>
        /// Indica se o ponto de Origin foi setado por alguma âncora
        /// </summary>
        public bool IsOriginPlaced {
            get;
            private set;
        }

        /// <summary>
        /// Recupera a Origem do Mundo, se o valor for atualizado, as posições são recalculadas
        /// </summary>
        public Transform WorldOrigin {
            get {
                return m_WorldOrigin;
            }

            set {
                IsOriginPlaced = true;
                m_WorldOrigin = value;

                Pose sessionPose = _ToWorldOriginPose(new Pose(SessionOrigin.transform.position,
                    SessionOrigin.transform.rotation));
                SessionOrigin.transform.SetPositionAndRotation(
                    sessionPose.position, sessionPose.rotation);
            }
        }

        /// <summary>
        /// Ativa ou Desativa a visão em AR
        /// </summary>
        /// <param name="value">Ativar ou desativar</param>
        public void ActivateARView(bool value) {

            SessionCore.SetActive(value);
            ARExtentions.SetActive(value);
            m_TimeSinceStart = 0.0f;
        }

        ///// <summary>
        ///// Callback handling Start Now button click event.
        ///// </summary>
        //public void OnStartNowButtonClicked() {
        //    _SwitchActiveScreen(ActiveScreen.ARScreen);
        //}

        ///// <summary>
        ///// Callback handling Learn More Button click event.
        ///// </summary>
        //public void OnLearnMoreButtonClicked() {
        //    Application.OpenURL(
        //        "https://developers.google.com/ar/cloud-anchors-privacy");
        //}

        /// <summary>
        /// O método Start() da Unity.
        /// </summary>
        public void Start() {
#pragma warning disable 618
            m_NetworkManager = GameLogic.Instance.networkManager;
#pragma warning restore 618

            PerformReset();
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update() {

            //Se ele está no Menu Principal
            if (GameLogic.Instance.CurrentGameState == GameState.MENU) {
                return;
            }

            //Se a aplicação não é nem Host ou Guest
            if (GameLogic.Instance.CurrentApplicationMode != ApplicationMode.Hosting &&
                GameLogic.Instance.CurrentApplicationMode != ApplicationMode.Resolving) {
                return;
            }

            // Se o modo atual é de Guest, espera um tempo até as mensagens serem lidas para poder avançar
            if (GameLogic.Instance.CurrentApplicationMode == ApplicationMode.Resolving && !m_PassedResolvingPreparedTime) {
                m_TimeSinceStart += Time.deltaTime;

                if (m_TimeSinceStart > k_ResolvingPrepareTime) {
                    m_PassedResolvingPreparedTime = true;
                    GameLogic.Instance.ShowAndroidToastMessage("Aguardando o Host selecionar o local da partida...");
                }
            }

            // Se vocé é o Guest e a posição ainda não foi definida, então Ok
            if (!GameLogic.Instance.PlayerIsAlone && (GameLogic.Instance.CurrentApplicationMode == ApplicationMode.Resolving) && !IsOriginPlaced) {
                return;
            }

            if (!(GameLogic.Instance.CurrentGameState == GameState.HOST_PREPARATION || GameLogic.Instance.CurrentGameState == GameState.GUEST_ALONE_PREPARATION))
                return;

            // Se o Player não tocou na tela, então OK
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) {
                return;
            }

            // Se o Player tocou na UI, então OK
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
                return;
            }

            //Lista de RaycastHis
            List<ARRaycastHit> hitResults = new List<ARRaycastHit>();

            //Carrega os resultados dos hits na lista a partir da posição que ele tocou
            RaycastManager.Raycast(Input.GetTouch(0).position, hitResults);

            //Se ele tocou em algum lugar
            if (hitResults.Count > 0) {

                //Se o Host ainda não definiu o local de Origem
                if (!IsOriginPlaced) {

                    //Cria uma Ancora
                    ARAnchor anchor = AnchorManager.AddAnchor(hitResults[0].pose);
                    WorldOrigin = anchor.transform;

                    if (GameLogic.Instance.CurrentGameState == GameState.HOST_PREPARATION) {
                        _InstantiateAnchor(anchor);
                        OnAnchorInstantiated(true);
                    }
                    else if (GameLogic.Instance.CurrentGameState == GameState.GUEST_ALONE_PREPARATION) {
                        GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
                            .InitializeWorld(anchor.transform);

                        OnAnchorInstantiated(false);

                        SetPlaneVisualization(false);

                        GameLogic.Instance.GuestAsHostBeforeMatchTooltip.UnloadHUD();
                        GameLogic.Instance.SetGameState(GameState.LOBBY);
                        GameObject.Find("LocalPlayer").GetComponent<MatchState>().ActiveLobby();
                    }

                }
            }
        }

        /// <summary>
        /// Indica se o tempo de preparação do convidado passou para que o AnchorController comece a resolver a âncora.
        /// </summary>
        /// <returns><c>true</c>, se o tempo passou, senão <c>false</c>.
        /// </returns>
        public bool IsResolvingPrepareTimePassed() {
            return GameLogic.Instance.CurrentApplicationMode != ApplicationMode.Ready &&
                m_TimeSinceStart > k_ResolvingPrepareTime;
        }


        /// <summary>
        /// Callback called when the resolving timeout is passed.
        /// </summary>
        public void OnResolvingTimeoutPassed() {
            if (GameLogic.Instance.CurrentApplicationMode == ApplicationMode.Ready) {
                Debug.LogWarning("Só pode ser chamado quando o Jogador está trackeando");
                return;
            }

            GameLogic.Instance.ShowAndroidToastMessage("Tentando encontrar a âncora... Certifique-se de apontar para o mesmo local do Host");
        }

        /// <summary>
        /// Executado Quando uma âncora é instaciada
        /// made.
        /// </summary>
        /// <param name="isHost">Indica se é um Host</param>
        public void OnAnchorInstantiated(bool isHost) {
            if (m_AnchorAlreadyInstantiated) {
                return;
            }

            m_AnchorAlreadyInstantiated = true;
        }

        /// <summary>
        /// Executado quando a Âncora for hospedada
        /// </summary>
        /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was hosted
        /// successfully.</param>
        /// <param name="response">The response string received.</param>
        public void OnAnchorHosted(bool success, string response) {
            m_AnchorFinishedHosting = success;
            if (!success) {
                GameLogic.Instance.ShowAndroidToastMessage(response);
            }
            else {
                SetPlaneVisualization(false);
                GameLogic.Instance.HostBeforeMatchTooltip.UnloadHUD();
                GameLogic.Instance.SetGameState(GameState.LOBBY);
                GameObject.Find("LocalPlayer").GetComponent<MatchState>().ActiveLobby();
            }

        }

        /// <summary>
        /// Executada quando a âncora for recuperada
        /// </summary>
        /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was resolved
        /// successfully.</param>
        /// <param name="response">The response string received.</param>
        public void OnAnchorResolved(bool success, string response) {
            m_AnchorFinishedHosting = success;
            if (!success) {
                GameLogic.Instance.ShowAndroidToastMessage(response);
            }
            else {
                SetPlaneVisualization(false);
                GameLogic.Instance.GuestBeforeMatchTooltip.UnloadHUD();
                GameLogic.Instance.SetGameState(GameState.LOBBY);
            }
        }

        /// <summary>
        /// Atualiza a Origem com a posição definida
        /// </summary>
        /// <param name="pose"></param>
        /// <returns></returns>
        private Pose _ToWorldOriginPose(Pose pose) {
            if (!IsOriginPlaced) {
                return pose;
            }

            Matrix4x4 anchorTWorld = Matrix4x4.TRS(
                m_WorldOrigin.position, m_WorldOrigin.rotation, Vector3.one).inverse;
            Quaternion rotation = Quaternion.LookRotation(
                anchorTWorld.GetColumn(2),
                anchorTWorld.GetColumn(1));
            return new Pose(
                anchorTWorld.MultiplyPoint(pose.position),
                pose.rotation * rotation);
        }

        /// <summary>
        /// Instância uma Ancora de Origem.
        /// </summary>
        /// <param name="anchor">O objeto ARAnchor que possui a âncora</param>
        private void _InstantiateAnchor(ARAnchor anchor) {

            (GameLogic.Instance.HostBeforeMatchTooltip as HostBeforeMatchTooltip).Text = "Tentando colocar a cidade no chão...";

            // The anchor will be spawned by the host, so no networking Command is needed.
            GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
                .SpawnAnchor(anchor);

        }

        /// <summary>
        /// Resets the internal status.
        /// </summary>
        public void PerformReset() {
            m_WorldOrigin = null;
            IsOriginPlaced = false;
        }

        public void ResetTimeSinceStart() {
            m_TimeSinceStart = 0.0f;
        }

        public void SetPlaneVisualization(bool value) {
            ARPlaneManager.enabled = value;

            foreach (ARPlane plane in ARPlaneManager.trackables)
                plane.gameObject.SetActive(false);
        }

    }
}

