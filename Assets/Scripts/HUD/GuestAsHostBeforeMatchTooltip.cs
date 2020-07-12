using PaintTower.Abstract;
using UnityEngine;
using UnityEngine.UI;

namespace PaintTower.Canvas {
    /// <summary>
    /// Classe referente aos objetos de HUD
    /// </summary>
    public class GuestAsHostBeforeMatchTooltip : HUD {
        public Text textElement;

        public string Text {
            get { return textElement.text; }
            set { textElement.text = value; }
        }

    }
}
