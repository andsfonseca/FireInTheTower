using PaintTower.Abstract;
using PaintTower.Enum;
using PaintTower.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

namespace PaintTower.Canvas {
    /// <summary>
    /// Classe referente aos objetos de HUD
    /// </summary>
    public class Menu : HUD {

        /// <summary>
        /// Dialogo do Menu
        /// </summary>
        [Header("Elementos")]
        public GameObject dialogMenu;

        /// <summary>
        /// Dialogo do Lobby
        /// </summary>
        public GameObject dialogLobby;

        /// <summary>
        /// Background Animado
        /// </summary>
        public GameObject animatedBackground;

        /// <summary>
        /// Item do Lobby
        /// </summary>
        [SerializeField]
        public List<GameObject> itemLobbyRoom;

        /// <summary>
        /// Velocidade da Animação pela Esquerda e Direita
        /// </summary>
        [Header("Animações")]
        public float leftAnimationVelocity;

        /// <summary>
        /// Velocidade da Animação por Cima e por Baixo
        /// </summary>
        public float topAnimationVelocity;



        /// <summary>
        /// Orientação dos lados esquerdo e direito
        /// </summary>
        private int m_leftOrientation;

        /// <summary>
        /// Orientação dos lados de cima e de baixo
        /// </summary>
        private float m_topOrientation;

        /// <summary>
        /// Posição Esquerda
        /// </summary>
        private float m_leftPosition = -200f;

        /// <summary>
        /// Posição Top
        /// </summary>
        private float m_topPosition = -200f;

        /// <summary>
        /// Transform do Background
        /// </summary>
        private RectTransform m_backgroudAnimationRect;

        /// <summary>
        /// Animações do Menu
        /// </summary>
        private Animator[] m_animator;

        /// <summary>
        /// Network Manager Principal
        /// </summary>
#pragma warning disable 618 
        private NetworkManager m_networkManager;
#pragma warning restore 618

        /// <summary>
        /// Lista de Partidas
        /// </summary>
#pragma warning disable 618
        private List<MatchInfoSnapshot> m_matches;
#pragma warning restore 618

        /// <summary>
        /// O método Awake() da Unity
        /// </summary>
        public void Awake() {
            dialogMenu.SetActive(false);
            dialogLobby.SetActive(false);
            m_animator = animatedBackground.GetComponentsInChildren<Animator>();
            m_backgroudAnimationRect = animatedBackground.GetComponent<RectTransform>();
        }

        /// <summary>
        /// O método Start() da Unity
        /// </summary>
        private void Start() {
            m_networkManager = GameLogic.Instance.networkManager;
        }

        /// <summary>
        /// O método Update() da Unity
        /// </summary>
        private void Update() {

            if (m_leftOrientation == 1) {
                m_leftPosition += Time.deltaTime * Random.Range(15, 30);
                if (m_leftPosition > 0) {
                    m_leftPosition = 0;
                    m_leftOrientation = -1;
                }
            }
            else {
                m_leftPosition -= Time.deltaTime * Random.Range(15, 30);
                if (m_leftPosition < -200) {
                    m_leftPosition = -200;
                    m_leftOrientation = 1;
                }

            }

            if (m_topOrientation == 1) {
                m_topPosition += Time.deltaTime * Random.Range(15, 50);
                if (m_topPosition > 0) {
                    m_topPosition = 0;
                    m_topOrientation = -1;
                }
            }
            else {
                m_topPosition -= Time.deltaTime * Random.Range(15, 50);
                if (m_topPosition < -200) {
                    m_topPosition = -200;
                    m_topOrientation = 1;
                }

            }

            m_backgroudAnimationRect.offsetMin = new Vector2(m_leftPosition, m_topPosition);
            m_backgroudAnimationRect.offsetMax = new Vector2(200 + m_leftPosition, 200 + m_topPosition);
        }

        /// <summary>
        /// Inicializa a HUD
        /// </summary>
        public override void InitializeHUD() {
            base.InitializeHUD();
            dialogMenu.SetActive(true);

            foreach (Animator animator in m_animator) {
                animator.Play("Fade", animator.GetLayerIndex("Fade"), Random.Range(0.0f, 1.0f));
            }
        }

        /// <summary>
        /// Desliga a HUD
        /// </summary>
        public override void UnloadHUD() {
            dialogMenu.SetActive(false);
            dialogLobby.SetActive(false);
            base.UnloadHUD();
        }

        /// <summary>
        /// Ao clica no botão de Dialogo
        /// </summary>
        public void OnClickPlayButton() {
            dialogLobby.SetActive(true);
            OnClickRefreshButton();
        }

        /// <summary>
        /// Ao clica no botão de Dialogo
        /// </summary>
        public void OnClickExitButton() {
            Application.Quit();
        }

        /// <summary>
        /// Ao clica no botão de Dialogo
        /// </summary>
        public void OnClickCloseButton() {
            dialogLobby.SetActive(false);
        }
        /// <summary>
        /// Ao clica no botão de Dialogo
        /// </summary>
        public void OnClickRefreshButton() {

            m_networkManager.matchMaker.ListMatches(
                startPageNumber: 0,
                resultPageSize: 4,
                matchNameFilter: string.Empty,
                filterOutPrivateMatchesFromResults: false,
                eloScoreTarget: 0,
                requestDomain: 0,
                callback: _OnMatchList);
        }

        /// <summary>
        /// Ao clica no botão de Dialogo
        /// </summary>
        public void OnClickCreateButton() {
            GameLogic.Instance.LockScreenHUD.InitializeHUD();
            //Cria a partida
            m_networkManager.matchMaker.CreateMatch(
                m_networkManager.matchName, m_networkManager.matchSize, true, string.Empty, string.Empty,
                string.Empty, 0, 0, _OnMatchCreate);
        }

        /// <summary>
        /// Callback executado quando a a função <see cref="NetworkMatch.CreateMatch"/> foi processada pelo servidor
        /// </summary>
        /// <param name="success">Informa se teve sucesso</param>
        /// <param name="extendedInfo">Um texto com a descrição de erro caso a criação falhe</param>
        /// <param name="matchInfo">Informações sobre a partida.</param>
#pragma warning disable 618
        private void _OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
#pragma warning restore 618
        {
            if (!success) {
                GameLogic.Instance.ShowAndroidToastMessage("Não foi possível criar a partida: " + extendedInfo);
                return;
            }

            m_networkManager.OnMatchCreate(success, extendedInfo, matchInfo);
            GameLogic.Instance.RoomNumber = _GetRoomNumberFromNetworkId(matchInfo.networkId);
            GameLogic.Instance.ShowAndroidToastMessage("Conectando ao servidor...");

            UnloadHUD();
            GameLogic.Instance.SetGameState(GameState.HOST_WARNING);
        }

        /// <summary>
        /// Callback executado quando <see cref="NetworkMatch.ListMatches"/> foi processado pelo servidor
        /// </summary>
        /// <param name="success">Indica se a requisão aconteceu</param>
        /// <param name="extendedInfo">A text description for the error if success is false.</param>
        /// <param name="matches">A list of matches corresponding to the filters set in the initial
        /// list request.</param>
#pragma warning disable 618
        private void _OnMatchList(
            bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
#pragma warning restore 618
        {
            if (!success) {
                GameLogic.Instance.ShowAndroidToastMessage("Não foi possível listar as partida: " + extendedInfo);
                return;
            }

            m_networkManager.OnMatchList(success, extendedInfo, matches);

            if (m_networkManager.matches != null) {

                //Checar se existem salas
                //NoPreviousRoomsText.gameObject.SetActive(m_Manager.matches.Count == 0);
                m_matches = matches;
                foreach (var item in itemLobbyRoom) {
                    item.SetActive(false);
                }

                int count = matches.Count;
                if (itemLobbyRoom.Count < count) {
                    count = itemLobbyRoom.Count;
                }

                for (int i = 0; i < count; i++) {
                    GameObject item = itemLobbyRoom[i];

                    var match = m_matches[i];

                    item.SetActive(true);

                    item.GetComponentInChildren<Text>().text = "Sala: " + _GetRoomNumberFromNetworkId(match.networkId);
                    Button button = item.GetComponentInChildren<Button>();

                    button.onClick.AddListener(() => _OnJoinRoomClicked(match));

                    button.onClick.AddListener( GameLogic.Instance.OnEnterResolvingModeClick);
                }

            }
        }

        /// <summary>
        /// Executado quando o usuário apertar o botão de entrar em uma partida
        /// </summary>
        /// <param name="match">As informações sobre a partida </param>
#pragma warning disable 618
        private void _OnJoinRoomClicked(MatchInfoSnapshot match)
#pragma warning restore 618
        {
            GameLogic.Instance.LockScreenHUD.InitializeHUD();

            m_networkManager.matchName = match.name;
            m_networkManager.matchMaker.JoinMatch(match.networkId, string.Empty, string.Empty,
                                         string.Empty, 0, 0, _OnMatchJoined);
        }

        /// <summary>
        /// Callback executado quando <see cref="NetworkMatch.JoinMatch"/> é requistado pelo servidor
        /// </summary>
        /// <param name="success">Informa o sucesso da operação</param>
        /// <param name="extendedInfo">O texto caso tenha algum erro</param>
        /// <param name="matchInfo">Informações da partida</param>
#pragma warning disable 618
        private void _OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
#pragma warning restore 618
        {
            if (!success) {
                GameLogic.Instance.ShowAndroidToastMessage("Não foi possível listar as partida: " + extendedInfo);
                return;
            }

            m_networkManager.OnMatchJoined(success, extendedInfo, matchInfo);
            GameLogic.Instance.RoomNumber = _GetRoomNumberFromNetworkId(matchInfo.networkId);
            GameLogic.Instance.ShowAndroidToastMessage("Conectando ao servidor...");

            UnloadHUD();
            GameLogic.Instance.SetGameState(GameState.GUEST_WARNING);
        }


        /// <summary>
        /// Calcula o ID da Sala
        /// </summary>
        /// <param name="networkID">Identificador da Network</param>
        /// <returns>String com o  número da sala</returns>
        private string _GetRoomNumberFromNetworkId(NetworkID networkID) {
            return (System.Convert.ToInt64(networkID.ToString()) % 10000).ToString();
        }

    }
}
