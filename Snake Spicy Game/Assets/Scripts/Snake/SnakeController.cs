using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SnakeController : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public int initialSize = 4;
    public List<Transform> segments = new List<Transform>();
    private Vector2Int input;
    public bool canMove = true;
    bool firstPush = false;

    private void Start()
    {
        StartUp();
    }

    private void Update()
    {
        input = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            input = Vector2Int.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            input = Vector2Int.down;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            input = Vector2Int.right;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            input = Vector2Int.left;
        }
        if (canMove)
        {
            if (input != Vector2Int.zero)
            {
                if (IsValidMove(input))
                {
                    SpriteManager.instance.ChangeHeadDirection(input);
                    direction = input;
                    MoveSnake();
                }
            }
        }
    }
    private void MoveSnake()
    {
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].position = segments[i - 1].position;
        }
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);
        if (!IsOnFloor() || !GameManager.instance.ObjectsOnFloor())
        {
            if (!GameManager.instance.ObjectsOnFloor())
            {
                GameManager.instance.failureReason = "An object fell into the void!";
            }
            else
            {
                GameManager.instance.failureReason = "The snake fell into the void!";
            }
            GameManager.instance.FailureState();
        }
    }
    
    public bool IsOnFloor()
    {
        foreach (Transform segment in segments)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(segment.position);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Floor"))
                {
                    return true;
                }
            }
        }
        if (!GameManager.instance.ObjectsOnFloor())
        {
            return false;
        }
        return false;
    }
    private bool IsValidMove(Vector2Int newDirection)
    {
        firstPush = false;
        int x = Mathf.RoundToInt(transform.position.x) + newDirection.x;
        int y = Mathf.RoundToInt(transform.position.y) + newDirection.y;
        Vector2 nextPosition = new Vector2(x, y);
        Vector3 tailPosition = segments[segments.Count - 1].position;
        foreach (Transform segment in segments)
        {
            if (segment.position != tailPosition &&
                Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return false;
            }
        }
        Collider2D[] colliders = Physics2D.OverlapPointAll(nextPosition);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return false;
            }
            if (collider.CompareTag("Object"))
            {
                HandleObjectCollision(collider.GetComponent<Object>());
                return false;
            }
        }

        return true;
    }
    public bool CanMoveBackwards()
{
    Vector2Int reverseDirection = -direction;

    // Check for any obstructions in the reverse direction for each snake segment
    foreach (Transform segment in segments)
    {
        int x = Mathf.RoundToInt(segment.position.x) + reverseDirection.x;
        int y = Mathf.RoundToInt(segment.position.y) + reverseDirection.y;
        Vector2 nextPosition = new Vector2(x, y);

        Collider2D[] colliders = Physics2D.OverlapPointAll(nextPosition);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall"))
            {
                return false; // Movement is blocked by a wall
            }

            if (collider.CompareTag("Object"))
            {
                Object obj = collider.GetComponent<Object>();
                if (!CanMoveObjectChain(obj, reverseDirection))
                {
                    return false; // The chain of objects cannot move in the reverse direction
                }
            }
        }
    }

    return true; // No obstructions; the snake can move backward
}

// Helper function to check if an object chain can move in the reverse direction
private bool CanMoveObjectChain(Object obj, Vector2Int reverseDirection)
{
    List<Object> chain = new List<Object>();
    Object currentObject = obj;

    // Collect the chain of objects in reverse order
    while (currentObject != null)
    {
        chain.Add(currentObject);

        int objX = Mathf.RoundToInt(currentObject.transform.position.x) + reverseDirection.x;
        int objY = Mathf.RoundToInt(currentObject.transform.position.y) + reverseDirection.y;
        Vector2 nextPosition = new Vector2(objX, objY);

        Collider2D[] colliders = Physics2D.OverlapPointAll(nextPosition);
        currentObject = null; // Reset for the next iteration

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall") || collider.CompareTag("Segment"))
            {
                return false; // The object chain is blocked by a wall or snake segment
            }

            if (collider.CompareTag("Object"))
            {
                currentObject = collider.GetComponent<Object>();
                break; // Continue the chain
            }
        }
    }

    return true; // The entire chain can move in the reverse direction
}
    public void MoveBackwards()
{
    Vector2Int reverseDirection = -direction;
    List<Vector3> newPositions = new List<Vector3>();

    // Move all snake segments backwards by one position
    foreach (Transform segment in segments)
    {
        int x = Mathf.RoundToInt(segment.position.x) + reverseDirection.x;
        int y = Mathf.RoundToInt(segment.position.y) + reverseDirection.y;
        newPositions.Add(new Vector3(x, y, segment.position.z));
    }

    // Move the snake's segments to their new positions
    for (int i = 0; i < segments.Count; i++)
    {
        segments[i].position = newPositions[i];
    }

    transform.position = newPositions[0];  // Move the snake's head to the new position

    // Move any objects in the same direction
    foreach (Transform segment in segments)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(segment.position);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Object"))
            {
                Object obj = collider.GetComponent<Object>();
                List<Object> chain = new List<Object>();
                Object currentObject = obj;

                // Collect the chain of objects in reverse order (to handle multiple connected objects)
                while (currentObject != null)
                {
                    chain.Add(currentObject);
                    int objX = Mathf.RoundToInt(currentObject.transform.position.x) + reverseDirection.x;
                    int objY = Mathf.RoundToInt(currentObject.transform.position.y) + reverseDirection.y;
                    Vector2 nextPosition = new Vector2(objX, objY);

                    Collider2D[] objectColliders = Physics2D.OverlapPointAll(nextPosition);
                    currentObject = null; // Reset for next iteration

                    foreach (Collider2D objectCollider in objectColliders)
                    {
                        if (objectCollider.CompareTag("Object"))
                        {
                            currentObject = objectCollider.GetComponent<Object>();
                            break;
                        }
                    }
                }

                // Move all objects in the chain if no blockage
                for (int i = chain.Count - 1; i >= 0; i--)
                {
                    Object chainObject = chain[i];
                    int objX = Mathf.RoundToInt(chainObject.transform.position.x) + reverseDirection.x;
                    int objY = Mathf.RoundToInt(chainObject.transform.position.y) + reverseDirection.y;
                    chainObject.transform.position = new Vector2(objX, objY);
                }
            }
        }
    }
}

    public void Grow()
{
    Transform lastSegment = segments[segments.Count - 1];
    Vector3 directionAsVector3 = new Vector3(direction.x, direction.y, 0);
    Vector3 preferredPosition = lastSegment.position - directionAsVector3;
    Collider2D[] preferredColliders = Physics2D.OverlapPointAll(preferredPosition);
    bool isPreferredBlocked = false;
    foreach (Collider2D collider in preferredColliders)
    {
        if (collider.CompareTag("Wall") || collider.CompareTag("Segment") || collider.CompareTag("Object"))
        {
            isPreferredBlocked = true;
            break;
        }
    }
    if (!isPreferredBlocked)
    {
        Transform newSegment = Instantiate(segmentPrefab, preferredPosition, Quaternion.identity);
        segments.Add(newSegment);
        return;
    }
    Vector3[] alternativePositions = new Vector3[]
    {
        lastSegment.position - Vector3.right,
        lastSegment.position + Vector3.right,
        lastSegment.position - Vector3.up,
        lastSegment.position + Vector3.up
    };
    foreach (Vector3 position in alternativePositions)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(position);
        bool isBlocked = false;
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall") || collider.CompareTag("Segment") || collider.CompareTag("Object"))
            {
                isBlocked = true;
                break;
            }
        }
        if (!isBlocked)
        {
            Transform newSegment = Instantiate(segmentPrefab, position, Quaternion.identity);
            segments.Add(newSegment);
            return;
        }
    }
    GameManager.instance.segments = segments;
}

    public void StartUp()
    {
        segments.Add(transform);
        for (int i = 0; i < initialSize - 1; i++)
        {
            Grow();
        }
        GameManager.instance.segments = segments;
    }
    private void HandleObjectCollision(Object @object)
{
    direction = input;
    List<Object> chain = new List<Object>();
    Object currentObject = @object;
    bool isBlocked = false;

    // Collect all objects in the chain
    while (currentObject != null)
    {
        chain.Add(currentObject);
        Vector2Int nextPosition = new Vector2Int(
            Mathf.RoundToInt(currentObject.transform.position.x) + direction.x,
            Mathf.RoundToInt(currentObject.transform.position.y) + direction.y
        );

        Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(nextPosition.x, nextPosition.y));
        currentObject = null; 

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall") || collider.CompareTag("Segment"))
            {
                isBlocked = true;
                break;
            }
            if (collider.CompareTag("Object"))
            {
                currentObject = collider.GetComponent<Object>();
                break;
            }
        }
        if (isBlocked) break; 
    }
    if (isBlocked)
    {
        Object firstObject = chain[0];
        firstObject.NextStep();
        SpriteManager.instance.ChangeHeadDirection(input);
        if (!firstPush)
        {
            firstPush = true;
            MoveSnake();
        }
        return;
    }
    foreach (Object obj in chain)
    {
        Vector2Int newPosition = new Vector2Int(
            Mathf.RoundToInt(obj.transform.position.x) + direction.x,
            Mathf.RoundToInt(obj.transform.position.y) + direction.y
        );
        obj.transform.position = new Vector2(newPosition.x, newPosition.y);
    }
    SpriteManager.instance.ChangeHeadDirection(input);
    if (!firstPush)
    {
        firstPush = true;
        MoveSnake();
    }
}
    public bool InBounds()
    {
        if (segments[0].transform.position.x <= GridManager.instance.leftXBound || 
            segments[0].transform.position.x >= GridManager.instance.rightXBound ||
            segments[0].transform.position.y <= GridManager.instance.downYBound ||
            segments[0].transform.position.y >= GridManager.instance.upYBound)
        {
            return false;
        }
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hole") && GameManager.instance.ExitOpen && canMove)
        {
            GameManager.instance.FinishLevel();
        }
    }
}