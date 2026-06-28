using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Animator characterAnimator;
    private Rigidbody[] ragdollParts;

    private void Awake()
    {
        ragdollParts = GetComponentsInChildren<Rigidbody>();
        SetRagdoll(false);
    }

    public void SetRagdoll(bool isRagdoll)
    {
        characterAnimator.enabled = !isRagdoll;
        foreach (var ragdollPart in ragdollParts)
        {
            ragdollPart.isKinematic = !isRagdoll;
        }
    }
}