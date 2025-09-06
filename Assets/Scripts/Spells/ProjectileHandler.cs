using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Tunning Variables")]
    [SerializeField] float speed = 30f;
    [SerializeField] int damageAmount = 1;
    [SerializeField] float lifetime = 5f;


    //Variables
    Rigidbody rb;
    bool isFired = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Fire()
    {
        if (rb != null)
        {
            Destroy(gameObject, lifetime);

            rb.isKinematic = false;
            rb.linearVelocity = transform.forward * speed;
            isFired = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isFired || other.CompareTag("Player")) return;

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
        }

        Destroy(gameObject, 0.01f);
    }
}
