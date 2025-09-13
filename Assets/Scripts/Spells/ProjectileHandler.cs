using UnityEngine;
using UnityEngine.VFX;

public class ProjectileController : MonoBehaviour
{

    [Header("Tunning Variables")]
    [SerializeField] public int damageAmount = 1;
    [SerializeField] public float speed = 30f;


    //Variables
    Rigidbody rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Fire(Vector3 targetPoint)
    {
        if (rb != null)
        {
            float distance = Vector3.Distance(transform.position, targetPoint);

            rb.isKinematic = false;
            rb.linearVelocity = transform.forward * speed;
            
            if (speed > 0)
            {
                float lifeTime = distance / speed;
                Destroy(gameObject, lifeTime);
            }
            else
            {
                Destroy(gameObject, 5f);
            }

        }
    }
}
