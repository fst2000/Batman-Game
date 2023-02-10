using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatmanBehaviour : MonoBehaviour
{
    [SerializeField] Batman batman;
    private void Start()
    {
        batman.Initialize(gameObject);
    }
    private void Update()
    {
        batman.Update();
    }
    private void FixedUpdate()
    {
        batman.FixedUpdate();
    }
    private void LateUpdate()
    {
        batman.CloakSimulation();
    }
    private void OnDrawGizmos()
    {
        batman.OnDrawGizmos();
    }
}
