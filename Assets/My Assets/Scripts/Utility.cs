﻿using UnityEngine;
using System.Collections;

public class Utility
{
	public static Vector3 WorldToGUIPoint(Vector3 position)
	{
		var pos = Camera.main.WorldToScreenPoint(position);
		pos.y = Screen.height - pos.y;
		if(pos.z < 0)
			pos *= -1;
		return pos;
	}

	public static GUIStyle BasicGuiStyle = new GUIStyle()
    {
        clipping = TextClipping.Overflow,
        wordWrap = false,
        normal = new GUIStyleState()
        {
            textColor = Color.white
        }
    };

	public static Vector2 ScreenCenter()
	{
		return new Vector2(Screen.width / 2f, Screen.height / 2f);
	}

	public static void DrawRotatedGuiTexture(Rect rect, float angle, Texture texture)
	{
		Matrix4x4 matrixBackup = GUI.matrix;
		GUIUtility.RotateAroundPivot(angle, rect.center);
		GUI.DrawTexture(rect, texture);
		GUI.matrix = matrixBackup;
	}

	public static void DrawRotatedGuiTexture(Rect screenRect, float angle, Texture texture, Rect srcRect, Material mat)
	{
		if (Event.current.type == EventType.Repaint)
		{
			Matrix4x4 matrixBackup = GUI.matrix;
			GUIUtility.RotateAroundPivot(angle, screenRect.center);
			Graphics.DrawTexture(screenRect, texture, srcRect, 0, 0, 0, 0, mat);
			GUI.matrix = matrixBackup;
		}
	}

	public static bool CanSeePoint(Vector3 from, Vector3 to, GameObject target)
	{
		RaycastHit hitInfo;
		bool b = Physics.Linecast(from, to, out hitInfo);
		if (!b || (target != null && hitInfo.collider.gameObject == target))
			return true;

		return false;
	}

	public static bool CanSeeBounds(Vector3 from, Bounds bounds, GameObject target)
	{
		RaycastHit hitInfo;
		bool b = Physics.Linecast(from, bounds.center, out hitInfo);
		if (!b || (target != null && hitInfo.collider.gameObject == target))
			return true;

		b = Physics.Linecast(from, bounds.max, out hitInfo);
		if (!b || (target != null && hitInfo.collider.gameObject == target))
			return true;

		b = Physics.Linecast(from, bounds.min, out hitInfo);
		if (!b || (target != null && hitInfo.collider.gameObject == target))
			return true;

		return false;
	}

	public static bool CanObjectSeeAnother(GameObject a, GameObject b)
	{
		RaycastHit hitInfo;
		if (Physics.Linecast(a.transform.position, b.transform.position, out hitInfo))
		{
			return hitInfo.collider.gameObject == b;
		}
		return false;
	}

	public static Rect GetCenteredRectangle(Vector2 center, float width, float height)
	{
		var rect = new Rect(center.x - width / 2, center.y - height / 2, width, height);
		return rect;
	}

	public static void AreaDamageEnemies(Vector3 location, float radius, float damage)
	{
		var objectsInRange = Physics.OverlapSphere(location, radius);
		foreach (var col in objectsInRange)
		{
			if (col.gameObject != null)
			{
				// test if enemy is exposed to blast, or behind cover:
				//RaycastHit hit;
				//var exposed = false;
				//                if (Physics.Raycast (location, (col.transform.position - location), out hit)) 
				//                {
				//                    exposed = (hit.collider == col);
				//                }
				//                
				//                if (exposed) 
				//                {
				// Damage Enemy! with a linear falloff of damage amount
				var proximity = (location - col.transform.position).magnitude;
				var effect = 1 - (proximity / radius);
				if (effect > 0)
					col.gameObject.SendMessage("ApplyDamage", damage * effect, SendMessageOptions.DontRequireReceiver);
				//}
			}
		}
	}

	//http://www.morgan-davidson.com/2012/06/19/3d-projectile-trajectory-prediction/
	Vector3 GetTrajectoryPoint(Vector3 startingPosition, Vector3 initialVelocity, float timestep, Vector3 gravity)
	{
		float physicsTimestep = Time.fixedDeltaTime;
		Vector3 stepVelocity = physicsTimestep * initialVelocity;
		
		//Gravity is already in meters per second, so we need meters per second per second
		Vector3 stepGravity = physicsTimestep * physicsTimestep * gravity;
		
		return startingPosition + (timestep * stepVelocity) + (((timestep * timestep + timestep) * stepGravity) / 2.0f);
	}

	public static Vector3 GetTrajectoryVelocity(Vector3 startingPosition, Vector3 targetPosition, float lob, Vector3 gravity)
	{
		float physicsTimestep = Time.fixedDeltaTime;
		float timestepsPerSecond = Mathf.Ceil(1f / physicsTimestep);
		
		//By default we set n so our projectile will reach our target point in 1 second
		float n = lob * timestepsPerSecond;
		
		Vector3 a = physicsTimestep * physicsTimestep * gravity;
		Vector3 p = targetPosition;
		Vector3 s = startingPosition;
		
		Vector3 velocity = (s + (((n * n + n) * a) / 2f) - p) * -1 / n;
		
		//This will give us velocity per timestep. The physics engine expects velocity in terms of meters per second
		velocity /= physicsTimestep;
		return velocity;
	}

	public static float GetTrajectoryAngle(Vector3 startingPosition, Vector3 targetPosition, float muzzleSpeed, Vector3 gravity)
	{
		var range = (targetPosition - startingPosition).magnitude;
		var angle = (Mathf.Asin(range * gravity.magnitude / (muzzleSpeed * muzzleSpeed))) / 2;

		return angle;
	}

	public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
	{
		intersection = Vector2.zero;

		float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num/*,offset*/;
		float x1lo, x1hi, y1lo, y1hi;
		Ax = p2.x - p1.x;
		Bx = p3.x - p4.x;
		
		// X bound box test/
		if (Ax < 0)
		{
			x1lo = p2.x;
			x1hi = p1.x;
		} 
		else
		{
			x1hi = p2.x;
			x1lo = p1.x;
		}

		if (Bx > 0)
		{
			if (x1hi < p4.x || p3.x < x1lo)
				return false;
		} 
		else
		{
			if (x1hi < p3.x || p4.x < x1lo)
				return false;
		}

		Ay = p2.y - p1.y;
		By = p3.y - p4.y;

		// Y bound box test//
		if (Ay < 0)
		{                  
			y1lo = p2.y;
			y1hi = p1.y;
			
		} 
		else
		{
			y1hi = p2.y;
			y1lo = p1.y;
		}

		if (By > 0)
		{
			if (y1hi < p4.y || p3.y < y1lo)
				return false;
		} 
		else
		{
			if (y1hi < p3.y || p4.y < y1lo)
				return false;
		}

		Cx = p1.x - p3.x;
		Cy = p1.y - p3.y;

		d = By * Cx - Bx * Cy;  // alpha numerator //
		f = Ay * Bx - Ax * By;  // both denominator //

		// alpha tests //
		if (f > 0)
		{
			if (d < 0 || d > f)
				return false;
		} 
		else
		{
			if (d > 0 || d < f)
				return false;
		}

		e = Ax * Cy - Ay * Cx;  // beta numerator //

		// beta tests //
		if (f > 0)
		{                          
			if (e < 0 || e > f)
				return false;
		} 
		else
		{
			if (e > 0 || e < f)
				return false;
		}

		// check if they are parallel
		if (f == 0)
			return false;
		
		// compute intersection coordinates //
		num = d * Ax; // numerator //
		
		//    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;   // round direction //
		
		//    intersection.x = p1.x + (num+offset) / f;
		intersection.x = p1.x + num / f;

		num = d * Ay;
		
		//    offset = same_sign(num,f) ? f*0.5f : -f*0.5f;
		
		//    intersection.y = p1.y + (num+offset) / f;
		intersection.y = p1.y + num / f;

		return true;
	}

	public static bool RectLineIntersection(Rect r, Vector2 p1, Vector2 p2, out Vector2 intersection)
	{
		// top
		if(LineIntersection(r.min, new Vector2(r.xMax, r.yMin), p1, p2, out intersection))
			return true;

		// left
		if(LineIntersection(r.min, new Vector2(r.xMin, r.yMax), p1, p2, out intersection))
			return true;

		// bottom
		if(LineIntersection(r.max, new Vector2(r.xMin, r.yMax), p1, p2, out intersection))
			return true;

		// right
		if(LineIntersection(r.max, new Vector2(r.xMax, r.yMin), p1, p2, out intersection))
			return true;

		intersection = Vector2.zero;
		return false;
	}
}
