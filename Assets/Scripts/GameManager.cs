using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LetterData[] letters;
    public ObjectSpawner spawner;
    public LetterBox letterBox;
    public LetterDisplay display;

    LetterData currentLetter;

    void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        currentLetter = letters[Random.Range(0, letters.Length)];

        display.ShowLetter(currentLetter.letter);

        spawner.SpawnObjects(
            currentLetter.correctObject,
            currentLetter.wrongObjects
        );

        letterBox.SetCorrectObject(currentLetter.correctObject.name);
    }

    public void CorrectAnswer()
    {
        Debug.Log("Correct!");
        Invoke(nameof(StartRound), 2f);
    }

    public void WrongAnswer()
    {
        Debug.Log("Wrong!");
    }
}