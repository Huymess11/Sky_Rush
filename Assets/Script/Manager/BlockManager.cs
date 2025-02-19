using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance;
    [SerializeField] public BlockData data;

    public BlockShapeType _type;
    [DisableInEditorMode]
    public List<Block> listBlock = new();

    private void Awake()
    {
         if(Instance == null) {Instance = this;}
    }
    [Button("Create Block")]
    private void CreateBlock()
    {
        for (int i = 0; i < data.blockInfors.Count; i++)
        {
            if (data.blockInfors[i].type == _type)
            {
                listBlock.Add(Instantiate(data.blockInfors[i].blockPrefab, transform).GetComponent<Block>());
            }
        }
    }
    [Button("Clear All Block")]
    private void ClearAllBlock()
    {
        if (listBlock.Count == 0) return;
        foreach(var child in listBlock)
        {
            if(child != null)
            {
                DestroyImmediate(child.gameObject);
            }
                

        }
        listBlock.Clear();
    }

    public void CheckWin()
    {
        foreach (var child in listBlock)
        {
            if (!child.isFull)
            {
                return;
            }
        }
        LevelManager.Instance.Victory();
    }
}
