using UnityEngine;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;

public class DentureGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Denture FBX")]
    public static void ShowWindow()
    {
        GetWindow<DentureGenerator>("Denture FBX Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Denture FBX Generator", EditorStyles.boldLabel);
        if (GUILayout.Button("Generate & Export Denture"))
        {
            GenerateAndExportDenture();
        }
    }

    private void GenerateAndExportDenture()
    {
        // Create parent object
        GameObject denture = new GameObject("Denture");

        // Parameters for teeth placement
        int perArch = 16;
        float archRadius = 0.5f;
        float yOffset = 0.0f;

        // Generate upper arch
        for (int i = 0; i < perArch; i++)
        {
            float angle = Mathf.Lerp(-90, 90, i / (float)(perArch - 1));
            CreateTooth(denture.transform, i + 1, angle, archRadius, yOffset);
        }

        // Generate lower arch (inverted)
        for (int i = 0; i < perArch; i++)
        {
            float angle = Mathf.Lerp(90, 270, i / (float)(perArch - 1));
            CreateTooth(denture.transform, perArch + i + 1, angle, archRadius, yOffset - 0.1f);
        }

        // Create palate halves
        CreatePalate(denture.transform, "Palate_Left", -0.15f);
        CreatePalate(denture.transform, "Palate_Right", 0.15f);

        // Prompt save path
        string path = EditorUtility.SaveFilePanel("Save Denture FBX", Application.dataPath, "DentureComplete.fbx", "fbx");
        if (!string.IsNullOrEmpty(path))
        {
            ModelExporter.ExportObject(path, denture);
            Debug.Log($"Denture FBX exported to: {path}");
        }

        // Clean up
        DestroyImmediate(denture);
    }

    private void CreateTooth(Transform parent, int index, float angleDeg, float radius, float yPos)
    {
        // Simple cylinder as tooth placeholder
        GameObject tooth = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tooth.name = $"Tooth_{index}";
        tooth.transform.SetParent(parent, false);
        tooth.transform.localScale = new Vector3(0.1f, 0.2f, 0.1f);

        // Position along an arc
        float rad = angleDeg * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * radius;
        float z = Mathf.Sin(rad) * radius;
        tooth.transform.localPosition = new Vector3(x, yPos, z);

        // Orient so top faces up
        tooth.transform.localRotation = Quaternion.Euler(0, angleDeg + 90, 0);
    }

    private void CreatePalate(Transform parent, string name, float xOffset)
    {
        GameObject palate = GameObject.CreatePrimitive(PrimitiveType.Cube);
        palate.name = name;
        palate.transform.SetParent(parent, false);
        palate.transform.localScale = new Vector3(0.5f, 0.05f, 0.3f);
        palate.transform.localPosition = new Vector3(xOffset, 0.05f, 0);
    }
}
