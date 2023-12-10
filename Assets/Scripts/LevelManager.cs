using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public List<Level> levels { get; private set; }

    public Level currentLevel { get; private set; }

    [SerializeField]
    private Level defaultLevel;

    void Awake() {
        DontDestroyOnLoad(gameObject);

        levels = new List<Level>();
    }

    void Start() {
        levels = new List<Level>();

        UnityEngine.Object[] objects = Resources.LoadAll("Levels");
        foreach (UnityEngine.Object obj in objects) {
            print("Level name: " + ((Level)obj).levelName);
            levels.Add((Level) obj);
        }

        // If the game is started on the gameplay scene (mainly for debugging/development)
        if (SceneManager.GetActiveScene().name == "GameplayScene") {
            LoadLevel(defaultLevel == null ? levels[0] : defaultLevel, false);
        }
    }

    public void LoadLevel(Level level, bool loadScene) {

        currentLevel = level;

        // Load/reload gameplay scene
        if (loadScene) {
            Scene scene = SceneManager.GetSceneByName("GameplayScene");
            SceneManager.LoadScene(scene.name);
            while (!scene.isLoaded) { } // Wait for scene to fully load
        }

        AssemblyGrid assemblyGrid = FindObjectOfType<AssemblyGrid>();
        assemblyGrid.PopulateAssemblies(level.recipes);

        // Load recipes into itemManager
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        itemManager.recipes.Clear();
        foreach (RecipeCount recipeCount in level.recipes) {
            itemManager.recipes.Add(new KeyVal<Recipe, int>(recipeCount.recipe, 0));
        }
        itemManager.StartCoroutine("ItemSpawning");
    }
}
