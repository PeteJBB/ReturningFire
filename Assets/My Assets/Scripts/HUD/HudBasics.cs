using UnityEngine;
using System.Collections;

public class HudBasics : MonoBehaviour 
{
	[SerializeField]
	Texture CenterTexture;

	[SerializeField]
	Texture HorizonTexture;

	[SerializeField]
	Texture CompassTexture;

	[SerializeField]
	Texture SlipTexture;

	Vector2 _horizonSize = new Vector2(400, 256);

	void OnGUI()
	{
		// center point
		GUI.DrawTexture(Utility.GetCenteredRectangle(Utility.ScreenCenter(), CenterTexture.width, CenterTexture.height), CenterTexture);

		RenderHorizon();
		RenderCompass();
		RenderSlip();
	}

	void RenderHorizon()
	{
		var center = Utility.ScreenCenter();
		var destRect = Utility.GetCenteredRectangle(center, _horizonSize.x, _horizonSize.y);
		
		var degrees = _horizonSize.y / Screen.height * Camera.main.fieldOfView;
		var scale = degrees / 180;
		var pitch = transform.localEulerAngles.x;
		var roll = transform.localEulerAngles.z;;
		
		var offset = ((-pitch % 180) / 180);
		var pan = 0.5f + offset - (scale/2);
		var srcRect = new Rect(0, pan, 1, scale);
		Utility.DrawRotatedGuiTexture(destRect, roll, HorizonTexture, srcRect);
	}

	void RenderCompass()
	{
		// compass
		var center = new Vector2(Screen.width / 2, 25);
		var widthScale = 0.75f;
		var destRect = Utility.GetCenteredRectangle(center, Screen.width * widthScale, 30);
		
		var degrees = Camera.main.fieldOfView * Camera.main.aspect * 0.75f;
		var scale = degrees / 360;
		
		var heading = transform.localEulerAngles.y;
		var offset = (heading % 360) / 360;
		
		var pan = 0.5f + offset - (scale / 2);
		var srcRect = new Rect(pan, 0, scale, 1);
		Utility.DrawRotatedGuiTexture(destRect, 0, CompassTexture, srcRect);
	}

	void RenderSlip()
	{
		var center = Utility.ScreenCenter();
		var vect = transform.worldToLocalMatrix * rigidbody.velocity;
		var slipVector = new Vector2(vect.x, -vect.z) * 3f;

		var slipCenter = new Vector2(center.x, Screen.height - 60);
		Drawing.DrawLine(slipCenter, slipCenter + slipVector, Color.white, 1f, true);
		
		GUI.DrawTexture(Utility.GetCenteredRectangle(slipCenter + slipVector, SlipTexture.width, SlipTexture.height), SlipTexture);

	}
}
