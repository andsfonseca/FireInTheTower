using System;
using UnityEngine;
using UnityEngine.Networking;

namespace PaintTower.Network {

    /// <summary>
    /// Uma Classe Network Manager com funções customizadas para quando um Cliente é conectado e desconectado.
    /// </summary>
#pragma warning disable 618
    public class CustomNetworkManager : NetworkManager
#pragma warning restore 618
    {
        /// <summary>
        /// Ação chamada quando um cliente é conectado a Network
        /// </summary>
        public event Action OnClientConnected;

        /// <summary>
        /// Ação chamada quando um cliente é desconectado da Network
        /// </summary>
        public event Action OnClientDisconnected;

        /// <summary>
        /// Executado quando o cliente conectar
        /// </summary>
        /// <param name="conn">Conexão com o Servidor</param>
#pragma warning disable 618
        public override void OnClientConnect(NetworkConnection conn)
#pragma warning restore 618
        {
            base.OnClientConnect(conn);
            if (conn.lastError == NetworkError.Ok) {
                Debug.Log("Conectado com sucesso com o servidor!");
            }
            else {
                Debug.LogError("Conectado com falhas ao servidor - " + conn.lastError);
            }

            OnClientConnected?.Invoke();
        }

        /// <summary>
        /// Executado quando o cliente desconectar
        /// </summary>
        /// <param name="conn">Conexão com o Servidor.</param>
#pragma warning disable 618
        public override void OnClientDisconnect(NetworkConnection conn)
#pragma warning restore 618
        {
            base.OnClientDisconnect(conn);
            if (conn.lastError == NetworkError.Ok) {
                Debug.Log("Desconectado com sucesso com o servidor!");
            }
            else {
                Debug.LogError("Desconectado com falhas do servidor - " + conn.lastError);
            }

            OnClientDisconnected?.Invoke();
        }
    }
}
