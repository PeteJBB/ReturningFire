using UnityEngine;
using System.Collections;

public class BasicExplosion : MonoBehaviour {

	[SerializeField]
	GameObject shrapnelObject;

	[SerializeField]
	int shrapnelCount = 5;
	
	[SerializeField]
	float violence = 1.0f;

	[SerializeField]
	bool renderChunks = false;

	// Use this for initialization
	void Start () {
		for(int i=0; i<shrapnelCount; i++)
		{
			var pos = transform.position;
			var s = (GameObject)Instantiate(shrapnelObject, pos, Quaternion.identity);
			s.renderer.enabled = renderChunks;
			var scale = Random.Range(0.1f, 0.3f);
			s.transform.localScale = new Vector3(scale,scale,scale);
			s.rigidbody.AddRelativeTorque(Vector3.right * violence * 10);

			var dir = Quaternion.Euler(Random.Range(-90, 90), Random.Range(0, 360), Random.Range(-90, 90)) * Vector3.up;
			s.rigidbody.AddForce(dir * violence * 100);
		}

		Destroy(gameObject, 1.5f);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
