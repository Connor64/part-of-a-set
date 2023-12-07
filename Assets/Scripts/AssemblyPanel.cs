using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssemblyPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private bool mouseOver = false;
    private ItemObject hoveringPart = null;
    private List<KeyVal<Item, int>> items;

    public GameObject LabelPrefab;
    private List<TextMeshProUGUI> itemTexts;
    
    [SerializeField]
    private Recipe recipe;
    
    // Start is called before the first frame update
    void Start() {
        items = new List<KeyVal<Item, int>>();

        Initialize(recipe);
    }

    public void Initialize(Recipe recipe) {
        this.recipe = recipe;

        itemTexts = new List<TextMeshProUGUI>();
        
        foreach (ItemQuantity itemQuantity in recipe.ingredients) {
            items.Add(new KeyVal<Item, int>(itemQuantity.item, 0));

            // Instantiate label object and set to corresponding sprite
            GameObject label = Instantiate(LabelPrefab, transform);
            label.GetComponentInChildren<Image>().sprite = itemQuantity.item.sprite;

            // Set up text for label
            TextMeshProUGUI textMesh = label.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.text = 0 + "/" + itemQuantity.quantity;
            itemTexts.Add(textMesh);
        }
    }

    // Update is called once per frame
    void Update() {
        
        if (mouseOver) {
            if ((hoveringPart != null) && Input.GetMouseButtonUp(0)) {

                // Check if the item is part of the recipe
                for (int i = 0; i < items.Count; i++) {
                    if ((items[i].Key == hoveringPart.GetItem()) && (recipe.ingredients[i].quantity > items[i].Val)) {
                        items[i].Val++;
                        itemTexts[i].text = items[i].Val + "/" + recipe.ingredients[i].quantity;

                        hoveringPart = null;
                        // Destroy(hoveringPart.gameObject);

                        // TODO: Check if the recipe is complete
                        break;
                    }
                }
            }

            hoveringPart = MouseManager.Instance.GetPart();
        } else {
            hoveringPart = null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        print("YES hovering!! " + gameObject.name);
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        print("NOT hovering!! " + gameObject.name);
        mouseOver = false;
    }
}
