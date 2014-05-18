using UnityEngine;
using System.Collections;

public class BuildingBehaviour : MonoBehaviour {

	[SerializeField] 
	GameObject explosion;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other)
	{
		var mesh = transform.Find("mesh");
		mesh.renderer.enabled = false;

		var ruin = transform.Find("ruin");
		if(ruin != null)
			ruin.renderer.enabled = true;
		collider.enabled = false;

		Instantiate(explosion, transform.position, transform.rotation);
	}
}
