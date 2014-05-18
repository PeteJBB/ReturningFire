using UnityEngine;
using System.Collections.Generic;

public class RadarScreen : MonoBehaviour
{
    [SerializeField]
    Texture BackgroundTexture;

    [SerializeField]
    Texture BlipTexture;

    float _radarRange = 350;

    GameObject _player;

    List<GameObject> detectedObjects = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    
    // Update is called once per frame
    void Update()
    {
        detectedObjects.Clear();
        foreach(var col in Physics.OverlapSphere(_player.transform.position, _radarRange))
        {
            if(col.gameObject.tag == "Enemy")
            {
                detectedObjects.Add(col.gameObject);
            }
        }
    }

    void OnGUI()
    {
        var center = new Vector2(Screen.width / 2, Screen.height / 2);

        var radarRect = new Rect(center.x - BackgroundTexture.width / 2, Screen.height - BackgroundTexture.height - 10, BackgroundTexture.width, BackgroundTexture.height);
        GUI.DrawTexture(radarRect, BackgroundTexture);

        var scale = radarRect.width / 2 / _radarRange;
        var orientation = Quaternion.AngleAxis(_player.transform.eulerAngles.y, Vector3.up);

        var playerPos = _player.transform.position;
        playerPos.y = 0;

        foreach(var obj in detectedObjects)
        {
            if(obj != null && obj.tag == "Enemy")
            {
                var centerPos=_player.transform.position;
                var extPos=obj.transform.position;
                
                // first we need to get the distance of the enemy from the player
                var dist=Vector3.Distance(centerPos,extPos);
                
                var dx=centerPos.x-extPos.x; // how far to the side of the player is the enemy?
                var dz=centerPos.z-extPos.z; // how far in front or behind the player is the enemy?
                
                // what's the angle to turn to face the enemy - compensating for the player's turning?
                var deltay=Mathf.Atan2(dx,dz)*Mathf.Rad2Deg - 270 - _player.transform.eulerAngles.y;
                
                // just basic trigonometry to find the point x,y (enemy's location) given the angle deltay
                var bX=dist*Mathf.Cos(deltay * Mathf.Deg2Rad);
                var bY=dist*Mathf.Sin(deltay * Mathf.Deg2Rad);

                var point = new Vector2(bX*scale, bY * scale);
                if(dist <= radarRect.width * 0.5 / scale)
                {
                    var blipRect = Utility.GetCenteredRectangle(radarRect.center + point, 8, 8);
                    GUI.DrawTexture(blipRect, BlipTexture);
                }
            }
        }
    }
}
