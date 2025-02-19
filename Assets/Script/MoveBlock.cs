
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

    private void Awake()
    {
        blockCollider = GetComponent<Collider>();
        bounds = blockCollider.bounds;
        outline = GetComponentInChildren<Outline>();
    }

    private void Start()
    {
        plane = new Plane(Vector3.up, Vector3.zero);
    }
    private void OnMouseDown()
    {
        if (block.blockType == BlockType.ICE) return;
        isDrag = true;
        rb = gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out var enter))
        {
            mousePosition = ray.GetPoint(enter);
            offsetMouseDownObject = transform.position - mousePosition;
        }
        outline.enabled = true;
    }
    private void FixedUpdate()
    {
        if (!isDrag) return;
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
            rb.velocity = (targetPos-transform.position) * moveSpeed;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
    }
    private void OnMouseUp()
    {
        isDrag = false;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Destroy(rb);
        }
        
        float halfWidth = bounds.size.x / 2f;
        float halfHeight = bounds.size.z / 2f;
        float snappedX = Mathf.Round((transform.position.x - halfWidth)) + halfWidth;
        float snappedZ = Mathf.Round((transform.position.z - halfHeight)) + halfHeight;

        transform.position = new Vector3(snappedX, 0, snappedZ);
        outline.enabled =  false;
    }
}
