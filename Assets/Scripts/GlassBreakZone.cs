using UnityEngine;
using System.Collections;

public class GlassBreakZone : MonoBehaviour
{
    [Header("����")]
    public KeyCode smashKey = KeyCode.F;
    public string hammerId = "Hammer";

    [Header("UI������� ClickToHide �������أ�")]
    public GameObject uiNeedHammer;
    public GameObject uiRunAway;

    [Header("����/����")]
    public Animator shatterAnimator;
    public string shatterTrigger = "Shatter";
    public float shatterDelay = 0.18f;
    public GameObject intact;
    public GameObject fracturedPrefab;
    public GameObject treasureModel;
    public AudioClip shatterSfx;

    [Header("����Ŀ��")]
    public Transform enemyTarget;

    private PlayerInventory _inv;
    private bool _sequenceStarted;
    private bool _vfxDone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _inv = other.GetComponent<PlayerInventory>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _inv != null && other.GetComponent<PlayerInventory>() == _inv)
            _inv = null;
    }

    private void Update()
    {
        if (_inv == null || _sequenceStarted) return;

        if (Input.GetKeyDown(smashKey))
        {
            if (_inv.HasKey(hammerId))
            {
                StartCoroutine(CoBreakSequence());
            }
            else
            {
                Show(uiNeedHammer);
            }
        }
    }

    private IEnumerator CoBreakSequence()
    {
        _sequenceStarted = true;

        // 1) ����������ѡ��
        if (shatterAnimator) shatterAnimator.SetTrigger(shatterTrigger);

        // 2) �������˲�䡱
        yield return new WaitForSeconds(shatterDelay);

        // 3) ִ��������Ч + ��ʾUI�����ٵȴ�UI�رգ�
        DoShatterVFX();

        // 4) ���̹㲥����ǰ��
        Vector3 target = enemyTarget ? enemyTarget.position :
                         (intact ? intact.transform.position : transform.position);
        Debug.Log($"[GBZ] ALERT immediately -> {target}");
        EnemyAlert.Alert(target);

        // 5) ��β
        Destroy(gameObject, 0.1f);
    }

    /// ����ִ������/����/��Ч/��ʾ�����ܡ�UI��Ҳ���ɶ����¼�ֱ�ӵ��ã�
    public void DoShatterVFX()
    {
        if (_vfxDone) return;
        _vfxDone = true;

        Vector3 pos = intact ? intact.transform.position : transform.position;
        Quaternion rot = intact ? intact.transform.rotation : transform.rotation;

        if (fracturedPrefab)
        {
            var fx = Instantiate(fracturedPrefab, pos, rot);
            Destroy(fx, 12f);
        }

        if (intact) intact.SetActive(false);
        if (treasureModel) treasureModel.SetActive(false);

        if (shatterSfx) AudioSource.PlayClipAtPoint(shatterSfx, pos);

        // ����ʾ������������
        Show(uiRunAway);
    }

    // ���� UI ��������������ͬʱ��ʾ�������Զ����� ����
    private void Show(GameObject go)
    {
        if (!go) return;
        if (uiNeedHammer && go != uiNeedHammer) uiNeedHammer.SetActive(false);
        if (uiRunAway && go != uiRunAway) uiRunAway.SetActive(false);
        go.SetActive(true);
    }
}