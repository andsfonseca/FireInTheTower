using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintProjectileBehavior : MonoBehaviour
{
    public Color paintColor = Color.white;
    public float paintDiameter = 1.5f;
    
    private bool isActive = false;

    private void Start()
    {
        if (paintDiameter > 0)
        {            
            isActive = true;
        }
        gameObject.GetComponent<MeshRenderer>().material.color = paintColor;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;
        Debug.Log(transform.position);
        Destroy(gameObject);
        ParticleSystem cloudParticle = Instantiate(PaintProjectileManager.GetInstance().cloudParticlePrefab, GameLogic.Instance.AR.GlobalAR.transform);
        ParticleSystem burstParticle = Instantiate(PaintProjectileManager.GetInstance().burstParticlePrefab, GameLogic.Instance.AR.GlobalAR.transform);
        cloudParticle.transform.position = transform.position;
        burstParticle.transform.position = transform.position;
        var cloudSettings = cloudParticle.main;
        cloudSettings.startColor = paintColor;
        var burstSettings = burstParticle.main;
        burstSettings.startColor = paintColor;
        cloudParticle.Play();
        burstParticle.Play();

        PaintProjectileManager manager = PaintProjectileManager.GetInstance();

        for (int i = 0; i < 14; ++i)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, manager.GetSphereRay(i), out hit, paintDiameter))
            {
                if (hit.collider is MeshCollider)
                {
                    MyShaderBehavior script = hit.collider.gameObject.GetComponent<MyShaderBehavior>();
                    if (null != script)
                    {
                        script.PaintOnColored(hit.textureCoord2, manager.GetRandomProjectileSplash(), paintColor);
                    }
                }
            }
        }
    }
}
