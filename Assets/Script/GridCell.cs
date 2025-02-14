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
    [TabGroup("GATE COLOR")]
    private GateColorType currentColor;
    private List<GateColorType> listGateColor;

    public GateColorType _currentColor
    {
        get { return currentColor; }
        set { currentColor = value; }
    }

    [Title("Neighbor")]
    [DisableInEditorMode]
    public bool T_Neighbor;
    [DisableInEditorMode]
    public bool B_Neighbor;
    [DisableInEditorMode]
    public bool R_Neighbor;
    [DisableInEditorMode]
    public bool L_Neighbor;


    float T_Neighbor_Index;
    float B_Neighbor_Index;
    float R_Neighbor_Index;
    float L_Neighbor_Index;
    public void SetGridPosition(float posX, float posY)
    {
        gridPosX = posX;
        gridPosY = posY;
        CalculateNeighborIndex();
    }
    private void CalculateNeighborIndex()
    {
        T_Neighbor_Index = gridPosY + 1;
        B_Neighbor_Index = gridPosY - 1;
        R_Neighbor_Index = gridPosX + 1;
        L_Neighbor_Index = gridPosX - 1;
    }

    public void CheckNeighbor(GridCell cellCheckNeighbor)
    {
        if (!T_Neighbor)
        {
            T_Neighbor = (T_Neighbor_Index == cellCheckNeighbor.gridPosY) && (cellCheckNeighbor.gridPosX == gridPosX);
        }
        if (!B_Neighbor)
        {
            B_Neighbor = B_Neighbor_Index == cellCheckNeighbor.gridPosY && (cellCheckNeighbor.gridPosX == gridPosX);
        }
        if (!R_Neighbor)
        {
            R_Neighbor = R_Neighbor_Index == cellCheckNeighbor.gridPosX && (cellCheckNeighbor.gridPosY == gridPosY);
        }
        if (!L_Neighbor)
        {
            L_Neighbor = L_Neighbor_Index == cellCheckNeighbor.gridPosX && (cellCheckNeighbor.gridPosY == gridPosY);
        }
    }

}
public enum GateColorType
{
   DO,
   CAM,
   VANG,
   XANH,
   TIM,
   HONG
}
