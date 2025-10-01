using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightMoveableAI : MonoBehaviour
{
    public NavMeshAgent nma;
    public Transform[] points;         // Ѳ��·����
    private int index = 0;             // ��ǰĿ�������

    public float rotationSpeed = 5f;   // �����Ҫ��ת�����ã�������Agent�Լ�ת
    private bool isGoingToLight = false;   // �Ƿ�����ǰ���ƹ�
    public Transform lightTrans;           // �ƹ��λ��

    public Animator pointLightAnimator;    // �ƹ�� Animator
    private string flashStateName = "LightBlink";  // ��˸������״̬��

    // ===== �������ڵ���ͣ������ȴ =====
    [Header("Light Attraction")]
    public float lingerAtLightSeconds = 5f;    // ������º�ͣ��ʱ��
    public float reAttractCooldown = 2f;       // �뿪����ȴ������ʵ���ϲ�������������Ϊֻ����һ�Σ��������Ա���չ��
    private bool isLingering = false;          // �Ƿ��ڵ���פ��
    private float nextAttractTime = 0f;        // �ٴ�����������ʱ���
    private Coroutine lingerRoutine = null;

    // ===== ������һ�����������أ��˹�/�˾�ֻ����һ�Σ�=====
    private bool attractedOnce = false;

    private void Awake()
    {
        if (nma == null) nma = GetComponent<NavMeshAgent>();
        if (nma == null)
        {
            Debug.LogError("��Ҫ NavMeshAgent ���");
            enabled = false;
            return;
        }
        // �ؼ����õ����ж�������߼�ƥ��
        nma.stoppingDistance = 0.1f;   // ���߱���Ĭ�ϣ�������Ƚ�ʱ��������
        nma.autoBraking = false;
        nma.updateRotation = true;     // ��Agent�Լ�ת��
    }

    private void Start()
    {
        if (points == null || points.Length == 0)
        {
            Debug.LogWarning("δ����Ѳ��·��");
            enabled = false;
            return;
        }
        SetDestination(points[index].position);
    }

    private void Update()
    {
        // ���� �Ķ��� 1��ֻ�ڡ�δǰ��/δפ��/��ȴ����/�һ�û������һ�Ρ�ʱ������Ӧ������˸
        if (!attractedOnce && !isGoingToLight && !isLingering && Time.time >= nextAttractTime && pointLightAnimator)
        {
            AnimatorStateInfo animState = pointLightAnimator.GetCurrentAnimatorStateInfo(0);
            if (animState.IsName(flashStateName))
            {
                attractedOnce = true;       // ��һ�δ���������������������Ӧ
                isGoingToLight = true;
                nma.isStopped = false;
                if (lightTrans != null)
                    SetDestination(lightTrans.position);
            }
        }

        // ���� ɾ�����������������������ָ�Ѳ�ߵ��߼�
        // ���ڻָ�Ѳ���ɡ�������º�ȴ�5s����Э�̸���

        if (nma.pathPending) return; // ·�����ڼ�����

        // �� Agent �ĵ����ж���ע���һ�����������⿨�߽磩
        bool reached = !nma.hasPath || nma.remainingDistance <= nma.stoppingDistance + 0.02f;

        if (reached)
        {
            if (isGoingToLight)
            {
                // ����ƹ������5sפ����Ȼ��ָ�Ѳ�ߣ�ֻ����һ�Σ�
                if (!isLingering && lingerRoutine == null)
                {
                    lingerRoutine = StartCoroutine(LingerThenResume());
                }
                return;
            }

            // Ѳ�ߣ�����һ���㲢ѭ��
            index = (index + 1) % points.Length;
            SetDestination(points[index].position);
        }
    }

    private IEnumerator LingerThenResume()
    {
        isLingering = true;
        nma.isStopped = true;
        nma.ResetPath(); // ���·������ֹ�����ж�����

        // ����ʵʱ�䣬���� Time.timeScale Ӱ��
        yield return new WaitForSecondsRealtime(lingerAtLightSeconds);

        // פ���������ָ�Ѳ�ߣ�ע�⣺attractedOnce �Ѿ����������������ٱ�������
        isLingering = false;
        isGoingToLight = false;
        nma.isStopped = false;
        nextAttractTime = Time.time + reAttractCooldown;

        // �ָ���ǰѲ�ߵ㣨Ҳ���Ըĳ���һ���㣩
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

    // ���� ��ѡ�������Ҫ�ڡ���һ��/�ؿ��֡��������������������ⲿ�����������
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