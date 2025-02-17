
using UnityEngine;

public class MoveBlock : MonoBehaviour
{
    public float moveSpeed = 20f;
    private Vector3 mousePosition;
    private Plane plane;
    private Rigidbody rb;
    private Vector3 offsetMouseDownObject;
    private Outline outline;
    bool isDrag;

    private void Start()
    {
        plane = new Plane(Vector3.up, Vector3.zero);
        outline = GetComponent<Outline>();
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
            transform.parent.position = new Vector3(transform.parent.position.x, 0.25f, transform.parent.position.z);
            rb.velocity = (mousePosition + offsetMouseDownObject - transform.position) * moveSpeed ;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
    }
    private void OnMouseUp()
    {
        transform.parent.position = new Vector3(transform.parent.position.x, 0, transform.parent.position.z);
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x) / transform.localScale.x, transform.position.y, Mathf.RoundToInt(transform.position.z) / transform.localScale.z);
        Destroy(rb);
        isDrag = false;
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector3.zero;
        outline.enabled = false;
    }
}
