using UnityEngine;

public class ClickToHide : MonoBehaviour
{
    public GameObject target; // 要隐藏的UI

    private void Awake()
    {
        if (target == null)
            target = gameObject; // 默认隐藏自己
    }

    private void Update()
    {
        if (target.activeSelf && Input.GetMouseButtonDown(0)) // 左键点击
        {
            target.SetActive(false);
        }
    }
}
