using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightMoveableAI : MonoBehaviour
{
    public NavMeshAgent nma;
    public Transform[] points;         // 巡逻路径点
    private int index = 0;             // 当前目标点索引

    public float speed = 1.0f;         // 移动速度
    public float rotationSpeed = 5f;   // 旋转速度（控制转向平滑度）
    private Vector3 target;            // 当前目标点坐标

    private bool isGoingToLight = false;   // 是否正在前往灯光
    public Transform lightTrans;           // 灯光的位置

    public Animator pointLightAnimator;    // 灯光的 Animator
    private string flashStateName = "LightBlink";  // 闪烁动画的状态名

    private void Start()
    {
        target = points[index].position;
    }

    private void Update()
    {
        // 检查动画当前是否处于闪烁状态
        AnimatorStateInfo animState = pointLightAnimator.GetCurrentAnimatorStateInfo(0);
        if (animState.IsName(flashStateName))
        {
            isGoingToLight = true;
        }

        if (!isGoingToLight)
        {
            MoveTowards(target);

            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                index++;
                if (index >= points.Length)
                {
                    index = 0;
                }
                target = points[index].position;
            }
        }
        else
        {
            MoveTowards(lightTrans.position);
        }
    }

    private void MoveTowards(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;

        if (direction != Vector3.zero)
        {
            // 平滑旋转朝向目标方向
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 向目标移动
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        nma.SetDestination(destination);
    }
}