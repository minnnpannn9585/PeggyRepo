// GrantKeyOnPickup.cs
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GrantKeyOnPickup : MonoBehaviour
{
    [Header("这把钥匙的ID（要和门/目标匹配）")]
    public string keyId = "EmployeeDoorKey";

    private void Reset()
    {
        // 只是个保险：如果没勾触发器，提醒你勾上
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 给玩家背包添加钥匙（不依赖你的 Pickup）
        var inv = other.GetComponent<PlayerInventory>() ?? FindObjectOfType<PlayerInventory>();
        if (inv != null) inv.AddKey(keyId);

        // 注意：不要在这里销毁；销毁仍由你自己的 Pickup.cs 处理
    }
}