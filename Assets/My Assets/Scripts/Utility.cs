using UnityEngine;
using System.Collections;

public class Utility {

    public static GUIStyle BasicGuiStyle = new GUIStyle()
    {
        clipping = TextClipping.Overflow,
        wordWrap = false,
        normal = new GUIStyleState()
        {
            textColor = Color.white
        }
    };

    public static bool CanSeePoint(Vector3 from, Vector3 to, GameObject target)
    {
        RaycastHit hitInfo;
        bool b = Physics.Linecast(from, to, out hitInfo);
        if(!b || (target != null && hitInfo.collider.gameObject == target))
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
                if(effect > 0)
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
		
		return startingPosition + (timestep * stepVelocity) + ((( timestep * timestep + timestep) * stepGravity ) / 2.0f);
	}

	public static Vector3 GetTrajectoryVelocity(Vector3 startingPosition, Vector3 targetPosition, float lob, Vector3 gravity)
	{
		float physicsTimestep = Time.fixedDeltaTime;
		float timestepsPerSecond = Mathf.Ceil(1f/physicsTimestep);
		
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
}
