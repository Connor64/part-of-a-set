using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour {
    private List<KeyVal<RecipeCount, int>> recipes;
    private List<KeyVal<Item, int>> items;
    private List<TextMeshProUGUI> recipeLabels;
    private List<TextMeshProUGUI> ingredientLabels;

    public ItemObject itemPrefab;
    public GameObject itemLabelPrefab;

    public Vector2 launchForce;
    public float rejectForce;

    public GameObject recipeLabelContainer;
    public GameObject ingredientLabelContainer;

    private ItemObject selectedItem;

    public SpriteRenderer currentRecipeIcon;
    private Recipe currentRecipe;

    public GameObject[] spawnLocations;

    [SerializeField]
    [Min(0)]
    private float minSpawnTime = 5, maxSpawnTime = 10;

    [SerializeField]
    [Min(0)]
    private float minRecipeTime = 100, maxRecipeTime = 200;

    void Awake() {
        recipes = new List<KeyVal<RecipeCount, int>>();
        items = new List<KeyVal<Item, int>>();
        recipeLabels = new List<TextMeshProUGUI>();
        ingredientLabels = new List<TextMeshProUGUI>();

        selectedItem = null;
    }

    public void Initialize(Level level) {
        recipes.Clear();
        items.Clear();
        recipeLabels.Clear();
        ingredientLabels.Clear();

        selectedItem = null;

        // Destroy any previous item objects on screen
        ItemObject[] objects = FindObjectsOfType<ItemObject>();
        foreach(ItemObject item in objects) {
            Destroy(item.gameObject);
        }

        // Destroy previous recipe quota labels (if any)
        for (int i = recipeLabelContainer.transform.childCount - 1; i >= 0; i--) {
            Destroy(recipeLabelContainer.transform.GetChild(i).gameObject);
        }

        // Populate arrays
        foreach (RecipeCount recipeCount in level.recipes) {
            recipes.Add(new KeyVal<RecipeCount, int>(recipeCount, 0));

            GameObject recipeLabel = Instantiate(itemLabelPrefab, recipeLabelContainer.transform);
            recipeLabel.GetComponentInChildren<Image>().sprite = recipeCount.recipe.product.sprite;
            recipeLabels.Add(recipeLabel.GetComponentInChildren<TextMeshProUGUI>());
        }

        ChangeRecipe();

        StopAllCoroutines();

        StartCoroutine(ItemSpawning());
        StartCoroutine(RecipeCycle());
    }

    public ItemObject GetSelectedItem() {
        return selectedItem;
    }

    public bool SetSelectedItem(ItemObject part) {
        if ((selectedItem != null) && (part != null) && (selectedItem != part)) return false;

        selectedItem = part;

        return true;
    }

    public IEnumerator ItemSpawning() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            // Choose an ingredient from one of the recipes to spawn
            int recipeIndex = Random.Range(0, recipes.Count);
            int itemIndex = Random.Range(0, recipes[recipeIndex].Key.recipe.ingredients.Count);

            // Spawn the item and initialize it
            SpawnItem(recipes[recipeIndex].Key.recipe.ingredients[itemIndex].item, launchForce);
        }
    }

    public void SpawnItem(Item item, Vector2 launch) {
        ItemObject obj = Instantiate(itemPrefab);
        int spawnIndex = Random.Range(0, spawnLocations.Length);
        obj.Initialize(item, spawnLocations[spawnIndex].transform.position);

        if (launch != Vector2.zero) {
            obj.GetComponent<Rigidbody2D>().AddForce(launch * (spawnIndex == 0 ? 1 : -1), ForceMode2D.Impulse);
        }
    }

    public void UpdateLabels() {
        for (int i = 0; i < recipes.Count; i++) {
            UpdateLabel(recipeLabels[i], recipes[i].Val, recipes[i].Key.amount);
        }

        for (int i = 0; i < items.Count; i++) {
            UpdateLabel(ingredientLabels[i], items[i].Val, currentRecipe.ingredients[i].amount);
        }
    }

    private void UpdateLabel(TextMeshProUGUI label, int numerator, int denominator) {
        label.color = (numerator >= denominator) ? Color.green : Color.white;
        label.text = numerator + "/" + denominator;
    }

    public bool AddIngredient(ItemObject itemObject) {
        Item ingredient = itemObject.GetItem();
        Destroy(itemObject.gameObject);

        for (int i = 0; i < items.Count; i++) {
            if (items[i].Key == ingredient) {
                items[i].Val++;

                UpdateLabels();

                CheckSpawnable();
                return true;
            }
        }

        foreach (KeyVal<Item, int> itemCount in items) {
            itemCount.Val = 0;
        }

        SpawnItem(LevelManager.trashItem, new Vector2(15, 0));

        UpdateLabels();

        return false;
    }

    public void SubmitItem(ItemObject itemObject) {
        Item item = itemObject.GetItem();
        Destroy(itemObject.gameObject);

        if (item == currentRecipe.product) {
            for (int i = 0; i < recipes.Count; i++) {
                if (recipes[i].Key.recipe == currentRecipe) {
                    recipes[i].Val++;
                    
                    UpdateLabels();

                    foreach (KeyVal<RecipeCount, int> recipeCount in recipes) {
                        if (recipeCount.Key.amount > recipeCount.Val) {
                            return;
                        }
                    }

                    // you won woo hoo yay kill me please
                    FindObjectOfType<MenuManager>().ShowWinLoseScreen(true);
                }
            }
        } else {
            print("nooo!!! you suck!!!!!!! loser!!!");
            // TODO: Add punishment for submitting incorrect product
        }
    }

    private void CheckSpawnable() {
        for (int i = 0; i < currentRecipe.ingredients.Count; i++) {
            if (currentRecipe.ingredients[i].amount > items[i].Val) {
                return; // Recipe cannot be crafted
            }
        }

        // If reached, recipe is craftable

        // Subtract necessary amount of ingredients
        for (int i = 0; i < currentRecipe.ingredients.Count; i++) {
            items[i].Val -= currentRecipe.ingredients[i].amount;
        }

        UpdateLabels();
        SpawnItem(currentRecipe.product, launchForce);
    }

    private IEnumerator RecipeCycle() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(minRecipeTime, maxRecipeTime));
            ChangeRecipe();
        }
    }

    private void ChangeRecipe() {
        if (recipes.Count == 1) {
            currentRecipe = recipes[0].Key.recipe;
        }

        int randomIndex = Random.Range(0, recipes.Count);

        while (recipes[randomIndex].Key.recipe == currentRecipe) {
            randomIndex = Random.Range(0, recipes.Count);
        }

        currentRecipe = recipes[randomIndex].Key.recipe;

        currentRecipeIcon.sprite = currentRecipe.product.sprite;

        items.Clear();
        ingredientLabels.Clear();

        // Destroy previous ingredient labels (if any)
        for (int i = ingredientLabelContainer.transform.childCount - 1; i >= 0; i--) {
            Destroy(ingredientLabelContainer.transform.GetChild(i).gameObject);
        }

        foreach (ItemCount itemCount in currentRecipe.ingredients) {
            items.Add(new KeyVal<Item, int>(itemCount.item, 0));

            GameObject ingredientLabel = Instantiate(itemLabelPrefab, ingredientLabelContainer.transform);
            ingredientLabel.GetComponentInChildren<Image>().sprite = itemCount.item.sprite;
            ingredientLabels.Add(ingredientLabel.GetComponentInChildren<TextMeshProUGUI>());
        }

        UpdateLabels();
    }
}
