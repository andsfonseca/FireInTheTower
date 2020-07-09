﻿using Assets.Scripts.Network;
using PaintTower.Abstract;
using PaintTower.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace PaintTower.Canvas {
    /// <summary>
    /// Classe referente aos objetos de HUD
    /// </summary>
    public class Play : HUD {

        /// <summary>
        /// A imagem do CrossHair
        /// </summary>
        public Image Crosshair;

        /// <summary>
        /// Material do Crosshair quando está pronto para atirar
        /// </summary>
        public Sprite spriteCrosshairIdle;

        /// <summary>
        /// Material do Crosshair quando não está atirando
        /// </summary>
        public Sprite spriteCrosshairShooting;

        /// <summary>
        /// Quantidade de Player na partida com a cor verde
        /// </summary>
        public Text greenLabel;


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

            switch (m_matchState.playerColor) {
                case Enum.Colors.RED: Crosshair.color = new Color(0.98f, 0.51f, 0.2f); break;
                case Enum.Colors.YELLOW: Crosshair.color = new Color(1, 0.85f, 0.28f); break;
                case Enum.Colors.GREEN: Crosshair.color = new Color(0.54f, 0.88f, 0.38f); break;
                case Enum.Colors.BLUE: Crosshair.color = new Color(0.21f, 0.73f, 0.95f); break;
                default:
                    Crosshair.color = Color.white;
                    break;
            }
        }

        /// <summary>
        /// Desliga a HUD
        /// </summary>
        public override void UnloadHUD() {
            base.UnloadHUD();
        }

        public void Start() {
            Crosshair.sprite = spriteCrosshairIdle;
        }


    }
}
