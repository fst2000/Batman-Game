using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Rendering;

public class WingsuitController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform pivotPosition;
    new Rigidbody rigidbody;
    TrailRenderer trailRenderer;
    [SerializeField] float backWingRotationAngle = 15f;
    [SerializeField] float forwardWingRotationAngle = 3f;
    [SerializeField] float dragForce = 10f;
    [SerializeField] float angularDrag = 2f;
    PlayerInput playerInput = new PlayerInput();

    Wing wingLeft = new Wing(Vector3.ClampMagnitude(new Vector3(3, -3, -10), 1), new Vector3(-0.5f, 1, 0), 0.5f, 1f);
    Wing wingRight = new Wing(Vector3.ClampMagnitude(new Vector3(-3, -3, -10), 1), new Vector3(0.5f, 1, 0), 0.5f, 1f);
    Wing wingBack = new Wing(Vector3.ClampMagnitude(new Vector3(0, -3, -10), 1), new Vector3(0, 0, 0), 0.5f, 1f);
    Wing wingStabilize = new Wing(Vector3.right, new Vector3(0, 0.2f, 0), 0.3f, 1f);
    Wing[] wings;
    private void Start()
    {
        rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rigidbody.angularDrag = angularDrag;
        rigidbody.mass = 60;
        wings = new Wing[] { wingLeft, wingRight, wingBack, wingStabilize };
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.TransformPoint(wingLeft.GetLocalPosition()), transform.TransformPoint(wingLeft.GetLocalPosition() + wingLeft.GetLocalNormal()));
        Gizmos.DrawLine(transform.TransformPoint(wingRight.GetLocalPosition()), transform.TransformPoint(wingRight.GetLocalPosition() + wingRight.GetLocalNormal()));
        Gizmos.DrawLine(transform.TransformPoint(wingBack.GetLocalPosition()), transform.TransformPoint(wingBack.GetLocalPosition() + wingBack.GetLocalNormal()));
        Gizmos.DrawLine(transform.TransformPoint(wingStabilize.GetLocalPosition()), transform.TransformPoint(wingStabilize.GetLocalPosition() + wingStabilize.GetLocalNormal()));

    }
    private void Update()
    {
        animator.SetBool("isFlying", true);
        animator.SetFloat("flyVertical", -playerInput.MoveVertical);
        animator.SetFloat("flyHorizontal", playerInput.MoveHorizontal);
        wingLeft.UpdateRotation(new Vector3(-playerInput.MoveHorizontal * forwardWingRotationAngle, 0, 0));
        wingRight.UpdateRotation(new Vector3(playerInput.MoveHorizontal * forwardWingRotationAngle, 0, 0));
        wingBack.UpdateRotation(new Vector3(-playerInput.MoveVertical * backWingRotationAngle, 0, 0));

        foreach (Wing wing in wings)
        {

            Vector3 globalNormal = transform.TransformDirection(wing.GetLocalNormal());
            Vector3 wingGlobalPosition = transform.TransformPoint(wing.GetLocalPosition());
            Vector3 globalVelocity = rigidbody.GetPointVelocity(wingGlobalPosition);
            rigidbody.AddForceAtPosition(globalNormal * Vector3.Dot(globalNormal, -globalVelocity) * wing.GetSquare() * dragForce, wingGlobalPosition);
        }



    }
}