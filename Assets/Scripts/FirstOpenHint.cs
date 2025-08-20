using UnityEngine;
using System.Collections;

public class FirstOpenHint : MonoBehaviour
{
    [Header("��Ҫ��Կ��ID")]
    public string requiredKeyId = "EmployeeDoorKey";

    [Header("��һ�ο���Ҫ��ʾ��ͼƬUI")]
    public GameObject hintUI;

    [Header("�ӳ���ʾʱ�䣨�룩")]
    public float delay = 1f;

    private bool _playerInside = false;
    private bool _shown = false; // �Ƿ��Ѿ���ʾ��

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _playerInside = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) _playerInside = false;
    }

    void Update()
    {
        if (_shown) return;        // ֻ��ʾһ��
        if (!_playerInside) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            var inv = FindObjectOfType<PlayerInventory>();
            if (inv != null && inv.HasKey(requiredKeyId))
            {
                StartCoroutine(ShowHintDelayed());
                _shown = true; // ���Ϊ����ʾ
            }
        }
    }

    private IEnumerator ShowHintDelayed()
    {
        yield return new WaitForSeconds(delay);
        if (hintUI) hintUI.SetActive(true);
    }
}