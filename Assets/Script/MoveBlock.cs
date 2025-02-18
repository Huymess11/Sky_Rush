
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


    private void Start()
    {
        plane = new Plane(Vector3.up, Vector3.zero);
        outline = GetComponentInChildren<Outline>();
    }
    private void OnMouseDown()
    {
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
            transform.position = new Vector3(transform.position.x, 0.15f, transform.position.z);
            Vector3 targetPos = mousePosition + offsetMouseDownObject;

            switch (block.blockType)
            {
                case BlockType.HORIZONTAL:
                    targetPos.z = transform.position.z;
                    break;
                case BlockType.VERTICAL:
                    targetPos.x = transform.position.x;
                    break;
                case BlockType.ICE:
                    break;
            }
            rb.velocity = (targetPos-transform.position) * moveSpeed;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
    }
    private void OnMouseUp()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        //transform.position = new Vector3(Mathf.RoundToInt(transform.position.x) / transform.localScale.x, transform.position.y, Mathf.RoundToInt(transform.position.z) / transform.localScale.z);
        Destroy(rb);
        isDrag = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector3.zero;
        outline.enabled = false;
    }
}
