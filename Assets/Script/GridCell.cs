using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class GridCell : MonoBehaviour
{
    public float gridPosX { get; private set; }
    public float gridPosY { get; private set; }

    public bool isGate;
    [ShowIf("isGate", true)]
    [TabGroup("GATE SETTING")]
    [SerializeField] private GateDir gateDir;
    [ShowIf("isGate", true)]
    [TabGroup("GATE SETTING")]
    [SerializeField] private GateColorType gateColor;
    private List<GateColorType> listGateColor;


    [FoldoutGroup("Wall")]
    public Collider T_Wall;
    [FoldoutGroup("Wall")]
    public Collider B_Wall;
    [FoldoutGroup("Wall")]
    public Collider R_Wall;
    [FoldoutGroup("Wall")]
    public Collider L_Wall;


    [FoldoutGroup("Neighbor")]
    [DisableInEditorMode]
    public bool T_Neighbor = false;
    [FoldoutGroup("Neighbor")]
    [DisableInEditorMode]
    public bool B_Neighbor = false;
    [FoldoutGroup("Neighbor")]
    [DisableInEditorMode]
    public bool R_Neighbor = false;
    [FoldoutGroup("Neighbor")]
    [DisableInEditorMode]
    public bool L_Neighbor = false;

    float T_Neighbor_Index;
    float B_Neighbor_Index;
    float R_Neighbor_Index;
    float L_Neighbor_Index;

    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer T_Carpet;
    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer B_Carpet;
    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer R_Carpet;
    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer L_Carpet;

    public void SetGridPosition(float posX, float posY)
    {
        gridPosX = posX;
        gridPosY = posY;
        CalculateNeighborIndex();
    }
    #region NEIGHBOR
    private void CalculateNeighborIndex()
    {
        T_Neighbor_Index = gridPosY + 1;
        B_Neighbor_Index = gridPosY - 1;
        R_Neighbor_Index = gridPosX + 1;
        L_Neighbor_Index = gridPosX - 1;
    }

    public void CheckNeighbor(List<GridCell> allCells)
    {
        foreach (var cell in allCells)
        {
            if (cell == this) continue; 

            if (cell.gridPosX == gridPosX && cell.gridPosY == T_Neighbor_Index)
            {
                T_Neighbor = true;
            }
            if (cell.gridPosX == gridPosX && cell.gridPosY == B_Neighbor_Index)
            {
                B_Neighbor = true;
            }
            if (cell.gridPosY == gridPosY && cell.gridPosX == R_Neighbor_Index)
            {
                R_Neighbor = true;
            }
            if (cell.gridPosY == gridPosY && cell.gridPosX == L_Neighbor_Index)
            {
                L_Neighbor = true;
            }
        }

        SetWall();
    }
    public void SetWall()
    {
        T_Wall.gameObject.SetActive(!T_Neighbor);
        B_Wall.gameObject.SetActive(!B_Neighbor);
        R_Wall.gameObject.SetActive(!R_Neighbor);
        L_Wall.gameObject.SetActive(!L_Neighbor);
    }
    #endregion

    [ShowIf("isGate", true)]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    [TabGroup("GATE SETTING")]
    [Button("SetGate", ButtonSizes.Medium)]
    public void SetGate()
    {
        SetGateDir();
        SetGateColor(gateColor);
    }
    private void SetGateDir()
    {
        T_Carpet.gameObject.SetActive(gateDir == GateDir.TOP);
        B_Carpet.gameObject.SetActive(gateDir == GateDir.BOTTOM);
        R_Carpet.gameObject.SetActive(gateDir == GateDir.RIGHT);
        L_Carpet.gameObject.SetActive(gateDir == GateDir.LEFT);
    }
    private void SetGateColor(GateColorType type)
    {
        object[] loadedObjects = Resources.LoadAll("Materials");
        Material material;
        foreach (var obj in loadedObjects)
        {
            material = obj as Material;
            if (material.name.StartsWith(type.ToString()))
            {
                T_Carpet.material = material;
                B_Carpet.material = material;
                R_Carpet.material = material;
                L_Carpet.material = material;
                break;
            }
        }
    }
}
public enum GateColorType
{
   NONE = -1,
   DO,
   CAM,
   VANG,
   XANH,
   TIM,
   HONG
}
public enum GateDir
{
    NONE = -1,
    TOP,
    BOTTOM,
    RIGHT,
    LEFT
}
