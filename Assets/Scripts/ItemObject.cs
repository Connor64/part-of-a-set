using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D), typeof(SpriteRenderer))]
public class ItemObject : MonoBehaviour {

    private Rigidbody2D body;
    private PolygonCollider2D _collider;
    private SpriteRenderer spriteRenderer;

    private Camera cam;
    private bool selected = false;
    
    [SerializeField]
    private Item item;

    [SerializeField] [Range(0.01f, 0.99f)]
    private float rotationSpeed = 0.2f;

    [SerializeField] [Range(0.01f, 0.99f)]
    private float followSpeed = 0.3f;

    [SerializeField] [Min(0.01f)]
    private float velocityInheritence = 0.2f;


    // Start is called before the first frame update
    void Start() {
        body = GetComponent<Rigidbody2D>();
        _collider = gameObject.GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.sprite;

        cam = FindObjectOfType<Camera>();
    }

    public void Initialize(Item item) {
        this.item = item;
        spriteRenderer.sprite = this.item.sprite;
    }

    public Item GetItem() {
        return item;
    }

    // Update is called once per frame
    void Update() {
        
    }

    void FixedUpdate() {
        if (selected) {
            _collider.enabled = false;
            body.angularVelocity = 0;

            Vector2 oldPos = body.position;

            // Move object to mouse position
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 smoothPos = (body.position * (1 - followSpeed)) + (mousePos * followSpeed);
            body.MovePosition(smoothPos);

            body.velocity = velocityInheritence * (smoothPos - oldPos) / Time.fixedDeltaTime;

            // Interpolate rotation to a neutral position
            float target = (body.rotation < 180) ? 0 : 360; // Choose rotation direction
            float smoothRot = (body.rotation * (1 - rotationSpeed)) + (target * rotationSpeed);
            body.MoveRotation(smoothRot);
        } else {
            _collider.enabled = true;
        }
    }

    void OnMouseDown() {
        selected = MouseManager.Instance.SetPart(this);
    }

    void OnMouseUp() {
        if (selected) {
            selected = !MouseManager.Instance.SetPart(null);
        }
    }

    void OnMouseExit() {
        if (!Input.GetMouseButton(0) && selected) {
            selected = !MouseManager.Instance.SetPart(null);
        }
    }
}
