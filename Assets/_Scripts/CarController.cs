using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private bool isBraking;
    private float currentBrakeForce;
    private float currentSteerAngle;

    private bool isBeingOperated;
    private float currentVelocity;

    private float currentMotorTorque = 0f;

    public bool BeingOperated {
        get {
            return isBeingOperated;
        }
        set {
            isBeingOperated = value;
        }
    }

    [SerializeField] private VehicleSO vehicleSOData;
    [SerializeField] private Camera _camera;

    [SerializeField] private AudioSource engineIdleSound;
    [SerializeField] private AudioSource driveSound;
    [SerializeField] private AudioSource engineStartSound;
    [SerializeField] private AudioSource engineStopSound;

    public Camera Camera {
        get {
            return _camera;
        }
    }

    public VehicleSO VehicleData {
        get {
            return vehicleSOData;
        }
    }

    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider backRightWheelCollider;
    [SerializeField] private WheelCollider backLeftWheelCollider;

    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform backRightWheelTransform;
    [SerializeField] private Transform backLeftWheelTransform;

    [SerializeField] private Outline ot;

    [SerializeField] public VehicleExitPosition exitPosition;

    [SerializeField] private TruckBed truckBed;

    void Update() {
        if(isBeingOperated) {
            ot.enabled = false;
        }
        if(Input.GetKeyDown(KeyCode.E) && !exitPosition.isColliding) {
            if(Singleton<PlayerController>.Instance.TryExitVehicle()) {
                engineStopSound.Play();
                engineIdleSound.Stop();
            }
        }
    }

    void FixedUpdate()
    {
        if(!isBeingOperated) {
            verticalInput = 0;
            horizontalInput = 0;
            isBraking = true;

            frontLeftWheelCollider.steerAngle = 0;
            frontRightWheelCollider.steerAngle = 0;
        } else {
            GetInput();
        }

        if(verticalInput == 0 && driveSound.isPlaying) {
            driveSound.Stop();
        }

        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBraking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        // Calculate the target motor torque based on input
        float targetMotorTorque = verticalInput * VehicleData.motorForce;

        // Determine if the vehicle is changing direction
        bool changingDirection = (currentMotorTorque > 0 && verticalInput < 0) || (currentMotorTorque < 0 && verticalInput > 0);

        // Gradually adjust the current motor torque towards the target torque, except when changing direction
        if (!changingDirection)
        {
            currentMotorTorque = Mathf.MoveTowards(currentMotorTorque, targetMotorTorque, Time.deltaTime * VehicleData.accelerationRate);
        }
        else
        {
            // When changing direction, set motor torque to zero to quickly decelerate
            currentMotorTorque = 0f;
        }

        // Apply the current motor torque to all wheels only if vertical input is not zero and not braking
        if (verticalInput != 0 && !isBraking)
        {
            frontLeftWheelCollider.motorTorque = currentMotorTorque;
            frontRightWheelCollider.motorTorque = currentMotorTorque;
            backLeftWheelCollider.motorTorque = currentMotorTorque;
            backRightWheelCollider.motorTorque = currentMotorTorque;
        }
        else
        {
            // If no acceleration input or braking, set motor torque to zero
            frontLeftWheelCollider.motorTorque = 0f;
            frontRightWheelCollider.motorTorque = 0f;
            backLeftWheelCollider.motorTorque = 0f;
            backRightWheelCollider.motorTorque = 0f;
        }

        // Apply braking force if the vehicle is braking
        currentBrakeForce = isBraking ? VehicleData.brakeForce : 0f;
        ApplyBraking();

        // Play drive sound when accelerating
        if (verticalInput > 0 && !driveSound.isPlaying)
        {
            driveSound.Play();
        }
    }

    private void ApplyBraking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        backLeftWheelCollider.brakeTorque = currentBrakeForce;
        backRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = VehicleData.maxSteeringAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle; 
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(backLeftWheelCollider, backLeftWheelTransform);
        UpdateSingleWheel(backRightWheelCollider, backRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider collider, Transform t)
    {
        Vector3 pos;
        Quaternion rot;

        collider.GetWorldPose(out pos, out rot);

        rot = rot * Quaternion.Euler(new Vector3(0, -90, 0));

        t.position = pos;
        t.rotation = rot;
    }

    private void OnMouseOver() {
        ot.enabled = Helper.IsWithinPlayerReach(transform);
    }

    private void OnMouseExit() {
        ot.enabled = false;
    }

    private void OnMouseDown() {
        if(Singleton<PlayerController>.Instance.TryEnterVehicle(this)) {
            engineStartSound.Play();
            engineIdleSound.Play();
        }
    }
}
