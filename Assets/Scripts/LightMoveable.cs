using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightMoveable : MonoBehaviour
{
    public NavMeshAgent nma;
    public Transform[] points;
    public int index = 0;
    public float speed = 0f;
    public float rotationSpeed = 0f;
    private Vector3 target;

    private bool isDoingLight = false;
    private bool isFlashLight = false;

    public Animator pointLightAnimator;
    private string flashStateName = "LightBlink";

    private void Start()
    {
        target = points[index].position;
    }

    private void Update()
    {
        AnimatorStateInfo state = pointLightAnimator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName(flashStateName))
        {
            isDoingLight = true;
        }
        else
        {
            isDoingLight = false;
        }

        if (!isDoingLight)
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
            MoveTowards(LightTrans.position);
        }
    }

    private void MoveTowards(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        nma.SetDestination(destination);
    }
}