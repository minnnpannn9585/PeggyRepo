using UnityEngine;

public class Pickup : MonoBehaviour
{
    public GameObject text;

    [Header("Sound")]
    public AudioClip pickupClip;       // ʰȡ��Ч
    public float volume = 1.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // ������Ч��������λ�ò��ţ�
            if (pickupClip)
            {
                AudioSource.PlayClipAtPoint(pickupClip, transform.position, volume);
            }

            if (text) text.SetActive(true);

            Destroy(gameObject);
        }
    }
}