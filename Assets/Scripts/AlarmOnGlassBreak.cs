using UnityEngine;

/// ������������󣬴��������˸������ʹ�� Trigger��
public class AlarmOnGlassBreak : MonoBehaviour
{
    [Header("Watch Target")]
    [Tooltip("GlassBreakZone �ű���ᱻ SetActive(false) �Ķ��󣬱��� intact ����")]
    public GameObject watchObject;

    [Header("Alarm Animation")]
    public Animator alarmAnimator;             // ���/������ Animator
    public string alarmTriggerName = "Alarm";  // Animator Trigger ������

    [Header("Optional SFX")]
    public AudioSource sirenAudio;             // ��ѡ��������

    private bool fired;

    void Update()
    {
        if (fired) return;
        if (!watchObject) return;

        // һ����⵽ watchObject �����أ�˵������������
        if (!watchObject.activeInHierarchy)
        {
            FireAlarm();
        }
    }

    private void FireAlarm()
    {
        fired = true;

        if (alarmAnimator && !string.IsNullOrEmpty(alarmTriggerName))
        {
            alarmAnimator.SetTrigger(alarmTriggerName);
        }

        if (sirenAudio)
        {
            sirenAudio.Play();
        }
    }
}