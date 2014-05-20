using UnityEngine;
using System.Collections;

public class TurretPlasma : TurretBase 
{
	protected override Vector3 CalculateAimPoint()
	{
		var vect = _player.transform.position - _launcher.position; 

		// compute lead
		var rocketTimeToTarget = vect.magnitude / 75;
		var aim = _player.transform.position + (_player.rigidbody.velocity * rocketTimeToTarget);
		
		// aim above target for lobbing
		var dist = new Vector3(vect.x, 0, vect.z).magnitude;
		aim += Vector3.up * dist * dist / 1000;

		return aim;
	}
    

}
