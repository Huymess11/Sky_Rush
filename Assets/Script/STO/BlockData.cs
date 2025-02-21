using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="BlockData", menuName ="STO/BlockData")]
public class BlockData : ScriptableObject
{
    public List<BlockInfor> blockInfors = new();
}
[Serializable]
public class BlockInfor
{
    public BlockShapeType type;
    public GameObject blockPrefab;
}
public enum BlockShapeType
{
    _1X1,
    _1X2,
    _1X3,
    _1X4,
    _2X2
}
