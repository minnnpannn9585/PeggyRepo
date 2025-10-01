using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverOnSight : MonoBehaviour
{
    // ✅ 单例
    public static GameOverOnSight Instance { get; private set; }

    [Header("引用")]
    public Eyesight eyesight;      // 拖你的敌人上的 Eyesight 组件
    public GameObject spottedUI;   // 拖一张“被发现”的UI图片

    [Header("结束游戏方式")]
    public bool pauseGameOnSpotted = true;   // 默认：暂停游戏
    public float delayBeforeAction = 0.1f;   // 显示UI后多少秒执行动作
    public string gameOverSceneName = "";    // 若想切到 GameOver 场景，填名字并把 pauseGameOnSpotted 关掉

    [Header("冷却/抑制")]
    public float cooldownAfterShown = 0.5f;  // 弹出后短暂免疫，防止重复触发
    private float cooldownUntil = 0f;        // 在此时间点之前不响应触发

    private bool triggered;

    void Awake()
    {
        Instance = this;
        if (spottedUI) spottedUI.SetActive(false);
    }

    void Reset()
    {
        if (!eyesight) eyesight = GetComponentInChildren<Eyesight>();
    }

    void Update()
    {
        if (triggered || eyesight == null) return;

        // 兼容你原本的“每帧轮询”逻辑（Eyesight.playerInSight）
        // 如果用了新版 Eyesight 的“连续可见≥阈值后内部调用 ReportSpotted()”，
        // 这里也没问题：被触发后 triggered=true，会阻止二次触发。
        if (Time.time >= cooldownUntil && eyesight.visibleProgress01 >= 1f)
        {
            TriggerGameOver();
        }
    }

    // ✅ 提供给 Eyesight（或别的地方）直接调用的接口
    public void ReportSpotted()
    {
        if (triggered) return;
        if (Time.time < cooldownUntil) return;
        TriggerGameOver();
    }

    // ✅ 提供给镜头/楼层切换抑制用
    public void SuppressFor(float seconds)
    {
        cooldownUntil = Mathf.Max(cooldownUntil, Time.time + seconds);
    }

    private void TriggerGameOver()
    {
        triggered = true;

        if (spottedUI) spottedUI.SetActive(true);

        // 弹出后进入冷却（避免 UI/逻辑重复触发）
        cooldownUntil = Time.time + cooldownAfterShown;

        // 延迟一点时间执行结束动作（保持你原有的行为）
        Invoke(nameof(DoEndAction), delayBeforeAction);
    }

    private void DoEndAction()
    {
        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
        else if (pauseGameOnSpotted)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // （可选）如果你希望从外部重置状态：
    public void ResetState()
    {
        triggered = false;
        if (spottedUI) spottedUI.SetActive(false);
        // 不改 cooldownUntil，避免立即再次触发
    }
}
