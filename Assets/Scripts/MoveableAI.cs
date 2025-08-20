using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveableAI : MonoBehaviour
{
    public NavMeshAgent nma;
    public Transform[] points;
    private int index = 0;

    private void Start()
    {
        if (nma == null) nma = GetComponent<NavMeshAgent>();
        if (points.Length > 0)
        {
            nma.SetDestination(points[index].position);
        }
    }

    private void Update()
    {
        // 等待路径计算完成
        if (nma.pathPending) return;

        // 到达路点
        if (nma.remainingDistance <= nma.stoppingDistance)
        {
            index++;
            if (index >= points.Length)
            {
                index = 0; // 循环巡逻
            }

            nma.SetDestination(points[index].position);
        }
    }
}
