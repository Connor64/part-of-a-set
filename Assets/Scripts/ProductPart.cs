using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ProductPart : MonoBehaviour {

    private Rigidbody2D rb;
    private CircleCollider2D circle;
    private Camera cam;
    private bool hovering = false;

    [Range(0.01f, 0.99f)]
    public float rotationSpeed = 0.2f;

    [Range(0.01f, 0.99f)]
    public float movementSpeed = 0.3f;

    public float velocityInheritence = 0.2f;
    

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        cam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void FixedUpdate() {
        if (hovering && Input.GetMouseButton(0)) {
            circle.enabled = false;
            rb.angularVelocity = 0;

            Vector2 oldPos = rb.position;

            // Move object to mouse position
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 smoothPos = (rb.position * (1 - movementSpeed)) + (mousePos * movementSpeed);
            rb.MovePosition(smoothPos);

            rb.velocity = velocityInheritence * (smoothPos - oldPos) / Time.fixedDeltaTime;

            // Interpolate rotation to a neutral position
            float target = (rb.rotation < 180) ? 0 : 360; // Choose rotation direction
            float smoothRot = (rb.rotation * (1 - rotationSpeed)) + (target * rotationSpeed);
            rb.MoveRotation(smoothRot);
        } else {
            circle.enabled = true;
        }
    }

    void OnMouseEnter() {
        if (!Input.GetMouseButton(0)) {
            hovering = true;
        }
    }

    void OnMouseExit() {
        if (!Input.GetMouseButton(0)) {
            hovering = false;
        }
    }

}
