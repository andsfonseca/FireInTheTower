using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningHUD : MonoBehaviour {
    public GameObject dialog;

    public void Awake() {
        dialog.SetActive(false);
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
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Ao clica no botão de Dialogo
    /// </summary>
    public void OnClickDialogOk() {
        UnloadHUD();

        if (GameLogic.Instance.ApplicationMode == ApplicationMode.Hosting)
            GameLogic.Instance.SelectOnTrackingPlanes = true;
    }


}
