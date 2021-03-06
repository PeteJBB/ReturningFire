﻿using UnityEngine;
using System.Collections.Generic;

public class RadarScreen : MonoBehaviour
{
	public Material BackgroundMaterial;
	public Texture OverlayTexture;
	public Texture BlipTexture;
	public Vector2 RadarSize;

	private Detector _detector;

	private GameObject _mapCamGameObject;
	private Camera _mapCam;

	private Rect _radarRect;

	void Start()
	{
		_detector = GetComponent<Detector>();

//		_mapCamGameObject = new GameObject("MapCamera");
//		_mapCam = _mapCamGameObject.AddComponent<Camera>();
//		_mapCam.aspect = 1;
//		_mapCam.isOrthoGraphic = true;
//		_mapCam.orthographicSize = _detector.DetectionRange / 2;
//		_mapCam.depth = 1;

	}

//	void Update()
//	{
//		var padding = 10f;
//		_radarRect = new Rect(Screen.width - RadarSize.x - padding, padding, RadarSize.x, RadarSize.y);
//
//		var ratio = Screen.width / Screen.height;
//		var height = RadarSize.y / Screen.height;
//		var width = height / ratio;
//		var px = 1f - (padding / Screen.width);
//		var py = 1f - (padding / Screen.height);
//		_mapCam.rect = new Rect(px - width, py - height, width, height);
//
//		var pos = transform.position;
//		pos.y = 500;
//		_mapCam.transform.position = pos;
//		_mapCam.transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y, 0);
//	}

    void OnGUI()
    {
		_radarRect = new Rect(Screen.width - RadarSize.x - 10, 10, RadarSize.x, RadarSize.y);

		// draw background
		var bgScale = new Vector2(_detector.DetectionRange / BackgroundMaterial.mainTexture.width, 
		                          _detector.DetectionRange / BackgroundMaterial.mainTexture.height);
		var bgOffset = new Vector2((0.5f - bgScale.x / 2) + (transform.position.x / Terrain.activeTerrain.terrainData.size.x),
		                           (0.5f - bgScale.x / 2) + (transform.position.z / Terrain.activeTerrain.terrainData.size.z));

		BackgroundMaterial.mainTextureScale = bgScale;
		BackgroundMaterial.mainTextureOffset = bgOffset;
		Utility.DrawRotatedGuiTexture(_radarRect, -transform.rotation.eulerAngles.y, BackgroundMaterial.mainTexture, new Rect(0,0,1,1), BackgroundMaterial);
		
		// draw overlay
		GUI.DrawTexture(_radarRect, OverlayTexture);

		// calculate blip positions
		var scale = RadarSize.x / 2 / _detector.DetectionRange;
        var playerPos = new Vector3(transform.position.x, 0, transform.position.z);
		foreach(var d in _detector.Detections.Values)
        {
			var obj = d.gameObject;
			if(obj != null)
			{
	        	// first we need to get the distance of the enemy from the player
				var enemyPos = new Vector3(obj.transform.position.x, 0, obj.transform.position.z);
				var dist = Vector3.Distance(playerPos, enemyPos);

				if(dist < _detector.DetectionRange)
				{
					var dx = playerPos.x - enemyPos.x; // how far to the side of the player is the enemy?
					var dz = playerPos.z - enemyPos.z; // how far in front or behind the player is the enemy?
		            
		            // what's the angle to turn to face the enemy - compensating for the player's turning?
		            var deltay = Mathf.Atan2(dx,dz) * Mathf.Rad2Deg - 270 - transform.eulerAngles.y;
		            
		            // just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
		            var bX = dist * Mathf.Cos(deltay * Mathf.Deg2Rad);
		            var bY = dist * Mathf.Sin(deltay * Mathf.Deg2Rad);

					var point = new Vector2(bX, bY) * scale;

					var blipRect = Utility.GetCenteredRectangle(_radarRect.center + point, 12, 12);
					var reverseDetector = obj.GetComponent<Detector>();
					if(reverseDetector != null && reverseDetector.CanSee(gameObject))
					{
						GUI.color = Color.yellow;
			            GUI.DrawTexture(blipRect, BlipTexture);
						Drawing.DrawLine(_radarRect.center, blipRect.center, Color.yellow, 1f, true);
					}
					else
					{
						GUI.color = Color.white;
						GUI.DrawTexture(blipRect, BlipTexture);
					}
				}
			}
        }

		// reset gui
		GUI.color = Color.white;
    }
}
