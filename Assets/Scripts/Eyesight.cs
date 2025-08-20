using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyesight : MonoBehaviour
{
    public float viewRadius = 50f;      // 视野半径
    [Range(0, 360)]
    public float viewAngle = 60f;       // 视野角度

    public LayerMask targetMask;        // 玩家所在层（建议 Inspector 勾 Player）
    public LayerMask obstructionMask;   // ✅ 新增：遮挡层（墙体/家具等，不包含 Player）

    [Header("视线射线起点（可选）")]
    public Transform eyePoint;          // 为空时用 transform.position + eyeHeight
    public float eyeHeight = 1.7f;

    [HideInInspector] public bool playerInSight;  // ✅ 新增：给外部脚本读取

    void Start()
    {
        if (targetMask == 0)
            targetMask = LayerMask.GetMask("Player");
        // obstructionMask 请在 Inspector 里设置（把会遮挡视线的层勾上，不要勾 Player）
    }

    void Update()
    {
        playerInSight = false; // 每帧重置

        // 在视野半径内找玩家
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        foreach (Collider target in targetsInViewRadius)
        {
            Transform player = target.transform;
            Vector3 toPlayer = (player.position - (eyePoint ? eyePoint.position : GetEyePos())).normalized;

            // 角度判断
            if (Vector3.Angle((eyePoint ? eyePoint.forward : transform.forward), toPlayer) < viewAngle / 2f)
            {
                float distToPlayer = Vector3.Distance((eyePoint ? eyePoint.position : GetEyePos()), player.position);

                // ✅ 关键改动：只用“遮挡层”做 Raycast，忽略玩家自身
                // 如果射线没有击中任何遮挡物，说明视线通畅 → 看到玩家
                bool blocked = Physics.Raycast(
                    (eyePoint ? eyePoint.position : GetEyePos()),
                    toPlayer,
                    distToPlayer,
                    obstructionMask,
                    QueryTriggerInteraction.Ignore
                );

                if (!blocked)
                {
                    playerInSight = true;
                    // Debug.Log("Player in sight!");
                    break; // 已经看到就可以退出
                }
            }
        }
    }

    private Vector3 GetEyePos()
    {
        return transform.position + Vector3.up * eyeHeight;
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
        Vector3 direction = new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        return rotation * direction;
    }
}
