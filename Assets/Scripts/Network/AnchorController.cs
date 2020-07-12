using Google.XR.ARCoreExtensions;
using PaintTower.ARCore;
using PaintTower.Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;

namespace PaintTower.Network {
    /// <summary>
    /// A Controladora da Âncora na Rede
    /// <see cref="ARCloudAnchor"/>.
    /// </summary>
#pragma warning disable 618
    public class AnchorController : NetworkBehaviour
#pragma warning restore 618
    {
        /// <summary>
        /// Tempo customizada para saber quando o tempo de requisitar a âncora passou do tempo necessário
        /// </summary>
        private const float k_ResolvingTimeout = 10.0f;

        /// <summary>
        /// O ID da Âncora <see cref="ARCloudAnchor"/>.
        /// Essa variável é sincronizadas entre todos os clientes
        /// </summary>
#pragma warning disable 618
        [SyncVar(hook = "_OnChangeId")]
#pragma warning restore 618
        private string m_ClouAnchorId = string.Empty;

        /// <summary>
        /// Indica se o script está sendo executado pelo hosst
        /// </summary>
        private bool m_IsHost = false;

        /// <summary>
        /// Indica se precisa realizar um tentativa de recuperar a âncoora
        /// </summary>
        private bool m_ShouldResolve = false;

        /// <summary>
        /// Indica se deve verificar a posição da Âncora na nuvem e atualiza-lo
        /// </summary>
        private bool m_ShouldUpdatePoint = false;

        /// <summary>
        /// Indica o tempo desde que começou a requisitar a âncora
        /// </summary>
        private float m_TimeSinceStartResolving = 0.0f;

        /// <summary>
        /// Indica se passou do tempo de pegar a âncora ou se ela já foi recuperada
        /// </summary>
        private bool m_PassedResolvingTimeout = false;

        /// <summary>
        /// A âncora Local, usada para monitorar o Status
        /// </summary>
        private ARCloudAnchor m_CloudAnchor;

        /// <summary>
        /// The Cloud Anchors example controller.
        /// </summary>
        private ARController m_ar;

        /// <summary>
        /// A controladora de Âncora do AR.
        /// </summary>
        private ARAnchorManager m_AnchorManager;

        /// <summary>
        /// O método Awake() da Unity.
        /// </summary>
        public void Awake() {
            m_ar = GameLogic.Instance.AR;
            m_AnchorManager = m_ar.AnchorManager;

            //Desativa objetos Internos da Âncora
            foreach (Transform t in transform)
                t.gameObject.SetActive(false);
        }

        /// <summary>
        ///O método OnStartClient() da Unity.
        /// </summary>
        public override void OnStartClient() {
            if (m_ClouAnchorId != string.Empty) {
                m_ShouldResolve = true;
            }
        }

        /// <summary>
        /// O método Update() da Unity.
        /// </summary>
        public void Update() {
            if (m_IsHost) {
                if (m_ShouldUpdatePoint) {
                    _UpdateHostedCloudAnchor();
                }
            }
            else {
                //Não precisa se resolver se o player estiver sozinho
                if (!GameLogic.Instance.PlayerIsAlone) {
                    if (m_ShouldResolve) {
                        if (!m_ar.IsResolvingPrepareTimePassed()) {
                            return;
                        }

                        if (!m_PassedResolvingTimeout) {
                            m_TimeSinceStartResolving += Time.deltaTime;

                            if (m_TimeSinceStartResolving > k_ResolvingTimeout) {
                                m_PassedResolvingTimeout = true;
                                m_ar.OnResolvingTimeoutPassed();
                            }
                        }

                        if (!string.IsNullOrEmpty(m_ClouAnchorId) && m_CloudAnchor == null) {
                            _ResolveCloudAnchorId(m_ClouAnchorId);
                        }
                    }

                    if (m_ShouldUpdatePoint) {
                        _UpdateResolvedCloudAnchor();
                    }
                }
            }
        }

        /// <summary>
        /// Roda o Comando para Atualizar Âncora
        /// </summary>
        /// <param name="cloudAnchorId">Novo ID da Âncora</param>
#pragma warning disable 618
        [Command]
#pragma warning restore 618
        public void CmdSetCloudAnchorId(string cloudAnchorId) {
            Debug.Log("ID da Âncora Atualizado para: " + cloudAnchorId);
            m_ClouAnchorId = cloudAnchorId;
        }

        /// <summary>
        /// Hospeda a âncora e associa ela a um ID
        /// </summary>
        /// <param name="anchor">Âncora</param>
        public void HostAnchor(ARAnchor anchor) {
            m_IsHost = true;
            m_ShouldResolve = false;
            transform.SetParent(anchor.transform);

            //Ativa objetos Internos da Âncora
            foreach (Transform t in transform)
                t.gameObject.SetActive(true);

            m_CloudAnchor = m_AnchorManager.HostCloudAnchor(anchor);

            GameLogic.Instance.ShowAndroidToastMessage("Hospendando...");
            if (m_CloudAnchor == null) {

                Debug.LogError("Falha ao Hospedar a Âncora");

                m_ar.OnAnchorHosted(
                    false, "Falha ao Hospedar a Âncora");

                m_ShouldUpdatePoint = false;
            }
            else {
                m_ShouldUpdatePoint = true;
            }
        }

        /// <summary>
        /// Resolve o ID da âncora na nuvem e instancia uma âncora na nuvem nela.
        /// </summary>
        /// <param name="cloudAnchorId">O ID da Ancora</param>
        private void _ResolveCloudAnchorId(string cloudAnchorId) {
            m_ar.OnAnchorInstantiated(false);
            //Pega a âncora
            m_CloudAnchor = m_AnchorManager.ResolveCloudAnchorId(cloudAnchorId);

            GameLogic.Instance.ShowAndroidToastMessage("Recuperando Âncora...");
            //Se Ancora é nula
            if (m_CloudAnchor == null) {
                Debug.LogErrorFormat("O Cliente não pode recuperar a Âncora com ID: {0}.", cloudAnchorId);
                m_ar.OnAnchorResolved(
                    false, "Cliente não pode recuperar a Âncora.");
                m_ShouldResolve = true;
                m_ShouldUpdatePoint = false;
            }
            else {
                m_ShouldResolve = false;
                m_ShouldUpdatePoint = true;
            }
        }

        /// <summary>
        /// Atuliza a Âncora se a hospedam foi bem sucedida
        /// </summary>
        private void _UpdateHostedCloudAnchor() {
            if (m_CloudAnchor == null) {
                Debug.LogError("Não existem âncoras");
                return;
            }

            CloudAnchorState cloudAnchorState = m_CloudAnchor.cloudAnchorState;
            if (cloudAnchorState == CloudAnchorState.Success) {
                CmdSetCloudAnchorId(m_CloudAnchor.cloudAnchorId);
                m_ar.OnAnchorHosted(
                    true, "Hospedagem da âncora foi bem-sucedida");
                m_ShouldUpdatePoint = false;
            }
            else if (cloudAnchorState != CloudAnchorState.TaskInProgress) {
                m_ar.OnAnchorHosted(false,
                    "Falha ao Hospedar a âncora: " + cloudAnchorState);
                m_ShouldUpdatePoint = false;
            }
        }

        /// <summary>
        /// Atuliza a Âncora se o guest conseguiu pegar os dados
        /// </summary>
        private void _UpdateResolvedCloudAnchor() {
            if (m_CloudAnchor == null) {
                Debug.LogError("Não existem âncoras");
                return;
            }

            CloudAnchorState cloudAnchorState = m_CloudAnchor.cloudAnchorState;
            if (cloudAnchorState == CloudAnchorState.Success) {
                transform.SetParent(m_CloudAnchor.transform, false);
                m_ar.OnAnchorResolved(
                    true,
                    "Sucesso ao Recuperar a Âncora.");

                //Define a Origem do Mundo
                m_ar.WorldOrigin = transform;

                //Ativa objetos Internos da Âncora
                foreach (Transform t in transform)
                    t.gameObject.SetActive(true);

                GameObject.Find("LocalPlayer").GetComponent<LocalPlayerController>()
                    .InitializeWorld(transform);

                // Mark resolving timeout passed so it won't fire OnResolvingTimeoutPassed event.
                m_PassedResolvingTimeout = true;
                m_ShouldUpdatePoint = false;
            }
            else if (cloudAnchorState != CloudAnchorState.TaskInProgress) {
                m_ar.OnAnchorResolved(
                    false, "Falha ao recuperar a âncora: " + cloudAnchorState);
                m_ShouldUpdatePoint = false;
            }
        }

        /// <summary>
        /// Função chamada quando o ID da âncora é Alterada
        /// </summary>
        /// <param name="newId">New Cloud Anchor Id.</param>
        private void _OnChangeId(string newId) {
            if (!m_IsHost && newId != string.Empty) {
                m_ClouAnchorId = newId;
                m_ShouldResolve = true;
                m_CloudAnchor = null;
            }
        }
    }
}