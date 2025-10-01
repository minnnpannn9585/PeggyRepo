using UnityEngine;

public class ClickToHide : MonoBehaviour
{
    public GameObject target; // Ҫ���ص�UI

    private void Awake()
    {
        if (!target) target = gameObject; // Ĭ�������Լ�
    }

    private void Update()
    {
        if (target && target.activeSelf && Input.GetMouseButtonDown(0))
        {
            Debug.Log("[ClickToHide] Deactivate => " + GetPath(target.transform));
            target.SetActive(false);
        }
    }

    private string GetPath(Transform t)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(t.name);
        while (t.parent) { t = t.parent; sb.Insert(0, t.name + "/"); }
        return sb.ToString();
    }
}