using UnityEngine;
using System.Collections;

public class Targeting : MonoBehaviour 
{
	private Detector _detector;
	private float _rectSize = 30;
	private float _animTime = 0.3f;

	void Start()
	{
		_detector = GetComponent<Detector>();
	}

	void OnGUI () 
	{
		var screenRect = new Rect(0,0,Screen.width, Screen.height);
		foreach(var d in _detector.Detections.Values)
		{
			var obj = d.gameObject;
			if(obj != null)
			{
				var vect = obj.transform.position - transform.position;
				var point = Utility.WorldToGUIPoint(obj.collider.bounds.center);

				// coloring
				var reverseDetector = obj.GetComponent<Detector>();
				var color = reverseDetector != null && reverseDetector.CanSee(gameObject)
					? Color.yellow
					: Color.white;

				if(screenRect.Contains(point) && point.z > 0)
				{
					// get screen rect
					var t = Mathf.Min((Time.fixedTime - d.firstDetected) / _animTime, 1);
					var size = Mathf.Lerp(Screen.width, _rectSize, t);
					var rect = Utility.GetCenteredRectangle(point, size, size);

					// draw rect
					Drawing.DrawLine(rect.min, new Vector2(rect.xMax, rect.yMin), color, 1, true);
					Drawing.DrawLine(rect.min, new Vector2(rect.xMin, rect.yMax), color, 1, true);
					Drawing.DrawLine(rect.max, new Vector2(rect.xMax, rect.yMin), color, 1, true);
					Drawing.DrawLine(rect.max, new Vector2(rect.xMin, rect.yMax), color, 1, true);

					// range label
					GUI.TextField(new Rect(rect.xMin, rect.yMax + 2, 1, 1), vect.magnitude.ToString("0"), Utility.BasicGuiStyle);
				}
				else
				{
					Vector2 edgePoint;
					if(Utility.RectLineIntersection(screenRect, screenRect.center, point, out edgePoint))
					{
						var dir = (edgePoint - screenRect.center).normalized;
						var p = new Vector2(dir.x * 20, dir.y * 20);

						Drawing.DrawLine(edgePoint - p, edgePoint, color, 5, false);
					}
				}


			}
		}
	}
}
