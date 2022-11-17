using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
    public float MoveVertical
    {
        get
        {
            return Input.GetAxis("Vertical");
        }
    }
    public float MoveHorizontal
    {
        get
        {
            return Input.GetAxis("Horizontal");
        }
    }
    public float MouseX
    {
        get
        {
            return Input.GetAxis("Mouse X");
        }
    }
    public float MouseY
    {
        get
        {
            return Input.GetAxis("Mouse Y");
        }
    }
    public Vector3 LookInput
    {
        get
        {
            Vector3 lookInput = new Vector3(-MouseY, MouseX, 0);
            return lookInput;
        }
    }
    public Vector3 WalkInput
    {
        get
        {
            Vector3 walkInput = Camera.main.transform.rotation * new Vector3(MoveHorizontal, 0, MoveVertical);
            walkInput.y = 0f;
            return walkInput.normalized;
        }
    }
    public bool IsAim
    {
        get
        {
            return Input.GetMouseButton(1);
        }
    }
}
