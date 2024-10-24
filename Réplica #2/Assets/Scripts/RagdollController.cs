using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ragdollController : MonoBehaviour
{
    public Rigidbody[] ragdollBones;

    void Start()
    {
        SetRagdollActive(false);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Car")
        {
            SetRagdollActive(true);
            Debug.Log("ragdoll");
        } else
        {
            Debug.Log("noCollision");
        }
    }

    void SetRagdollActive(bool isActive)
    {
        Debug.Log(!isActive);
        foreach (Rigidbody bone in ragdollBones)
        {
            bone.isKinematic = !isActive; // Physics or no physics
        }
    }
}
