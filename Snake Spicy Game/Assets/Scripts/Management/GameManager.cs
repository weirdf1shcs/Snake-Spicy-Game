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
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
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

    public bool ObjectsOnFloor()
    {
        foreach (GameObject tempObject in objects)
        {
            bool lackOfFloor = true;
            Collider2D[] colliders = Physics2D.OverlapPointAll(tempObject.gameObject.transform.position);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Floor"))
                {
                    lackOfFloor = false;
                    break; 
                }
            }
            if (lackOfFloor)
            {
                return false;
            }
        }
        
        return true;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        snakeController.canMove = false;
        successUI.SetActive(true);
    }

    public void GoBackwards()
    {
        snakeController.canMove = false;
        StartCoroutine(GoBackwardsUntil());
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        snakeController.canMove = false;
        failureText.text = failureReason;
        failureUI.SetActive(true);
        //Show the player that they have failed the level, and offer to restart. Optionally, show a reason.
    }
}
