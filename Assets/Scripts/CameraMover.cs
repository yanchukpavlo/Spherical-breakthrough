using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public static CameraMover current;

    [SerializeField] Transform topPoint;
    public Transform TopPoint { get { return topPoint; } }

    [SerializeField] Transform lowerPoint;
    public Transform LowerPoint { get { return lowerPoint; } }

    float speed = 2;
    Vector3 startPos;
    bool isMove;

    private void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }

        current = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        EventsManager.current.onGameStateChangeTrigger += GameStateChange;
        startPos = transform.position;
    }

    private void Update()
    {
        if (isMove)
        {
            transform.position += Vector3.forward * Time.deltaTime * speed;
        }
    }

    private void OnDestroy()
    {
        EventsManager.current.onGameStateChangeTrigger -= GameStateChange;
    }

    private void GameStateChange(EventsManager.GameState state)
    {
        switch (state)
        {
            case EventsManager.GameState.Start:
                speed = GameManager.current.LevelSpeed;
                transform.position = startPos;
                isMove = true;
                break;

            case EventsManager.GameState.Win:
                isMove = false;
                break;

            case EventsManager.GameState.GameOver:
                isMove = false;
                break;

            default:
                break;
        }
    }
}
