using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyesight : MonoBehaviour
{
    public float viewRadius = 50f;            // 视野半径
    [Range(0, 360)]
    public float viewAngle = 60f;             // 视野角度
    public LayerMask targetMask;    // 玩家所在层

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

            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2f)
            {
                float distToPlayer = Vector3.Distance(transform.position, player.position);

                if (!Physics.Raycast(transform.position, dirToPlayer, distToPlayer))
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

        Vector3 viewAngleA = DirectionFromAngle(-viewAngle / 2, transform.rotation);
        Vector3 viewAngleB = DirectionFromAngle(viewAngle / 2, transform.rotation);


        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    }

    private Vector3 DirectionFromAngle(float angleInDegrees, Quaternion rotation)
    {
        // 基于角色的旋转调整方向向量
        Vector3 direction = new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        return rotation * direction; // 应用角色的旋转
    }


}
