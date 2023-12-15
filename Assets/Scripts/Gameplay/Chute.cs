using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChuteType {
    TRASH,
    INGREDIENT
}

public class Chute : MonoBehaviour {

    public ChuteType chuteType;
    private ItemManager itemManager;

    // Start is called before the first frame update
    void Start() {
        itemManager = FindObjectOfType<ItemManager>();
    }

    public void AcceptItem(ItemObject itemObject) {
        if (chuteType == ChuteType.TRASH) {
            Destroy(itemObject.gameObject);
            return;
        }

        itemManager.AddIngredient(itemObject);
    }
}
