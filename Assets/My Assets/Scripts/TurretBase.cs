﻿using UnityEngine;
using System.Collections;

public class TurretBase : MonoBehaviour 
{
	[SerializeField]
	protected float DetectionRange = 100;
	
	[SerializeField]
	protected float FiringRange = 80;
	
	[SerializeField]
	protected float ReloadTime = 2;
	
	[SerializeField]
	protected float LockTime = 2;
	
	[SerializeField]
	protected float AttentionSpan = 2; // how long after losing sight will turret begin searching again
	
	[SerializeField]
	protected float LockSize = 10;
	
	[SerializeField]
	protected float TurretSpeed = 200;
	
	[SerializeField]
	protected float MaxHealth = 10;
	
	[SerializeField] 
	protected GameObject DeathExplosion;
	
	[SerializeField]
	protected GameObject Projectile;
	
	protected GameObject _target;
	protected Transform _platform;
	protected Transform _launcher;
	
	protected Vector3 _aimPoint;
	protected float _lastSeenTime;
	protected float _lockResetTime;
	protected float _lastShootTime;
	
	protected GameObject _hud;
	
	protected float _health;
	
	// Use this for initialization
	protected void Start () 
	{
		Debug.Log("Base start");

		_health = MaxHealth;
		
		_target = GameObject.FindGameObjectWithTag("Player");
		_platform = transform.FindChild("platform");
		_launcher = _platform.FindChild("launcher");
		
		_hud = GameObject.FindGameObjectWithTag("HUD");
	}
	
	// Update is called once per frame
	protected void Update () 
	{
		Debug.Log("Base Update");
		var vect = _target.transform.position - _launcher.position; 
		if(vect.magnitude <= DetectionRange 
		   && Utility.CanObjectSeeAnother(_launcher.gameObject, _target))
		{
			// can see target  
			_hud.SendMessage("Detected", gameObject, SendMessageOptions.DontRequireReceiver);
			_lastSeenTime = Time.fixedTime;
			
			// compute lead
			_aimPoint = CalculateAimPoint();
			RotateTowards(_aimPoint);
			
			var aimVect = _aimPoint - _launcher.position;
			var angle = Vector3.Angle(_launcher.forward, aimVect);
			if(angle <= LockSize && Utility.CanSeePoint(_launcher.transform.position, _aimPoint, _target))
			{
				Debug.DrawLine(_launcher.position, _aimPoint, Color.green);
				if(aimVect.magnitude <= FiringRange // in range
				   && Time.fixedTime - _lockResetTime >= LockTime // locked
				   && Time.fixedTime - _lastShootTime >= ReloadTime) // loaded
				{
					Fire();
				}
			}
			else
			{
				Debug.DrawLine(_launcher.position, _aimPoint, Color.red);
				_lockResetTime = Time.fixedTime;
			}
		}
		else
		{
			if(Time.fixedTime - _lastSeenTime > AttentionSpan)
			{
				// pick a new random aim point and wander
				_aimPoint = _launcher.transform.position + new Vector3(Random.Range(-500, 500), Random.Range(-50, 50), Random.Range(-500, 500));
				_lastSeenTime = Time.fixedTime;
			}
			RotateTowards(_aimPoint);
		}
	}

	protected virtual Vector3 CalculateAimPoint()
	{
		var vect = _target.transform.position - _launcher.position;
		var rocketTimeToTarget = vect.magnitude / 100;
		var aim = _target.transform.position + (_target.rigidbody.velocity * rocketTimeToTarget);
		return aim;
	}
	
	protected void RotateTowards(Vector3 point)
	{
		var vect = point - _launcher.position;
		
		// rotate platform (y-axis only)
		var v = new Vector3(vect.x, 0, vect.z);
		var desiredRotation = Quaternion.LookRotation(v);
		_platform.rotation = Quaternion.RotateTowards(_platform.rotation, desiredRotation, TurretSpeed * Time.deltaTime);
		
		// rotate lancher - (x-axis only)
		v = new Vector3(0, vect.y, v.magnitude);
		desiredRotation = Quaternion.LookRotation(v);
		_launcher.localRotation = Quaternion.RotateTowards(_launcher.localRotation, desiredRotation, TurretSpeed * Time.deltaTime);
	}
	
	protected void Fire()
	{
		_lastShootTime = Time.fixedTime;
		
		var pos = _launcher.position;
		var clone = (GameObject)Instantiate(Projectile, pos, transform.rotation);
		clone.SendMessage("SetParent", gameObject, SendMessageOptions.DontRequireReceiver);
		Physics.IgnoreCollision(collider, clone.collider);
		clone.transform.rotation = _launcher.rotation;
		clone.rigidbody.AddForce(clone.transform.forward * 100, ForceMode.Impulse);
	}
	
	protected virtual void ApplyDamage(float amount)
	{
		_health -= amount;
		if(_health <= 0) 
		{
			Instantiate(DeathExplosion, transform.position, transform.rotation);
			Destroy(gameObject);
		}
	}
}
