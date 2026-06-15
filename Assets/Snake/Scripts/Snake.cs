using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public Transform segmentPrefab;
    public int InitialSize = 3;
    public float speed = 20f; 
    public float speedIncreasePerSegment = 0.5f; 
    private Vector2 _direction = Vector2.right;
    private List<Transform> _segments = new List<Transform>();
    private float _stepTimer = 0f;

    public GameManager gameManager;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        ResetState();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetDirection(Vector2.right);
        }
        // Movement is handled in FixedUpdate using a timed step based on `speed`.
    }

    private void SetDirection(Vector2 newDirection)
    {
        if (newDirection == -_direction)
        {
            return;
        }
        _direction = newDirection;
    }

    private void FixedUpdate()
    {
        // accumulate time and move only when interval reached (grid-step movement)
        _stepTimer += Time.fixedDeltaTime;
        // increase speed as snake grows: each extra segment adds `speedIncreasePerSegment`
        float currentSpeed = speed + (_segments.Count - 1) * speedIncreasePerSegment;
        float interval = 1f / Mathf.Max(0.0001f, currentSpeed);
        if (_stepTimer < interval) return;
        _stepTimer = 0f;

        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        this.transform.position = new Vector3(Mathf.Round(this.transform.position.x) + _direction.x, Mathf.Round(this.transform.position.y) + _direction.y, 0.0f);
    }
    
    private void Grow()
    {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;

        _segments.Add(segment);
    }

    public void ResetState()
    {
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }

        _segments.Clear();
        _segments.Add(this.transform);

        for (int i = 1; i < InitialSize; i++)
        {
            Transform segment = Instantiate(this.segmentPrefab);
            segment.position = _segments[_segments.Count - 1].position;

            _segments.Add(segment);
        }

        this.transform.position = Vector3.zero;
        _direction = Vector2.right;
        _stepTimer = 0f;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Food")
        {
            Grow();
        }
        else if (other.tag == "Obstacle")
        {
            ResetState();
        }
    }
}
 
