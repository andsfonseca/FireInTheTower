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
        /// O Tempo decorrido desde que o lobby começou
        /// </summary>
        [SyncVar] public float matchTime = 0f;
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
        [HideInInspector]
        /// <summary>
        /// Se o Lobby está aberto
        /// </summary>
        [SyncVar] public bool MatchIsOpen = false;
        /// <summary>
        /// Se a partida acabou
        /// </summary>
        [SyncVar] public bool MatchIsOver = false;
        /// <summary>
        /// Posição do Player em relação a câmera
        /// </summary>
        //[SyncVar] public Vector3 playerCameraPosition;
        [SyncVar] float redPercent = 0;
        [SyncVar] float yellowPercent = 0;
        [SyncVar] float greenPercent = 0;
        [SyncVar] float bluePercent = 0;

        public float[] percents {
            get {
                float[] vector = { redPercent, yellowPercent, greenPercent, bluePercent };
                return vector;
            }
        }

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

            if (GameLogic.Instance.CurrentGameState == GameState.LOBBY) {
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
                        matchTime = Instance.matchTime;
                    }
                }

                if (lobbyTime > GameLogic.Instance.MaxLobbyTime) {
                    GameLogic.Instance.LobbyMatch.UnloadHUD();
                    GameLogic.Instance.SetGameState(GameState.PLAY);
                    ActiveMatch();
                }
            }

            if (GameLogic.Instance.CurrentGameState == GameState.PLAY) {
                //Apenas o Host
                if (isHost) {
                    //Se o Lobby está aberto
                    if (MatchIsOpen) {
                        //Atualiza o tempo do Lobby
                        matchTime += Time.deltaTime;
                    }

                    if (!MatchIsOver && matchTime > GameLogic.Instance.MaxMatchTime) {
                        CalcPercent(GameObject.FindObjectsOfType(typeof(PaintCalculation)) as PaintCalculation[]);
                        MatchIsOver = true;
                    }
                }

                //Todos podem fazer isso
                if (isLocalPlayer) {
                    //playerCameraPosition = GameLogic.Instance.AR.WorldOrigin.position - GameLogic.Instance.Camera.transform.position;
                    //Só deve executar se dono da sessão estiver online
                    if (Instance) {
                        //Atualiza o tempo
                        lobbyTime = Instance.lobbyTime;
                        matchTime = Instance.matchTime;
                        MatchIsOver = Instance.MatchIsOver;

                        if (MatchIsOver) {
                            redPercent = Instance.redPercent;
                            yellowPercent = Instance.yellowPercent;
                            greenPercent = Instance.greenPercent;
                            bluePercent = Instance.bluePercent;
                            
                            GameLogic.Instance.Play.UnloadHUD();
                            GameLogic.Instance.SetGameState(GameState.GAMEOVER);
                        }
                    }
                }

            }
        }

        public void ActiveLobby() {
            if (isHost) {
                lobbyTime = 0f;
                LobbyIsOpen = true;
                MatchIsOpen = false;
            }
        }

        public void ActiveMatch() {
            if (isHost) {
                LobbyIsOpen = false;
                matchTime = 0f;
                MatchIsOpen = true;
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

        public void SendHit(Vector3 position) {
            if (isLocalPlayer) {
                CmdSendHit(position);
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

        /// <summary>
        /// Comando de Alteraçã de Cor vinda do Cliente
        /// </summary>
        /// <param name="color"></param>
#pragma warning disable 618
        [ClientRpc]
#pragma warning restore 618
        void RpcChangePlayerPostion(Vector3 vector) {
            //playerCameraPosition = vector;
        }

        /// <summary>
        /// Comando de Alterar cor
        /// </summary>
        /// <param name="color">Qual a cor</param>
#pragma warning disable 618
        [Command]
#pragma warning restore 618
        void CmdChangePlayerPostion(Vector3 vector) {
            //Pega a Indentidade
#pragma warning disable 618
            var identity = GetComponent<NetworkIdentity>();
#pragma warning restore 618

            //Coloca a autoridade no Cliente
            identity.AssignClientAuthority(connectionToClient);

            //Faz o que tem que fazer
            RpcChangePlayerPostion(vector);

            //Remove a autoridade
            identity.RemoveClientAuthority(connectionToClient);
        }


        /// <summary>
        /// Comando de Alterar cor
        /// </summary>
        /// <param name="color">Qual a cor</param>
#pragma warning disable 618
        [Command]
#pragma warning restore 618
        void CmdSendHit(Vector3 position) {
            //Pega a Indentidade
#pragma warning disable 618
            var identity = GetComponent<NetworkIdentity>();
#pragma warning restore 618

            //Coloca a autoridade no Cliente
            identity.AssignClientAuthority(connectionToClient);

            //Faz o que tem que fazer
            RpcHit(position);

            //Remove a autoridade
            identity.RemoveClientAuthority(connectionToClient);
        }

        /// <summary>
        /// Comando de Alteraçã de Cor vinda do Cliente
        /// </summary>
        /// <param name="color"></param>
#pragma warning disable 618
        [ClientRpc]
#pragma warning restore 618
        void RpcHit(Vector3 vector) {
            switch (playerColor) {
                case Colors.RED: PaintProjectileManager.GetInstance().MakeAHit(vector, new Color(0.98f, 0.51f, 0.2f)); break;
                case Colors.YELLOW: PaintProjectileManager.GetInstance().MakeAHit(vector, new Color(1, 0.85f, 0.28f)); break;
                case Colors.GREEN: PaintProjectileManager.GetInstance().MakeAHit(vector, new Color(0.54f, 0.88f, 0.38f)); break;
                case Colors.BLUE: PaintProjectileManager.GetInstance().MakeAHit(vector, new Color(0.21f, 0.73f, 0.95f)); break;
            }

        }

        public void CalcPercent(PaintCalculation[] objectsToPaint) {
            float totalRedCount = 0;
            float totalYellowCount = 0;
            float totalGreenCount = 0;
            float totalBlueCount = 0;
            foreach (PaintCalculation towerPiece in objectsToPaint) {
                int[] temp;
                temp = towerPiece.calcPaint();
                totalRedCount += temp[0];
                totalYellowCount += temp[1];
                totalGreenCount += temp[2];
                totalBlueCount += temp[3];
            }
            float total = 0;
            total = totalRedCount + totalYellowCount + totalGreenCount + totalBlueCount;
            if (total != 0) {
                if (totalRedCount != 0) {
                    redPercent = totalRedCount / total;
                }
                if (totalYellowCount != 0) {
                    yellowPercent = totalYellowCount / total;
                }
                if (totalGreenCount != 0) {
                    greenPercent = totalGreenCount / total;
                }
                if (totalBlueCount != 0) {
                    bluePercent = totalBlueCount / total;
                }
            }
            //Debug.Log("BIG PERCENT DEBUG ==> RED: " + redPercent + " YELLOW: " + yellowPercent + " GREEN: " + greenPercent + " BLUE: " + bluePercent + " TOTAL: " + total);
            //Debug.Log("BUG COUNT ==> RED: " + totalRedCount + " YELLOW: " + totalYellowCount + " GREEN: " + totalGreenCount + " BLUE: " + totalBlueCount + " TOTAL: " + total); 
        }
    }
}


