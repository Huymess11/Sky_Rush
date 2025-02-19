using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime.Collections;

public class Customer : MonoBehaviour
{
    [DisableInEditorMode] public ColorType customerColor;
    [SerializeField] private MeshRenderer mr;
    public bool transportComplete;
    public void SetCustomerColor(ColorType type)
    {
        object[] loadedObjects = Resources.LoadAll("Materials");
        Material material;
        foreach (var obj in loadedObjects)
        {
            material = obj as Material;
            if (material.name.StartsWith(type.ToString()))
            {
                mr.material = material;
                break;
            }
        }
        customerColor = type;
    }
    public void JumpInBlock(Transform trans)
    {
        Vector3 centerPos = (transform.position + trans.position) / 2 + Vector3.up * 1.5f;
        centerPos.y += 0.5f;
        Vector3[] path = new Vector3[] {centerPos, trans.position };
        transform.DOPath(path, 0.1f, PathType.CatmullRom)
            .OnComplete(() =>
            {
                transform.SetParent(trans);
                transform.localPosition =  Vector3.zero;
            });
    }

    public void MoveQueue(Vector3 pos)
    {
        transform.DOMove(pos, 0.05f);
    }
}
