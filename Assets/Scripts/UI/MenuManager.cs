using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    public GameObject pauseMenuContainer, pauseBackground;
    public AnimatedElement pausePanel;

    public GameObject winLoseContainer, winLoseBackground;
    public AnimatedElement winPanel, losePanel;

    private bool gameOver = false;

    // Start is called before the first frame update
    void Start() {
        pauseBackground.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePauseMenu();
        } if (Input.GetKeyDown(KeyCode.Space)) {
            ShowWinLoseScreen(true);
        } else if (Input.GetKeyDown(KeyCode.H)) {
            ShowWinLoseScreen(false);
        }
    }

    public void TogglePauseMenu() {
        if (gameOver) return;

        Time.timeScale = pauseBackground.activeInHierarchy ? 1 : 0; // Pause time if the menu is not active
        pauseBackground.SetActive(!pauseBackground.activeInHierarchy);
        pausePanel.TogglePosition(!pausePanel.isAnimating);
    }

    public void ShowWinLoseScreen(bool win) {
        gameOver = true;
        Time.timeScale = 0;
        winLoseBackground.SetActive(true);

        if (win) {
            winPanel.TogglePosition(true);
        } else {
            losePanel.TogglePosition(true);
        }
    }

    public void LoadNextLevel() {
        Time.timeScale = 1;
        FindObjectOfType<LevelManager>().LoadNextLevel();
        gameOver = false;
        winLoseBackground.SetActive(false);
        winPanel.SetPosition(false, AnimatedElement.AnimationElementPosition.START_POSITION);
        losePanel.SetPosition(false, AnimatedElement.AnimationElementPosition.START_POSITION);
    }

    public void ReloadLevel() {
        Time.timeScale = 1;
        FindObjectOfType<LevelManager>().ReloadLevel();
        gameOver = false;
        winLoseBackground.SetActive(false);
        winPanel.SetPosition(false, AnimatedElement.AnimationElementPosition.START_POSITION);
        losePanel.SetPosition(false, AnimatedElement.AnimationElementPosition.START_POSITION);
    }
}
