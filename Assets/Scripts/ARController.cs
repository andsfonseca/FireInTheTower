using GoogleARCore;
using GoogleARCore.Examples.CloudAnchors;
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
    public GameObject _globalARSelected = null;

    /// <summary>
    /// The helper that will calculate the World Origin offset when performing a raycast or
    /// generating planes.
    /// </summary>
    public ARCoreWorldOriginHelper ARCoreWorldOriginHelper;

    /// <summary>
    /// Define o Marcador para o objeto ser instanciado
    /// </summary>
    public GameObject GlobalAR {
        get { return _globalARSelected; }
    }

    /// <summary>
    /// Record the time since the room started. If it passed the resolving prepare time,
    /// applications in resolving mode start resolving the anchor.
    /// </summary>
    private float m_TimeSinceStart = 0.0f;

    /// <summary>
    /// The time between room starts up and ARCore session starts resolving.
    /// </summary>
    private const float k_ResolvingPrepareTime = 3.0f;

    /// <summary>
    /// Indicates whether passes the resolving prepare time.
    /// </summary>
    private bool m_PassedResolvingPreparedTime = false;

    /// <summary>
    /// Indicates whether the Cloud Anchor finished hosting.
    /// </summary>
    private bool m_AnchorFinishedHosting = false;

    /// <summary>
    /// Indicates whether the Anchor was already instantiated.
    /// </summary>
    private bool m_AnchorAlreadyInstantiated = false;

    /// <summary>
    /// Gets a value indicating whether the Origin of the new World Coordinate System,
    /// i.e. the Cloud Anchor was placed.
    /// </summary>
    public bool IsOriginPlaced { get; private set; }

    /// <summary>
    /// The Unity Awake() method.
    /// </summary>
    public void Awake() {
        // Enable ARCore to target 60fps camera capture frame rate on supported devices.
        // Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
        Application.targetFrameRate = 60;
    }

    public void Start() {
        ResetStatus();
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    public void Update() {

        if (GameLogic.Instance.ApplicationMode == ApplicationMode.Resolving && !m_PassedResolvingPreparedTime) {
            m_TimeSinceStart += Time.deltaTime;

            if (m_TimeSinceStart > k_ResolvingPrepareTime) {
                m_PassedResolvingPreparedTime = true;
            }
        }

        //Se a seleção está desabilitada não precisa começar a interface de seleção
        if (!GameLogic.Instance.SelectOnTrackingPlanes) {
            return;
        }

        // Se o player não tocou na tela, não precisa continuar
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) {
            return;
        }

        // Se o player esta tocando na UI
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
            return;
        }

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (ARCoreWorldOriginHelper.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit)) {
        //if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit)) {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0) {
                Debug.Log("O hit foi dado na parte de trás do plano");
            }
            else if (hit.Trackable is DetectedPlane) {

                DetectedPlane detectedPlane = hit.Trackable as DetectedPlane;

                //Se ele não estiver clicado em um plano na parede
                if (detectedPlane.PlaneType != DetectedPlaneType.Vertical) {

                    // Cria uma âncora na posição selecioanda
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                    //Coloca o marcador como filho da Âncora
                    /*globalARSelected.transform.parent = anchor.transform;*/

                    ARCoreWorldOriginHelper.SetWorldOrigin(anchor.transform);

                    GameObject.Find("LocalPlayer").GetComponent<LocalPlayerPaintingController>()
                        .SpawnAnchor(Vector3.zero, Quaternion.identity, anchor);
                    //Desabilita a seleção para um local
                    GameLogic.Instance.SelectOnTrackingPlanes = false;
                }

            }
        }
    }


    /// <summary>
    /// Resets the internal status.
    /// </summary>
    public void ResetStatus() {
        if (_globalARSelected != null) {
            Destroy(_globalARSelected.gameObject);
        }
        //IsOriginPlaced = false;
        _globalARSelected = null;
    }

    /// <summary>
    /// Indicates whether the resolving prepare time has passed so the AnchorController
    /// can start to resolve the anchor.
    /// </summary>
    /// <returns><c>true</c>, if resolving prepare time passed, otherwise returns <c>false</c>.
    /// </returns>
    public bool IsResolvingPrepareTimePassed() {
        return GameLogic.Instance.ApplicationMode != ApplicationMode.Ready &&
            m_TimeSinceStart > k_ResolvingPrepareTime;
    }

    /// <summary>
    /// Callback called when the resolving timeout is passed.
    /// </summary>
    public void OnResolvingTimeoutPassed() {
        if (GameLogic.Instance.ApplicationMode != ApplicationMode.Resolving) {
            Debug.LogWarning("OnResolvingTimeoutPassed shouldn't be called" +
                "when the application is not in resolving mode.");
            return;
        }

    }

    /// <summary>
    /// Callback indicating that the Cloud Anchor was hosted.
    /// </summary>
    /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was hosted
    /// successfully.</param>
    /// <param name="response">The response string received.</param>
    public void OnAnchorHosted(bool success, string response) {
        m_AnchorFinishedHosting = success;
        GameLogic.Instance.ShowAndroidToastMessage((success) ? "Sucesso!" : "Erro: " + response);
    }

    /// <summary>
    /// Callback indicating that the Cloud Anchor was instantiated and the host request was
    /// made.
    /// </summary>
    /// <param name="isHost">Indicates whether this player is the host.</param>
    public void OnAnchorInstantiated(bool isHost) {
        if (m_AnchorAlreadyInstantiated) {
            return;
        }

        m_AnchorAlreadyInstantiated = true;
    }

    /// <summary>
    /// Callback indicating that the Cloud Anchor was resolved.
    /// </summary>
    /// <param name="success">If set to <c>true</c> indicates the Cloud Anchor was resolved
    /// successfully.</param>
    /// <param name="response">The response string received.</param>
    public void OnAnchorResolved(bool success, string response) {
        GameLogic.Instance.ShowAndroidToastMessage((success) ? "Sucesso!" : "Erro: " + response);
    }

    /// <summary>
    /// Sets the apparent world origin so that the Origin of Unity's World Coordinate System
    /// coincides with the Anchor. This function needs to be called once the Cloud Anchor is
    /// either hosted or resolved.
    /// </summary>
    /// <param name="anchorTransform">Transform of the Cloud Anchor.</param>
    public void SetWorldOrigin(Transform anchorTransform) {
        if (IsOriginPlaced) {
            Debug.LogWarning("The World Origin can be set only once.");
            return;
        }

        IsOriginPlaced = true;

        ARCoreWorldOriginHelper.SetWorldOrigin(anchorTransform);

    }

}
