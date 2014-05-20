using UnityEngine;
using System.Collections.Generic;

public class Detector : MonoBehaviour
{
	[SerializeField]
	float DetectionRange = 200;

	[SerializeField]
	float ScanDelay = 1;

	private bool _canSeePlayer = false;
	public bool CanSeePlayer { get { return _canSeePlayer; } }
	public Vector3 Origin { get; set; }

	private GameObject _player;
	private float _lastScanTime;


	void Start()
	{
		Origin = transform.position;
		_player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update()
	{
		if(Time.fixedTime - _lastScanTime > ScanDelay)
		{
			// scan
			var vect = _player.transform.position - Origin; 
			_canSeePlayer = vect.magnitude <= DetectionRange && Utility.CanSeePoint(Origin, _player.transform.position, _player);
			_lastScanTime = Time.fixedTime;
		}
	}
}
