using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LockedDoorHint : MonoBehaviour
{
    [Header("��Ҫ��Կ��ID������һ�£�")]
    public string requiredKeyId = "EmployeeDoorKey";

    [Header("ûԿ��ʱҪ��ʾ��UIͼƬ")]
    public GameObject lockedUI;

    [Tooltip("�뿪������ʱ�Ƿ��Զ����أ�����ֻ��������أ��͹ص���")]
    public bool hideOnExit = false;

    private bool _playerInside;

    void Reset()
    {
        // ȷ�����Ǹ�������
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;

        var inv = other.GetComponent<PlayerInventory>() ?? FindObjectOfType<PlayerInventory>();
        bool hasKey = inv && inv.HasKey(requiredKeyId);

        // ֻ��ûԿ�ײ���ʾ UI
        if (!hasKey && lockedUI) lockedUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;

        if (hideOnExit && lockedUI) lockedUI.SetActive(false);
    }
}
