using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Quiz : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI quesionText;
    [SerializeField] List<QuestionSO> questionSOs = new List<QuestionSO>();
    QuestionSO currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;
    int correctAnswerIndex;
    int currentAnswerIndex = -1;
    bool hasAnseredEarly = true;

    [Header("Button control")]
    [SerializeField] GameObject[] buttonControl = new GameObject[2];

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;
    [SerializeField] Sprite wrongAnswerSprite;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("ProgressBar")]
    [SerializeField] Slider progressBar;

    public bool isComplete;

    private void Awake()
    {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questionSOs.Count;
        progressBar.value = 0;
    }

    private void Update()
    {
        timerImage.fillAmount = timer.fillFaction;
        if (timer.loadNextQuestion)
        {

            if (progressBar.value == progressBar.maxValue)
            {
                isComplete = true;
                return;
            }
            hasAnseredEarly = false;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        }
        else if (!hasAnseredEarly && !timer.isAnsweringQuestion)
        {
            //DisplayAnswer();
        }
    }

    private void DisplayQuestion()
    {
        quesionText.text = currentQuestion.GetQuestion();
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.GetAnswer(i);
        }
    }

    public void OnAnswerSelected(int index)
    {
        currentAnswerIndex = index;
    }

    public void ConfirmAnswer()
    {
        if (currentAnswerIndex != -1)
        {
            hasAnseredEarly = true;
            timer.CancelTimer();
            if (currentAnswerIndex == currentQuestion.GetCorrectAnswerIndex())
            {
                answerButtons[currentAnswerIndex].GetComponent<Image>().sprite = correctAnswerSprite;
                scoreKeeper.IncrementCorrectAnswers();
            }
            else
            {
                answerButtons[currentAnswerIndex].GetComponent<Image>().sprite = wrongAnswerSprite;
                answerButtons[currentQuestion.GetCorrectAnswerIndex()].GetComponent<Image>().sprite = correctAnswerSprite;
            }
            scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";
            SetButtonState(false);
        }
    }

    public void SkipQuestion()
    {
        SetButtonState(false);
        timer.CancelTimer();
        hasAnseredEarly = true;
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";
    }

    void GetNextQuestion()
    {
        if (questionSOs.Count > 0)
        {
            SetButtonState(true);
            SetDefaultButtonSprites();
            GetRandomQuestion();
            DisplayQuestion();
            progressBar.value++;
            scoreKeeper.IncrementQuestionsSeen();
        }
    }

    private void GetRandomQuestion()
    {
        int index = Random.Range(0, questionSOs.Count);
        currentQuestion = questionSOs[index];
        if (questionSOs.Contains(currentQuestion))
            questionSOs.Remove(currentQuestion);
    }

    private void SetDefaultButtonSprites()
    {
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].GetComponent<Image>().sprite = defaultAnswerSprite;
    }

    void SetButtonState(bool state)
    {
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].GetComponent<Button>().interactable = state;
        for (int i = 0; i < buttonControl.Length; i++)
            buttonControl[i].GetComponent<Button>().interactable = state;
    }
}
