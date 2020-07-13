using Assets.Scripts.Network;
using PaintTower.Abstract;
using PaintTower.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace PaintTower.Canvas {
    /// <summary>
    /// Classe referente aos objetos de HUD
    /// </summary>
    public class GameOver : HUD {
        private float redEnd = 0;
        private float yellowEnd = 0;
        private float greenEnd = 0;
        private float blueEnd = 0;
        public PaintCalculation[] objectsToPaint { private get; set; }
        /// <summary>
        /// Barra vermelha
        /// </summary>
        public RectTransform redBar;

        /// <summary>
        /// Barra Amarela
        /// </summary>
        public RectTransform yellowBar;

        /// <summary>
        /// Barra verde
        /// </summary>
        public RectTransform greenBar;

        /// <summary>
        /// Barra Azul
        /// </summary>
        public RectTransform blueBar;

        /// <summary>
        /// Velocidade de Progresso da Barra
        /// </summary>
        public float barVelocity;

        private float m_barProgress;

        private float delay = 2f;
        /// <summary>
        /// Definições da Partida
        /// </summary>
        private MatchState m_matchState;

        /// <summary>
        /// Inicializa a HUD
        /// </summary>
        public override void InitializeHUD() {
            base.InitializeHUD();
            m_matchState = GameObject.Find("LocalPlayer").GetComponent<MatchState>();
            m_barProgress = 0;

            redBar.anchorMax = new Vector2(0, 1);
            yellowBar.anchorMax = new Vector2(0, 1);
            greenBar.anchorMax = new Vector2(0, 1);
            blueBar.anchorMax = new Vector2(0, 1);

            redBar.offsetMin = Vector2.zero;
            redBar.offsetMax = Vector2.zero;
            yellowBar.offsetMin = Vector2.zero;
            yellowBar.offsetMax = Vector2.zero;
            greenBar.offsetMin = Vector2.zero;
            greenBar.offsetMax = Vector2.zero;
            blueBar.offsetMin = Vector2.zero;
            blueBar.offsetMax = Vector2.zero;

        }


        /// <summary>
        /// Desliga a HUD
        /// </summary>
        public override void UnloadHUD() {
            base.UnloadHUD();
        }

        public void Start() {
            InitializeHUD();
        }

        public void Update() {


            if (delay > 0) {
                delay -= Time.deltaTime;
                return;
            }

            float[] percents = m_matchState.percents;
            redEnd = percents[0];
            yellowEnd = percents[1];
            greenEnd = percents[2];
            blueEnd = percents[3];

            if (m_barProgress < 1) {

                m_barProgress += Time.deltaTime * barVelocity;

                if (m_barProgress > 1)
                    m_barProgress = 1;

                if (m_barProgress < redEnd) {
                    redBar.anchorMax = new Vector2(m_barProgress, 1);
                }
                else {
                    redBar.anchorMax = new Vector2(redEnd, 1);
                }
                
                if (m_barProgress < yellowEnd) {
                    yellowBar.anchorMax = new Vector2(m_barProgress, 1);
                }
                else {
                    yellowBar.anchorMax = new Vector2(yellowEnd, 1);
                }

                if (m_barProgress < greenEnd) {
                    greenBar.anchorMax = new Vector2(m_barProgress, 1);
                }
                else {
                    greenBar.anchorMax = new Vector2(greenEnd, 1);
                }

                if (m_barProgress < blueEnd) {
                    blueBar.anchorMax = new Vector2(m_barProgress, 1);
                }
                else {
                    blueBar.anchorMax = new Vector2(blueEnd, 1);
                }

            }

            //Reseta o Offset

            redBar.offsetMin = Vector2.zero;
            redBar.offsetMax = Vector2.zero;
            yellowBar.offsetMin = Vector2.zero;
            yellowBar.offsetMax = Vector2.zero;
            greenBar.offsetMin = Vector2.zero;
            greenBar.offsetMax = Vector2.zero;
            blueBar.offsetMin = Vector2.zero;
            blueBar.offsetMax = Vector2.zero;
        }

        public void OnClickBackToMenu() {
            UnloadHUD();
            GameLogic.Instance.SetGameState(Enum.GameState.MENU);
        }

        
    }
}
