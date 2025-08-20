using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorInteractor : MonoBehaviour
{
    public string requiredKeyId = "EmployeeDoorKey"; // ��Կ�׵� keyId һ��
    public Animator doorAnimator;                    // �� Door1 �� Animator
    public Collider doorBlocker;                     // �� Door1 �ķǴ��� BoxCollider
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
                // ���ڴ˵���������UI����� UI �Ϲ� ClickToHide ���ɣ�
            }
        }
    }

    void OpenOnce()
    {
        _opened = true;
        if (doorAnimator && !string.IsNullOrEmpty(openTriggerName))
            doorAnimator.SetTrigger(openTriggerName);

        if (doorBlocker) doorBlocker.enabled = false;     // ����������Collider�ƶ�
        GetComponent<Collider>().enabled = false;         // ��ֹ�ٴδ���
    }
}
