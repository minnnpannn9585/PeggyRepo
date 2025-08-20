// GrantKeyOnPickup.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrantKeyOnPickup : MonoBehaviour
{
    [Header("���Կ�׵�ID��Ҫ����/Ŀ��ƥ�䣩")]
    public string keyId = "EmployeeDoorKey";

    private void Reset()
    {
        // ֻ�Ǹ����գ����û���������������㹴��
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // ����ұ������Կ�ף���������� Pickup��
        var inv = other.GetComponent<PlayerInventory>() ?? FindObjectOfType<PlayerInventory>();
        if (inv != null) inv.AddKey(keyId);

        // ע�⣺��Ҫ���������٣������������Լ��� Pickup.cs ����
    }
}