using UnityEngine;

public class DrillCarver : MonoBehaviour
{
    public LayerMask toothMask;
    public ToothCarvePainter painter;
    public float radiusUV = 0.01f;   // raggio pennello in UV
    public float strength = 0.9f;    // forza pennello
    public float maxDistance = 0.02f;// distanza max punta-punto impatto
    public Transform tip;            // Transform della punta

    void Update()
    {
        // Ray in avanti dalla punta
        var origin = tip.position;
        var dir    = tip.forward;
Debug.DrawRay(tip.position, tip.forward * 0.05f, Color.cyan);

        if (Physics.Raycast(origin, dir, out var hit, 0.05f, toothMask))
        {
            if (Vector3.Distance(tip.position, hit.point) <= maxDistance)
            {
                painter.PaintAtUV(hit.textureCoord, radiusUV, strength);
            }
        }
    }
}
