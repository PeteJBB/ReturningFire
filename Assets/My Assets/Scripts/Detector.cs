using UnityEngine;
using System.Collections.Generic;

public class Detector : MonoBehaviour
{
	public float DetectionRange = 200;
	public float ScanDelay = 1;
	public string DetectTag = "";
	public Vector3 Offset;

	public Dictionary<int,DetectionRecord> Detections { get; set; }

	private float _lastScanTime;
	private Vector3 _origin { get { return transform.position + Offset; }	}

	void Start()
	{
		Detections = new Dictionary<int,DetectionRecord>();
	}

	void Update()
	{
		if(Time.fixedTime - _lastScanTime > ScanDelay)
		{
			// look for new objects
			foreach(var obj in GameObject.FindGameObjectsWithTag(DetectTag))
			{
				var id = obj.GetInstanceID();
				if(Detections.ContainsKey(id))
				{
					var d = Detections[id];
					d.canSee = RunDetectionTest(obj);
					if(d.canSee)
						d.lastSeen = Time.fixedTime;
				}
				else if(RunDetectionTest(obj))
				{
					Detections.Add(id, new DetectionRecord(obj));
				}
			}

			_lastScanTime = Time.fixedTime;
		}
	}

	private bool RunDetectionTest(GameObject obj)
	{
		var vect = obj.transform.position - _origin; 
		return vect.magnitude <= DetectionRange && Utility.CanSeeBounds(_origin, obj.collider.bounds, obj);
	}

	public bool CanSee(GameObject obj)
	{
		var id = obj.GetInstanceID();
		return Detections.ContainsKey(id) && Detections[id].canSee;
	}
}

public class DetectionRecord
{
	public GameObject gameObject;
	public Vector3 position;
	public bool canSee;
	public float firstDetected;
	public float lastSeen;

	public DetectionRecord(GameObject obj)
	{
		gameObject = obj;
		position = obj.transform.position;
		canSee = true;
		firstDetected = Time.fixedTime;
		lastSeen = Time.fixedTime;
	}
}
