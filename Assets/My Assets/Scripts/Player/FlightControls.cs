﻿using UnityEngine;
using System.Collections;

public class FlightControls : MonoBehaviour
{
    [SerializeField] 
    float PitchStrength = 24;

    [SerializeField] 
    private float YawStrength = 20;

    [SerializeField] 
    float RollStrength = 12;

    [SerializeField] 
    float EnginePower = 400;

    float _collective; // what the pilot has set
    Vector3 dragVector; // the calculated drag this frame
    float deltaMultiplier;
    float _lastHitTime;
 

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        deltaMultiplier = Time.deltaTime / 0.02f;
        UpdateFlightControls();

        // reset
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel(Application.loadedLevelName);
        }
    }

    void UpdateFlightControls()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _collective = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _collective = 0;
        }
        else
        {
            // hover
            var thrustAngle = Mathf.Deg2Rad * Vector3.Angle(Physics.gravity, -transform.up);
            var energy = (rigidbody.mass * Physics.gravity.magnitude) - (rigidbody.velocity.y * Mathf.Abs(rigidbody.velocity.y));
            var thrustNeeded = energy / Mathf.Cos(thrustAngle);
            _collective = Mathf.Clamp(thrustNeeded / EnginePower, 0, 1);
        }

        this.rigidbody.AddRelativeForce(new Vector3(0, EnginePower * _collective * deltaMultiplier, 0));
        
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
            -relativeVel.x * Mathf.Abs(relativeVel.x) * 0.5f,
            -relativeVel.y * Mathf.Abs(relativeVel.y) * 0.5f,
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

    void ApplyDamage(float amount)
    {
        print("You've been hit! (" + amount + ")");
        _lastHitTime = Time.fixedTime;
    }

    void OnGUI()
    {
		GUI.TextArea(new Rect(20, 20, 100, 20), "Alt: " + transform.position.y, Utility.BasicGuiStyle );
		GUI.TextArea(new Rect(20, 50, 100, 20), "Collective: " + _collective * 100 + "%", Utility.BasicGuiStyle);

    }
}
