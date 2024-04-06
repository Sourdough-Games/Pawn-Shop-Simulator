using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ProductPositionData
{
    public Vector3 Position;
    public Vector3 Rotation;
    //public Vector3 Scale = Vector3.one;

    public ProductPositionData(Vector3 position, Vector3 rotation/*, Vector3 scale*/) {
        this.Position = position;
        this.Rotation = rotation;
        //this.Scale = scale;
    }
}