using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class WallScript : MonoBehaviour
{
    [SerializeField] bool inCollision;

    Rigidbody rb;
    float trust = 10;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!inCollision)
        {
            if (!rb.useGravity)
            {
                rb.useGravity = true;
            }

            if (other.gameObject.CompareTag("Player"))
            {
                rb.AddForce(Vector3.up * trust, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (inCollision)
        {
            if (!rb.useGravity)
            {
                rb.useGravity = true;
            }

            if (collision.gameObject.CompareTag("Player"))
            {
                rb.AddForce(Vector3.up * trust, ForceMode.Impulse);
            }
        }
    }
}
