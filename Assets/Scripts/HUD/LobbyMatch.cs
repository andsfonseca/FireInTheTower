using Assets.Scripts.Network;
using PaintTower.Abstract;
using PaintTower.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace PaintTower.Canvas {
    /// <summary>
    /// Classe referente aos objetos de HUD
    /// </summary>
    public class LobbyMatch : HUD {

        /// <summary>
        /// O texto do Lobby
        /// </summary>
        public Text lobbyTitle;

        /// <summary>
        /// Quantidade de Player na partida com a cor vermelho
        /// </summary>
        public Text redLabel;

        /// <summary>
        /// Quantidade de Player na partida com a cor amarelo
        /// </summary>
        public Text yellowLabel;

        /// <summary>
        /// Quantidade de Player na partida com a cor verde
        /// </summary>
        public Text greenLabel;

        /// <summary>
        /// Quantidade de Player na partida com a cor azul
        /// </summary>
        public Text blueLabel;

        /// <summary>
        /// GameObject que indica se o Player selecionou vermelho
        /// </summary>
        public GameObject redImageConfirmation;

        /// <summary>
        /// GameObject que indica se o Player selecionou amarelo
        /// </summary>
        public GameObject yellowImageConfirmation;

        /// <summary>
        /// GameObject que indica se o Player selecionou verde
        /// </summary>
        public GameObject greenImageConfirmation;

        /// <summary>
        /// GameObject que indica se o Player selecionou azul
        /// </summary>
        public GameObject blueImageConfirmation;

        /// <summary>
        /// Texto Padrão do Lobby
        /// </summary>
        private MatchState m_matchState;

        /// <summary>
        /// Texto Padrão do Lobby
        /// </summary>
        private string m_defaultLobbyText;
        /// <summary>
        /// O método Awake() da Unity
        /// </summary>
        public void Awake() {
            m_defaultLobbyText = lobbyTitle.text;
        }

        /// <summary>
        /// Inicializa a HUD
        /// </summary>
        public override void InitializeHUD() {
            base.InitializeHUD();
            m_matchState = GameObject.Find("LocalPlayer").GetComponent<MatchState>();
        }

        /// <summary>
        /// Desliga a HUD
        /// </summary>
        public override void UnloadHUD() {
            base.UnloadHUD();
        }

        /// <summary>
        /// A função Update da Unity
        /// </summary>
        public void Update() {
            //Atualiza o Tempo Restante
            lobbyTitle.text = m_defaultLobbyText + ((int)(GameLogic.Instance.MaxLobbyTime - m_matchState.lobbyTime)).ToString() + "s.";

            //Atualiza as quantidades de player com cada cor selecionadas
            MatchState[] matchstates = FindObjectsOfType<MatchState>();
            int redCount = 0, yellowCount= 0, greenCount= 0, blueCount = 0; 
            for (int i = 0; i < matchstates.Length; i++) {
                switch (matchstates[i].playerColor) {
                    case Enum.Colors.RED: redCount++; break;
                    case Enum.Colors.YELLOW: yellowCount++; break;
                    case Enum.Colors.GREEN: greenCount++; break;
                    case Enum.Colors.BLUE: blueCount++; break;
                }
            }

            redLabel.text = redCount.ToString();
            yellowLabel.text = yellowCount.ToString();
            greenLabel.text = greenCount.ToString();
            blueLabel.text = blueCount.ToString();
        }

        /// <summary>
        /// Ao clicar na Opção Vermelho
        /// </summary>
        public void OnClickRedButton() {
            redImageConfirmation.SetActive(true);
            yellowImageConfirmation.SetActive(false);
            greenImageConfirmation.SetActive(false);
            blueImageConfirmation.SetActive(false);

            m_matchState.SetColor(Enum.Colors.RED);
        }

        /// <summary>
        /// Ao clicar na Opção Vermelho
        /// </summary>
        public void OnClickYellowButton() {
            redImageConfirmation.SetActive(false);
            yellowImageConfirmation.SetActive(true);
            greenImageConfirmation.SetActive(false);
            blueImageConfirmation.SetActive(false);

            m_matchState.SetColor(Enum.Colors.YELLOW);
        }

        /// <summary>
        /// Ao clicar na Opção Vermelho
        /// </summary>
        public void OnClickGreenButton() {
            redImageConfirmation.SetActive(false);
            yellowImageConfirmation.SetActive(false);
            greenImageConfirmation.SetActive(true);
            blueImageConfirmation.SetActive(false);

            m_matchState.SetColor(Enum.Colors.GREEN);
        }

        /// <summary>
        /// Ao clicar na Opção Vermelho
        /// </summary>
        public void OnClickBlueButton() {
            redImageConfirmation.SetActive(false);
            yellowImageConfirmation.SetActive(false);
            greenImageConfirmation.SetActive(false);
            blueImageConfirmation.SetActive(true);

            m_matchState.SetColor(Enum.Colors.BLUE);
        }

    }
}
