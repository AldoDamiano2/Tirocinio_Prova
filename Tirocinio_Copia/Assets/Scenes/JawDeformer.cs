using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class JawDeformer : MonoBehaviour
{
    public Transform jawPivot;
    [Range(0, 45)]
    public float openAngle = 0f;

    Mesh mesh;
    Vector3[] originalVerts;

    void Start()
    {
        // 1) Prendi il MeshFilter e il suo sharedMesh
        var mf = GetComponent<MeshFilter>();
        var shared = mf.sharedMesh;
        if (shared == null)
        {
            Debug.LogError("JawDeformer: nessun mesh presente!");
            return;
        }

        // 2) Clona il mesh e assegnalo a mesh e al MeshFilter
        mesh = Instantiate(shared);
        mf.mesh = mesh;  

        // 3) Conserva i vertici originali
        originalVerts = mesh.vertices;
        
        // Debug
        Debug.Log($"JawDeformer: mesh clonata con {originalVerts.Length} vertici.");
        if (jawPivot == null)
            Debug.LogError("JawDeformer: jawPivot non assegnato!");
    }

    void Update()
    {
        if (jawPivot == null || mesh == null) return;

        // Calcola pivot in locale
        Vector3 pivotLocal = transform.InverseTransformPoint(jawPivot.position);

        var verts = new Vector3[originalVerts.Length];
        Quaternion rot = Quaternion.AngleAxis(openAngle, Vector3.right);

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = originalVerts[i];
            if (v.y < pivotLocal.y)
            {
                Vector3 dir = v - pivotLocal;
                verts[i] = pivotLocal + rot * dir;
            }
            else
            {
                verts[i] = v;
            }
        }

        // Applica la deformazione
        mesh.vertices = verts;
        // Ora è il *tuo* mesh runtime, quindi puoi ricalcolare le normali
        mesh.RecalculateNormals();
    }
}
