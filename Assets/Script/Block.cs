using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Block : MonoBehaviour
{
    public List<Vector3> listFirstDefaultPoint; 
    public List<Transform> listSittingTransform;
    [SerializeField] private Transform sittingPositionParent;
    public ColorType blockColor;
    public BlockType blockType;
    public MeshRenderer mr;
    public TextMeshProUGUI stepUnlockText;
    private Collider blockCollider;

    [ShowIf("blockType",BlockType.ICE)]
    [TabGroup("ICE TYPE")]
    public int stepUnlock;
    private Outline outline; 


    private void Awake()
    {
        outline = GetComponentInChildren<Outline>();
    }
    private void OnEnable()
    {
        ObserverManager.OnDefrost += ReduceStep;
    }
    private void OnDisable()
    {
        ObserverManager.OnDefrost -= ReduceStep;
    }
    private void Start()
    {
        stepUnlockText.gameObject.SetActive(blockType == BlockType.ICE);
        SetTextUnlockBlock(stepUnlock);
        blockCollider  = GetComponent<Collider>();
    }

    [Button ("Create Sitting Position")]
    private void CreateSittingPosition()
    {
        ClearSittingPosition();
        float offSetFloor = 0f;
        for(int i = 0; i < 4; i++)
        {
            offSetFloor += 0.25f;
            for (int j = 0; j< listFirstDefaultPoint.Count; j++)
            {
                GameObject newSitting = new GameObject("Sitting Position" + i);
                newSitting.transform.SetParent(sittingPositionParent);
                Vector3 newPos = listFirstDefaultPoint[j];
                newPos.y = offSetFloor;
                newSitting.transform.localPosition = newPos;
                listSittingTransform.Add(newSitting.transform);
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var sitting in listSittingTransform)
        {
            if (sitting != null)
            {
                Gizmos.DrawSphere(sitting.transform.position, 0.2f);
            }
        }
    }
    [Button("Clear Sitting Position")]
    private void ClearSittingPosition()
    {
        if(listSittingTransform != null)
        {
            foreach (var sitting in listSittingTransform)
            {
                if (sitting != null)
                {
                    DestroyImmediate(sitting.gameObject);
                }
            }
            listSittingTransform.Clear();
        }
    }
    [Button("Rotate Block")]
    public void Rotate()
    {
        transform.localEulerAngles += new Vector3(0, 90, 0);
    }
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
    private void ReduceStep()
    {
        if (stepUnlock == 0) return;
        stepUnlock--;
        blockType = stepUnlock == 0 ? BlockType.NONE : BlockType.ICE;
        stepUnlockText.gameObject.SetActive(stepUnlock!=0);
        SetTextUnlockBlock(stepUnlock);
    }
    private void SetTextUnlockBlock(int value)
    {
        stepUnlockText.text = value.ToString();
    }
    public void CheckDeparture()
    {
        if(listSittingTransform.Count == 0)
        {
            blockCollider.enabled = false;
            outline.enabled = false;
            ObserverManager.Defrost();
            Departure();
        }
    }
    private void Departure()
    {
        Vector3 screen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
        Vector3 firstPos = new Vector3(transform.position.x, 5f, transform.position.z);
        Vector3 finalPos = new Vector3((transform.position.x >= 0) ? (screen.x / 2 + 10f) : (- (screen.x/2) - 10f),5f, transform.position.z);
        Vector3[] path = { firstPos, finalPos };
        transform.DOPath(path, 2f).OnComplete(() =>
        {
            gameObject.SetActive(false);

        });
     }
}
public enum BlockType
{
    NONE,
    HORIZONTAL,
    VERTICAL,
    ICE
}
