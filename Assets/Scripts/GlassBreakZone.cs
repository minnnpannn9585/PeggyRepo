using UnityEngine;
using System.Collections;

public class GlassBreakZone : MonoBehaviour
{
    [Header("交互")]
    public KeyCode smashKey = KeyCode.F;
    public string hammerId = "Hammer";

    [Header("UI（由你的 ClickToHide 控制隐藏）")]
    public GameObject uiNeedHammer;
    public GameObject uiRunAway;

    [Header("玻璃/宝物")]
    public Animator shatterAnimator;
    public string shatterTrigger = "Shatter";
    public float shatterDelay = 0.18f;
    public GameObject intact;
    public GameObject fracturedPrefab;
    public GameObject treasureModel;
    public AudioClip shatterSfx;

    [Header("敌人目标")]
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

        // 1) 播动画（可选）
        if (shatterAnimator) shatterAnimator.SetTrigger(shatterTrigger);

        // 2) 到“冲击瞬间”
        yield return new WaitForSeconds(shatterDelay);

        // 3) 执行破碎特效 + 显示UI（不再等待UI关闭）
        DoShatterVFX();

        // 4) 立刻广播敌人前往
        Vector3 target = enemyTarget ? enemyTarget.position :
                         (intact ? intact.transform.position : transform.position);
        Debug.Log($"[GBZ] ALERT immediately -> {target}");
        EnemyAlert.Alert(target);

        // 5) 收尾
        Destroy(gameObject, 0.1f);
    }

    /// 真正执行破碎/隐藏/音效/显示“快跑”UI（也可由动画事件直接调用）
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

        // 仅显示，不阻塞流程
        Show(uiRunAway);
    }

    // —— UI 辅助：避免两张同时显示，不做自动隐藏 ——
    private void Show(GameObject go)
    {
        if (!go) return;
        if (uiNeedHammer && go != uiNeedHammer) uiNeedHammer.SetActive(false);
        if (uiRunAway && go != uiRunAway) uiRunAway.SetActive(false);
        go.SetActive(true);
    }
}