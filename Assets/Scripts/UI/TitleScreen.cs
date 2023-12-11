using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour {

    [SerializeField]
    private GameObject levelGrid, levelSelectPanel;

    [SerializeField]
    private GameObject levelIconPrefab;

    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start() {
        levelManager = FindObjectOfType<LevelManager>();

        foreach (Level level in levelManager.levels) {
            GameObject obj = Instantiate(levelIconPrefab, levelGrid.transform);
            obj.GetComponent<LevelPanel>().Initalize(level);
        }
        
    }

    // Update is called once per frame
    void Update() {

    }
}
