using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureObj : MonoBehaviour
{
    public LayerMask layerMask;
    bool isGrabbing = false;
    public Material transparentMat;
    public Material propMat;

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !isGrabbing)
        {
            Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f, layerMask);
            if (hit.collider != null)
            {
                GameObject prop = Instantiate(hit.collider.gameObject, transform);
                prop.GetComponent<MeshRenderer>().material = transparentMat;
                prop.GetComponent<Rigidbody>().isKinematic = true;
                prop.GetComponent<BoxCollider>().enabled = false;
                prop.transform.localPosition = new Vector3(0, 0, 2f);
                prop.transform.localRotation = Quaternion.Euler(0, 0, 0);
                isGrabbing = true;
            }
        }
        else if (Input.GetMouseButtonDown(0) && isGrabbing)
        {
            Transform newProp = transform.GetChild(0);
            newProp.GetComponent<MeshRenderer>().material = propMat;

            
            newProp.SetParent(null);
            newProp.GetComponent<Rigidbody>().isKinematic = false;
            newProp.GetComponent<BoxCollider>().enabled = true;
            isGrabbing = false;
        }

        if(isGrabbing)
        {
            if(Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                transform.GetChild(0).transform.localPosition += new Vector3(0, 0, 0.1f);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                transform.GetChild(0).transform.localPosition -= new Vector3(0, 0, 0.1f);
            }
        }
    }
}
