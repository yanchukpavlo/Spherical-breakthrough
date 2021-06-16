using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    [Header("Settings")]
    [SerializeField] float levelSpeed = 4f;
    [SerializeField] float waitTime = 2f;
    [SerializeField] float groundYOffset = -0.5f;
    [SerializeField] int startObstacleCount = 5;
    [SerializeField] float distanceBetweenObstacle = 5f;

    [Header("Slommotion")]
    [SerializeField] float slommotionFactor = 0.05f;
    [SerializeField] float slommotionTime = 3f;

    [Header("Prefabs")]
    [SerializeField] GameObject playerPref;
    [SerializeField] GameObject groundPref;
    [SerializeField] GameObject finishPref;
    [SerializeField] List<GameObject> obstaclePrefs;

    int score = 0;
    int obstacleCount;
    GameObject level;
    float levelHalfWidth = 9f;

    public float LevelSpeed { get { return levelSpeed; } }
    public float LevelWidth { get { return levelHalfWidth; } }
    public float WaitTime { get { return waitTime; } }

    int currentResult = 0;
    public int CurrentResult { get { return currentResult; } }

    int bestResult = 0;
    public int BestResult { get { return bestResult; } }


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

    private void Start()
    {
        obstacleCount = startObstacleCount;
        EventsManager.current.onGameStateChangeTrigger += OnGameStateChange;
    }

    private void OnDestroy()
    {
        EventsManager.current.onGameStateChangeTrigger -= OnGameStateChange;
    }

    private void OnGameStateChange(EventsManager.GameState state)
    {
        switch (state)
        {
            case EventsManager.GameState.Start:
                GenerateLevel();
                break;

            case EventsManager.GameState.Win:
                Slommotion();

                obstacleCount++;
                score++;
                StartCoroutine(WaitToGenerateLevel(waitTime));
                break;

            case EventsManager.GameState.GameOver:
                Slommotion();

                obstacleCount = startObstacleCount;
                currentResult = score;
                bestResult = GetBestRsult();
                score = 0;
                break;

            default:
                break;
        }
    }

    void GenerateLevel()
    {
        if (level != null)
        {
            Destroy(level);
        }

        level = new GameObject("Level");
        float levelLength = distanceBetweenObstacle * obstacleCount + distanceBetweenObstacle;

        GameObject ground = Instantiate(groundPref, level.transform);
        ground.transform.position = new Vector3(0, groundYOffset, -20f);
        ground.transform.localScale = new Vector3(1, 1, levelLength + 100f);

        GameObject finish = Instantiate(finishPref, level.transform);
        finish.transform.position = new Vector3(0, groundYOffset, levelLength);

        GameObject player = Instantiate(playerPref, level.transform);

        float distance = distanceBetweenObstacle;
        for (int i = 1; i <= obstacleCount; i++)
        {
            GameObject obstacle = Instantiate(obstaclePrefs[Random.Range(0, obstaclePrefs.Count)], level.transform);
            obstacle.transform.position = new Vector3(0, 0f, distance);

            distance += distanceBetweenObstacle;
        }
    }

    IEnumerator WaitToGenerateLevel(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        GenerateLevel();
    }

    int GetBestRsult()
    {
        int best = 0;
        best = PlayerPrefs.GetInt("bestResult", 0);
        
        if (best < currentResult)
        {
            best = currentResult;
            PlayerPrefs.SetInt("bestResult", currentResult);
            PlayerPrefs.Save();
        }

        return best;
    }

    void Slommotion()
    {
        Time.timeScale = slommotionFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        StartCoroutine(DoSlommotion());
    }

    IEnumerator DoSlommotion()
    {
        float time = (1f / slommotionTime) * Time.unscaledDeltaTime;

        yield return new WaitForSecondsRealtime(time);

        Time.timeScale += time;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

         if (Time.timeScale < 1) StartCoroutine(DoSlommotion());
    }
}
