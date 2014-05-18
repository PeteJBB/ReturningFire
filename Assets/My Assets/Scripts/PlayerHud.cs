using UnityEngine;
using System.Collections;

public class PlayerHud : MonoBehaviour {

    PlayerBehaviour _player;

    Texture2D crosshairTex;
    Vector2 crosshairLocation;

	// Use this for initialization
	void Start () {
        _player = this.GetComponent(typeof(PlayerBehaviour)) as PlayerBehaviour;

        crosshairTex = (Texture2D)Resources.Load("HUD/crosshair_rocket");    
	}
	
	// Update is called once per frame
	void Update () {
        crosshairLocation = Camera.main.WorldToScreenPoint(_player.aimPoint);
        crosshairLocation.y = Screen.height - crosshairLocation.y;
	}

    void OnGUI(){

        // crosshair
        var center = new Vector2(Screen.width / 2, Screen.height / 2);
        var size = 30;
        GUI.DrawTexture(new Rect(crosshairLocation.x - size / 2, crosshairLocation.y - size / 2, size, size), crosshairTex);

        // guages
        GUI.TextArea(new Rect(20, 20, 100, 20), "Alt: " + transform.position.y, Utility.BasicGuiStyle );
        GUI.TextArea(new Rect(20, 50, 100, 20), "Collective: " + _player.collective * 100 + "%", Utility.BasicGuiStyle);
        //GUI.TextArea(new Rect(20, 80, 100, 20), "Drag: " + rigidbody.drag, Utility.BasicGuiStyle);
    }
}
