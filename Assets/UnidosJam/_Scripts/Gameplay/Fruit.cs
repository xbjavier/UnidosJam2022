using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using System;

public class Fruit : Interactable
{
    Animator animator;
    int cutHash;

    public SpriteRenderer responseSpriteRenderer;

    bool correctResponse = false;
    bool canInteract = true;

    public Sprite Check;
    public Sprite Cross;

    public TextMeshProUGUI responseText;

    List<Action> ResponseSelectedCompleted = new List<Action>();
    List<Action<bool>> ResponseSelected = new List<Action<bool>>();

    GameManager gameManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cutHash = Animator.StringToHash("Cut");
    }

    public override void Interact()
    {
        if (!canInteract) return;

        canInteract = false;
        animator.SetTrigger(cutHash);
        for (int i = 0; i < ResponseSelected.Count; i++)
        {
            ResponseSelected[i]?.Invoke(correctResponse);
        }
    }

    public void ShowResponse()
    {
        responseSpriteRenderer.gameObject.transform.localScale = new Vector3();

        if (correctResponse || !gameManager.RunningGame)
        {
            responseSpriteRenderer.sprite = Check;
        }
        else
        {
            responseSpriteRenderer.sprite = Cross;
        }

        responseSpriteRenderer.gameObject.SetActive(true);
        responseSpriteRenderer.gameObject.transform.DOScale(Vector3.one, 1.0f).OnComplete(() =>
        {
            responseSpriteRenderer.gameObject.transform.DOScale(Vector3.zero, 1.0f).OnComplete(() =>
            {
                for (int i = 0; i < ResponseSelectedCompleted.Count; i++)
                {
                    ResponseSelectedCompleted[i]?.Invoke();
                    animator.Play("Idle");
                    canInteract = true;
                }
            });
        });
    }

    public void SetupFruit(string responseText, bool isRightResponse)
    {
        correctResponse = isRightResponse;
        SetResponseText(responseText);
    }

    private void SetResponseText(string val)
    {
        responseText.text = val;
    }

    public void AddResponseSelectionStartedObserver(System.Action<bool> callback)
    {
        ResponseSelected.Add(callback);
    }

    public void AddResponseSelectionCompletedObserver(System.Action callback)
    {
        ResponseSelectedCompleted.Add(callback);
    }

    public void SetGameManager(GameManager manager)
    {
        gameManager = manager;
    }

    public void canBeSliced(bool state)
    {
        canInteract = state;
    }
}
