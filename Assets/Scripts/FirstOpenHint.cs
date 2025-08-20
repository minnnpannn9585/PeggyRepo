using UnityEngine;
using System.Collections;

public class FirstOpenHint : MonoBehaviour
{
    [Header("需要的钥匙ID")]
    public string requiredKeyId = "EmployeeDoorKey";

    [Header("第一次开门要显示的图片UI")]
    public GameObject hintUI;

    [Header("延迟显示时间（秒）")]
    public float delay = 1f;

    private bool _playerInside = false;
    private bool _shown = false; // 是否已经显示过

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
        if (_shown) return;        // 只显示一次
        if (!_playerInside) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            var inv = FindObjectOfType<PlayerInventory>();
            if (inv != null && inv.HasKey(requiredKeyId))
            {
                StartCoroutine(ShowHintDelayed());
                _shown = true; // 标记为已显示
            }
        }
    }

    private IEnumerator ShowHintDelayed()
    {
        yield return new WaitForSeconds(delay);
        if (hintUI) hintUI.SetActive(true);
    }
}