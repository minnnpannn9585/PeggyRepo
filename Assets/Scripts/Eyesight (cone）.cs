using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEyesight : MonoBehaviour
{
    public float viewRadius = 50f;            // 视野半径
    [Range(0, 360)]
    public float viewAngle = 60f;             // 视野角度
    public LayerMask targetMask = LayerMask.GetMask("player");    // 玩家所在层

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

                if (!Physics.Raycast(transform.position, dirToPlayer, distToPlayer, obstacleMask))
                {
                    Debug.Log("Player in sight!");
                }
            }
        }
    }
}
