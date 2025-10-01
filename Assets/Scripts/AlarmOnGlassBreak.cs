using UnityEngine;

/// 当玻璃被砸碎后，触发红灯闪烁动画（使用 Trigger）
public class AlarmOnGlassBreak : MonoBehaviour
{
    [Header("Watch Target")]
    [Tooltip("GlassBreakZone 脚本里会被 SetActive(false) 的对象，比如 intact 玻璃")]
    public GameObject watchObject;

    [Header("Alarm Animation")]
    public Animator alarmAnimator;             // 红灯/警报的 Animator
    public string alarmTriggerName = "Alarm";  // Animator Trigger 参数名

    [Header("Optional SFX")]
    public AudioSource sirenAudio;             // 可选：警报声

    private bool fired;

    void Update()
    {
        if (fired) return;
        if (!watchObject) return;

        // 一旦检测到 watchObject 被隐藏，说明玻璃已砸碎
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