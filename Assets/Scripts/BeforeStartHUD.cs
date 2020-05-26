using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeStartHUD : MonoBehaviour
{
    public GameObject dialog;
    public GameObject dialogWarning;

    public void Awake() {
        dialog.SetActive(false);
        dialogWarning.SetActive(false);
    }

    /// <summary>
    /// Inicializa a HUD
    /// </summary>
    public void InitializeHUD() {
        this.gameObject.SetActive(true);
        dialog.SetActive(true);
    }

    /// <summary>
    /// Desliga a HUD
    /// </summary>
    public void UnloadHUD() {
        dialog.SetActive(false);
        dialogWarning.SetActive(false);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Ao clica no botão de Dialogo
    /// </summary>
    public void OnClickDialogOk() {
        dialog.SetActive(false);
        dialogWarning.SetActive(true);

        GameLogic.Instance.enableSelectOnTrackingPlanes = true;
    }


}
