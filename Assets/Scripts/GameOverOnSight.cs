using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverOnSight : MonoBehaviour
{
    [Header("����")]
    public Eyesight eyesight;      // ����ĵ����ϵ� Eyesight ���
    public GameObject spottedUI;   // ��һ�š������֡���UIͼƬ

    [Header("������Ϸ��ʽ")]
    public bool pauseGameOnSpotted = true;   // Ĭ�ϣ���ͣ��Ϸ
    public float delayBeforeAction = 0.1f;   // ��ʾUI�������ִ�ж���
    public string gameOverSceneName = "";    // ������е� GameOver �����������ֲ��� pauseGameOnSpotted �ص�

    private bool triggered;

    void Reset()
    {
        // �Զ�Ѱ��ͬ������������ϵ� Eyesight
        if (!eyesight) eyesight = GetComponentInChildren<Eyesight>();
    }

    void Update()
    {
        if (triggered || eyesight == null) return;

        if (eyesight.playerInSight)
        {
            triggered = true;
            if (spottedUI) spottedUI.SetActive(true);

            // �ӳ�һ��ʱ��ִ�н�������
            Invoke(nameof(DoEndAction), delayBeforeAction);
        }
    }

    void DoEndAction()
    {
        if (!string.IsNullOrEmpty(gameOverSceneName))
        {
            // �л��� GameOver ����
            SceneManager.LoadScene(gameOverSceneName);
        }
        else if (pauseGameOnSpotted)
        {
            // ��ͣ��Ϸ
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
