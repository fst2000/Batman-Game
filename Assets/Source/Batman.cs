using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Batman
{
    [SerializeField] public float WalkSpeed{get;} = 2.5f;
    [SerializeField] public float RunSpeed{get;} = 5f;
    [SerializeField] float wingRotationAngle = 20f;
    [SerializeField] Vector3 centerOfMass = new Vector3(0, 1f, 0.2f);
    [SerializeField] Transform[] rotatedBones = new Transform[5];
    PlayerInput playerInput;
    Rigidbody rigidbody;    
    Animator animator;
    Transform transform;
    IState state;

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
        state = new WalkState(this);

        wingL = new Wing(Vector3.ClampMagnitude(new Vector3(1, -1, -10), 1), new Vector3(-1f,1,0), 2f,1f);
        wingR = new Wing(Vector3.ClampMagnitude(new Vector3(-1, -1, -10), 1), new Vector3(1f,1,0), 2f,1f);
        wingBack = new Wing(Vector3.back, Vector3.zero, 1f, 0.5f);
        wingStabilize = new Wing(Vector3.right, new Vector3(0,0,0.2f), .25f, .5f);
        wings = new Wing[] {wingL,wingR, wingBack ,wingStabilize};
    }
    public void Update()
    {
        state.OnUpdate();
        if(state != state.NextState())
        {
            state.OnExit();
            state = state.NextState();
            state.OnEnter();
        }
    }
    public void FixedUpdate()
    {
        state.OnFixedUpdate();
    }
    public bool IsOnGround()
    {
        return Physics.CheckCapsule(transform.position,transform.position + transform.rotation * new Vector3(0, 1.8f, 0),0.6f);
    }
    public void MoveOnEnter()
    {
        rigidbody.velocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(Vector3.up);
        animator.CrossFade("MoveBlend", 0);
    }
    public void Move(float moveSpeed)
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        Vector3 velocity = rigidbody.velocity;
        Vector3 moveVelocity = playerInput.WalkInput * moveSpeed;
        rigidbody.velocity = moveVelocity + new Vector3(0, velocity.y, 0);
        transform.rotation = Quaternion.LookRotation(playerInput.WalkInput,Vector3.up);
    }
    public void FlyOnEnter()
    {
        rigidbody.AddForce(transform.forward * RunSpeed, ForceMode.VelocityChange);
        transform.rotation = Quaternion.Euler(90,0,0) * transform.rotation;
        animator.CrossFade("FlyBlend", 0);
    }
    public void Fly()
    {
        rigidbody.constraints = RigidbodyConstraints.None;
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
            float rotateAngle = 0.5f - Mathf.PerlinNoise(Time.time * 10f,Vector3.Dot(transform.up,bone.transform.up));
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
