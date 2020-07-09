using PaintTower.Canvas;
using PaintTower.Enum;
using PaintTower.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Network {
    /// <summary>
    /// Configurações da Partida Atual
    /// </summary>
#pragma warning disable 618
    class MatchState : NetworkBehaviour {
#pragma warning restore 618

        /// <summary>
        /// 
        /// </summary>
        //Alguma HUD

#pragma warning disable 618
        [HideInInspector]
        /// <summary>
        /// O Tempo decorrido desde que o lobby começou
        /// </summary>
        [SyncVar] public float lobbyTime = 0f;
        [HideInInspector]
        /// <summary>
        /// Se Esta classe pertence ao HOST
        /// </summary>
        [SyncVar] public bool isHost = false;
        [HideInInspector]
        /// <summary>
        /// Se o Lobby está aberto
        /// </summary>
        [SyncVar] public bool LobbyIsOpen = false;

#pragma warning restore 618

        /// <summary>
        /// A cor do Player
        /// </summary>
        public Colors playerColor = Colors.WHITE;

        MatchState Instance;

        void Start() {
            //Unicamente do Servidor
            if (isServer) {
                if (isLocalPlayer) {
                    Instance = this;
                    isHost = true;
                }
            }
            //Unicamente de Player Locais
            else if (isLocalPlayer) {

                MatchState[] matchstates = FindObjectsOfType<MatchState>();
                for (int i = 0; i < matchstates.Length; i++) {

                    //Sincronização do Tempo
                    if (matchstates[i].isHost) {
                        Instance = matchstates[i];
                    }
                }

            }

        }

        void Update() {
            //Apenas o Host
            if (isHost) {
                //Se o Lobby está aberto
                if (LobbyIsOpen) {

                    //Atualiza o tempo do Lobby
                    lobbyTime += Time.deltaTime;
                }
            }

            //Todos podem fazer isso
            if (isLocalPlayer) {

                //Só deve executar se dono da sessão estiver online
                if (Instance) {
                    //Atualiza o tempo
                    lobbyTime = Instance.lobbyTime;

                }
            }

            if (lobbyTime > GameLogic.Instance.MaxLobbyTime) {
                GameLogic.Instance.LobbyMatch.UnloadHUD();
                GameLogic.Instance.SetGameState(GameState.PLAY);
            }
        }

        public void ActiveLobby() {
            if (isHost) {
                lobbyTime = 0f;
                LobbyIsOpen = true;
            }
        }

        /// <summary>
        /// Altera a cor do Player
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Colors color) {
            if (isLocalPlayer) {
                CmdSetColor(color);
            }
        }

        /// <summary>
        /// Comando de Alteraçã de Cor vinda do Cliente
        /// </summary>
        /// <param name="color"></param>
#pragma warning disable 618
        [ClientRpc]
#pragma warning restore 618
        void RpcPaint(Colors color) {
            playerColor = color;
        }

        /// <summary>
        /// Comando de Alterar cor
        /// </summary>
        /// <param name="color">Qual a cor</param>
#pragma warning disable 618
        [Command]
#pragma warning restore 618
        void CmdSetColor(Colors color) {
            //Pega a Indentidade
#pragma warning disable 618
            var identity = GetComponent<NetworkIdentity>(); 
#pragma warning restore 618

            //Coloca a autoridade no Cliente
            identity.AssignClientAuthority(connectionToClient);

            //Faz o que tem que fazer
            RpcPaint(color);

            //Remove a autoridade
            identity.RemoveClientAuthority(connectionToClient);
        }
    }

}


