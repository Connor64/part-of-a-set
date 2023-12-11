using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour {
    private List<KeyVal<RecipeCount, int>> recipes;
    private List<TextMeshProUGUI> recipeLabels;

    public ItemObject itemPrefab;
    public GameObject itemLabelPrefab;
    public Vector2 launchForce, rejectForce;

    public GameObject recipeLabelContainer;

    private ItemObject heldPart;
    private LevelManager levelManager;

    [SerializeField] [Min(0)]
    private float minSpawnTime = 5, maxSpawnTime = 10;

    void Awake() {
        recipes = new List<KeyVal<RecipeCount, int>>();
        recipeLabels = new List<TextMeshProUGUI>();

        heldPart = null;
        levelManager = FindObjectOfType<LevelManager>();
    }

    public void Initialize(Level level) {
        recipes.Clear();
        recipeLabels.Clear();

        for (int i = recipeLabelContainer.transform.childCount - 1; i >= 0; i--) {
            Destroy(recipeLabelContainer.transform.GetChild(i).gameObject);
        }

        foreach (RecipeCount recipeCount in level.recipes) {
            recipes.Add(new KeyVal<RecipeCount, int>(recipeCount, 0));

            GameObject recipeLabel = Instantiate(itemLabelPrefab, recipeLabelContainer.transform);
            recipeLabel.GetComponentInChildren<Image>().sprite = recipeCount.recipe.product.sprite;

            TextMeshProUGUI recipeText = recipeLabel.GetComponentInChildren<TextMeshProUGUI>();
            recipeText.text = "0/" + recipeCount.amount;
            recipeLabels.Add(recipeText);
        }

        StartCoroutine("ItemSpawning");
    }

    public ItemObject GetSelectedItem() {
        return heldPart;
    }

    public bool SetSelectedItem(ItemObject part) {
        if ((heldPart != null) && (part != null) && (heldPart != part)) return false;

        heldPart = part;

        return true;
    }

    public IEnumerator ItemSpawning() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            // Choose an ingredient from one of the recipes to spawn
            int recipeIndex = Random.Range(0, recipes.Count);
            int itemIndex = Random.Range(0, recipes[recipeIndex].Key.recipe.ingredients.Count);

            // Spawn the item and initialize it
            SpawnItem(recipes[recipeIndex].Key.recipe.ingredients[itemIndex].item, transform.position, launchForce);
        }
    }

    public void SpawnItem(Item item, Vector2 pos, Vector2 launch) {
        ItemObject obj = Instantiate(itemPrefab);
        obj.Initialize(item, pos);
        if (launch != Vector2.zero) {
            obj.GetComponent<Rigidbody2D>().AddForce(launch, ForceMode2D.Impulse);
        }
    }

    public void DepositItem(ItemObject itemObject) {

        // Check if the item is a product of one of the recipes of the level
        foreach (KeyVal<RecipeCount, int> recipeCount in recipes) {
            if (recipeCount.Key.recipe.product == itemObject.GetItem()) {
                recipeCount.Val++;

                UpdateLabels();
                
                Destroy(itemObject.gameObject);
                return;
            }
        }

        // If this is reached, it's not a valid product
        StartCoroutine(itemObject.RejectItem(Vector2.zero));
    }

    public void UpdateLabels() {
        for (int i = 0; i < recipes.Count; i++) {
            recipeLabels[i].text = recipes[i].Val + "/" + recipes[i].Key.amount;
        }
    }
}
