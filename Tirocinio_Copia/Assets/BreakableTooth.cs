using UnityEngine;

public class BreakableTooth : MonoBehaviour
{
    [Header("Soglia di rottura (aumenta se si rompe troppo spesso)")]
    public float breakThreshold = 30f;   // massa*velocità del colpo
    public float minSpeedToCount = 1.5f; // ignora sfioramenti lenti

    [Header("Prefab e FX (opzionali)")]
    public GameObject fracturedPrefab;   // dente rotto (può essere null)
    public ParticleSystem breakVfx;      // particelle (può essere null)
    public AudioSource breakSfx;         // suono (può essere null)

    [Header("Spinta sui frammenti (se fracturedPrefab ha Rigidbodies)")]
    public float shardExplosionForce = 50f;
    public float shardExplosionRadius = 0.1f;

    bool broken = false;

    void OnCollisionEnter(Collision col)
    {
        if (broken) return;

        // prendi solo i colpi della TESTA del martello (tag "Hammer")
        if (!col.collider.CompareTag("Hammer")) return;

        float speed = col.relativeVelocity.magnitude;
        if (speed < minSpeedToCount) return;

        float mass = col.rigidbody ? col.rigidbody.mass : 1f;
        float impact = mass * speed; // stima energia del colpo

        // Debug opzionale: apri la Console per vedere
        // Debug.Log($"Impatto: {impact}");

        if (impact >= breakThreshold)
        {
            Vector3 hitPoint = col.GetContact(0).point;
            Break(hitPoint);
        }
    }

    void Break(Vector3 hitPoint)
    {
        broken = true;

        // FX
        if (breakVfx)
        {
            breakVfx.transform.position = hitPoint;
            breakVfx.Play();
        }
        if (breakSfx) breakSfx.Play();

        // Se hai il prefab fratturato, instanzialo
        if (fracturedPrefab)
        {
            var brokenObj = Instantiate(fracturedPrefab, transform.position, transform.rotation);

            // Dai una piccola spinta ai pezzi, se hanno rigidbody
            foreach (var rb in brokenObj.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(shardExplosionForce, hitPoint, shardExplosionRadius);
            }
        }

        // Nascondi il dente intero (o Destroy)
        gameObject.SetActive(false);
    }
}
