using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;
    public int initialSize = 4;
    public bool moveThroughWalls = false;
    public List<Transform> segments = new List<Transform>();
    private Vector2Int input;
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
            MoveBackwards();
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

        if (input != Vector2Int.zero)
        {
            if (IsValidMove(input))
            {
                direction = input;
                MoveSnake();
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
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return false;
            }
        }
        // more logic here later
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
        // more logic here later
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
        // If no valid position was found, error
        throw new System.Exception("Snake can't grow!");
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
    }
    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }
        return false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            Grow();
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            ResetState();
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls)
            {
                Traverse(other.transform);
            }
            else
            {
                ResetState();
            }
        }
    }
    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f)
        {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        }
        else if (direction.y != 0f)
        {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }
        transform.position = position;
    }
}