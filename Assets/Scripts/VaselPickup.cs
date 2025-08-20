using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VaselPickup : MonoBehaviour
{
    [Header("��Ҫ��Կ��ID")]
    public string requiredKeyId = "VaselCaseKey";

    [Header("Ҫ�����ߵĴ�������")]
    public GameObject itemToTake;

    [Header("����ѡ����Կ��ʱ��ʾ����F����")]
    public GameObject uiPressF;

    [Header("����ѡ��ûԿ��ʱ��ʾ����ҪԿ��")]
    public GameObject uiNeedKey;

    [Header("����ѡ�����ߺ���ʾ�������Ʒ��ʾ")]
    public GameObject uiGotItem;

    private bool _inside;
    private bool _taken;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _inside = true;

        var inv = other.GetComponent<PlayerInventory>() ?? FindObjectOfType<PlayerInventory>();
        bool hasKey = inv && inv.HasKey(requiredKeyId);

        HideAllUI();

        if (_taken) return;

        if (hasKey) { if (uiPressF) uiPressF.SetActive(true); }
        else { if (uiNeedKey) uiNeedKey.SetActive(true); }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _inside = false;

        HideAllUI();
    }

    void Update()
    {
        if (!_inside || _taken) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            var inv = FindObjectOfType<PlayerInventory>();
            bool hasKey = inv && inv.HasKey(requiredKeyId);

            if (hasKey)
            {
                _taken = true;

                if (itemToTake) Destroy(itemToTake);

                HideAllUI();

                if (uiGotItem) uiGotItem.SetActive(true);

                var col = GetComponent<Collider>();
                if (col) col.enabled = false;
            }
            else
            {
                if (uiNeedKey) uiNeedKey.SetActive(true);
                if (uiPressF) uiPressF.SetActive(false);
            }
        }
    }

    private void HideAllUI()
    {
        if (uiPressF) uiPressF.SetActive(false);
        if (uiNeedKey) uiNeedKey.SetActive(false);
        if (uiGotItem) uiGotItem.SetActive(false);
    }
}
