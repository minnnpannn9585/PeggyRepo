using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightMoveableAI : MonoBehaviour
{
    public NavMeshAgent nma;
    public Transform[] points;         // 巡逻路径点
    private int index = 0;             // 当前目标点索引

    public float rotationSpeed = 5f;   // 如果你要自转向，再用；现在让Agent自己转
    private bool isGoingToLight = false;   // 是否正在前往灯光
    public Transform lightTrans;           // 灯光的位置

    public Animator pointLightAnimator;    // 灯光的 Animator
    private string flashStateName = "LightBlink";  // 闪烁动画的状态名

    private void Awake()
    {
        if (nma == null) nma = GetComponent<NavMeshAgent>();
        if (nma == null)
        {
            Debug.LogError("需要 NavMeshAgent 组件");
            enabled = false;
            return;
        }
        // 关键：让到达判定和你的逻辑匹配
        nma.stoppingDistance = 0.1f;   // 或者保留默认，但后面比较时加上余量
        nma.autoBraking = false;
        nma.updateRotation = true;     // 让Agent自己转向
    }

    private void Start()
    {
        if (points == null || points.Length == 0)
        {
            Debug.LogWarning("未设置巡逻路点");
            enabled = false;
            return;
        }
        SetDestination(points[index].position);
    }

    private void Update()
    {
        // 根据动画切换“去灯光/巡逻”状态
        AnimatorStateInfo animState = pointLightAnimator.GetCurrentAnimatorStateInfo(0);
        if (animState.IsName(flashStateName))
        {
            if (!isGoingToLight)
            {
                isGoingToLight = true;
                SetDestination(lightTrans.position);
            }
        }
        else if (isGoingToLight)
        {
            // 闪烁结束，恢复巡逻
            isGoingToLight = false;
            SetDestination(points[index].position);
        }

        if (nma.pathPending) return; // 路径还在计算中

        // 用 Agent 的到达判定（注意加一点余量，避免卡边界）
        bool reached = !nma.hasPath || nma.remainingDistance <= nma.stoppingDistance + 0.02f;

        if (reached)
        {
            if (isGoingToLight)
            {
                // 到达灯光就停留，等闪烁结束会自动恢复巡逻
                return;
            }

            // 巡逻：切下一个点并循环
            index = (index + 1) % points.Length;
            SetDestination(points[index].position);
        }
    }

    private void SetDestination(Vector3 dst)
    {
        if (!nma.isOnNavMesh) return;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(dst, out hit, 1.0f, NavMesh.AllAreas))
            nma.SetDestination(hit.position);
        else
            nma.SetDestination(dst);
    }
}