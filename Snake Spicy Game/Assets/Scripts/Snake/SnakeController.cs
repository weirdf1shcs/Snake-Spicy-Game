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
    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        input = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.G))
        {
            Grow();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (CanMoveBackwards())
            {
                MoveBackwards();
            }
        }
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
                if (input == Vector2Int.up)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0); 
                }
                else if (input == Vector2Int.down)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                else if (input == Vector2Int.left)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else if (input == Vector2Int.right)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 270);
                }
                if (IsValidMove(input))
                {
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
    }
    private bool IsValidMove(Vector2Int newDirection)
    {
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
                    return false;
                }
                if (collider.CompareTag("Object"))
                {
                    Object obj = collider.GetComponent<Object>();
                    if (obj != null)
                    {
                        int objX = Mathf.RoundToInt(obj.transform.position.x) + reverseDirection.x;
                        int objY = Mathf.RoundToInt(obj.transform.position.y) + reverseDirection.y;
                        Vector2 objNextPosition = new Vector2(objX, objY);
                        Collider2D[] objectColliders = Physics2D.OverlapPointAll(objNextPosition);
                        foreach (Collider2D objectCollider in objectColliders)
                        {
                            if (objectCollider.CompareTag("Wall"))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
        }
        return true; 
    }
    public void MoveBackwards()
    {
        Vector2Int reverseDirection = -direction;
        List<Vector3> newPositions = new List<Vector3>();
        foreach (Transform segment in segments)
        {
            int x = Mathf.RoundToInt(segment.position.x) + reverseDirection.x;
            int y = Mathf.RoundToInt(segment.position.y) + reverseDirection.y;
            newPositions.Add(new Vector3(x, y, segment.position.z));
        }
        for (int i = 0; i < segments.Count; i++)
        {
            Vector3 segmentPosition = newPositions[i];
            Collider2D[] colliders = Physics2D.OverlapPointAll(segmentPosition);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Object"))
                {
                    Object obj = collider.GetComponent<Object>();
                    Vector2Int objectNewPosition = new Vector2Int(
                        Mathf.RoundToInt(obj.transform.position.x) + reverseDirection.x,
                        Mathf.RoundToInt(obj.transform.position.y) + reverseDirection.y);
                    obj.transform.position = new Vector2(objectNewPosition.x, objectNewPosition.y);
                }
            }
        }
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].position = newPositions[i];
        }
        transform.position = newPositions[0];
    }

    public void Grow()
    {
        Transform lastSegment = segments[segments.Count - 1];
        Vector3[] potentialPositions = new Vector3[]
        {
            lastSegment.position - Vector3.right,
            lastSegment.position + Vector3.right,
            lastSegment.position - Vector3.up,
            lastSegment.position + Vector3.up
        };
        foreach (Vector3 position in potentialPositions)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(position);
            if (colliders.Length == 0)
            {
                Transform newSegment = Instantiate(segmentPrefab, position, Quaternion.identity);
                segments.Add(newSegment);
                return;
            }
        }

        GameManager.instance.segments = segments;
    }

    public void ResetState()
    {
        transform.position = Vector3.zero;
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }
        segments.Clear();
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
        Vector2Int newObjectPosition = new Vector2Int(
            Mathf.RoundToInt(@object.transform.position.x) + direction.x,
            Mathf.RoundToInt(@object.transform.position.y) + direction.y
        );
        Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(newObjectPosition.x, newObjectPosition.y));
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall") || collider.CompareTag("Segment"))
            {
                MoveSnake();
                @object.NextStep();
                return;
            }
        }
        @object.transform.position = new Vector2(newObjectPosition.x, newObjectPosition.y);
        MoveSnake();
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
}