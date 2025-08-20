using UnityEngine;

public class ClickToHide : MonoBehaviour
{
    public GameObject target; // Ҫ���ص�UI

    private void Awake()
    {
        if (target == null)
            target = gameObject; // Ĭ�������Լ�
    }

    private void Update()
    {
        if (target.activeSelf && Input.GetMouseButtonDown(0)) // ������
        {
            target.SetActive(false);
        }
    }
}
