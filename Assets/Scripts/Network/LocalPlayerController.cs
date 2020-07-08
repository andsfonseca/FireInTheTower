
using PaintTower.Scripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.ARFoundation;

namespace PaintTower.Network {
    /// <summary>
    /// Local Player Controller, controla o player local na Network
    /// </summary>
#pragma warning disable 618
    public class LocalPlayerController : NetworkBehaviour
#pragma warning restore 618
    {
        /// <summary>
        /// Modelo da Âncora
        /// </summary>
        public GameObject AnchorPrefab;

        /// <summary>
        /// Objeto a ser Instanciado na Âncora
        /// </summary>
        public GameObject BasicCityPrefab;

        /// <summary>
        /// O método OnStartLocalPlayer() da Unity.
        /// </summary>
        public override void OnStartLocalPlayer() {
            base.OnStartLocalPlayer();

            //Nome do Objeto para ser encontrado posteriomente
            gameObject.name = "LocalPlayer";
        }

        /// <summary>
        /// Will spawn the origin anchor and host the Cloud Anchor. Must be called by the host.
        /// </summary>
        /// <param name="anchor">The AR Anchor to be hosted.</param>
        public void SpawnAnchor(ARAnchor anchor) {
            // Instantiate Anchor model at the hit pose.
            var anchorObject = Instantiate(AnchorPrefab, Vector3.zero, Quaternion.identity);

            // Anchor must be hosted in the device.
            anchorObject.GetComponent<AnchorController>().HostAnchor(anchor);

            // Host can spawn directly without using a Command because the server is running in this
            // instance.
#pragma warning disable 618
            NetworkServer.Spawn(anchorObject);
#pragma warning restore 618

            InitializeWorld(anchor.transform);
        }

        public void InitializeWorld(Transform transform) {
            //Coloca o template da Cidade
            GameObject basic = Instantiate(BasicCityPrefab, Vector3.zero, Quaternion.identity);
            basic.transform.SetParent(transform, false);
            basic.GetComponentInChildren<BuildGenerator>().Create();
        }
    }

}