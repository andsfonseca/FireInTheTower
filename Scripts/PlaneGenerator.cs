using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

/// <summary>
/// Controla o Gerenciamento de Planos detectados
/// </summary>
public class PlaneGenerator : MonoBehaviour {
    /// <summary>
    /// Objeto que irá ser spawnado quando o plano for detectados
    /// </summary>
    public GameObject DetectedPlanePrefab;

    /// <summary>
    /// Lista de Planos detectados
    /// </summary>
    private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();

    /// <summary>
    /// The Unity Update method.
    /// </summary>
    public void Update() {
        // Se não estiver rastreando, ignora o Update
        if (Session.Status != SessionStatus.Tracking) {
            return;
        }

        //Para cada plano detectado
        //Obs. Novos Planos são colocados como novos planos DetectedPlans
        Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
        for (int i = 0; i < m_NewPlanes.Count; i++) {

            // Instantiate a plane visualization prefab and set it to track the new plane. The
            // transform is set to the origin with an identity rotation since the mesh for our
            // prefab is updated in Unity World coordinates.
            GameObject planeObject =
                Instantiate(DetectedPlanePrefab, Vector3.zero, Quaternion.identity, transform);

            planeObject.GetComponent<PlaneVisualizer>().Initialize(m_NewPlanes[i]);
        }
    }
}

