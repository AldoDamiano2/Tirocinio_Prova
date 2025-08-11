using UnityEngine;
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;

// Un semplice Editor Window
public class ProceduralFbxTool : EditorWindow
{
    enum Shape { Cube, Sphere, Cylinder }

    Shape selectedShape = Shape.Cube;
    string fbxName = "NewObject";

    [MenuItem("Tools/Procedural FBX Tool")]
    static void OpenWindow()
    {
        GetWindow<ProceduralFbxTool>("Procedural FBX");
    }

    void OnGUI()
    {
        GUILayout.Label("Genera & Esporta FBX", EditorStyles.boldLabel);

        // 1) Scegli la forma
        selectedShape = (Shape)EditorGUILayout.EnumPopup("Shape", selectedShape);
        fbxName     = EditorGUILayout.TextField("FBX Name", fbxName);

        if (GUILayout.Button("Generate & Export"))
            GenerateAndExport();
    }

    void GenerateAndExport()
    {
        // 2) Crea un GameObject temporaneo
        GameObject go = null;
        switch (selectedShape)
        {
            case Shape.Cube:
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            case Shape.Sphere:
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                break;
            case Shape.Cylinder:
                go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                break;
        }
        go.name = fbxName;

        // 3) Posizionalo al centro
        go.transform.position = Vector3.zero;

        // 4) Chiedi dove salvare
        string path = EditorUtility.SaveFilePanel(
            "Save FBX",
            Application.dataPath,
            fbxName + ".fbx",
            "fbx"
        );
        if (string.IsNullOrEmpty(path))
        {
            DestroyImmediate(go);
            return;
        }

        // 5) Esporta selezione in FBX
        ModelExporter.ExportObject(path, go);
        Debug.Log($"✅ Esportato FBX: {path}");

        // 6) Pulisci la scena
        DestroyImmediate(go);
    }
}
