using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public List<Transform> listSittingTransform;
    public ColorType blockColor;
    public BlockType blockType;
    public MeshRenderer mr;
    public TextMeshProUGUI stepUnlockText;
    private Collider collider;
    public bool isFull;

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
        collider  = GetComponent<Collider>();
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
            outline.enabled = false;
            collider.enabled = false;
            isFull = true;
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
