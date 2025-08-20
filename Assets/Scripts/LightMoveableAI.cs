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
        // ���ݶ����л���ȥ�ƹ�/Ѳ�ߡ�״̬
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
            // ��˸�������ָ�Ѳ��
            isGoingToLight = false;
            SetDestination(points[index].position);
        }

        if (nma.pathPending) return; // ·�����ڼ�����

        // �� Agent �ĵ����ж���ע���һ�����������⿨�߽磩
        bool reached = !nma.hasPath || nma.remainingDistance <= nma.stoppingDistance + 0.02f;

        if (reached)
        {
            if (isGoingToLight)
            {
                // ����ƹ��ͣ��������˸�������Զ��ָ�Ѳ��
                return;
            }

            // Ѳ�ߣ�����һ���㲢ѭ��
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