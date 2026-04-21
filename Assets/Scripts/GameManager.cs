using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game References")]
    public LetterData[] letters;
    public ObjectSpawner spawner;
    public LetterBox letterBox;
    public LetterDisplay display;

    public float nextRoundDelay = 0.7f;

    [Header("Audio Setup")]
    public AudioSource audioSource;
    public AudioClip wrongSound;
    public AudioClip correctSound;

    [Header("Box Visuals")]
    public Renderer boxRenderer; // Drag your letter box - cube here in the Inspector
    public Color defaultColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public float glowIntensity = 2.0f; // Adjust this if the glow is too bright or too dim

    private int currentIndex = 0;
    private bool roundFinished = false;
    private LetterData currentLetter;

    void Start()
    {
        if (letters == null || letters.Length == 0)
        {
            Debug.LogError("Masīvā letters trūkst datu.");
            return;
        }

        currentIndex = 0;
        StartRound();
    }

    public void StartRound()
    {
        if (currentIndex >= letters.Length)
        {
            Debug.Log("Spēle beigusies.");
            display.ShowFinished();
            spawner.ClearObjects();
            return;
        }

        roundFinished = false;
        currentLetter = letters[currentIndex];

        // Reset the box color to white at the start of a new round
        ChangeBoxColor(defaultColor);

        display.ShowLetter(currentLetter);

        spawner.SpawnObjects(
        currentLetter.correctObject,
        letters
        );

        letterBox.SetCorrectItem(currentLetter.correctItemId);

        Debug.Log("Pašreizējais burts: " + currentLetter.letter + " | Pareizais ID: " + currentLetter.correctItemId);
    }

    public void CorrectAnswer()
    {
        if (roundFinished) return;

        roundFinished = true;
        Debug.Log("Pareizi!");

        // 1. Play Correct Sound
        if (audioSource != null && correctSound != null)
        {
            audioSource.PlayOneShot(correctSound);
        }

        // 2. Change Box Color to Green
        ChangeBoxColor(correctColor);

        currentIndex++;
        Invoke(nameof(StartRound), nextRoundDelay);
    }

    public void WrongAnswer()
    {
        if (roundFinished) return;

        Debug.Log("Nepareizais! Kaut kas cits, te nav pareizais.");

        // 1. Play Wrong Sound
        if (audioSource != null && wrongSound != null)
        {
            audioSource.PlayOneShot(wrongSound);
        }

        // 2. Change Box Color to Red
        ChangeBoxColor(wrongColor);
    }

    // --- HELPER METHOD TO CHANGE THE GLOW COLOR ---
    private void ChangeBoxColor(Color color)
    {
        if (boxRenderer != null)
        {
            // Get the material of the box
            Material boxMat = boxRenderer.material;

            // Ensure Unity knows the Emission property is active
            boxMat.EnableKeyword("_EMISSION");

            // Set the color, multiplied by the intensity so it glows brightly
            boxMat.SetColor("_EmissionColor", color * glowIntensity);
        }
    }
}