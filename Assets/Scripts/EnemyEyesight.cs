using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEyeSight : MonoBehaviour
{
    public float viewRadius = 50f;   //  ”“∞∞Îæ∂
    [Range(0, 360)]
    public float viewAngle = 60f;    //  ”“∞Ω«∂»
    public LayerMask targetMask;

    void Start()
    {
        targetMask = LayerMask.GetMask("Player");
    }

    void Update()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (Collider target in targetsInViewRadius)
        {
            Transform player = target.transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float distToPlayer = Vector3.Distance(transform.position, player.position);

                if (Physics.Raycast(transform.position, dirToPlayer, distToPlayer))
                {
                    Debug.Log("Player in sight!");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, transform.rotation);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, transform.rotation);

        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    private Vector3 DirFromAngle(float angleDegrees, Quaternion rotation)
    {
        Vector3 direction = new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
        return rotation * direction;
    }
}