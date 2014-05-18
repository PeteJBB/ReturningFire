using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {

    GameObject _player;
    PlayerBehaviour _playerBehaviour;

    GameObject _horizon;
    float _horizonScaleOffset;

    GameObject _compass;
    float _compassScaleOffset;

    [SerializeField]
    Texture FinalRenderTexture;

    [SerializeField]
    Texture CenterTexture;

    [SerializeField]
    Texture SlipTexture;
    Vector2 slipVector;

	// Use this for initialization
	void Start () 
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerBehaviour = _player.GetComponent<PlayerBehaviour>();

        _horizon = transform.Find("horizon").gameObject;
        _compass = transform.Find("compass").gameObject;

    }

	// Update is called once per frame
	void Update () 
    {
        UpdateHorizon();
        UpdateCompass();
        UpdateSlip();
	}

    void UpdateHorizon()
    {
        // how many degreen does the quad cover?
        var degrees = (_horizon.transform.localScale.y / 2) * Camera.main.fieldOfView;

        // scale texture so that each pip covers 10 degrees
        var textureScale = degrees / 180;

        _horizon.renderer.material.mainTextureScale = new Vector2(1, textureScale);
        _horizonScaleOffset = ((1 - textureScale) / 2) + (_horizon.transform.position.y * textureScale);

        var pitch = _player.transform.localEulerAngles.x;
        var yOffset = (-pitch % 180) / 180; // texture provides pips for 180 degrees
        _horizon.renderer.material.mainTextureOffset = new Vector2(0, yOffset + _horizonScaleOffset);
        
        var roll = _player.transform.localEulerAngles.z;
        _horizon.transform.rotation = Quaternion.Euler(0, 0, -roll);
    }

    void UpdateCompass()
    {
        var direction = _player.transform.localEulerAngles.y;
        var offset = (direction % 360) / 360;
        _compass.renderer.material.mainTextureOffset = new Vector2(offset + _compassScaleOffset, 0);
    }

    void UpdateSlip()
    {
        var vect = _player.transform.worldToLocalMatrix * _player.rigidbody.velocity;
        slipVector = new Vector2(vect.x, -vect.z) * 3f;
    }

    void OnGUI()
    {
        var center = new Vector2(Screen.width / 2f, Screen.height / 2f);

        // horizon / compass overlay
        GUI.DrawTexture(new Rect(center.x - Screen.height / 2, 0, Screen.height, Screen.height), FinalRenderTexture, ScaleMode.ScaleAndCrop, true);

        // center point (cross)
        GUI.DrawTexture(Utility.GetCenteredRectangle(center, CenterTexture.width, CenterTexture.height), CenterTexture);

        // slip
        var slipCenter = new Vector2(center.x, Screen.height - 148);
        Drawing.DrawLine(slipCenter, slipCenter + slipVector, Color.white, 1f, true);

        GUI.DrawTexture(Utility.GetCenteredRectangle(slipCenter + slipVector, SlipTexture.width, SlipTexture.height), SlipTexture);

    }


}
