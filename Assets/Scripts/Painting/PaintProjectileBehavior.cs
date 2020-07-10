using PaintTower.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintTower.Painting {

    public class PaintProjectileBehavior : MonoBehaviour {

        private MeshRenderer m_mesh;

        public Color PaintColor {
            get {
                return m_mesh.material.color;
            }
            set {
                m_mesh.material.color = value;
            }
        }

        public float paintDiameter = 1.5f;

        private bool isActive = false;

        public void Awake() {
            m_mesh = gameObject.GetComponent<MeshRenderer>();
        }

        private void Start() {
            if (paintDiameter > 0) {
                isActive = true;
            }
           
        }

        void OnTriggerEnter(Collider other) {
            if (!isActive)
                return;

            Destroy(gameObject);
            ParticleSystem cloudParticle = Instantiate(PaintProjectileManager.GetInstance().cloudParticlePrefab, GameLogic.Instance.AR.WorldOrigin);
            ParticleSystem burstParticle = Instantiate(PaintProjectileManager.GetInstance().burstParticlePrefab, GameLogic.Instance.AR.WorldOrigin);
            cloudParticle.transform.position = transform.position;
            burstParticle.transform.position = transform.position;
            var cloudSettings = cloudParticle.main;
            cloudSettings.startColor = PaintColor;
            var burstSettings = burstParticle.main;
            burstSettings.startColor = PaintColor;
            cloudParticle.Play();
            burstParticle.Play();

            PaintProjectileManager manager = PaintProjectileManager.GetInstance();

            for (int i = 0; i < 14; ++i) {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, manager.GetSphereRay(i), out hit, paintDiameter)) {
                    if (hit.collider is MeshCollider) {
                        MyShaderBehavior script = hit.collider.gameObject.GetComponent<MyShaderBehavior>();
                        if (null != script) {
                            script.PaintOnColored(hit.textureCoord2, manager.GetRandomProjectileSplash(), PaintColor);
                        }
                    }
                }
            }
        }
    }
}