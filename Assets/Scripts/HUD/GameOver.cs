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
            float[] percents = calcPercent();
            redEnd = percents[0];
            yellowEnd = percents[1];
            greenEnd = percents[2];
            blueEnd = percents[3];
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

        private float[] calcPercent() {
            float totalRedCount = 0;
            float totalYellowCount = 0;
            float totalGreenCount = 0;
            float totalBlueCount = 0;
            float redPercent = 0;
            float yellowPercent = 0;
            float greenPercent = 0;
            float bluePercent = 0;
            foreach (PaintCalculation towerPiece in objectsToPaint) {
                int[] temp;
                temp = towerPiece.calcPaint();
                totalRedCount += temp[0];
                totalYellowCount += temp[1];
                totalGreenCount += temp[2];
                totalBlueCount += temp[3];
            }
            float total = 0;
            total = totalRedCount + totalYellowCount + totalGreenCount + totalBlueCount;
            if (total != 0) {
                if (totalRedCount != 0) {
                    redPercent = totalRedCount / total;
                }
                if (totalYellowCount != 0) {
                    yellowPercent = totalYellowCount / total;
                }
                if (totalGreenCount != 0) {
                    greenPercent = totalGreenCount / total;
                }
                if (totalBlueCount != 0) {
                    bluePercent = totalBlueCount / total;
                }
            }
            float[] percents = { redPercent, yellowPercent, greenPercent, bluePercent };
            return percents;
            //Debug.Log("BIG PERCENT DEBUG ==> RED: " + redPercent + " YELLOW: " + yellowPercent + " GREEN: " + greenPercent + " BLUE: " + bluePercent + " TOTAL: " + total);
            //Debug.Log("BUG COUNT ==> RED: " + totalRedCount + " YELLOW: " + totalYellowCount + " GREEN: " + totalGreenCount + " BLUE: " + totalBlueCount + " TOTAL: " + total); 
        }
    }
}
