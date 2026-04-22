using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game References")]
    public LetterData[] letters;
    public ObjectSpawner spawner;
    public LetterBox letterBox;
    public LetterDisplay display;
    public MenuUIManager menuUI;

    public float nextRoundDelay = 0.7f;

    [Header("Character")]
    public PointToPointWalker character;

    [Header("Audio Setup")]
    public AudioSource audioSource;
    public AudioClip wrongSound;
    public AudioClip correctSound;

    [Header("Box Visuals")]
    public Renderer boxRenderer;
    public Color defaultColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public float glowIntensity = 2.0f;

    [Header("Optional")]
    public float wrongColorResetDelay = 0.5f;

    private int currentIndex = 0;
    private bool roundFinished = false;
    private bool gameRunning = false;
    private LetterData currentLetter;

    public bool IsGameRunning => gameRunning;

    private void Start()
    {
        if (letters == null || letters.Length == 0)
        {
            Debug.LogError("Masīvā letters trūkst datu.");
            return;
        }

        CancelInvoke();
        gameRunning = false;
        roundFinished = false;
        currentIndex = 0;

        if (spawner != null)
            spawner.ClearObjects();

        if (display != null)
            display.gameObject.SetActive(false);

        ChangeBoxColor(defaultColor);
    }

    public void BeginGame()
    {
        if (letters == null || letters.Length == 0)
        {
            Debug.LogError("Masīvā letters trūkst datu.");
            return;
        }

        CancelInvoke();

        currentIndex = 0;
        roundFinished = false;
        gameRunning = true;

        if (spawner != null)
            spawner.ClearObjects();

        if (display != null)
            display.gameObject.SetActive(true);

        ChangeBoxColor(defaultColor);

        StartRound();
    }

    public void StartRound()
    {
        if (!gameRunning) return;

        if (currentIndex >= letters.Length)
        {
            FinishGame();
            return;
        }

        roundFinished = false;
        currentLetter = letters[currentIndex];

        ChangeBoxColor(defaultColor);

        if (display != null)
            display.ShowLetter(currentLetter);

        if (spawner != null)
        {
            spawner.SpawnObjects(
                currentLetter.correctObject,
                letters
            );
        }

        if (letterBox != null)
            letterBox.SetCorrectItem(currentLetter.correctItemId);

        Debug.Log("Pašreizējais burts: " + currentLetter.letter + " | Pareizais ID: " + currentLetter.correctItemId);
    }

    public void CorrectAnswer()
    {
        if (!gameRunning) return;
        if (roundFinished) return;

        roundFinished = true;
        Debug.Log("Pareizi!");

        if (audioSource != null && correctSound != null)
        {
            audioSource.PlayOneShot(correctSound);
        }

        ChangeBoxColor(correctColor);

        if (character != null)
        {
            Debug.Log("Calling PlayDance on character: " + character.name);
            character.PlayDance();
        }
        else
        {
            Debug.LogError("Character is NULL in GameManager");
        }

        currentIndex++;
        Invoke(nameof(StartRound), nextRoundDelay);
    }

    public void WrongAnswer()
    {
        if (!gameRunning) return;
        if (roundFinished) return;

        Debug.Log("Nepareizais! Kaut kas cits, te nav pareizais.");

        if (audioSource != null && wrongSound != null)
        {
            audioSource.PlayOneShot(wrongSound);
        }

        ChangeBoxColor(wrongColor);

        if (character != null)
        {
            character.PlaySad();
        }

        CancelInvoke(nameof(ResetBoxColorAfterWrong));
        Invoke(nameof(ResetBoxColorAfterWrong), wrongColorResetDelay);
    }

    private void ResetBoxColorAfterWrong()
    {
        if (!gameRunning) return;
        if (roundFinished) return;

        ChangeBoxColor(defaultColor);
    }

    private void FinishGame()
    {
        gameRunning = false;
        roundFinished = true;

        CancelInvoke();

        if (spawner != null)
            spawner.ClearObjects();

        if (display != null)
            display.gameObject.SetActive(false);

        ChangeBoxColor(defaultColor);

        if (menuUI != null)
            menuUI.ShowEndMenu();
        else
            Debug.Log("Spēle beigusies.");
    }

    private void ChangeBoxColor(Color color)
    {
        if (boxRenderer == null) return;

        Material boxMat = boxRenderer.material;
        boxMat.EnableKeyword("_EMISSION");
        boxMat.SetColor("_EmissionColor", color * glowIntensity);
    }
}