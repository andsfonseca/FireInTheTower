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

        /// <summary>
        /// Definições da Partida
        /// </summary>
        private MatchState m_matchState;

        /// <summary>
        /// Inicializa a HUD
        /// </summary>
        public override void InitializeHUD() {
            base.InitializeHUD();

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

            if (m_barProgress < 1) {

                m_barProgress += Time.deltaTime * barVelocity;

                if (m_barProgress > 1)
                    m_barProgress = 1;

                if (m_barProgress < 0.5) {
                    redBar.anchorMax = new Vector2(m_barProgress, 1);
                }
                else {
                    redBar.anchorMax = new Vector2(0.5f, 1);
                }
                
                if (m_barProgress < 0.6) {
                    yellowBar.anchorMax = new Vector2(m_barProgress, 1);
                }
                else {
                    yellowBar.anchorMax = new Vector2(0.6f, 1);
                }

                if (m_barProgress < 0.7) {
                    greenBar.anchorMax = new Vector2(m_barProgress, 1);
                }
                else {
                    greenBar.anchorMax = new Vector2(0.7f, 1);
                }

                if (m_barProgress < 0.8) {
                    blueBar.anchorMax = new Vector2(m_barProgress, 1);
                }
                else {
                    blueBar.anchorMax = new Vector2(0.8f, 1);
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
