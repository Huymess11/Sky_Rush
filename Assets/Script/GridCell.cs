using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : SerializedMonoBehaviour
{
    [DisableInEditorMode] public float gridPosX;
    [DisableInEditorMode] public float gridPosY;

    public bool isGate;
    [ShowIf("isGate", true)]
    [TabGroup("GATE SETTING")]
    [SerializeField] private GateDir gateDir;
    [ShowIf("isGate", true)]
    [TabGroup("GATE SETTING")]
    [SerializeField] private ColorType gateColor;
    [DisableInEditorMode] public List<ColorType> listGateColor = new();
    [ShowIf("isGate", true)]
    [TabGroup("GATE SETTING")]
    [SerializeField] private List<GateInfor> gateInfor = new();
    [DisableInEditorMode] public List<Vector3> listQueueCustomerPosition = new();
    [DisableInEditorMode] public List<Customer> listCustomer = new();
    [ShowIf("isGate", true)]
    [TabGroup("GATE SETTING")]
    [SerializeField] private float customerDistance = 0.4f;
    [ShowIf("isGate", true)]
    [TabGroup("GATE SETTING")]
    [SerializeField] private Customer customerPrefab;

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

    [SerializeField] private float T_Neighbor_Index;
    [SerializeField] private float B_Neighbor_Index;
    [SerializeField] private float R_Neighbor_Index;
    [SerializeField] private float L_Neighbor_Index;

    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer T_Carpet;
    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer B_Carpet;
    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer R_Carpet;
    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer L_Carpet;

    private void Start()
    {
        if (isGate)
        {
            SetGateColor(listCustomer[0].customerColor);
        }
    }
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
    #region GATE
    [ShowIf("isGate", true)]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    [TabGroup("GATE SETTING")]
    [Button("SetupGate", ButtonSizes.Medium)]
    public void SetGate()
    {
        SetGateDir();
        SpawnQueuePosition();
    }
    private void SetGateDir()
    {
        T_Carpet.gameObject.SetActive(gateDir == GateDir.TOP);
        B_Carpet.gameObject.SetActive(gateDir == GateDir.BOTTOM);
        R_Carpet.gameObject.SetActive(gateDir == GateDir.RIGHT);
        L_Carpet.gameObject.SetActive(gateDir == GateDir.LEFT);
    }
    private void SetGateColor(ColorType type)
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
    private void SpawnQueuePosition()
    {
        listQueueCustomerPosition = new();
        int count = 0;
        for(int i = 0; i < gateInfor.Count; i++)
        {
            count += gateInfor[i].quantity; 
        }
        for(int i = 0; i < count; i++)
        {

            switch (gateDir)
            {
                case GateDir.TOP:
                    listQueueCustomerPosition.Add(T_Carpet.transform.position + new Vector3(0, 0, 1) * customerDistance * i);
                    break;
                case GateDir.BOTTOM:
                    listQueueCustomerPosition.Add(B_Carpet.transform.position + new Vector3(0, 0, -1) * customerDistance * i);
                    break;
                case GateDir.LEFT:
                    listQueueCustomerPosition.Add(L_Carpet.transform.position + new Vector3(-1, 0, 0) * customerDistance * i);
                    break;
                case GateDir.RIGHT:
                    listQueueCustomerPosition.Add(R_Carpet.transform.position + new Vector3(1, 0, 0) * customerDistance * i);
                    break;
                default:
                    break;
            }

        }
        SpawnCustomer();
    }
    private void SpawnCustomer()
    {
        DestroyAllCustomer();
        listCustomer = new();
        foreach (var position in listQueueCustomerPosition)
        {
           Customer customer=  Instantiate(customerPrefab, position, Quaternion.identity, transform);
            customer.gameObject.transform.localPosition = new Vector3(customer.transform.localPosition.x, 0.5f, customer.transform.localPosition.z);
            customer.gameObject.transform.localScale = Vector3.one*3f;
            listCustomer.Add(customer);
        }
        AddListGateColor();
    }
    private void AddListGateColor()
    {
        listGateColor = new();
        for(int i = 0; i < gateInfor.Count; i++)
        {
            for(int j = 0; j < gateInfor[i].quantity; j++)
            {
                listGateColor.Add(gateInfor[i].colorGate);
            }
        }
        SetCustomColor();
    }
    private void SetCustomColor()
    {
        for(int i = 0; i < listCustomer.Count; i++)
        {
            listCustomer[i].SetCustomerColor(listGateColor[i]);
        }
    }
    private void DestroyAllCustomer()
    {
        if(listCustomer != null)
        {
            foreach (var child in listCustomer)
            {
                if(child != null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            listCustomer.Clear();
        }

    }
    #endregion
}
public enum ColorType
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
[Serializable]
public class GateInfor
{
    public int quantity;
    public ColorType colorGate;
}
