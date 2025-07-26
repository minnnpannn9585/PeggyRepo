using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{

    public Transform[] points;
    private int index = 0;

    public float speed = 1.0f;
    private Vector3 target;

    bool isGoingToLight = false;
    public Transform lightTrans;

    private void Start()
    {
        //target = end.position;
        target = points[index].position;
    }

    private void Update()
    {
        if (!isGoingToLight)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.1f)
            {

                index++;
                if (index < points.Length)
                    transform.forward = points[index].position - points[index - 1].position;
                if (index == points.Length)
                {
                    index = 0;
                    transform.forward = points[index].position - points[points.Length - 1].position;
                }
                target = points[index].position;
            }
        }

        else
        {
            transform.position = Vector3.MoveTowards(transform.position, lightTrans.position, speed * Time.deltaTime);
        }

        

        
    }
}
