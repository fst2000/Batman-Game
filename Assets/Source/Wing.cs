using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing
{
    Vector3 localNormal = Vector3.up;
    Vector3 localPosition = Vector3.zero;
    float sizeX, sizeY;
    Quaternion rotation; 
    public Wing(Vector3 localNormal, Vector3 localPosition, float sizeX, float sizeY)
    {
        this.localNormal = localNormal;
        this.localPosition = localPosition;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        rotation = Quaternion.identity;
    }
    public float GetSquare()
    {
        return sizeX * sizeY;
    }
    public Vector3 GetLocalPosition()
    {
        return localPosition;
    }
    public Vector3 GetLocalNormal()
    {
        return rotation * localNormal;
    }
    public Vector3 GetSize()
    {
        return new Vector3(sizeX, sizeY,0);
    }
    public void UpdateRotation(Vector3 euler)
    {
        rotation = Quaternion.Euler(euler);
    }
    public Quaternion GetWingRotation() 
    {
        return rotation;
    }
    
}
