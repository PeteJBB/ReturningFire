using UnityEngine;
using System.Collections;

public class PlasmaShot : MonoBehaviour {

    float _lifeTime = 20;
    float _birthday;
    GameObject _parentObject; // who fired me?
    Transform _smoke;

	// Use this for initialization
	void Start ()
    {
        _birthday = Time.fixedTime;
        _smoke = transform.Find("smoke");
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(rigidbody.velocity.magnitude > 0)
            transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
        var age = Time.fixedTime - _birthday;
        if(age >= _lifeTime)
            Destroy(gameObject);
		else
			transform.localScale = Vector3.one * (1 - (age/_lifeTime));


	}

    void OnDestroy()
    {
        DetatchSmoke();
    }

    void DetatchSmoke()
    {
        // This splits the particle off so it doesn't get deleted with the parent
        _smoke.parent = null;
        
        // this stops the particle from creating more bits
        _smoke.particleSystem.Stop();
        
        Destroy(_smoke.gameObject, _smoke.particleSystem.startLifetime + 0.1f);
    }

    void OnTriggerEnter(Collider other)
    {
        if(collider.gameObject != _parentObject)
        {
            // move back along path a bit to help with explosions
            transform.position -= (transform.forward * 2);
            Explode();
        }
    }

    void Explode()
    {
        Destroy(gameObject);
    }

    void SetParent(GameObject parent)
    {
        _parentObject = parent;
    }
}
