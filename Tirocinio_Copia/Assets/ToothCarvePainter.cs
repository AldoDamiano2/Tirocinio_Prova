using UnityEngine;

[DisallowMultipleComponent]
public class ToothCarvePainter : MonoBehaviour
{
    [Header("Renderer del dente")]
    public Renderer target;                 // Trascina qui il MeshRenderer/SkinnedMeshRenderer del dente

    [Header("Shader property (deve combaciare col Graph)")]
    public string property = "_RemovalMask";

    [Header("Dimensione della mask")]
    public int maskSize = 1024;

    private Texture2D removalMask;          // <-- NON va assegnata da Inspector
    private Color[] buf;

    void Awake()
    {
        // Auto-aggancio del renderer se ti dimentichi lo slot
        if (!target) target = GetComponent<Renderer>();
        if (!target) target = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        if (!target)
        {
            Debug.LogError("ToothCarvePainter: assegna 'target' al Renderer del dente.");
            enabled = false;
            return;
        }

        // Crea una Texture2D in RAM (monocanale) dove 'pitturare'
        removalMask = new Texture2D(maskSize, maskSize, TextureFormat.R8, false, true);
        removalMask.wrapMode = TextureWrapMode.Clamp;

        // Inizializza nera (0 = non scavato)
        buf = new Color[maskSize * maskSize];
        for (int i = 0; i < buf.Length; i++) buf[i] = new Color(0, 0, 0, 1);
        removalMask.SetPixels(buf);
        removalMask.Apply(false, false);

        // Istanzia il materiale (per non modificare l'asset) e collega la texture
        var mat = target.material;
        if (!mat.HasProperty(property))
            Debug.LogWarning($"Il materiale non ha la property '{property}'. Controlla la Reference nello Shader Graph.");
        mat.SetTexture(property, removalMask);

        Debug.Log($"[ToothCarvePainter] Mask creata {maskSize}x{maskSize} e collegata a '{property}'.");
    }

    // Chiamata dalla turbina
    public void PaintAtUV(Vector2 uv, float radiusUV, float strength)
    {
        if (removalMask == null || buf == null) return;

        int w = removalMask.width, h = removalMask.height;
        int cx = Mathf.RoundToInt(Mathf.Clamp01(uv.x) * (w - 1));
        int cy = Mathf.RoundToInt((1f - Mathf.Clamp01(uv.y)) * (h - 1));
        int r  = Mathf.CeilToInt(Mathf.Clamp01(radiusUV) * w);

        for (int y = -r; y <= r; y++)
        for (int x = -r; x <= r; x++)
        {
            int px = cx + x, py = cy + y;
            if ((uint)px >= w || (uint)py >= h) continue; // bound check veloce
            float t = 1f - Mathf.Clamp01(new Vector2(x, y).magnitude / r); // bordo morbido
            int idx = py * w + px;
            float v = buf[idx].r;
            v = Mathf.Clamp01(Mathf.Lerp(v, 1f, t * strength)); // 1 = scavato
            buf[idx] = new Color(v, v, v, 1);
        }

        removalMask.SetPixels(buf);
        removalMask.Apply(false, false);
    }
}
