using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Transform> segments = new List<Transform>();
    private SnakeController snakeController;
    void Awake()
    {
        instance = this; 
    }

    void Start()
    {
        snakeController = GameObject.FindWithTag("Snake").GetComponent<SnakeController>();
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
                yield return new WaitForSeconds(.2f);
                if (!snakeController.InBounds())
                {
                    ableToGoBackwards = false;
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

    void FailureState()
    {
        
    }
}
