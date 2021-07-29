using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    public Transform ikTarget;
    public LayerMask raycastLayers;
    public float stepDistance = 1.0f;
    public float animationDuration = 1.0f;

    public AnimationCurve xCurve;
    public AnimationCurve yCurve;

    [Header("Body")]
    public Transform legAnchor;
    public CreatureController creature;

    [Header("Other")]
    public LegController adjacentLeg1;
    public LegController adjacentLeg2;
    public LegController oppositeLeg;

    public bool Moving { get; private set; } = false;

    private Vector3 walkTargetPosition;
    private Vector3 walkOriginOffset;

    private Vector3 walkTargetPositionBeforeCast;

    void Start()
    {
        walkOriginOffset = creature.transform.InverseTransformPoint(transform.position);
    }

    void LateUpdate()
    {
        // Move IK target to leg anchor
        ikTarget.position = legAnchor.position;
        ikTarget.rotation = legAnchor.rotation;


        // Find new grounded walk target point
        walkTargetPosition = creature.body.TransformPoint(walkOriginOffset) + creature.transform.TransformDirection(creature.MoveDirection * stepDistance * 0.5f);
        walkTargetPositionBeforeCast = walkTargetPosition;

        RaycastHit hit;
        if (Physics.Raycast(walkTargetPosition + creature.transform.up * 2.0f, -creature.transform.up, out hit, Mathf.Infinity, raycastLayers))
        {
            walkTargetPosition = hit.point;
        }


        // Move foot if too far from walk target and adjacent leg is grounded
        if (!Moving)
        {
            float distFac = (walkTargetPosition - transform.position).sqrMagnitude / (stepDistance * stepDistance);
            //diff.y = 0.0f;

            if (distFac > 1.0f)
            {
                if (!(adjacentLeg1.Moving || adjacentLeg2.Moving) || distFac > (1.2f*1.2f))
                {
                    MoveFoot();
                }
            }
        }
    }

    public void MoveFoot()
    {
        if (!Moving)
        {
            Moving = true;
            StartCoroutine(MoveFootCoroutine(walkTargetPosition));
        }
    }

    private IEnumerator MoveFootCoroutine(Vector3 targetPosition)
    {         
        Vector3 startPosition = transform.position;
        float time = 0.0f;
        
        while(time < 1.0f)
        {
            time += Time.deltaTime / animationDuration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, xCurve.Evaluate(time)) + Vector3.up * yCurve.Evaluate(time);
            yield return null;
        }

        Moving = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(walkTargetPositionBeforeCast, walkTargetPosition);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(walkTargetPosition, 0.04f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(walkTargetPositionBeforeCast, 0.03f);
    }
}
