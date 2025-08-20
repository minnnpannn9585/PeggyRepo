namespace SatProductions
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class BreakObjectOnCollide : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision == null) { return; }

            if (collision.gameObject.GetComponent<BreakableObject>())
            {
                BreakableObject breakableObject = collision.gameObject.GetComponent<BreakableObject>();

                if (breakableObject.BreakOnHit)
                {
                    breakableObject.Break();
                }
            }
        }
    }
}