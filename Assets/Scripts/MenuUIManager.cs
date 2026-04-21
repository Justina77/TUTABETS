using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startMenuPanel;
    public GameObject endMenuPanel;

    [Header("Texts")]
    public TextMeshProUGUI startTitleText;
    public TextMeshProUGUI endTitleText;

    [Header("Gameplay")]
    public GameObject worldRoot;
    public GameManager gameManager;

    private void Start()
    {
        ShowStartMenu();
    }

    public void ShowStartMenu()
    {
        if (startTitleText != null)
            startTitleText.text = "Alfabēta spēle";

        if (startMenuPanel != null)
            startMenuPanel.SetActive(true);

        if (endMenuPanel != null)
            endMenuPanel.SetActive(false);

        if (worldRoot != null)
            worldRoot.SetActive(false);
    }

    public void StartGame()
    {
        if (startMenuPanel != null)
            startMenuPanel.SetActive(false);

        if (endMenuPanel != null)
            endMenuPanel.SetActive(false);

        if (worldRoot != null)
            worldRoot.SetActive(true);

        if (gameManager != null)
            gameManager.BeginGame();
    }

    public void ShowEndMenu()
    {
        if (endTitleText != null)
            endTitleText.text = "Spēle pabeigta!";

        if (startMenuPanel != null)
            startMenuPanel.SetActive(false);

        if (endMenuPanel != null)
            endMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}