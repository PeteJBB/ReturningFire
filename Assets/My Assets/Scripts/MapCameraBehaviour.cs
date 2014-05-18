using UnityEngine;
using System.Collections;

public class MapCameraBehaviour : MonoBehaviour {

	GameObject _player;

//	[SerializeField]
//	Texture MapTexture;
	
	[SerializeField]
	Vector2 MapSize;
	
	[SerializeField]
	Texture MapPositionIcon;

	// Use this for initialization
	void Start () {
		_player = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(_player.transform.position.x, 250, _player.transform.position.z);
		transform.rotation = Quaternion.Euler(90, _player.transform.rotation.eulerAngles.y, 0);
	}

	void OnGUI() {
		var mapRect = new Rect(Screen.width - MapSize.x - 10, 10, MapSize.x, MapSize.y);
		GUI.DrawTexture(mapRect, camera.targetTexture, ScaleMode.ScaleToFit, false, MapSize.x / MapSize.y);	
		
		
		var iconRect = new Rect(mapRect.center.x - MapPositionIcon.width / 2,
		                        mapRect.center.y - MapPositionIcon.height / 2,
		                        MapPositionIcon.width, MapPositionIcon.height);
		
		GUI.DrawTexture(iconRect, MapPositionIcon, ScaleMode.ScaleToFit, true, 1);	

	}
}
