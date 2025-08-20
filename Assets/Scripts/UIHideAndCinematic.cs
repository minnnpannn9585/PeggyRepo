using UnityEngine;
using UnityEngine.Playables;   // ������� Timeline����ѡ
// using Cinemachine;         // ������� Cinemachine��ȡ������ע��

public class UIHideAndCinematic : MonoBehaviour
{
    [Header("������ʽ")]
    [Tooltip(">=0 ʱ���õ���ʱ��<0 ���õ���ʱ����֧������ر�")]
    public float timer = -1f;

    [Header("Ҫ�л��Ķ�����ѡ��")]
    public GameObject thingToClose;   // ���� MainCamera
    public GameObject thingToOpen;    // ���� Camera01��������/Timeline��

    [Header("��ѡ������/ʱ����")]
    public Animator animator;         // ���о�ͷ��Ҫ���� Animator
    public string animatorTrigger = "Play";
    public PlayableDirector timeline; // ���� Timeline���ر� UI ʱ����

    [Header("UI ��Ϊ")]
    [Tooltip("�ر�ʱ�����أ��Ƽ����������� UI ����")]
    public bool destroyThisUI = false;

    bool _armed;     // �������������ʾ���̱�����ص�
    bool _done;

    void OnEnable()
    {
        _armed = false;
        _done = false;
        // ��һ֡������������ֹ��
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

        // 1) ��������ر�
        if (_armed && Input.GetMouseButtonDown(0))
        {
            Finish();
            return;
        }

        // 2) ��ѡ������ʱ�����ر�
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

        // �л����/����
        if (thingToClose) thingToClose.SetActive(false);
        if (thingToOpen) thingToOpen.SetActive(true);

        // ��������/ʱ���ߣ���ѡ��һ��Ҫ��
        if (animator && !string.IsNullOrEmpty(animatorTrigger))
            animator.SetTrigger(animatorTrigger);

        if (timeline)
            timeline.Play();

        // �ر����� UI
        if (destroyThisUI) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
