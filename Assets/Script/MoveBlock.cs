using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveBlock : MonoBehaviour
{
    public float moveSpeed = 20f;
    private Vector3 mousePosition;
    private Plane plane;
    private Rigidbody rb;
    bool isDrag;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        plane = new Plane(Vector3.up, Vector3.zero);
    }
    private void OnMouseDown()
    {
        isDrag = true;
    }
    private void FixedUpdate()
    {
        if (!isDrag) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out var enter))
        {
            mousePosition = ray.GetPoint(enter);
            mousePosition += new Vector3(0f,1f,0f);
            rb.velocity = (mousePosition - transform.position).normalized * moveSpeed * Time.fixedDeltaTime;
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
    private void OnMouseUp()
    {
        isDrag = false;
        transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector3.zero;

    }
}
