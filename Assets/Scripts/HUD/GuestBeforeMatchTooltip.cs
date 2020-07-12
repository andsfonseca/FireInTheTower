using PaintTower.Abstract;
using PaintTower.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace PaintTower.Canvas {
    /// <summary>
    /// Classe referente aos objetos de HUD
    /// </summary>
    public class GuestBeforeMatchTooltip : HUD {

        /// <summary>
        /// Dialogo de Confirmação
        /// </summary>
        public GameObject confirmationDialog;

        /// <summary>
        /// Dialogo de Confirmação
        /// </summary>
        public GameObject tooltip;

        /// <summary>
        /// Dialogo de Confirmação
        /// </summary>
        public GameObject buttonDefinePosition;


        public Text textElement;

        /// <summary>
        /// Inicializa a HUD
        /// </summary>
        public override void InitializeHUD() {
            base.InitializeHUD();
            tooltip.SetActive(true);
            buttonDefinePosition.SetActive(true);
            confirmationDialog.SetActive(false);
        }

        /// <summary>
        /// Desliga a HUD
        /// </summary>
        public override void UnloadHUD() {
            confirmationDialog.SetActive(false);
            tooltip.SetActive(false);
            buttonDefinePosition.SetActive(false);
            base.UnloadHUD();
        }

        public string Text {
            get { return textElement.text; }
            set { textElement.text = value; }
        }

        public void OnClickDefinePositionButton() {
            confirmationDialog.SetActive(true);
            tooltip.SetActive(false);
            buttonDefinePosition.SetActive(false);
        }

        public void OnClickCancelButton() {
            confirmationDialog.SetActive(false);
            tooltip.SetActive(true);
            buttonDefinePosition.SetActive(true);
        }

        public void OnClickConfirmDefinePositionButton() {
            UnloadHUD();
            GameLogic.Instance.SetGameState(Enum.GameState.GUEST_ALONE_PREPARATION);
        }

    }
}
