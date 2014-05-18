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
//        foreach(var hit in Physics.SphereCastAll(location, radius, Vector3.forward, 0))
//        {
//            if(hit.collider.gameObject
//        }



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


}
