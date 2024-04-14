using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VehicleSO")]
public class VehicleSO : ScriptableObject
{
    public GameObject prefab;

    public string VehicleName;

    public string LocalizationKey;

    public float maxSteeringAngle;
    public float motorForce;
    public float brakeForce;
    public float accelerationRate;

    public GenericPositionData sitPosition;
}
