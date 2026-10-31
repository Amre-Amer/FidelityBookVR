using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Hands;

public class HandsMgr : MonoBehaviour
{
    public GlobalsMgr g;
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
        xrOrigin = FindFirstObjectByType<XROrigin>();
    }

    public bool SetPosLeftIfAvailable()
    {
        if (m_HandSubsystem == null || !m_HandSubsystem.running) {
            return false;
        }
        g.posLeftHand = GetHandWristPosition(m_HandSubsystem.leftHand);
        // g.posRightHand = GetHandWristPosition(m_HandSubsystem.rightHand);
        return true;
    }

    Vector3 GetHandWristPosition(XRHand hand)
    {
        if (!hand.isTracked) return Vector3.zero;
        Vector3 position = Vector3.zero;
        var wristJoint = hand.GetJoint(XRHandJointID.Wrist);
        if (wristJoint.TryGetPose(out Pose jointPose))
        {
            position = xrOrigin.transform.TransformPoint(jointPose.position);
        }
        return position;
    }
}
