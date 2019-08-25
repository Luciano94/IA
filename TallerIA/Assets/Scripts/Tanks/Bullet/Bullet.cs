using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
	[SerializeField]
    private float damage = 10f;
	[SerializeField]
    private float maxLifeTime = 2f;
	[SerializeField]
	private ParticleSystem explosionParticles;


	private void Start ()
	{
		Destroy (gameObject, maxLifeTime);
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Tank")
		{
			TankBase tank = other.GetComponent<TankBase>();
			tank.Damage(damage);
		}

		explosionParticles.transform.parent = null;

		explosionParticles.Play();

		ParticleSystem.MainModule mainModule = explosionParticles.main;
		Destroy (explosionParticles.gameObject, mainModule.duration);
		
		Destroy (gameObject);
	}
}
