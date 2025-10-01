using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyFootsteps : MonoBehaviour
{
    public AudioSource audioSource;      // ���ŽŲ����� AudioSource
    public AudioClip[] footstepClips;    // �Ų���Ч�����Էż�����������ţ�

    [Header("Step Settings")]
    public float stepInterval = 0.5f;    // ������·ʱ�ļ�����룩
    public float minSpeed = 0.1f;        // ��������ٶȲ������Ų�

    private NavMeshAgent agent;
    private float stepTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // ��ǰ�ٶ�
        float speed = agent.velocity.magnitude;

        if (speed > minSpeed && !agent.isStopped)
        {
            stepTimer += Time.deltaTime;

            // �ٶ�Խ�죬���Խ�̣�3.5f ���滻Ϊ���Ĭ�� walkSpeed��
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
        audioSource.pitch = Random.Range(0.95f, 1.05f); // ���һ�����ߣ������е��
        audioSource.PlayOneShot(clip);
    }
}