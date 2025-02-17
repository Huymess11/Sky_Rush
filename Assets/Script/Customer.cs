using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    [DisableInEditorMode] public ColorType customerColor;
    [SerializeField] private MeshRenderer mr;
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
}
