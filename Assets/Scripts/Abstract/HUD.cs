using UnityEngine;

namespace PaintTower.Abstract {
    /// <summary>
    /// Classe referente aos objetos de HUD
    /// </summary>
    public abstract class HUD : MonoBehaviour{

        /// <summary>
        /// Inicializa a HUD
        /// </summary>
        public virtual void InitializeHUD() {
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Desliga a HUD. Ao usar o Base, colocar sempre no final
        /// </summary>
        public virtual void UnloadHUD() {
            this.gameObject.SetActive(false);
        }
    }
}
