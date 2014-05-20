using UnityEngine;
using System.Collections.Generic;

public class RadarScreen : MonoBehaviour
{
    [SerializeField]
    Texture BackgroundTexture;

	[SerializeField]
	Material BackgroundMaterial;

	[SerializeField]
	Texture OverlayTexture;

    [SerializeField]
    Texture BlipTexture;

	[SerializeField]
	Vector2 RadarSize;

    float _radarRange = 250;

	GameObject _mapCamera;

    GameObject _player;

    List<GameObject> detectedObjects = new List<GameObject>();
	List<GameObject> detectedBy = new List<GameObject>();

	float _warningClearTimer = 1;
	float _lastClearedWarning;

    // Use this for initialization
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

		_mapCamera = new GameObject("Camera");
		_mapCamera.AddComponent<Camera>();
		_mapCamera.camera.backgroundColor = Color.black;
		_mapCamera.camera.aspect = 1;
		_mapCamera.camera.targetTexture = (RenderTexture)BackgroundTexture;
		_mapCamera.camera.isOrthoGraphic = true;
		_mapCamera.camera.orthographicSize = _radarRange;
		_mapCamera.camera.cullingMask = 1 << (int)Layers.Terrain;
    }
    
    // Update is called once per frame
    void Update()
    {
		_mapCamera.transform.position = new Vector3(_player.transform.position.x, 250, _player.transform.position.z);
		_mapCamera.transform.rotation = Quaternion.Euler(90, _player.transform.rotation.eulerAngles.y, 0);

//		if(Time.fixedTime - _lastClearedWarning > _warningClearTimer)
//		{
//			detectedBy.Clear();
//			_lastClearedWarning = Time.fixedTime;
//		}
		detectedObjects.Clear();
        foreach(var col in Physics.OverlapSphere(_player.transform.position, _radarRange))
        {
            if(col.gameObject.tag == "Enemy")
            {
                detectedObjects.Add(col.gameObject);
            }
        }
    }

	// message reciever - enemy can see you!
	void Detected(GameObject enemy)
	{
		if(!detectedBy.Contains(enemy))
			detectedBy.Add(enemy);
	}

	// message reciever - enemy has lost sight of you
	void Undetected(GameObject enemy)
	{
		if(detectedBy.Contains(enemy))
			detectedBy.Remove(enemy);
	}

    void OnGUI()
    {
		var radarRect = new Rect(Screen.width - RadarSize.x - 10, 10, RadarSize.x, RadarSize.y);

		// draw background
		Graphics.DrawTexture(radarRect, BackgroundTexture, BackgroundMaterial);

		// draw overlay
		GUI.DrawTexture(radarRect, OverlayTexture);

		// draw blips
        var scale = RadarSize.x / 2 / _radarRange;
        var playerPos = new Vector3(_player.transform.position.x, 0, _player.transform.position.z);

        foreach(var obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
        	// first we need to get the distance of the enemy from the player
			var enemyPos = new Vector3(obj.transform.position.x, 0, obj.transform.position.z);
			var dist = Vector3.Distance(playerPos, enemyPos);

			if(dist < _radarRange) 
			{
				var dx = playerPos.x - enemyPos.x; // how far to the side of the player is the enemy?
				var dz = playerPos.z - enemyPos.z; // how far in front or behind the player is the enemy?
                
                // what's the angle to turn to face the enemy - compensating for the player's turning?
                var deltay = Mathf.Atan2(dx,dz) * Mathf.Rad2Deg - 270 - _player.transform.eulerAngles.y;
                
                // just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
                var bX = dist * Mathf.Cos(deltay * Mathf.Deg2Rad);
                var bY = dist * Mathf.Sin(deltay * Mathf.Deg2Rad);

				var point = new Vector2(bX, bY) * scale;

	            var blipRect = Utility.GetCenteredRectangle(radarRect.center + point, 12, 12);
				if(detectedBy.Contains(obj))
				{
					GUI.color = Color.yellow;
		            GUI.DrawTexture(blipRect, BlipTexture);
					Drawing.DrawLine(radarRect.center, blipRect.center, Color.yellow, 1f, true);
				}
				else
				{
					GUI.color = Color.white;
					GUI.DrawTexture(blipRect, BlipTexture);
				}
			}
        }

		// reset gui
		GUI.color = Color.white;
    }
}
