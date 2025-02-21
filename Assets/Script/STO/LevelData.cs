using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "STO/LevelData")]
public class LevelData : ScriptableObject
{
    public List<LevelInfor> levelInfors = new();
}
[Serializable]
public class LevelInfor
{
    public GameObject levelPrefabs;
}