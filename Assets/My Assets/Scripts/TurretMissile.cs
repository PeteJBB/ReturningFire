using UnityEngine;
using System.Collections;

public class TurretMissile : MonoBehaviour 
{
    [SerializeField]
    float DetectionRange = 100;
    
    [SerializeField]
    float FiringRange = 80;
    
    [SerializeField]
    float ReloadTime = 2;
    
    [SerializeField]
    float LockTime = 2;
    
    [SerializeField]
    float AttentionSpan = 2; // how long after losing sight will turret begin searching again
    
    [SerializeField]
    float LockSize = 10;
    
    [SerializeField]
    float TurretSpeed = 200;
    
    [SerializeField]
    float MaxHealth = 10;
    
    [SerializeField] 
    GameObject DeathExplosion;
    
    [SerializeField]
    GameObject Projectile;
    
    GameObject _target;
    Transform _platform;
    Transform _launcher;
    
    Vector3 _aimPoint;
    float _lastSeenTime;
    float _lockResetTime;
    float _lastShootTime;
    
	GameObject _hud;

    float _health;
    
    // Use this for initialization
    void Start () 
    {
        _health = MaxHealth;
        
        _target = GameObject.FindGameObjectWithTag("Player");
        _platform = transform.FindChild("platform");
        _launcher = _platform.FindChild("launcher");

		_hud = GameObject.FindGameObjectWithTag("HUD");
    }
    
    // Update is called once per frame
    void Update () 
    {
        var vect = _target.transform.position - _launcher.position; 
        if(vect.magnitude <= DetectionRange 
           && Utility.CanObjectSeeAnother(_launcher.gameObject, _target))
        {
			// can see target  
			_hud.SendMessage("Detected", gameObject, SendMessageOptions.DontRequireReceiver);

            _lastSeenTime = Time.fixedTime;
            
            // compute lead
            var rocketTimeToTarget = vect.magnitude / 100;
            _aimPoint = _target.transform.position + (_target.rigidbody.velocity * rocketTimeToTarget);
            RotateTowards(_aimPoint);
            
            var aimVect = _aimPoint - _launcher.position;
            _lockAngle = Vector3.Angle(_launcher.forward, aimVect);
            if(_lockAngle <= LockSize && Utility.CanSeePoint(_launcher.transform.position, _aimPoint, _target))
            {
                Debug.DrawLine(_launcher.position, _aimPoint, Color.green);
                if(aimVect.magnitude <= FiringRange // in range
                   && Time.fixedTime - _lockResetTime >= LockTime // locked
                   && Time.fixedTime - _lastShootTime >= ReloadTime) // loaded
                {
                    Fire();
                }
            }
            else
            {
                Debug.DrawLine(_launcher.position, _aimPoint, Color.red);
                _lockResetTime = Time.fixedTime;
            }
        }
        else
        {
            if(Time.fixedTime - _lastSeenTime > AttentionSpan)
            {
                // pick a new random aim point and wander
                _aimPoint = _launcher.transform.position + new Vector3(Random.Range(-500, 500), Random.Range(-50, 50), Random.Range(-500, 500));
                _lastSeenTime = Time.fixedTime;
            }
            RotateTowards(_aimPoint);
        }
    }
    
    void RotateTowards(Vector3 point)
    {
        var vect = point - _launcher.position;
        
        // rotate platform (y-axis only)
        var v = new Vector3(vect.x, 0, vect.z);
        var desiredRotation = Quaternion.LookRotation(v);
        _platform.rotation = Quaternion.RotateTowards(_platform.rotation, desiredRotation, TurretSpeed * Time.deltaTime);
        
        // rotate lancher - (x-axis only)
        v = new Vector3(0, vect.y, v.magnitude);
        desiredRotation = Quaternion.LookRotation(v);
        _launcher.localRotation = Quaternion.RotateTowards(_launcher.localRotation, desiredRotation, TurretSpeed * Time.deltaTime);
    }
    
    void Fire()
    {
        _lastShootTime = Time.fixedTime;
        
        var pos = _launcher.position;
        var clone = (GameObject)Instantiate(Projectile, pos, transform.rotation);
        clone.SendMessage("SetParent", gameObject, SendMessageOptions.DontRequireReceiver);
        Physics.IgnoreCollision(collider, clone.collider);
        clone.transform.rotation = _launcher.rotation;
        clone.rigidbody.AddForce(clone.transform.forward * 100, ForceMode.Impulse);
    }
    
    float _lockAngle;
    void OnGUI()
    {
        if(this.name == "turret_flak")
            GUI.TextArea(new Rect(120, 20, 100, 20), "Angle: " + _lockAngle, Utility.BasicGuiStyle);
        
    }
    
    void ApplyDamage(float amount)
    {
        _health -= amount;
        if(_health <= 0) 
        {
            Instantiate(DeathExplosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
