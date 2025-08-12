using UnityEngine;

public class SyringeInjector : MonoBehaviour
{
    [Header("Input")]
    public KeyCode injectKey = KeyCode.E;   // tieni premuto per iniettare

    [Header("Serbatoio")]
    public float capacity = 1f;             // dose totale caricata
    public float injectionRate = 0.5f;      // dose al secondo
    public float remaining;                 // dose rimanente (visibile in Inspector)

    [Header("Modelli visivi")]
    public GameObject fullVisual;           // ← trascina qui il GameObject "Siringa"
    public GameObject emptyVisual;          // ← trascina qui il GameObject "Siringa Vuota"

    [Header("FX (opzionali)")]
    public ParticleSystem sprayVfx;         // particelle sulla punta
    public AudioSource spraySfx;            // audio spray

    private AnesthetizableTooth currentTooth;

    void Start()
    {
        remaining = Mathf.Max(0f, capacity);
        UpdateVisuals();
        if (spraySfx) spraySfx.playOnAwake = false;
    }

    void OnTriggerEnter(Collider other)
    {
        currentTooth = other.GetComponentInParent<AnesthetizableTooth>();
    }

    void OnTriggerExit(Collider other)
    {
        var t = other.GetComponentInParent<AnesthetizableTooth>();
        if (t != null && t == currentTooth) currentTooth = null;
    }

    void Update()
    {
        bool canInject = currentTooth != null && remaining > 0f && Input.GetKey(injectKey);

        if (canInject)
        {
            float delta = Mathf.Min(injectionRate * Time.deltaTime, remaining);
            currentTooth.ApplyAnesthetic(delta, transform.position);
            remaining -= delta;

            if (sprayVfx && !sprayVfx.isPlaying) sprayVfx.Play();
            if (spraySfx && !spraySfx.isPlaying) spraySfx.Play();

            if (remaining <= 0f)
            {
                remaining = 0f;
                UpdateVisuals();
                StopFx();
            }
        }
        else
        {
            if (sprayVfx && sprayVfx.isPlaying) sprayVfx.Stop();
            if (spraySfx && spraySfx.isPlaying) spraySfx.Stop();
        }
    }

    void UpdateVisuals()
    {
        bool hasDose = remaining > 0f;
        if (fullVisual)  fullVisual.SetActive(hasDose);
        if (emptyVisual) emptyVisual.SetActive(!hasDose);
    }

    void StopFx()
    {
        if (sprayVfx && sprayVfx.isPlaying) sprayVfx.Stop();
        if (spraySfx && spraySfx.isPlaying) spraySfx.Stop();
    }
}
