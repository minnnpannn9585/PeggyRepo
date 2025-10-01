using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAlertListener : MonoBehaviour
{
    [Header("到达后停留秒数")]
    public float investigateWait = 3f;

    [Header("在路上最长追踪秒数（<=0 表示不超时）")]
    public float timeout = 20f;

    [Header("要在警报时禁用的脚本（可留空，脚本会自动找 MoveableAI）")]
    public MonoBehaviour[] scriptsToDisable;

    private NavMeshAgent agent;
    private Coroutine running;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // 自动把同物体上的 MoveableAI 加入禁用列表（避免你忘了拖）
        if (scriptsToDisable == null || scriptsToDisable.Length == 0)
        {
            var auto = GetComponent<MoveableAI>();
            if (auto) scriptsToDisable = new MonoBehaviour[] { auto };
        }
    }

    void OnEnable() { EnemyAlert.Register(this); Debug.Log($"[EAL] Register {name}"); }
    void OnDisable() { EnemyAlert.Unregister(this); Debug.Log($"[EAL] Unregister {name}"); }

    public void GoTo(Vector3 pos)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(CoGoTo(pos));
    }

    private IEnumerator CoGoTo(Vector3 pos)
    {
        if (!agent)
        {
            Debug.LogWarning($"[EAL] {name} no NavMeshAgent");
            yield break;
        }
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"[EAL] {name} agent not on NavMesh");
            yield break;
        }

        // 0) 把目标点投射到 NavMesh（半径按场景调）
        if (NavMesh.SamplePosition(pos, out var hit, 2.0f, NavMesh.AllAreas))
        {
            pos = hit.position;
        }
        else
        {
            Debug.LogWarning($"[EAL] {name} ALERT pos not on NavMesh and sampling failed.");
        }

        // 1) 禁用会改 destination 的脚本（如 MoveableAI）
        foreach (var s in scriptsToDisable) if (s) s.enabled = false;

        // 2) 开始移动
        agent.isStopped = false;
        agent.ResetPath();
        agent.SetDestination(pos);
        Debug.Log($"[EAL] {name} moving to {pos}");

        // 3) 等到达或超时
        float t0 = Time.time;
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance + 0.1f)
        {
            if (timeout > 0f && Time.time - t0 > timeout)
            {
                Debug.LogWarning($"[EAL] {name} timeout going to {pos}");
                break;
            }
            yield return null;
        }

        // 4) 到达后停留
        float stayed = 0f;
        while (stayed < investigateWait) { stayed += Time.deltaTime; yield return null; }

        // 5) 恢复原脚本
        foreach (var s in scriptsToDisable) if (s) s.enabled = true;
        Debug.Log($"[EAL] {name} resume patrol");

        running = null;
    }
}