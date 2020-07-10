using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemSelfDestructScript : MonoBehaviour
{
    private ParticleSystem particle;
    
	void Start ()
    {
        particle = GetComponent<ParticleSystem>();
        Destroy(gameObject, 3.0f);
	}
	
	void Update ()
    {
        if (null != particle)
            if (!particle.IsAlive())
                Destroy(gameObject);
	}
}
