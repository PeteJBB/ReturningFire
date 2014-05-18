using UnityEngine;
using System.Collections;

public class ShrapnelBehaviour : MonoBehaviour {

	float lifeTime = 1;
	float birthday;
	Transform smoke;

	// Use this for initialization
	void Start () {
		birthday = Time.fixedTime;
		smoke = transform.Find("smoke");
	}
	
	// Update is called once per frame
	void Update () {
		var age = Time.fixedTime - birthday;
		if(age >= lifeTime)
		{
			DetachParticles();
			Destroy(gameObject);
		}
	}

	void DetachParticles()
	{
		// This splits the particle off so it doesn't get deleted with the parent
		smoke.particleSystem.transform.parent = null;
		
		// this stops the particle from creating more bits
		smoke.particleSystem.Stop();
		
		Destroy(smoke.gameObject, smoke.particleSystem.startLifetime + 0.1f);
	}
}
