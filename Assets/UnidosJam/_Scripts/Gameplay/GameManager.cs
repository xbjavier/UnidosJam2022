using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public delegate void EquationReady(string equation);
    public event EquationReady OnEquationReady;

    public delegate void ResponseSelected(bool isRightAnswer);
    public event ResponseSelected OnResponseSelected;

    public delegate void EducationalBlocker();
    public event EducationalBlocker OnEducationalBlocker;

    public delegate void GameStarted();
    public event GameStarted OnGameStarted;

    public delegate void ShowCredits();
    public event ShowCredits OnShowCredits;

    public delegate void TimeComplete();
    public event TimeComplete OnTimeCompleted;

    public NumbWordsSO numbers;

    public int minNumber = 0;
    public int maxNumber = 51;
    public float responseTimeInseconds = 60.0f;

    int result;

    public TMP_Text equationText;
    public TextMeshProUGUI timer;

    public Fruit[] fruits;

    Dictionary<char, string> operations = new Dictionary<char, string>();

    IEnumerator timerRoutine;
    WaitForSeconds waitForSeconds = new WaitForSeconds(1);

    int rightResponses = 0;
    int wrongResponses = 0;

    bool isEducationalBlocker = true;
    int educationalBlockerCount = 0;

    string currentTime;
    bool pauseTimer = false;
    bool gameStarted = false;
    public bool RunningGame => gameStarted;
    private void Awake()
    {
        for(int i = 0; i < fruits.Length; i++)
        {
            fruits[i].AddResponseSelectionStartedObserver((isRightResponse) =>
            {
                PauseTimer();
                for(int i = 0; i < fruits.Length; i++)
                {
                    fruits[i].canBeSliced(false);
                }

                OnResponseSelected?.Invoke(isRightResponse);      
                if(isRightResponse)
                {
                    if (!gameStarted)
                    {
                        StartNewGame();
                        return;
                    }

                    if (isEducationalBlocker)
                    {
                        isEducationalBlocker = false;
                        StartCoroutine(StartTimer());
                    }

                    rightResponses++;
                }
                else
                {
                    if (!gameStarted)
                    {
                        DisplayCredits();
                        return;
                    }

                    if(isEducationalBlocker)
                    {
                        wrongResponses++;
                        educationalBlockerCount++;
                        return;
                    }

                    wrongResponses++;
                }
            });

            fruits[i].AddResponseSelectionCompletedObserver(() =>
            {
                if(gameStarted)
                    TriggerNewResponses();
            });

            fruits[i].SetGameManager(this);
        }

       
    }
    private void Start()
    {
        operations.Add('+', "sum");
        operations.Add('-', "substraction");
        SetStartSetupFruits();
        DisplayCredits();
    }

    public void StartNewGame()
    {
        OnGameStarted?.Invoke();
        isEducationalBlocker = true;
        rightResponses = 0;
        wrongResponses = 0;
        educationalBlockerCount = 0;
        gameStarted = true;
        StartCoroutine(EducationBlockerStarter());
    }

    public void DisplayCredits()
    {      
        OnShowCredits?.Invoke();
    }

    void PauseTimer()
    {
        pauseTimer = true;
    }

    protected virtual void GenerateResponses()
    {
        //select one of the fruits to be the correct one
        int randomCorrectFruit = Random.Range(0, fruits.Length);
        int randomLanguage = Random.Range(0, 2);

        string equation = string.Empty;
        equationText.text = equation;

        NumbWord r1 = GenerateNumbword(out equation);
        

        equationText.text = equation;
        OnEquationReady?.Invoke(equation);

        fruits[randomCorrectFruit].SetupFruit(r1.words[randomLanguage].word, true);

        for (int i = 0; i < fruits.Length; i++)
        {
            if (i == randomCorrectFruit) continue;
            string tmp;
            NumbWord r2 = GenerateNumbword(out tmp, true);
            while (r2.number == r1.number)
            {
                r2 = GenerateNumbword(out tmp, true);
            }

            fruits[i].SetupFruit(r2.words[randomLanguage].word, false);
        }

        if(pauseTimer)
            pauseTimer = false;
    }

    protected NumbWord GenerateNumbword(out string equation, bool ignoreEducationBlocker = false)
    {
        int firstNumber;
        int secondNumber;

        if (isEducationalBlocker && !ignoreEducationBlocker)
        {
            firstNumber = 1;
            secondNumber = 1;
        }
        else
        {
            firstNumber = Random.Range(minNumber, maxNumber);
            secondNumber = Random.Range(minNumber, maxNumber - firstNumber);
        }

        char randomOperation = isEducationalBlocker ? '+' : operations.ElementAt(Random.Range(0, operations.Count)).Key;
        equation = string.Empty;
        foreach (var key in operations.Keys)
        {
            if (key == randomOperation)
            {
                if (key == '+')
                {
                    result = firstNumber + secondNumber;
                    equation = $"{firstNumber}+{secondNumber}";
                }
                else if (key == '-')
                {
                    if (firstNumber > secondNumber)
                    {
                        result = firstNumber - secondNumber;
                        equation = $"{firstNumber}-{secondNumber}";
                    }

                    else if (secondNumber > firstNumber)
                    {
                        result = secondNumber - firstNumber;
                        equation = $"{secondNumber}-{firstNumber}";
                    }

                }
                break;
            }
        }

        NumbWord numbword = GetResultString(result);
        return numbword;
    }

    protected virtual NumbWord GetResultString(int result)
    {
        NumbWord response = null;
        if (result <= 20)
        {
            response = numbers.list[result];
        }
        else if (result == 100)
        {
            response = numbers.list[numbers.list.Count - 1];
        }

        //if number less than 100
        else if (result < 100)
        {
            //convert to decimal
            int decimalPart = (result / 10) * 10;
            int unitPart = result % 10;

            string decimalStringES = string.Empty;
            string decimalStringEN = string.Empty;
            for (int i = 20; i < numbers.list.Count; i++)
            {
                if (numbers.list[i].number == decimalPart)
                {
                    decimalStringES = numbers.list[i].words[0].word; //0 is spanish
                    decimalStringEN = numbers.list[i].words[1].word; // 1 is english, no time for fancy stuff is a jam
                }
            }

            NumbWord unitTemp = numbers.list[unitPart];

            response = new NumbWord();
            response.number = result;
            response.words = new List<LanguageWord>();

            response.words.Add(new LanguageWord()
            {
                language = "es",
                word = string.Empty
            });

            response.words.Add(new LanguageWord()
            {
                language = "en",
                word = string.Empty
            }); ; ;

            if (unitTemp.number == 0)
            {
                response.words[0].word = decimalStringES.Contains("veinte") ? $"veinte" : $"{decimalStringES}";
                response.words[1].word = $"{decimalStringEN}";
            }
            else
            {
                response.words[0].word = decimalStringES.Contains("veinte") ? $"veinti{unitTemp.words[0].word}" : $"{decimalStringES}  y {unitTemp.words[0].word}";
                response.words[1].word = $"{decimalStringEN} {unitTemp.words[1].word}";
            }


        }
        return response;
    }

    [ContextMenu("Trigger New Responses")]
    public void TriggerNewResponses()
    {
      
        GenerateResponses();
      
    }


    public void ActivateFruits()
    {
        for (int i = 0; i < fruits.Length; i++)
        {
            fruits[i].canBeSliced(true);
        }
    }

    protected IEnumerator StartTimer()
    {
        float actualTime = responseTimeInseconds;
        currentTime = string.Empty;
        while(actualTime > 0.0f)
        {        
            if(pauseTimer)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                currentTime = (((int)actualTime / 60).ToString("00") + ":" + (((int)actualTime) % 60).ToString("00"));
                timer.text = currentTime;
                actualTime -= 1;
                yield return waitForSeconds;
            }
            
        }

        actualTime = 0.0f;
        timer.text = "00:00";
        TimeCompleted();  
        
    }

    void TimeCompleted()
    {
        OnTimeCompleted?.Invoke();
        equationText.text = $"Good Job! Muy bien! You got {rightResponses} correct!";
        gameStarted = false;
        SetStartSetupFruits();
    }

    void SetStartSetupFruits()
    {
        fruits[0].SetupFruit("New Game", true);
        fruits[1].SetupFruit("Credits", false);
    }


    protected void StopTimer()
    {
        if (timerRoutine == null) return;
        StopCoroutine(timerRoutine);
        timerRoutine = null;
    }

    protected IEnumerator EducationBlockerStarter()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        if (educationalBlockerCount == 0)
            equationText.text = $"Can you solve this?";
        else
            equationText.text = $"Try again :D";

        OnEducationalBlocker?.Invoke();
        yield return new WaitForSecondsRealtime(5.0f);
        GenerateResponses();

        yield return null;
    }
}
