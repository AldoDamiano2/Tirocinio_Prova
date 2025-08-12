using UnityEngine;

public class AnesthetizableTooth : MonoBehaviour
{
    public float requiredDose = 1f;
    [Range(0f, 1f)] public float anesthesiaLevel = 0f;

    public Renderer targetRenderer;
    public Color anesthetizedTint = new Color(0.7f, 0.9f, 1f, 1f);

    Color _orig;
    void Start()
    {
        if (targetRenderer == null) targetRenderer = GetComponentInChildren<Renderer>();
        if (targetRenderer) _orig = targetRenderer.material.color;
    }

    public void ApplyAnesthetic(float amount, Vector3 point)
    {
        if (anesthesiaLevel >= 1f) return;
        float step = requiredDose > 0f ? amount / requiredDose : amount;
        anesthesiaLevel = Mathf.Clamp01(anesthesiaLevel + step);
        if (anesthesiaLevel >= 1f && targetRenderer) targetRenderer.material.color = anesthetizedTint;
    }
}
