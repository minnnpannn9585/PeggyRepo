using UnityEngine;
using UnityEngine.Playables;   // 如果你用 Timeline，可选
// using Cinemachine;         // 如果你用 Cinemachine，取消这行注释

public class UIHideAndCinematic : MonoBehaviour
{
    [Header("触发方式")]
    [Tooltip(">=0 时启用倒计时；<0 禁用倒计时，仅支持左键关闭")]
    public float timer = -1f;

    [Header("要切换的对象（任选）")]
    public GameObject thingToClose;   // 例如 MainCamera
    public GameObject thingToOpen;    // 例如 Camera01（带动画/Timeline）

    [Header("可选：动画/时间线")]
    public Animator animator;         // 若切镜头需要触发 Animator
    public string animatorTrigger = "Play";
    public PlayableDirector timeline; // 若用 Timeline，关闭 UI 时播放

    [Header("UI 行为")]
    [Tooltip("关闭时是隐藏（推荐）还是销毁 UI 对象")]
    public bool destroyThisUI = false;

    bool _armed;     // 防抖：避免刚显示立刻被点击关掉
    bool _done;

    void OnEnable()
    {
        _armed = false;
        _done = false;
        // 下一帧才允许点击，防止误触
        StartCoroutine(ArmNextFrame());
    }

    System.Collections.IEnumerator ArmNextFrame()
    {
        yield return null;
        _armed = true;
    }

    void Update()
    {
        if (_done) return;

        // 1) 左键立即关闭
        if (_armed && Input.GetMouseButtonDown(0))
        {
            Finish();
            return;
        }

        // 2) 可选：倒计时到点后关闭
        if (timer >= 0f)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Finish();
            }
        }
    }

    void Finish()
    {
        if (_done) return;
        _done = true;

        // 切换相机/物体
        if (thingToClose) thingToClose.SetActive(false);
        if (thingToOpen) thingToOpen.SetActive(true);

        // 触发动画/时间线（任选其一或都要）
        if (animator && !string.IsNullOrEmpty(animatorTrigger))
            animator.SetTrigger(animatorTrigger);

        if (timeline)
            timeline.Play();

        // 关闭这张 UI
        if (destroyThisUI) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
