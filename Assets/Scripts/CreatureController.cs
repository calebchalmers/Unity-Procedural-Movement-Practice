using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    public LegController legLB;
    public LegController legLF;
    public LegController legRB;
    public LegController legRF;
    public Transform body;

    public float maxMoveSpeed = 1.0f;
    public float maxRotateSpeed = 10.0f;
    public float yLerpSpeed = 20f;
    public float rotationSlerpSpeed = 20f;

    [Header("Approximation")]
    public int approxSteps = 10;
    public float approxFactor = 0.05f;

    public Vector3 MoveDirection { get; set; }
    public bool StartedMoving { get; set; }

    private Vector3 approxNormal;
    private bool wasMoving = false;

    public void Start()
    {

    }

    public void Update()
    {
        //float moveSpeed = Input.GetAxis("Vertical") * maxMoveSpeed;
        //float rotateSpeed = Input.GetAxis("Horizontal") * maxRotateSpeed;

        MoveDirection = (new Vector3(Input.GetAxisRaw("Move X"), 0f, Input.GetAxisRaw("Move Y"))).normalized;
        Vector3 moveDelta = transform.TransformDirection(MoveDirection) * maxMoveSpeed * Time.deltaTime;

        bool moving = MoveDirection.sqrMagnitude > 0.01f;
        StartedMoving = moving && !wasMoving;
        wasMoving = moving;

        transform.Rotate(0f, Input.GetAxis("Rotate") * maxRotateSpeed * Time.deltaTime, 0f, Space.World);

        Vector3 leg1 = legLB.transform.position;
        Vector3 leg2 = legLF.transform.position;
        Vector3 leg3 = legRB.transform.position;
        Vector3 leg4 = legRF.transform.position;
        Vector3 legAvg = (leg1 + leg2 + leg3 + leg4) / 4.0f;

        Vector3 newPos = transform.position;
        newPos.y = Mathf.Lerp(newPos.y, legAvg.y, yLerpSpeed * Time.deltaTime);
        transform.position = newPos + moveDelta;
        body.position = transform.position;

        approxNormal = ApproximateDirection(leg1 - legAvg, leg2 - legAvg, leg3 - legAvg, leg4 - legAvg);
        Quaternion targetBodyRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.FromToRotation(Vector3.up, approxNormal), 0.8f) * transform.rotation;
        body.rotation = Quaternion.Slerp(body.rotation, targetBodyRotation, rotationSlerpSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + approxNormal);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.forward);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.forward + new Vector3(0f, -Vector3.Dot(Vector3.forward, approxNormal) / approxNormal.y, 0f));
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
