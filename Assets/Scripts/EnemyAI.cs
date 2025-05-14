using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform trans01;
    public Transform trans02;
    bool isGoingToTrans01 = true;

    void Update()
    {
        EnemyMove();
    }

    void EnemyMove()
    {
        if (isGoingToTrans01)
        {
            if (Vector3.Distance(transform.position, trans01.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, trans01.position, Time.deltaTime);
                transform.LookAt(trans01.position);
            }
            else
            {
                isGoingToTrans01 = false;
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, trans02.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, trans02.position, Time.deltaTime);
                transform.LookAt(trans02.position);
            }
            else
            {
                isGoingToTrans01 = true;
            }
        }
    }
}
