using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class GridCell : SerializedMonoBehaviour
{
    [DisableInEditorMode] public float gridPosX;
    [DisableInEditorMode] public float gridPosY;

    [OnValueChanged("ResetGrid")]
    public bool isGate;
    [ShowIf("isGate", true)]
    [TabGroup("GATE SETTING")]
    [SerializeField] private GateDir gateDir;
    [ShowIf("isGate", true)]
    [TabGroup("GATE SETTING")]
    [SerializeField] private ColorType gateColor;
    [FoldoutGroup("List")]
    public List<ColorType> listGateColor = new();
    [ShowIf("isGate", true)]
    [TabGroup("GATE SETTING")]
    [SerializeField] private List<GateInfor> gateInfor = new();
    [FoldoutGroup("List")]
    [DisableInEditorMode] public List<Vector3> listQueueCustomerPosition = new();
    [FoldoutGroup("List")]
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

    [FoldoutGroup("Neighbor")]
    [DisableInEditorMode]
    [SerializeField] private float T_Neighbor_Index;
    [FoldoutGroup("Neighbor")]
    [DisableInEditorMode]
    [SerializeField] private float B_Neighbor_Index;
    [FoldoutGroup("Neighbor")]
    [DisableInEditorMode]
    [SerializeField] private float R_Neighbor_Index;
    [FoldoutGroup("Neighbor")]
    [DisableInEditorMode]
    [SerializeField] private float L_Neighbor_Index;

    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer T_Carpet;
    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer B_Carpet;
    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer R_Carpet;
    [FoldoutGroup("Carpet")]
    [SerializeField] private MeshRenderer L_Carpet;

    [SerializeField] private GridLevelController gridLevelController;
    public GridLevelController _gridLevelController
    {
        get
        {
            return gridLevelController;
        }
        set
        {
            gridLevelController = value;
        }
    }

    private LayerMask mask;
    BoxCollider collider;
    Block block;
    bool isOnBlock;
    private Coroutine transportCoroutine;

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
    }
    private void OnEnable()
    {
        ObserverManager.OnReviewGate += ReviewListCusomer;
    }
    private void OnDisable()
    {
        ObserverManager.OnReviewGate -= ReviewListCusomer;
    }
    private void Start()
    {

        if (isGate)
        {
            collider.enabled = true;
            SetGateColor();
            mask = LayerMask.GetMask("Car");
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
    private void SetGateColor()
    {

        if(listCustomer.Count <= 0)
        {
            gateDir = GateDir.NONE;
            SetGateDir();
            return;
        }
        ColorType type = listCustomer[0].customerColor;
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
        for (int i = 0; i < gateInfor.Count; i++)
        {
            count += gateInfor[i].quantity;
        }
        float offset = 0.75f;
        Vector3 startPosition = Vector3.zero;
        Vector3 moveDir = new Vector3(0, 1, 0);
        switch (gateDir)
        {
            case GateDir.TOP:
                startPosition = T_Carpet.transform.position;
                startPosition.z += offset;
                break;
            case GateDir.BOTTOM:
                startPosition = B_Carpet.transform.position;
                startPosition.z -= offset;
                break;
            case GateDir.LEFT:

                startPosition = L_Carpet.transform.position;
                startPosition.x -= offset;  
                break;
            case GateDir.RIGHT:
                startPosition = R_Carpet.transform.position;
                startPosition.x += offset;
                break;
        }
        for(int i = 0; i < count; i++)
        {
            listQueueCustomerPosition.Add(startPosition + moveDir * customerDistance * i);
        }

        SpawnCustomer();
    }
    private void SpawnCustomer()
    {
        DestroyAllCustomer();
        listCustomer = new();
        foreach (var position in listQueueCustomerPosition)
        {
            Customer customer = Instantiate(customerPrefab, position, Quaternion.identity, transform);
           // customer.gameObject.transform.localPosition = new Vector3(0f, customer.transform.localPosition.y, 0f);
            customer.gameObject.transform.localScale = Vector3.one * 500f;
            customer.gameObject.transform.eulerAngles =  new Vector3(90f, 0f, 0f);
            //customer.transform.forward = -transform.position;
            listCustomer.Add(customer);
            _gridLevelController._levelManager.listAllCustomer.Add(customer);  
        }
        AddListGateColor();
    }
    private void AddListGateColor()
    {
        listGateColor = new();
        for (int i = 0; i < gateInfor.Count; i++)
        {
            for (int j = 0; j < gateInfor[i].quantity; j++)
            {
                listGateColor.Add(gateInfor[i].colorGate);
            }
        }
        SetCustomColor();
    }
    [FoldoutGroup("List")]
    [GUIColor(0.3f, 0.8f, 0.3f)]
    [Button("Init Color")]
    private void SetCustomColor()
    {
        for (int i = 0; i < listCustomer.Count; i++)
        {
            listCustomer[i].SetCustomerColor(listGateColor[i]);
        }
    }
    private void DestroyAllCustomer()
    {
        if (listCustomer != null)
        {
            foreach (var child in listCustomer)
            {
                if (child != null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            listCustomer.Clear();
            _gridLevelController._levelManager.ReviewList();
        }

    }
    public void ReviewListCusomer()
    {
        if (!isGate) return; 
        if (listCustomer.Count > 0)
        {
            listCustomer.RemoveAll(item => item == null);
        }
        for (int i = 0; i < listCustomer.Count; i++)
        {
            listCustomer[i].MoveQueue(listQueueCustomerPosition[i]);
        }
        SetGateColor();

    }
    #endregion
    private void ResetGrid()
    {
        gateDir = GateDir.NONE;
        SetGateDir();
        DestroyAllCustomer();
        listCustomer.Clear();
        listQueueCustomerPosition.Clear();
        listGateColor.Clear();
        gateInfor.Clear();
    }
    private IEnumerator TransportCoroutine()
    {
        if (listCustomer.Count == 0 || block.listSittingTransform.Count == 0)
        {
            yield break; 
        }

        if (block.blockColor == listCustomer[0].customerColor)
        {
            if (listCustomer.Count > 0 && block.listSittingTransform.Count > 0)
            {
                listCustomer[0].JumpInBlock(block.listSittingTransform[0]);
                block.listSittingTransform.RemoveAt(0);
                listCustomer.RemoveAt(0);
                for(int i = 0; i < listCustomer.Count; i++)
                {
                    listCustomer[i].MoveQueue(listQueueCustomerPosition[i]);
                }
                SetGateColor();
                block.CheckDeparture();
            }
            yield return new WaitForSeconds(0.15f);
            transportCoroutine = StartCoroutine(TransportCoroutine());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (transportCoroutine != null) return;
        if (!isGate || listCustomer.Count == 0) return;
        if ((mask.value & (1 << other.gameObject.layer)) > 0)
        {
            block = other.gameObject.GetComponent<Block>();
            Debug.Log("Hello");
            transportCoroutine = StartCoroutine(TransportCoroutine());

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if ((mask.value & (1 << other.gameObject.layer)) > 0)
        {
            if (!collider.bounds.Contains(other.transform.position)) 
            {
                if (transportCoroutine != null)
                {
                    StopCoroutine(transportCoroutine);
                    transportCoroutine = null;
                }
            }
        }
    }
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
