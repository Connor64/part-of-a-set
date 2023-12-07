using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemQuantity {
    public Item item;
    public int quantity;
}

[CreateAssetMenu]
public class Recipe : ScriptableObject {
    public List<ItemQuantity> ingredients;
    public Item product;
}
