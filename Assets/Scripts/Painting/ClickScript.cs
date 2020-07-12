using PaintTower.Scripts;
using PaintTower.Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintTower.Painting {
    public class ClickScript : MonoBehaviour {
        public Camera mainCamera;
        public Texture2D splashTexture;
        public Animator gunCameraAnimator;
        public float shootCooldown;
        private float m_cooldown;

        public Color ColorProjectile { private get; set; } = Color.white;

        private const int c_colorArrayLength = 6;
        private readonly Color[] c_colorArray = new Color[c_colorArrayLength] { Color.blue, Color.yellow, Color.red, Color.green, Color.white, Color.black };
        private int m_currColor = 0;

        void Update() {

            if (m_cooldown > 0) {
                m_cooldown -= Time.deltaTime;
                return;
            }

            if (GameLogic.Instance.CurrentGameState == GameState.PLAY) {
                if (Input.touchCount == 1) {
                    if (Input.GetTouch(0).phase == TouchPhase.Began) {
                        gunCameraAnimator.SetTrigger("Shoot");
                        Invoke("Shoot", 0.3f);
                        m_cooldown = shootCooldown;
                    }
                }
                //if (Input.touchCount == 2) {
                //    if (Input.GetTouch(1).phase == TouchPhase.Began) {
                //        PaintProjectileManager.GetInstance().paintBombColor = c_colorArray[m_currColor];
                //        m_currColor++;
                //        if (m_currColor == c_colorArrayLength)
                //            m_currColor = 0;
                //    }
                //}
                //if (Input.GetMouseButtonDown(0)) {
                //    /*RaycastHit hit;
                //    if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
                //    {
                //        MyShaderBehavior script = hit.collider.gameObject.GetComponent<MyShaderBehavior>();
                //        if (null != script)
                //            script.PaintOnColored(hit.textureCoord2, PaintProjectileManagerStatic.GetInstance().GetRandomProjectileSplash(), PaintProjectileManagerStatic.GetInstance().paintBombColor);
                //    }*/
                //    GameObject projectile = Instantiate(PaintProjectileManager.GetInstance().paintBombPrefab, mainCamera.transform.position, mainCamera.transform.rotation, GameLogic.Instance.AR.WorldOrigin);
                //    projectile.GetComponent<PaintProjectileBehavior>().paintColor = PaintProjectileManager.GetInstance().paintBombColor;
                //    projectile.GetComponent<Rigidbody>().velocity = mainCamera.transform.forward * 5;
                //    //projectile.transform.localScale = GameLogic.Instance.AR.GlobalAR.transform.localScale;//new Vector3(0.07f, 0.07f, 0.07f);

                //}

                //if (Input.GetMouseButtonDown(1)) {
                //    PaintProjectileManager.GetInstance().paintBombColor = c_colorArray[m_currColor];
                //    m_currColor++;
                //    if (m_currColor == c_colorArrayLength)
                //        m_currColor = 0;
                //}
            }
        }

        public void Shoot() {
            GameObject projectile = Instantiate(PaintProjectileManager.GetInstance().paintBombPrefab, mainCamera.transform.position, mainCamera.transform.rotation, GameLogic.Instance.AR.WorldOrigin);
            projectile.GetComponent<PaintProjectileBehavior>().PaintColor = ColorProjectile;
            projectile.GetComponent<Rigidbody>().velocity = mainCamera.transform.forward * 5;
        }
    }
}