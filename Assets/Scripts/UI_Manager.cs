using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager current;

    [SerializeField] TMP_Text mainText;
    [SerializeField] GameObject joystick;

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
        HideText();
        joystick.SetActive(false);
        EventsManager.current.onGameStateChangeTrigger += GameStateChange;
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
                joystick.SetActive(true);
                HideText();
                break;

            case EventsManager.GameState.Win:
                ShowText("To the next Level!");
                StartCoroutine(WaitToAction(true));
                break;

            case EventsManager.GameState.GameOver:
                joystick.SetActive(false);
                ShowText("Level Failed!");
                StartCoroutine(WaitToAction(false));
                break;

            default:
                break;
        }
    }

    public void StartButton()
    {
        EventsManager.current.GameStateChangeTrigger(EventsManager.GameState.Start);
    }

    public void ShowText(string text)
    {
        mainText.text = text;
        mainText.gameObject.SetActive(true);
    }

    IEnumerator WaitToAction(bool isNextLevel)
    {
        yield return new WaitForSecondsRealtime(GameManager.current.WaitTime);
        if (isNextLevel)
        {
            EventsManager.current.GameStateChangeTrigger(EventsManager.GameState.Start);
        }
        else
        {
            mainText.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            ShowText($"Best result: {GameManager.current.BestResult}\n\nYour result: {GameManager.current.CurrentResult}");
        }
    }

    public void HideText()
    {
        mainText.gameObject.SetActive(false);
    }
}
