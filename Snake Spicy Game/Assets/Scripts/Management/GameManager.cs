using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Transform> segments = new List<Transform>();
    private SnakeController snakeController;
    public List<GameObject> objects = new List<GameObject>();
    [SerializeField] private Sprite finishHoleSprite;
    public bool ExitOpen = false;
    public GameObject failureUI;
    public GameObject successUI;
    [SerializeField] TextMeshProUGUI failureText;
    public String failureReason;
    void Awake()
    {
        instance = this; 
    }

    void Start()
    {
        snakeController = GameObject.FindWithTag("Snake").GetComponent<SnakeController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CheckForCompletion()
    {
        if (objects.Count == 0)
        {
            GameObject.FindWithTag("Hole").GetComponent<SpriteRenderer>().sprite = finishHoleSprite;
            ExitOpen = true;
        }
    }

    public void FinishLevel()
    {
        snakeController.canMove = false;
        successUI.SetActive(true);
        Debug.Log("Wow. You did it!");
    }

    public void GoBackwards()
    {
        snakeController.canMove = false;
        StartCoroutine(GoBackwardsUntil());
    }
    
    private IEnumerator GoBackwardsUntil()
    {
        yield return new WaitForSeconds(.1f);
        bool ableToGoBackwards = true;
        while (ableToGoBackwards)
        {
            if (snakeController.CanMoveBackwards())
            {
                snakeController.MoveBackwards();
                yield return new WaitForSeconds(.1f);
                if (!snakeController.InBounds())
                {
                    ableToGoBackwards = false;
                    failureReason = "The snake is flying forever!";
                    FailureState();
                }
            }
            else
            {
                ableToGoBackwards = false;
                snakeController.canMove = true;
            }
        }
        yield return null;
    }

    public void FailureState()
    {
        snakeController.canMove = false;
        failureText.text = failureReason;
        failureUI.SetActive(true);
        //Show the player that they have failed the level, and offer to restart. Optionally, show a reason.
    }
}
