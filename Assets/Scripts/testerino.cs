using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class testerino : MonoBehaviour
{
    public Transform t1;
    public Transform t2;
    public Transform t3;
    public Transform t4;
    public Material mat;

    public Transform plane;

    public float radius = 1.0f;

    public int approxSteps = 0;
    public float approxFactor = 0.2f;

    private Vector3 approxNormal;

    void Update()
    {
        transform.localScale = Vector3.one * radius * 2.0f;

        if (t1 == null || t2 == null || t3 == null || t4 == null || mat == null)
            return;

        //Vector3 avg = (t1.position + t2.position + t3.position + t4.position) / 4.0f;
        Vector3 avg = Vector3.zero;
        Vector3 x1 = t1.position - avg;
        Vector3 x2 = t2.position - avg;
        Vector3 x3 = t3.position - avg;
        Vector3 x4 = t4.position - avg;

        mat.SetVector("_x1", x1);
        mat.SetVector("_x2", x2);
        mat.SetVector("_x3", x3);
        mat.SetVector("_x4", x4);

        approxNormal = ApproximateDirection(x1, x2, x3, x4);

        //plane.position = avg;
        plane.rotation = Quaternion.FromToRotation(Vector3.up, approxNormal);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + approxNormal * radius, 0.03f);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(t1.position, 0.05f);
        Gizmos.DrawSphere(t2.position, 0.05f);
        Gizmos.DrawSphere(t3.position, 0.05f);
        Gizmos.DrawSphere(t4.position, 0.05f);
    }

    private Vector3 ApproximateDirection(Vector3 x1, Vector3 x2, Vector3 x3, Vector3 x4)
    {
        Vector3 n = new Vector3(0f, 1f, 0f);

        for (int i = 0; i < approxSteps; i++)
        {
            Vector4 An = new Vector4(x1.x * n.x + x1.y * n.y + x1.z * n.z, x2.x * n.x + x2.y * n.y + x2.z * n.z, x3.x * n.x + x3.y * n.y + x3.z * n.z, x4.x * n.x + x4.y * n.y + x4.z * n.z);
            Vector3 ATAn = x1 * An.x + x2 * An.y + x3 * An.z + x4 * An.w;
            Vector3 grad = ATAn - An.sqrMagnitude * n;

            n = (n - grad * approxFactor).normalized;
        }

        return n;
    }
}
