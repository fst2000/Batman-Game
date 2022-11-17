using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatmanBehaviour : MonoBehaviour
{
    [SerializeField] Batman batman;
    private void Start()
    {
        batman = new Batman(gameObject);
    }
    private void Update()
    {
        //batman.Update();
    }
    private void OnDrawGizmos()
    {
        batman.OnDrawGizmos();
    }
    private void FixedUpdate()
    {
        batman.Update();
    }
}
