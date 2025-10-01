using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFootsteps : MonoBehaviour
{
    public AudioSource audioSource;      // 播放脚步声的 AudioSource
    public AudioClip[] footstepClips;    // 脚步音效（可以放几条，随机播放）

    [Header("Step Settings")]
    public float stepInterval = 0.5f;    // 正常走路时的间隔（秒）
    public float minSpeed = 0.1f;        // 低于这个速度不触发脚步

    private NavMeshAgent agent;
    private float stepTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // 当前速度
        float speed = agent.velocity.magnitude;

        if (speed > minSpeed && !agent.isStopped)
        {
            stepTimer += Time.deltaTime;

            // 速度越快，间隔越短（3.5f 可替换为你的默认 walkSpeed）
            float interval = stepInterval * (3.5f / Mathf.Max(speed, 0.01f));

            if (stepTimer >= interval)
            {
                stepTimer = 0f;
                PlayFootstep();
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length == 0 || !audioSource) return;

        AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];
        audioSource.pitch = Random.Range(0.95f, 1.05f); // 随机一点音高，避免机械感
        audioSource.PlayOneShot(clip);
    }
}