using PaintTower.Abstract;
using UnityEngine;
using UnityEngine.UI;

namespace PaintTower.Canvas {
    /// <summary>
    /// Classe referente aos objetos de HUD
    /// </summary>
    public class LockScreen : HUD {

        /// <summary>
        /// Texto de carregamento.
        /// </summary>
        public Text loadingText;

        /// <summary>
        /// Máximo de Pontos no carregando
        /// </summary>
        public int maxDots;

        /// <summary>
        /// Quantos por segundo;
        /// </summary>
        public float dotsPerSecond;

        /// <summary>
        /// Texto padrão
        /// </summary>
        private string m_defaultText = "";

        /// <summary>
        /// Tempo Passado
        /// </summary>
        private float timeleft = 0f;

        /// <summary>
        /// Tempo que os pontos foram colocados
        /// </summary>
        private int dotsPlotted = 0;

        /// <summary>
        /// A função Awake da Unity
        /// </summary>
        public void Awake() {
            m_defaultText = loadingText.text;
        }

        public void Update() {
            if (dotsPlotted > maxDots) {
                loadingText.text = m_defaultText;
                timeleft = 0f;
                dotsPlotted = 0;
            }
            else {
                timeleft += Time.deltaTime;

                if (timeleft > dotsPerSecond) {
                    timeleft = 0f;
                    dotsPlotted++;
                    loadingText.text = loadingText.text + ".";
                }
            }
        }

        /// <summary>
        /// Inicializa a HUD
        /// </summary>
        public override void InitializeHUD() {
            base.InitializeHUD();
            loadingText.text = m_defaultText;
            timeleft = 0f;
            dotsPlotted = 0;
        }


        /// <summary>
        /// Desliga a HUD
        /// </summary>
        public override void UnloadHUD() {
            base.UnloadHUD();
        }


    }
}
