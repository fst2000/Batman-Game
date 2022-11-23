using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform origin;
    [SerializeField] float cameraHeight = 1f;
    [SerializeField] float cameraDistance = -3f;
    [SerializeField] float cameraSpeed = 0.1f;
    Vector3 cameraAngles = new Vector3();
    void Update()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        Vector3 mouseInput = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        cameraAngles += mouseInput * cameraSpeed;
        cameraAngles.x = Math.Clamp(cameraAngles.x,-45,70);
        Quaternion cameraRotation = Quaternion.Euler(cameraAngles);
        Vector3 cameraPosition = origin.position + origin.rotation * new Vector3(0,cameraHeight,0) + cameraRotation * new Vector3(0, 0, cameraDistance); ;
        Vector3 cameraRotatedPosition = cameraRotation * cameraPosition;
        transform.rotation = cameraRotation;
        transform.position = cameraPosition;

    }
}
