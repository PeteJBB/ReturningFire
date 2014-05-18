using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] 
    float PitchStrength = 24;

    [SerializeField] 
    private float YawStrength = 20;

    [SerializeField] 
    float RollStrength = 12;

    [SerializeField] 
    float EnginePower = 400;


    // flight controls
    public float collective; // what the pilot has set
    public Vector3 aimPoint;

    Vector3 dragVector; // the calculated drag this frame


    // weapons
    [SerializeField]
    GameObject Rocket;
    
    bool nextRocketLeftSide = false;
    Vector2 crosshairLocation = new Vector2(Screen.width / 2, Screen.height / 2);

    float deltaMultiplier;

    BlurEffect _blurEffect;
    float _lastHitTime;
 

    // Use this for initialization
    void Start()
    {
        _blurEffect = Camera.main.GetComponent<BlurEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaMultiplier = Time.deltaTime / 0.02f;
        UpdateFlightControls();
        AimAndFireRockets();

        // reset
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel("scene2");
        }
    }

    void UpdateFlightControls()
    {
        if (Input.GetKey(KeyCode.W))
        {
            collective = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            collective = 0;
        }
        else
        {
            // hover
            var thrustAngle = Mathf.Deg2Rad * Vector3.Angle(Physics.gravity, -transform.up);
            var energy = (rigidbody.mass * Physics.gravity.magnitude) - (rigidbody.velocity.y * Mathf.Abs(rigidbody.velocity.y));
            var thrustNeeded = energy / Mathf.Cos(thrustAngle);
            collective = Mathf.Clamp(thrustNeeded / EnginePower, 0, 100);
        }

        this.rigidbody.AddRelativeForce(new Vector3(0, EnginePower * collective * deltaMultiplier, 0));
        
        // pitch
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.rigidbody.AddRelativeTorque(new Vector3(PitchStrength * deltaMultiplier, 0, 0));
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.rigidbody.AddRelativeTorque(new Vector3(-PitchStrength * deltaMultiplier, 0, 0));
        }

        // yaw
        if (Input.GetKey(KeyCode.D))
        {
            this.rigidbody.AddRelativeTorque(new Vector3(0, YawStrength * deltaMultiplier, -RollStrength * 0.5f  * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.rigidbody.AddRelativeTorque(new Vector3(0, -YawStrength * deltaMultiplier, RollStrength * 0.5f * Time.deltaTime));
        }

        // roll
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.rigidbody.AddRelativeTorque(new Vector3(0, 0, RollStrength * deltaMultiplier));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.rigidbody.AddRelativeTorque(new Vector3(0, 0, -RollStrength * deltaMultiplier));
        }

        // drag by facing area
        var relativeVel = rigidbody.transform.worldToLocalMatrix * rigidbody.velocity;
        dragVector = new Vector3(
            -relativeVel.x * Mathf.Abs(relativeVel.x) * 0.4f,
            -relativeVel.y * Mathf.Abs(relativeVel.y) * 0.3f,
            -relativeVel.z * Mathf.Abs(relativeVel.z) * 0.15f
        );
        rigidbody.AddRelativeForce(dragVector * deltaMultiplier);

        // lateral motion causes rotation towards that dir
        var correctionVector = new Vector3(
            -relativeVel.y * Mathf.Abs(relativeVel.y) * 0.01f,
            relativeVel.x * Mathf.Abs(relativeVel.x) * 0.1f,
            0
        );
        rigidbody.AddRelativeTorque(correctionVector * deltaMultiplier);

    }

    void AimAndFireRockets()
    {
        // rockets
        var launchPoint = transform.position - transform.up;
        aimPoint = launchPoint + (transform.forward * 500);
        
        // try and set aim point at the correct point we are aiming at
        // this will get rockets to converge nicely
        RaycastHit hitInfo;
        if (Physics.Raycast(launchPoint, transform.forward, out hitInfo))
        {
            aimPoint = hitInfo.point;
        }

        // fire
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // position rocket on left or right
            var pos = launchPoint + (nextRocketLeftSide ? -transform.right : transform.right) * 2;
            nextRocketLeftSide = !nextRocketLeftSide;
            
            // create and fire
            var clone = (GameObject)Instantiate(Rocket, pos, transform.rotation);
            clone.SendMessage("SetParent", gameObject, SendMessageOptions.DontRequireReceiver);
            Physics.IgnoreCollision(collider, clone.collider);
            clone.transform.LookAt(aimPoint);
            clone.rigidbody.AddForce(clone.transform.forward * 100, ForceMode.Impulse);
        }

        if(_blurEffect.enabled)
        {
            var time = Time.fixedTime - _lastHitTime;
            var blurTime = 1;
            if(time >= blurTime)
                _blurEffect.enabled = false;
            else
                _blurEffect.blurSpread = 1 - time/blurTime;
        }
    }

    void ApplyDamage(float amount)
    {
        print("You've been hit! (" + amount + ")");
        _lastHitTime = Time.fixedTime;
        _blurEffect.enabled = true;
        _blurEffect.blurSpread = 1;
    }

    void OnGUI()
    {
        //GUI.TextArea(new Rect(20, 140, 100, 20), "Drag: " + dragVector, Utility.BasicGuiStyle);
    }
}
