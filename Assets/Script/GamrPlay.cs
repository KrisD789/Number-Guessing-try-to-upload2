using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GamrPlay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI attempts;
    public TextMeshProUGUI currentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;

    [Header("Input")]
    public TMP_InputField guessInputField;
    public Button submitButton;
    public Button newGameButton;

    [Header("Game Setting")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttemps = 12;

    private int targetNumber;
    private int currentAttemps;
    private bool isPlayerTurn;
    private bool gameActive;

    void InitializedUI() //ผูกปุ่มเข้ากับฟังชั่น
    {
        submitButton.onClick.AddListener(SubmitGuess);
        newGameButton.onClick.AddListener(StartNewGame);
        guessInputField.onSubmit.AddListener(delegate { SubmitGuess(); });

    }

    void SubmitGuess()
    {
        if (!gameActive || !isPlayerTurn) return;

        string input = guessInputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        int guess;

        if (!int.TryParse(input, out guess))
        {
            gameState.text = "Plese enter a valid number";
            return;
        }
        if (guess < minNumber ||  guess > maxNumber)
        {
            gameState.text = $"Please enter a number between {minNumber} - {maxNumber}";
            return;
        }
        ProcessGuess(guess, true);
        guessInputField.text = " ";

    }

    IEnumerator computerTurn(bool targetIsHigher)
    {
        yield return new WaitForSeconds(2f); //Wait for simulate thinking
        
        if (!gameActive) yield break;

        int computerGuess = Random.Range(minNumber, maxNumber + 1);
        ProcessGuess(computerGuess, false);
    }
    void EndGame()
    {
        gameActive = false;
        guessInputField.interactable = false;   
        submitButton.interactable = false;
        currentPlayer.text = " ";
        gameState.text += "GameOver - Click New Game to Start again";
        Canvas.ForceUpdateCanvases();
    }

    void ProcessGuess(int guess, bool isPlayerTurn)
    {
        currentAttemps++;
        string playerName = isPlayerTurn ? "Player" : "Computer";

        gameLog.text += $"{playerName} guess: {guess}\n";

        if (guess == targetNumber)
        {
            //Win
            gameLog.text += $"{playerName} got it right!!\n";
            EndGame();
        }

        else if (currentAttemps >= maxAttemps) 
        {
            //Lose
            gameLog.text += $"GameOver!! The correct number was {targetNumber}\n";
            EndGame();
        }
        else
        {
            //Wrong guess - Give hint
            string hint = guess < targetNumber ? "Too Low" : "Too High";
            gameLog.text += $"{hint}\n";

            //switch player turn
            isPlayerTurn = !isPlayerTurn;
            currentPlayer.text = isPlayerTurn ? "Player" : "Computer";
            attempts.text = $"Attemps Left: {maxAttemps - currentAttemps}";

            if (!isPlayerTurn) 
            { 
                guessInputField.interactable = false;
                submitButton.interactable = false;
                StartCoroutine(computerTurn(guess < targetNumber));
            }
            else
            {
                guessInputField.interactable = true;
                submitButton.interactable = true;
                guessInputField.text = " ";
                guessInputField.Select();
                guessInputField.ActivateInputField();
            }
        }
    }

    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttemps = 0;
        isPlayerTurn = true;
        gameActive = true;

        currentPlayer.text = "Player Turn";
        attempts.text = $"Attemps left: {maxAttemps}";
        gameLog.text = "=== Game Log ===\n";
        gameState.text = "NewGame started!! Player goes first";

        guessInputField.interactable = true;
        submitButton.interactable = true;
        guessInputField.text = " ";
        guessInputField.Select();
        guessInputField.ActivateInputField();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializedUI();
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
