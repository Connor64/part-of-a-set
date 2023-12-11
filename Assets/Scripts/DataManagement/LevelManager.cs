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

    private static LevelManager instance;

    [SerializeField]
    private Level defaultLevel;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        levels = new List<Level>();
        UnityEngine.Object[] objects = Resources.LoadAll("Levels");
        foreach (UnityEngine.Object obj in objects) {
            print("Level name: " + ((Level)obj).levelName);
            levels.Add((Level) obj);
        }
    }

    void Start() {
        // If the game is started on the gameplay scene (mainly for debugging/development)
        if (SceneManager.GetActiveScene().name == "GameplayScene") {
            LoadLevel(defaultLevel == null ? levels[0] : defaultLevel, false);
        }
    }

    public void LoadLevel(Level level, bool loadScene) {

        currentLevel = level;

        // Load/reload gameplay scene
        if (loadScene) {
            SceneManager.LoadSceneAsync("GameplayScene");
        }

        StartCoroutine(LoadLevelContent(loadScene));
    }

    private IEnumerator LoadLevelContent(bool loadScene) {
        while (loadScene && (SceneManager.GetActiveScene().name != "GameplayScene")) {
            print("waiting...");
            yield return null;
        }

        AssemblyGrid assemblyGrid = FindObjectOfType<AssemblyGrid>();
        assemblyGrid.PopulateAssemblies(currentLevel.recipes);

        // Load recipes into itemManager
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        itemManager.Initialize(currentLevel);
    }
}
