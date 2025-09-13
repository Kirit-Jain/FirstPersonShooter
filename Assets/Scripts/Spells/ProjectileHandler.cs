using UnityEngine;
using UnityEngine.VFX;

public class ProjectileController : MonoBehaviour
{
    [Header("Reference Variables")]
    [SerializeField] GameObject HitVFXPrefab;

    [Header("Tunning Variables")]
    [SerializeField] float speed = 30f;
    [SerializeField] int damageAmount = 1;
    [SerializeField] float lifetime = 5f;
    // [SerializeField] float vfxSpawnOffset = 0.2f;


    //Variables
    Rigidbody rb;
    Vector3 vfxSpawnPoint;
    bool isFired = false;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Fire(Vector3 hitPoint)
    {
        if (rb != null)
        {
            vfxSpawnPoint = hitPoint;
            Destroy(gameObject, lifetime);

            rb.isKinematic = false;
            rb.linearVelocity = transform.forward * speed;
            isFired = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isFired || collision.gameObject.CompareTag("Player")) return;

        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damageAmount);
        }

        // Vector3 hitPoint = other.ClosestPoint(transform.position);

        // Vector3 surfaceNormal = (transform.position - hitPoint).normalized;

        
            
        Instantiate(HitVFXPrefab, this.transform.position, Quaternion.identity);
        

        Destroy(gameObject, 0.01f);
    }
}
