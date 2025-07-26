using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public float timer;
    public GameObject thingToClose;
    public GameObject thingToOpen;

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if(thingToClose != null)
            {
                thingToClose.SetActive(false);
            }
            
            thingToOpen.SetActive(true);
            Destroy(gameObject);
        }
    }
}
