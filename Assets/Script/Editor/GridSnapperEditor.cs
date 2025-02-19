using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSnapper))] 
public class GridSnapperEditor : Editor
{
#if UNITY_EDITOR
    static GridSnapperEditor()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if (Application.isPlaying) return;
        if (Selection.activeGameObject == null) return;

        GridSnapper snapper = Selection.activeGameObject.GetComponent<GridSnapper>();
        if (snapper == null) return;
        if (snapper.notSnap) return;
        Bounds bound = snapper.gameObject.GetComponent<Collider>().bounds;
        Transform t = snapper.transform;
        Vector3 position = t.position;
        float halfWidth = bound.size.x / 2f;
        float halfHeight = bound.size.z / 2f;
        position.x = Mathf.Round(snapper.gameObject.transform.position.x - halfWidth) + halfWidth;
        position.z = Mathf.Round(snapper.gameObject.transform.position.z - halfHeight) + halfHeight;
        t.position = position;
    }
#endif
}