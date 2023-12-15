using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemCount {
    public Item item;
    public int amount;
}

[CreateAssetMenu]
public class Recipe : ScriptableObject {
    public List<ItemCount> ingredients;
    public Item product;
}
