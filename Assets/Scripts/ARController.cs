using GoogleARCore;
using System.Dynamic;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Controls the HelloAR example.
/// </summary>
public class ARController : MonoBehaviour {
    /// <summary>
    /// A camêra que será usada em primeira pessoa
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// Indica o ponto selecionado do Mundo onde a cidade vai ser instanciada
    /// </summary>
    private GameObject _globalARSelected = null;

    /// <summary>
    /// Define o Marcador para o objeto ser instanciado
    /// </summary>
    public GameObject GlobalAR {
        get { return _globalARSelected; }
    }


    /// <summary>
    /// A rotação em graus precisa ser aplicada ao objeto quando for colocada.
    /// </summary>
    private const float k_PrefabRotation = 180.0f;

  
    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake() {
        // Enable ARCore to target 60fps camera capture frame rate on supported devices.
        // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
        Application.targetFrameRate = 60;
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update() {

        // If the player has not touched the screen, we are done with this update.
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) {
            return;
        }

        // Should not handle input if the player is pointing on UI.
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
            return;
        }

        //Se a seleção está desabilitada, não precisar capturar os pontos selecionadas
        if (!GameLogic.Instance.SelectOnTrackingPlanes) {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit)) {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0) {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else {

                ///Caso a posição seja nula, cria o objeto
                if (_globalARSelected == null) {
                    _globalARSelected = new GameObject();
                    _globalARSelected.name = "Global Position Marker";
                    GameObject marker = Instantiate(_globalARSelected, Vector3.zero, Quaternion.identity, _globalARSelected.transform);
                    marker.name = "Marker";

                }

                _globalARSelected.transform.position = hit.Pose.position;
                _globalARSelected.transform.rotation = hit.Pose.rotation;

                // Compensate for the hitPose rotation facing away from the raycast (i.e.
                // camera).
                _globalARSelected.transform.Rotate(0, k_PrefabRotation, 0, Space.Self);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                // Make game object a child of the anchor.
                _globalARSelected.transform.parent = anchor.transform;

                //Desabilita a seleção para um local
                GameLogic.Instance.SelectOnTrackingPlanes = false;

            }
        }
    }

}
