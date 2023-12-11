using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelPanel : MonoBehaviour {

    private Level level;

    [SerializeField]
    private TextMeshProUGUI levelName, score;

    public void Initalize(Level level) {
        this.level = level;
        levelName.text = level.levelName;
    }

    public void LoadLevel() {
        FindObjectOfType<LevelManager>().LoadLevel(level, true);
    }
}
