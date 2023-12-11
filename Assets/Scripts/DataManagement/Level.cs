using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RecipeCount {
  public Recipe recipe;
  public int amount;
}

[CreateAssetMenu]
public class Level : ScriptableObject {
  public List<RecipeCount> recipes;
  public string levelName;
  [Min(1)]
  public int difficulty;
}
