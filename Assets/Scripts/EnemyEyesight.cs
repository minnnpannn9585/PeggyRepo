using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEyesight : MonoBehaviour
{
    public LayerMask layer;

    void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f, layer))
        {
            if(hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player in sight");
            }
        }
    }
}
