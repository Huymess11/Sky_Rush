using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
    public Renderer[] renderers;

    private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

    [Serializable]
    private class ListVector3
    {
        public List<Vector3> data;
    }

    [Header("Optional")]

    [SerializeField, HideInInspector]
    private List<Mesh> bakeKeys = new List<Mesh>();

    [SerializeField, HideInInspector]
    private List<ListVector3> bakeValues = new List<ListVector3>();



    [Button]
    public void GetRenderer()
    {
        renderers = new Renderer[1] { GetComponent<Renderer>() };
    }
    [Button]
    public void GetRendererInChild()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Awake()
    {
        LoadSmoothNormals();
    }

    void OnEnable()
    {
        foreach (var renderer in renderers)
        {

            var materials = renderer.sharedMaterials.ToList();

            materials.Add(OutlineShare.Instance.outlineMaskMaterial);
            materials.Add(OutlineShare.Instance.enableOutlineFillMaterial);

            renderer.materials = materials.ToArray();
        }
    }

    void OnDisable()
    {
        if (isApplicationQuitting) return;

        foreach (var renderer in renderers)
        {

            var materials = renderer.sharedMaterials.ToList();

            materials.Remove(OutlineShare.Instance.outlineMaskMaterial);
            materials.Remove(OutlineShare.Instance.enableOutlineFillMaterial);

            renderer.materials = materials.ToArray();
        }
    }

    private bool isApplicationQuitting = false;
    private void OnApplicationQuit()
    {
        isApplicationQuitting = true;
    }

    void OnValidate()
    {
        if (bakeKeys.Count == 0)
        {
            Bake();
        }
    }

    void Bake()
    {

        var bakedMeshes = new HashSet<Mesh>();

        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {

            if (!bakedMeshes.Add(meshFilter.sharedMesh))
            {
                continue;
            }

            var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

            bakeKeys.Add(meshFilter.sharedMesh);
            bakeValues.Add(new ListVector3() { data = smoothNormals });
        }
    }

    void LoadSmoothNormals()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
        var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

        meshFilter.sharedMesh.SetUVs(3, smoothNormals);

        var renderer = meshFilter.GetComponent<Renderer>();

        if (renderer != null)
        {
            CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
        }

        if(TryGetComponent(out SkinnedMeshRenderer skinMesh))
        {
            skinMesh.sharedMesh.uv4 = new Vector2[skinMesh.sharedMesh.vertexCount];
            CombineSubmeshes(skinMesh.sharedMesh, skinMesh.sharedMaterials);
        }
    }

    List<Vector3> SmoothNormals(Mesh mesh)
    {

        var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

        var smoothNormals = new List<Vector3>(mesh.normals);

        foreach (var group in groups)
        {
            if (group.Count() == 1)
            {
                continue;
            }

            var smoothNormal = Vector3.zero;

            foreach (var pair in group)
            {
                smoothNormal += smoothNormals[pair.Value];
            }

            smoothNormal.Normalize();

            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }

        return smoothNormals;
    }

    void CombineSubmeshes(Mesh mesh, Material[] materials)
    {
        if (mesh.subMeshCount == 1)
        {
            return;
        }

        if (mesh.subMeshCount > materials.Length)
        {
            return;
        }

        mesh.subMeshCount++;
        mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }
}
