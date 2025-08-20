using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LockedDoorHint : MonoBehaviour
{
    [Header("需要的钥匙ID（与门一致）")]
    public string requiredKeyId = "EmployeeDoorKey";

    [Header("没钥匙时要显示的UI图片")]
    public GameObject lockedUI;

    [Tooltip("离开触发区时是否自动隐藏（若你只想左键隐藏，就关掉）")]
    public bool hideOnExit = false;

    private bool _playerInside;

    void Reset()
    {
        // 确保这是个触发器
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;

        var inv = other.GetComponent<PlayerInventory>() ?? FindObjectOfType<PlayerInventory>();
        bool hasKey = inv && inv.HasKey(requiredKeyId);

        // 只有没钥匙才显示 UI
        if (!hasKey && lockedUI) lockedUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;

        if (hideOnExit && lockedUI) lockedUI.SetActive(false);
    }
}
