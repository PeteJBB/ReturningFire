using UnityEngine;
using System.Collections;

public class RocketBehaviour : MonoBehaviour 
{
	float _fuelTime = 1;
	float _lifeTime = 20;
    float _enginePower = 100;

	float _birthday;
	Transform _smoke;
	bool _isEngineRunning;

	[SerializeField] 
	GameObject Explosion;
	
    [SerializeField] 
    float Damage;

    [SerializeField] 
    float DamageRadius;

    [SerializeField] 
    float ProximityFuze;

    float _inaccuracyMultiplier = 35;
    GameObject _parentObject; // who fired me?

	void Start()
	{
		_birthday = Time.fixedTime;
		_smoke = transform.Find("smoke");
		_isEngineRunning = true;
	}

	void Update()
	{
		var age = Time.fixedTime - _birthday;
        var deltaMultiplier = Time.deltaTime / 0.02f;
		if(_isEngineRunning)
		{
			// add main engine power
            rigidbody.AddForce(transform.forward * _enginePower * deltaMultiplier);

			if(age >= _fuelTime)
			{
                DetatchSmoke();
				_isEngineRunning = false;
			}
		}

		// add some sideways force to give the rocket a more interesting flight path
		var forceDir = Quaternion.Euler(0,0, Random.Range(0, 360));
        rigidbody.AddRelativeForce(forceDir * Vector3.right * _inaccuracyMultiplier * deltaMultiplier);

		if(age > _lifeTime)
			Destroy(gameObject);	

        else if(ProximityFuze > 0)
        {
            foreach(var col in Physics.OverlapSphere(transform.position, ProximityFuze, (int)Layers.Projectiles))
            {
                if(col.gameObject != gameObject
                   && col.gameObject != _parentObject
                   && col.tag != "Terrain")
                {
                    Explode();
                }
            }
        }
	}

	void OnDestroy()
	{
		if(_isEngineRunning)
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
        Instantiate(Explosion, transform.position, transform.rotation);
        Utility.AreaDamageEnemies(transform.position, DamageRadius, Damage);
        Destroy(gameObject);
        //Physics.SphereCastAll(
    }

    void SetParent(GameObject parent)
    {
        _parentObject = parent;
    }

}
