using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using System;

public class TarooController : MonoBehaviour
{
    Animator animator;
    GameManager gameManager;
    [SerializeField] GameObject card;
    [SerializeField] GameObject credits;

    int neutralHash;
    int happyHash;

    private void OnEnable()
    {
        if(gameManager == null)
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
        }

        gameManager.OnEquationReady += OnEquationReady;
        gameManager.OnResponseSelected += OnResponseSelected;
        gameManager.OnEducationalBlocker += OnEducationalBlocker;
        gameManager.OnTimeCompleted += OnTimeCompleted;
        gameManager.OnShowCredits += OnShowCredits;
        gameManager.OnGameStarted += OnGameStarted;
    }

   

    private void OnDisable()
    {
        gameManager.OnEquationReady -= OnEquationReady;
        gameManager.OnResponseSelected -= OnResponseSelected;
        gameManager.OnEducationalBlocker -= OnEducationalBlocker;
        gameManager.OnTimeCompleted -= OnTimeCompleted;
        gameManager.OnShowCredits -= OnShowCredits;
    }

    private void OnGameStarted()
    {
        credits.transform.DOScale(Vector3.zero, .5f);
    }
    private void OnShowCredits()
    {
        animator?.SetTrigger(neutralHash);
        card.transform.DOScale(Vector3.zero, .5f).OnComplete(() =>
        {
            credits.transform.DOScale(Vector3.one, .5f);
        });
        
    }
    private void OnTimeCompleted()
    {
        animator?.SetTrigger(neutralHash);
        card.transform.DOScale(Vector3.one, .5f);
        
    }

    private void OnEducationalBlocker()
    {
        animator?.SetTrigger(neutralHash);
        card.transform.DOScale(Vector3.one, .5f);
    }

    private void OnResponseSelected(bool isRightAnswer)
    {
        card.transform.DOScale(Vector3.zero, .5f);
        if (isRightAnswer)
        {           
            animator?.SetTrigger(happyHash);
        }
        else
        {
            animator?.SetTrigger(neutralHash);
        }
    }

    private void OnEquationReady(string equation)
    {
        animator?.SetTrigger(neutralHash);
        card.transform.DOScale(Vector3.one, .5f).OnComplete(() =>
        {
            gameManager.ActivateFruits();
        });
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        neutralHash = Animator.StringToHash("Neutral");
        happyHash = Animator.StringToHash("Happy");

    }
}
