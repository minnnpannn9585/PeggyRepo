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

    // ===== 新增：在灯下停留与冷却 =====
    [Header("Light Attraction")]
    public float lingerAtLightSeconds = 5f;    // 到达灯下后停留时长
    public float reAttractCooldown = 2f;       // 离开后冷却（这里实际上不会再吸引，因为只吸引一次，但保留以备扩展）
    private bool isLingering = false;          // 是否在灯下驻留
    private float nextAttractTime = 0f;        // 再次允许被吸引的时间戳
    private Coroutine lingerRoutine = null;

    // ===== 新增：一次性吸引开关（此关/此局只吸引一次）=====
    private bool attractedOnce = false;

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
        // —— 改动点 1：只在“未前往/未驻留/冷却结束/且还没吸引过一次”时，才响应动画闪烁
        if (!attractedOnce && !isGoingToLight && !isLingering && Time.time >= nextAttractTime && pointLightAnimator)
        {
            AnimatorStateInfo animState = pointLightAnimator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName(flashStateName))
            {
                attractedOnce = true;       // 第一次触发就锁死：后续不再响应
                isGoingToLight = true;
                nma.isStopped = false;
                if (lightTrans != null)
                    SetDestination(lightTrans.position);
            }
        }

        // —— 删除：依赖“动画结束”来恢复巡逻的逻辑
        // 现在恢复巡逻由“到达灯下后等待5s”的协程负责

        if (nma.pathPending) return; // 路径还在计算中

        // 用 Agent 的到达判定（注意加一点余量，避免卡边界）
        bool reached = !nma.hasPath || nma.remainingDistance <= nma.stoppingDistance + 0.02f;

        if (reached)
        {
            if (isGoingToLight)
            {
                // 到达灯光后，启动5s驻留，然后恢复巡逻（只触发一次）
                if (!isLingering && lingerRoutine == null)
                {
                    lingerRoutine = StartCoroutine(LingerThenResume());
                }
                return;
            }

            // 巡逻：切下一个点并循环
            index = (index + 1) % points.Length;
            SetDestination(points[index].position);
        }
    }

    private IEnumerator LingerThenResume()
    {
        isLingering = true;
        nma.isStopped = true;
        nma.ResetPath(); // 清掉路径，防止到达判定抖动

        // 用真实时间，不受 Time.timeScale 影响
        yield return new WaitForSecondsRealtime(lingerAtLightSeconds);

        // 驻留结束：恢复巡逻（注意：attractedOnce 已经锁死，后续不会再被吸引）
        isLingering = false;
        isGoingToLight = false;
        nma.isStopped = false;
        nextAttractTime = Time.time + reAttractCooldown;

        // 恢复当前巡逻点（也可以改成下一个点）
        SetDestination(points[index].position);
        lingerRoutine = null;
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

    // —— 可选：如果需要在“下一关/重开局”重新允许被吸引，可在外部调用这个方法
    public void ResetAttractionOnce()
    {
        attractedOnce = false;
        isGoingToLight = false;
        isLingering = false;
        nextAttractTime = 0f;
        if (lingerRoutine != null)
        {
            StopCoroutine(lingerRoutine);
            lingerRoutine = null;
        }
    }
}