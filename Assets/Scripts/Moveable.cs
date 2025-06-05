using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    public Transform start;
    public Transform end;

    public float speed = 1.0f;
    private Vector3 target;

    private void Start()
    {
        target = end.position;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = target == end.position ? start.position : end.position;
        }

        transform.forward = target - transform.position;
    }
}
