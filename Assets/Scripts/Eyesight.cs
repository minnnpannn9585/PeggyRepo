using UnityEngine;

public class Eyesight : MonoBehaviour
{
    [Header("FOV")]
    public float viewRadius = 50f;
    [Range(0, 360)] public float viewAngle = 60f;

    [Header("Layer Masks")]
    public LayerMask targetMask;        
    public LayerMask obstructionMask;   

    [Header("Eye Origin")]
    public Transform eyePoint;          
    public float eyeHeight = 1.7f;

    [Header("稳定判定（防抖）")]
    public float requiredVisibleSeconds = 1.0f;   
    public bool useDecayWhenHidden = true;        
    public float hiddenDecayPerSecond = 2.0f;     
    public float exitGraceSeconds = 0.15f;        

    [HideInInspector] public bool playerInSight;  
    public float visibleProgress01 => Mathf.Clamp01(visibleTimer / requiredVisibleSeconds); 

    private float visibleTimer = 0f;   
    private float lastSeenTime = -999f;
    private float suppressUntil = 0f;  

    void Start()
    {
        if (targetMask == 0) targetMask = LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (Time.time < suppressUntil)
        {
            playerInSight = false;
            visibleTimer = 0f;
            return;
        }

        Vector3 eyePos = eyePoint ? eyePoint.position : (transform.position + Vector3.up * eyeHeight);
        Vector3 eyeFwd = eyePoint ? eyePoint.forward : transform.forward;

        bool seenThisFrame = false;

        // 在视野半径内找玩家
        var targets = Physics.OverlapSphere(eyePos, viewRadius, targetMask, QueryTriggerInteraction.Collide);
        foreach (var t in targets)
        {
            var player = t.transform;
            Vector3 toPlayer = player.position - eyePos;
            float dist = toPlayer.magnitude;
            if (dist < 0.001f) continue;

            Vector3 dir = toPlayer / dist;
            if (Vector3.Angle(eyeFwd, dir) > viewAngle * 0.5f) continue;

            // 射线检查：只打在遮挡层上
            bool blocked = Physics.Raycast(eyePos, dir, dist, obstructionMask, QueryTriggerInteraction.Ignore);
            if (!blocked)
            {
                seenThisFrame = true;
                lastSeenTime = Time.time;
                break; 
            }
        }

        playerInSight = seenThisFrame;

        // ——“连续看到”计时逻辑——
        if (seenThisFrame || Time.time - lastSeenTime <= exitGraceSeconds)
        {
            // 本帧看到（或处于短暂宽限期）：累加
            visibleTimer = Mathf.Min(requiredVisibleSeconds, visibleTimer + Time.deltaTime);
        }
        else
        {
            // 本帧未看到：清零或衰减
            if (useDecayWhenHidden)
                visibleTimer = Mathf.Max(0f, visibleTimer - hiddenDecayPerSecond * Time.deltaTime);
            else
                visibleTimer = 0f;
        }

        // ——达到阈值才算“真正被看见”——
        if (visibleTimer >= requiredVisibleSeconds)
        {
            // 这里触发一次“被抓到”的事件（只需触发一次）
            // 建议你的 GameOver 管理器内部自己做冷却，避免每帧重复弹窗
            GameOverOnSight.Instance?.ReportSpotted();
            // 触发后可选：清零或保持满格，根据你的需求
            visibleTimer = 0f; // 触发后清空，防止重复触发
        }
    }

    /// <summary>相机/楼层切换、瞬移时调用：在一小段时间内完全不判定</summary>
    public void Suppress(float seconds)
    {
        suppressUntil = Mathf.Max(suppressUntil, Time.time + seconds);
        visibleTimer = 0f;
        playerInSight = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = eyePoint ? eyePoint.position : (transform.position + Vector3.up * eyeHeight);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(origin, viewRadius);
        Vector3 A = DirectionFromAngle(-viewAngle / 2, transform.rotation);
        Vector3 B = DirectionFromAngle(+viewAngle / 2, transform.rotation);
        Gizmos.color = Color.green; Gizmos.DrawLine(origin, origin + A * viewRadius);
        Gizmos.DrawLine(origin, origin + B * viewRadius);
    }

    private Vector3 DirectionFromAngle(float deg, Quaternion rot)
    {
        Vector3 dir = new Vector3(Mathf.Sin(deg * Mathf.Deg2Rad), 0, Mathf.Cos(deg * Mathf.Deg2Rad));
        return rot * dir;
    }
}
