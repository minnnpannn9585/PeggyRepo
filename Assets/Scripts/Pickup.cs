using UnityEngine;

public class Pickup : MonoBehaviour
{
    public GameObject text;

    [Header("Sound")]
    public AudioClip pickupClip;       // 拾取音效
    public float volume = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 播放音效（在物体位置播放）
            if (pickupClip)
            {
                AudioSource.PlayClipAtPoint(pickupClip, transform.position, volume);
            }

            if (text) text.SetActive(true);

            Destroy(gameObject);
        }
    }
}