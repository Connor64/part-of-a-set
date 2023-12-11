using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class ItemObject : MonoBehaviour {

    private Rigidbody2D body;
    private PolygonCollider2D _collider;
    private SpriteRenderer spriteRenderer;

    private float defaultGravity;

    private ItemManager itemManager;

    private Camera cam;
    private bool selected = false;

    private bool inTrigger = false;

    [SerializeField]
    private Item item;

    [SerializeField]
    [Range(0.01f, 0.99f)]
    private float followSpeed = 0.25f;

    [SerializeField]
    [Range(0, 1)]
    private float velocityInheritence = 0.5f;

    void Awake() {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemManager = FindObjectOfType<ItemManager>();
        cam = FindObjectOfType<Camera>();

        body.mass = item.mass;
        defaultGravity = body.gravityScale;

        spriteRenderer.sprite = item.sprite;
        _collider = gameObject.AddComponent<PolygonCollider2D>();
    }

    public void Initialize(Item item, Vector2 pos) {
        this.item = item;

        body.mass = item.mass;
        transform.position = pos;
        spriteRenderer.sprite = item.sprite;

        if ((_collider = GetComponent<PolygonCollider2D>()) != null) {
            Destroy(_collider);
        }

        _collider = gameObject.AddComponent<PolygonCollider2D>();
    }

    public Item GetItem() {
        return item;
    }

    void FixedUpdate() {
        if (selected) {

            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.gravityScale = 0;

            // Move object to mouse position
            Vector2 oldPos = body.position;
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 smoothPos = (oldPos * (1 - followSpeed)) + (mousePos * followSpeed);

            // Adds back in proper collisions (maybe please god)
            RaycastHit2D hit = Physics2D.Raycast(oldPos, (smoothPos - oldPos).normalized, (smoothPos - oldPos).magnitude, 1 << 6);
            if (hit.collider != null) {
                smoothPos = hit.point;
            }

            body.MovePosition(smoothPos);
            body.velocity = velocityInheritence * (smoothPos - oldPos) / Time.fixedDeltaTime;
        } else {
            body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            body.gravityScale = defaultGravity;
        }
    }

    void OnMouseDown() {
        selected = itemManager.SetSelectedItem(this);
    }

    void OnMouseUp() {
        if (selected) {
            selected = !itemManager.SetSelectedItem(null);
        }
    }

    void OnMouseExit() {
        if (!Input.GetMouseButton(0) && selected) {
            selected = !itemManager.SetSelectedItem(null);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (!selected && (other.gameObject.name == "DeathPlane")) {
            Destroy(gameObject);
        } else if ((other.gameObject.tag == "Assembly") || (other.gameObject.tag == "Deposit")) {
            inTrigger = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Assembly") {
            selected = false;
            itemManager.SetSelectedItem(null);
            other.GetComponentInParent<AssemblyPanel>().AddIngredient(this);
            inTrigger = true;
        } else if (other.gameObject.tag == "Deposit") {
            selected = false;
            itemManager.SetSelectedItem(null);
            itemManager.DepositItem(this);
            inTrigger = true;
        }
    }

    public IEnumerator RejectItem(Vector2 boost) {
        yield return new WaitForSeconds(0.25f);

        float forceX = itemManager.rejectForce.x * body.mass;
        Vector2 force = (new Vector2(0, itemManager.rejectForce.y) + boost) * body.mass;

        while (inTrigger) {
            force.x = Random.Range(-forceX, forceX);
            body.AddForce(force, ForceMode2D.Impulse);
            yield return null;
        }
    }
}
