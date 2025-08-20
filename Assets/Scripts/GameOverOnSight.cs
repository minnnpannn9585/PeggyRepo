using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverOnSight : MonoBehaviour
{
    [Header("引用")]
    public Eyesight eyesight;      // 拖你的敌人上的 Eyesight 组件
    public GameObject spottedUI;   // 拖一张“被发现”的UI图片

    [Header("结束游戏方式")]
    public bool pauseGameOnSpotted = true;   // 默认：暂停游戏
    public float delayBeforeAction = 0.1f;   // 显示UI后多少秒执行动作
    public string gameOverSceneName = "";    // 如果想切到 GameOver 场景，填名字并把 pauseGameOnSpotted 关掉

    private bool triggered;

    void Reset()
    {
        // 自动寻找同物体或子物体上的 Eyesight
        if (!eyesight) eyesight = GetComponentInChildren<Eyesight>();
    }

    void Update()
    {
        if (triggered || eyesight == null) return;

        if (eyesight.playerInSight)
        {
            triggered = true;
            if (spottedUI) spottedUI.SetActive(true);

            // 延迟一点时间执行结束动作
            Invoke(nameof(DoEndAction), delayBeforeAction);
        }
    }

    void DoEndAction()
    {
        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            // 切换到 GameOver 场景
            SceneManager.LoadScene(gameOverSceneName);
        }
        else if (pauseGameOnSpotted)
        {
            // 暂停游戏
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
