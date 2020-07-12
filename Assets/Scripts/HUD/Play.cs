using Assets.Scripts.Network;
using PaintTower.Abstract;
using PaintTower.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PaintTower.Canvas {
    /// <summary>
    /// Classe referente aos objetos de HUD
    /// </summary>
    public class Play : HUD {

        /// <summary>
        /// A imagem do CrossHair
        /// </summary>
        public Image Crosshair;

        /// <summary>
        /// Material do Crosshair quando está pronto para atirar
        /// </summary>
        public Sprite spriteCrosshairIdle;

        /// <summary>
        /// Material do Crosshair quando não está atirando
        /// </summary>
        public Sprite spriteCrosshairShooting;

        /// <summary>
        /// O tempo restante da partida
        /// </summary>
        public Text PlayTimerText;

        /// <summary>
        /// Arma do Player
        /// </summary>
        private GameObject m_gunCamera;


        /// <summary>
        /// Definições da Partida
        /// </summary>
        private MatchState m_matchState;

        private Dictionary<MatchState, GameObject> m_elements = new Dictionary<MatchState, GameObject>();

        public GameObject playerReferecePrefab;

        /// <summary>
        /// Inicializa a HUD
        /// </summary>
        public override void InitializeHUD() {
            base.InitializeHUD();
            m_gunCamera.SetActive(true);
            m_matchState = GameObject.Find("LocalPlayer").GetComponent<MatchState>();

            switch (m_matchState.playerColor) {
                case Enum.Colors.RED: Crosshair.color = new Color(0.98f, 0.51f, 0.2f); break;
                case Enum.Colors.YELLOW: Crosshair.color = new Color(1, 0.85f, 0.28f); break;
                case Enum.Colors.GREEN: Crosshair.color = new Color(0.54f, 0.88f, 0.38f); break;
                case Enum.Colors.BLUE: Crosshair.color = new Color(0.21f, 0.73f, 0.95f); break;
                default:
                    Crosshair.color = Color.white;
                    break;
            }

            MatchState[] matchstates = FindObjectsOfType<MatchState>();

            for (int i = 0; i < matchstates.Length; i++) {
                GameObject go = Instantiate(playerReferecePrefab);
                go.transform.position = matchstates[i].playerCameraPosition;
                m_elements.Add(matchstates[i], go);
            }
            
        }

        /// <summary>
        /// Desliga a HUD
        /// </summary>
        public override void UnloadHUD() {
            m_gunCamera.SetActive(false);
            base.UnloadHUD();
        }

        public void Awake() {
            m_gunCamera = GameLogic.Instance.GunCamera;
        }

        public void Start() {
            Crosshair.sprite = spriteCrosshairIdle;
        }

        public void Update() {
            PlayTimerText.text = ((int)(GameLogic.Instance.MaxMatchTime - m_matchState.matchTime)).ToString() + "s.";

            foreach (KeyValuePair<MatchState, GameObject> entry in m_elements) {
                entry.Value.transform.position = entry.Key.playerCameraPosition;
            }
        }


    }
}
