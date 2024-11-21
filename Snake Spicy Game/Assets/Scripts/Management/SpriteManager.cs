using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager instance;
    private SnakeController snakeController;
    void Awake()
    {
        instance = this; 
    }

    private void Start()
    {
        snakeController = GameObject.FindWithTag("Snake").GetComponent<SnakeController>();
    }

    public void ChangeHeadDirection(Vector2Int input)
    {
        if (input == Vector2Int.up)
        {
            snakeController.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0); 
        }
        else if (input == Vector2Int.down)
        {
            snakeController.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (input == Vector2Int.left)
        {
            snakeController.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (input == Vector2Int.right)
        {
            snakeController.gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
        }
    }
    
    
    
}
