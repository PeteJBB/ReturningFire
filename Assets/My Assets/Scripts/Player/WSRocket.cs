using UnityEngine;
using System.Collections;

/// <summary>
/// Weapon System - Rockets
/// </summary>
public class WSRocket : MonoBehaviour 
{
	[SerializeField]
	Texture2D CrosshairTexture;

	bool nextRocketLeftSide = false;
	Vector2 crosshairLocation;

	// weapons
	[SerializeField]
	GameObject Rocket;
	
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		var launchPoint = transform.position - transform.up;
		var aimPoint = launchPoint + (transform.forward * 500);

		// try and set aim point at the correct point we are aiming at
		// this will get rockets to converge nicely
		RaycastHit hitInfo;
		if (Physics.Raycast(launchPoint, transform.forward, out hitInfo))
		{
			aimPoint = hitInfo.point;
		}

		crosshairLocation = Camera.main.WorldToScreenPoint(aimPoint);
		crosshairLocation.y = Screen.height - crosshairLocation.y;

		// fire
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// position rocket on left or right
			var pos = launchPoint + (nextRocketLeftSide ? -transform.right : transform.right) * 2;
			nextRocketLeftSide = !nextRocketLeftSide;
			
			// create and fire
			var clone = (GameObject)Instantiate(Rocket, pos, transform.rotation);
			clone.SendMessage("SetParent", gameObject, SendMessageOptions.DontRequireReceiver);
			Physics.IgnoreCollision(collider, clone.collider);
			clone.transform.LookAt(aimPoint);
			clone.rigidbody.AddForce(clone.transform.forward * 100, ForceMode.Impulse);
		}
	}
	
	void OnGUI()
	{
		// crosshair
		var center = new Vector2(Screen.width / 2, Screen.height / 2);
		var size = 30;
		GUI.DrawTexture(new Rect(crosshairLocation.x - size / 2, crosshairLocation.y - size / 2, size, size), CrosshairTexture);
	}
}
