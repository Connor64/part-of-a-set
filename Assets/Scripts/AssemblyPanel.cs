using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssemblyPanel : MonoBehaviour {
    private List<KeyVal<Item, int>> items;

    public GameObject LabelPrefab;
    public Transform spawnLocation;

    private List<TextMeshProUGUI> itemTexts;
    private ItemManager itemManager;

    [SerializeField]
    public Recipe recipe { get; private set; }

    public void Initialize(Recipe recipe) {
        this.recipe = recipe;
        itemManager = FindObjectOfType<ItemManager>();

        itemTexts = new List<TextMeshProUGUI>();
        items = new List<KeyVal<Item, int>>();

        for (int i = 0; i < recipe.ingredients.Count; i++) {
            items.Add(new KeyVal<Item, int>(recipe.ingredients[i].item, 0));

            // Instantiate label object and set to corresponding sprite
            GameObject label = Instantiate(LabelPrefab, transform);
            label.GetComponentInChildren<Image>().sprite = recipe.ingredients[i].item.sprite;

            // Set up text for label
            TextMeshProUGUI textMesh = label.GetComponentInChildren<TextMeshProUGUI>();
            itemTexts.Add(textMesh);
            RefreshLabel(i);
        }
    }

    public void AddIngredient(ItemObject itemObject) {
        for (int i = 0; i < items.Count; i++) {
            if ((items[i].Key == itemObject.GetItem()) && (recipe.ingredients[i].quantity > items[i].Val)) {
                items[i].Val++;
                Destroy(itemObject.gameObject);

                // Check which ingredients have been completed
                for (int j = 0; j < items.Count; j++) {
                    if (recipe.ingredients[j].quantity > items[j].Val) {
                        RefreshLabel(i);
                        return;
                    }
                }

                // If this is reached, the recipe is complete

                // Spawn product
                itemManager.SpawnItem(recipe.product, spawnLocation.position, Vector2.zero);

                // Reset the counts of all ingredients and update label
                RefreshLabels(true);

                return;
            }
        }

        // If this is reached, the item is not a valid ingredient or there is no more room for it
        StartCoroutine(itemObject.RejectItem(Vector2.zero));
    }

    private void RefreshLabels(bool reset) {
        for (int i = 0; i < items.Count; i++) {
            if (reset) items[i].Val = 0;
            RefreshLabel(i);
        }
    }

    private void RefreshLabel(int index) {
        itemTexts[index].text = items[index].Val + "/" + recipe.ingredients[index].quantity;
    }
}
