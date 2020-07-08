using PaintTower.Abstract;
using PaintTower.Scripts;
using UnityEngine;


namespace PaintTower.Canvas {
    /// <summary>
    /// Classe referente aos objetos de HUD
    /// </summary>
    public class HostingWarning : HUD {

        /// <summary>
        /// Dialogo do Aviso
        /// </summary>
        [Header("Elementos")]
        public GameObject dialog;

        /// <summary>
        /// O método Awake() da Unity
        /// </summary>
        public void Awake() {
            dialog.SetActive(false);
        }

        /// <summary>
        /// Inicializa a HUD
        /// </summary>
        public override void InitializeHUD() {
            base.InitializeHUD();
            dialog.SetActive(true);

        }

        /// <summary>
        /// Desliga a HUD
        /// </summary>
        public override void UnloadHUD() {
            dialog.SetActive(false);
            base.UnloadHUD();
        }

        /// <summary>
        /// Ao clica no botão de Dialogo
        /// </summary>
        public void OnClickConfirmButton() {
            UnloadHUD();
            (GameLogic.Instance.HostBeforeMatchTooltip as HostBeforeMatchTooltip).Text = "Selecione uma área para colocar a cidade.";
            GameLogic.Instance.SetGameState(Enum.GameState.HOST_PREPARATION);
        }

    }
}
