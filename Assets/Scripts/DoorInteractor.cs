using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorInteractor : MonoBehaviour
{
    public string requiredKeyId = "EmployeeDoorKey"; // 与钥匙的 keyId 一致
    public Animator doorAnimator;                    // 拖 Door1 的 Animator
    public Collider doorBlocker;                     // 拖 Door1 的非触发 BoxCollider
    public string openTriggerName = "OpenTrigger";

    bool _inside, _opened;

    void Reset() { GetComponent<Collider>().isTrigger = true; }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) _inside = true; }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) _inside = false; }

    void Update()
    {
        if (!_inside || _opened) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            var inv = FindObjectOfType<PlayerInventory>();
            if (inv != null && inv.HasKey(requiredKeyId)) OpenOnce();
            else
            {
                // 可在此弹“上锁”UI（你的 UI 上挂 ClickToHide 即可）
            }
        }
    }

    void OpenOnce()
    {
        _opened = true;
        if (doorAnimator && !string.IsNullOrEmpty(openTriggerName))
            doorAnimator.SetTrigger(openTriggerName);

        if (doorBlocker) doorBlocker.enabled = false;     // 若动画不带Collider移动
        GetComponent<Collider>().enabled = false;         // 防止再次触发
    }
}
