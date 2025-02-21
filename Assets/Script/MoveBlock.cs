
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class MoveBlock : MonoBehaviour
{
    public float moveSpeed = 20f;
    [SerializeField] Block block;
    private Vector3 mousePosition;
    private Plane plane;
    private Rigidbody rb;
    private Vector3 offsetMouseDownObject;
    private Outline outline;
    bool isDrag;
    public GameObject child;
    Bounds bounds;
    Collider blockCollider;
    bool isHintDestroy;

    private void Awake()
    {
        blockCollider = GetComponent<Collider>();
        bounds = blockCollider.bounds;
        outline = GetComponentInChildren<Outline>();
    }
    private void OnEnable()
    {
        ObserverManager.OnHintDestroy += SetIsHintDestroy;
    }
    private void OnDisable()
    {
        ObserverManager.OnHintDestroy -= SetIsHintDestroy;
    }

    private void Start()
    {
        plane = new Plane(Vector3.up, Vector3.zero);
    }
    private void OnMouseDown()
    {
        TimeManager.Instance.StartTimer();
        if (block.blockType == BlockType.ICE) return;
        if (isHintDestroy)
        {
            LevelManager.Instance.DestroyCustomer(block.listSittingTransform.Count, block.blockColor);
            ObserverManager.HintDestroy(false);
            ObserverManager.Defrost();
            Destroy(gameObject);
        }
        else
        {
            isDrag = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out var enter))
            {
                mousePosition = ray.GetPoint(enter);
                offsetMouseDownObject = transform.position - mousePosition;
            }
            outline.enabled = true;
        }
    }
    private void FixedUpdate()
    {
        if (!isDrag) return;
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out var enter))
        {
            mousePosition = ray.GetPoint(enter);

            Vector3 targetPos = mousePosition + offsetMouseDownObject;

            switch (block.blockType)
            {
                case BlockType.HORIZONTAL:
                    targetPos.z = transform.position.z;
                    break;
                case BlockType.VERTICAL:
                    targetPos.x = transform.position.x;
                    break;
                default:
                    break;
            }
            transform.position = new Vector3(transform.position.x, 0.15f, transform.position.z);
            if (rb != null)
            {
                rb.velocity = (targetPos - transform.position) * moveSpeed;
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }

        }
    }
    private void OnMouseUp()
    {
        isDrag = false;
        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        float halfWidth = bounds.size.x / 2f;
        float halfHeight = bounds.size.z / 2f;
        float snappedX = Mathf.Round((transform.position.x - halfWidth)) + halfWidth;
        float snappedZ = Mathf.Round((transform.position.z - halfHeight)) + halfHeight;

        Vector3 posMove = new Vector3(snappedX, 0, snappedZ);
        transform.DOMove(posMove, 0.05f).OnComplete(() =>
        {
            Destroy(rb);
        });
        outline.enabled = false;
    }
    public void SetIsHintDestroy(bool status)
    {
        outline.enabled = status;
        isHintDestroy = status;
    }
}
