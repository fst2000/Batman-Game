using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Batman
{
    [SerializeField] float walkSpeed = 2.5f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float wingRotationAngle = 20f;
    [SerializeField] Vector3 centerOfMass = new Vector3(0, 1f, 0.2f);
    [SerializeField] Transform[] rotatedBones = new Transform[5];
    PlayerInput playerInput;
    Rigidbody rigidbody;    
    Animator animator;
    Transform transform;
    bool isFlyingStarted;

    Wing[] wings;
    Wing wingL;
    Wing wingR;
    Wing wingBack;
    Wing wingStabilize;
    public void Initialize(GameObject gameObject)
    {
        animator = gameObject.GetComponent<Animator>();
        playerInput = new PlayerInput();
        rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rigidbody.mass = 90f;
        rigidbody.centerOfMass = centerOfMass;
        rigidbody.angularDrag = 2f;
        transform = gameObject.transform;

        wingL = new Wing(Vector3.ClampMagnitude(new Vector3(1, -1, -10), 1), new Vector3(-1f,1,0), 2f,1f);
        wingR = new Wing(Vector3.ClampMagnitude(new Vector3(-1, -1, -10), 1), new Vector3(1f,1,0), 2f,1f);
        wingBack = new Wing(Vector3.back, Vector3.zero, 1f, 0.5f);
        wingStabilize = new Wing(Vector3.right, new Vector3(0,0,0.2f), .25f, .5f);
        wings = new Wing[] {wingL,wingR, wingBack ,wingStabilize};
    }
    public void Update()
    {
        
        if (Physics.CheckSphere(transform.position, 0.6f))
        {
            Move(Input.GetKey("left shift") ? runSpeed : walkSpeed);
        }
        else Fly();
    }
    void Move(float moveSpeed)
    {
        isFlyingStarted = true;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        bool isMoving = playerInput.WalkInput != Vector3.zero;
        Vector3 velocity = rigidbody.velocity;
        Vector3 moveVelocity = new Vector3(playerInput.WalkInput.x * moveSpeed, velocity.y, playerInput.WalkInput.z * moveSpeed);
        rigidbody.velocity = moveVelocity;
        if (isMoving)
        {
            transform.rotation = Quaternion.LookRotation(playerInput.WalkInput, Vector3.up);
        }
        animator.SetBool("isFlying", false);
        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("moveSpeedBlend", moveSpeed);
    }
    void Fly()
    {
        if (isFlyingStarted)
        {
            transform.rotation = transform.rotation * Quaternion.Euler(100, 0, 0);
        }
        isFlyingStarted = false;
        rigidbody.constraints = RigidbodyConstraints.None;
        animator.SetBool("isMoving", false);
        animator.SetBool("isFlying", true);
        animator.SetFloat("flyBlend", -playerInput.MoveVertical);
        rigidbody.AddRelativeTorque(0, 0, rigidbody.velocity.magnitude * 2f * -playerInput.MoveHorizontal);
        wingBack.UpdateRotation(new Vector3(-playerInput.MoveVertical * wingRotationAngle, 0, 0));

        foreach (Wing wing in wings)
        {

            Vector3 globalNormal = transform.TransformDirection(wing.GetLocalNormal());
            Vector3 wingGlobalPosition = transform.TransformPoint(wing.GetLocalPosition());
            Vector3 globalVelocity = rigidbody.GetPointVelocity(wingGlobalPosition);
            rigidbody.AddForceAtPosition(globalNormal * Vector3.Dot(globalNormal, -globalVelocity) * wing.GetSquare() * globalVelocity.magnitude, wingGlobalPosition);
        }
    }
    public void CloakSimulation()
    {
        foreach (Transform bone in rotatedBones)
        {
            Quaternion localRotation = bone.localRotation;
            float rotateAngle = 0.5f - Mathf.PerlinNoise(Time.time * 10f,0);
            bone.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateAngle * Mathf.Clamp(rigidbody.velocity.magnitude * 0.2f, 0, 15))) * localRotation;
        }
    }
    public void OnDrawGizmos()
    {
        if (wings == null) return;

        foreach (Wing wing in wings)
        {
            Matrix4x4 matrixRotation = Matrix4x4.Rotate(transform.rotation * wing.GetWingRotation());
            Matrix4x4 matrixPosition = Matrix4x4.Translate(transform.TransformPoint(wing.GetLocalPosition()));
            Gizmos.matrix = matrixPosition * matrixRotation;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(Vector3.zero, Vector3.forward);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.zero, wing.GetSize());
        }

    }
}
