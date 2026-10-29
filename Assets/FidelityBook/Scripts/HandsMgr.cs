using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Hands;

public class HandsMgr : MonoBehaviour
{
    public Vector3 posLeft;
    public Vector3 posRight;
    private XRHandSubsystem m_HandSubsystem;
    private XROrigin xrOrigin;

    void Start()
    {
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);
        if (handSubsystems.Count > 0)
        {
            m_HandSubsystem = handSubsystems[0];
        }
        xrOrigin = Object.FindFirstObjectByType<XROrigin>();
    }

    void Update()
    {
        if (m_HandSubsystem == null || !m_HandSubsystem.running) return;

        // 1. Left Hand Wrist / Root
        posLeft = GetHandWristPosition(m_HandSubsystem.leftHand);

        // 2. Right Hand Wrist / Root
        // posRight = GetHandWristPosition(m_HandSubsystem.rightHand);
    }

    Vector3 GetHandWristPosition(XRHand hand)
    {
        Vector3 position = Vector3.zero;

        if (!hand.isTracked) return Vector3.zero;

        // Option A: Using hand.rootPose (Represents the wrist/root pose of the hand)
        // if (hand.rootPose != null)
        // {
            // position = hand.rootPose.position;
            // // Option B: Using the specific XRHandJointID.Wrist joint
            var wristJoint = hand.GetJoint(XRHandJointID.Wrist);
            if (wristJoint.TryGetPose(out Pose jointPose))
            {
                // position = jointPose.position;
                position = xrOrigin.transform.TransformPoint(jointPose.position);
            }
        // }
        return position;
    }
}
