using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public List<Transform> listSittingTransform;
    public ColorType blockColor;
    public BlockType blockType;
    public MeshRenderer mr;

    [Button("SetUpBlock")]
    private void SetUp()
    {
        SetBlockColor();
    }
    private void SetBlockColor()
    {
        object[] loadedObjects = Resources.LoadAll("Materials");
        Material material;
        foreach (var obj in loadedObjects)
        {
            material = obj as Material;
            if (material.name.StartsWith(blockColor.ToString()))
            {
                mr.material = material;
                break;
            }
        }
    }

}
public enum BlockType
{
    NONE,
    HORIZONTAL,
    VERTICAL,
    ICE
}
