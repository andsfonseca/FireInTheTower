using PaintTower.Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintTower.Scripts {
    public class GlobalEnvironment : MonoBehaviour {
        /// <summary>
        /// Define a música a ser tocada no Ambiente
        /// </summary>
        public AudioClip menuClip;

        /// <summary>
        /// Recurso de Música
        /// </summary>
        private AudioSource m_audioSource;

        // Start is called before the first frame update
        void Awake() {
            m_audioSource = gameObject.GetComponentInChildren<AudioSource>();
        }

        /// <summary>
        /// Define a Música de Ambiente a ser tocada
        /// </summary>
        /// <param name="state">Game State que define a música</param>
        /// <param name="PlayOnSet">Se deve executar logo em seguida</param>
        public void SetEnvironmentMusic(GameState state, bool PlayOnSet = false) {
            AudioClip clip = null;

            switch (state) {
                case GameState.MENU:
                    clip = menuClip;
                    break;
            }

            if (clip != null && clip != m_audioSource.clip) { 
                m_audioSource.Stop();
                m_audioSource.clip = clip;

                if (PlayOnSet) {
                    m_audioSource.Play();
                }
            }
        }

    }
}
