using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour {
    public List<KeyVal<Recipe, int>> recipes;
    public ItemObject itemPrefab;
    public Vector2 launchForce;

    private ItemObject heldPart;
    private LevelManager levelManager;

    [SerializeField] [Min(0)]
    private float minSpawnTime = 5, maxSpawnTime = 10;

    void Awake() {
        recipes = new List<KeyVal<Recipe, int>>();
        heldPart = null;
        levelManager = FindObjectOfType<LevelManager>();
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
            int itemIndex = Random.Range(0, recipes[recipeIndex].Key.ingredients.Count);

            // Spawn the item and initialize it
            SpawnItem(recipes[recipeIndex].Key.ingredients[itemIndex].item, transform.position, launchForce);
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
        foreach (KeyVal<Recipe, int> recipe in recipes) {
            if (recipe.Key.product == itemObject.GetItem()) {
                recipe.Val++;
                Destroy(itemObject.gameObject);
                return;
            }
        }

        // If this is reached, it's not a valid product
        StartCoroutine(itemObject.RejectItem(Vector2.zero));
    }
}
