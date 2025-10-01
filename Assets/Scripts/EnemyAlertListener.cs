using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAlertListener : MonoBehaviour
{
    [Header("�����ͣ������")]
    public float investigateWait = 3f;

    [Header("��·���׷��������<=0 ��ʾ����ʱ��")]
    public float timeout = 20f;

    [Header("Ҫ�ھ���ʱ���õĽű��������գ��ű����Զ��� MoveableAI��")]
    public MonoBehaviour[] scriptsToDisable;

    private NavMeshAgent agent;
    private Coroutine running;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // �Զ���ͬ�����ϵ� MoveableAI ��������б������������ϣ�
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

        // 0) ��Ŀ���Ͷ�䵽 NavMesh���뾶����������
        if (NavMesh.SamplePosition(pos, out var hit, 2.0f, NavMesh.AllAreas))
        {
            pos = hit.position;
        }
        else
        {
            Debug.LogWarning($"[EAL] {name} ALERT pos not on NavMesh and sampling failed.");
        }

        // 1) ���û�� destination �Ľű����� MoveableAI��
        foreach (var s in scriptsToDisable) if (s) s.enabled = false;

        // 2) ��ʼ�ƶ�
        agent.isStopped = false;
        agent.ResetPath();
        agent.SetDestination(pos);
        Debug.Log($"[EAL] {name} moving to {pos}");

        // 3) �ȵ����ʱ
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

        // 4) �����ͣ��
        float stayed = 0f;
        while (stayed < investigateWait) { stayed += Time.deltaTime; yield return null; }

        // 5) �ָ�ԭ�ű�
        foreach (var s in scriptsToDisable) if (s) s.enabled = true;
        Debug.Log($"[EAL] {name} resume patrol");

        running = null;
    }
}