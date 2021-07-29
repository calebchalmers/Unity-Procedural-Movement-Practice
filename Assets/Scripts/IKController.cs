using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    public Transform jointA;
    public Transform jointB;

    private float jointDistance;

    public void Start()
    {
        jointDistance = Vector3.Distance(jointA.position, jointB.position);
    }

    public void LateUpdate()
    {
        Vector3 targetDifference = transform.position - jointA.position;
        float targetDistance = targetDifference.magnitude;
        Vector3 targetDirection = targetDifference / targetDistance;

        Vector3 guideDirection = transform.up;
        Vector3 bendDirection = guideDirection - targetDirection * Vector3.Dot(guideDirection, targetDirection);

        Vector3 orthoDirection = Vector3.Cross(targetDirection, bendDirection);
        Quaternion baseRotation = Quaternion.LookRotation(bendDirection, targetDirection);
        float bendAngle = Mathf.Acos(Mathf.Min(targetDistance / jointDistance / 2.0f, 1.0f)) * Mathf.Rad2Deg;

        jointA.rotation = Quaternion.AngleAxis(bendAngle, orthoDirection) * baseRotation;
        jointB.localRotation = Quaternion.AngleAxis(-bendAngle * 2.0f, Vector3.right);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.04f);

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, transform.position + guideDirection.normalized * 0.4f);
    }
}
