using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private GameManager gameManager;

	[SerializeField, Header("Game Screen")] private TMP_Text scoreText;

	[SerializeField] private GameObject infoPanel;
	[SerializeField] private Button infoButton;
	[SerializeField] private Sprite infoOnSprite;
	[SerializeField] private Sprite infoOffSprite;

	[SerializeField] private Button volumeButton;
	[SerializeField] private Sprite volumeOnSprite;
	[SerializeField] private Sprite volumeOffSprite;

	[SerializeField] private Button promptRestartButton;
	[SerializeField] private GameObject restartPanel;

	[SerializeField] private Button yesRestartButton;
	[SerializeField] private Button noRestartButton;

	[SerializeField, Header("End Screen")] private GameObject endScreen;
	[SerializeField] private Sprite gameOverSprite;
	[SerializeField] private Sprite highScoreSprite;
	[SerializeField] private TMP_Text finalScoreText;
	[SerializeField] private Button endRestartButton;

	private bool volumeOn;
	private bool isPlaying;

	private void Awake()
	{
		volumeOn = true;
	}

	private void FixedUpdate()
	{
		if (gameManager == null || !isPlaying) return;
		scoreText.text = ((int)gameManager.Score).ToString();
	}

	private void StartGameHandler(BrokerEvent<GameStateEvents.StartGame> inEvent)
	{
		gameManager = inEvent.Payload.GameManager;
		isPlaying = true;
	}

	private void EndGameHandler(BrokerEvent<GameStateEvents.EndGame> inEvent)
	{
		endScreen.SetActive(true);
		finalScoreText.text = scoreText.text;
		isPlaying = false;
	}

	private void UpdateEndUIHandler(BrokerEvent<UIEvents.UpdateEndUI> inEvent)
	{
		endScreen.GetComponent<Image>().sprite = inEvent.Payload.NewHighscore ? highScoreSprite : gameOverSprite;
	}

	private void ToggleInfo()
	{
		infoPanel.SetActive(!infoPanel.activeSelf);
		infoButton.GetComponent<Image>().sprite = infoPanel.activeSelf ? infoOnSprite : infoOffSprite;
	}
	
	private void ToggleVolume()
	{
		volumeOn = !volumeOn;
		eventBroker.Publish(this, new AudioEvents.ChangeMusicVolume(volumeOn ? 0.25f : 0));
		eventBroker.Publish(this, new AudioEvents.ChangeSFXVolume(volumeOn ? 0.25f : 0));
		volumeButton.GetComponent<Image>().sprite = volumeOn ? volumeOnSprite : volumeOffSprite;
	}

	private void PromptRestart()
	{
		if (gameManager == null) return;

		Time.timeScale = 0f;
		restartPanel.SetActive(true);
	}

	private void RestartGame()
	{
		eventBroker.Publish(this, new GameStateEvents.RestartGame());
		endScreen.SetActive(false);
	}

	private void CancelRestart()
	{
		Time.timeScale = 1f;
		restartPanel.SetActive(false);
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<GameStateEvents.StartGame>(StartGameHandler);
		eventBroker.Subscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Subscribe<UIEvents.UpdateEndUI>(UpdateEndUIHandler);

		infoButton.onClick.AddListener(ToggleInfo);
		volumeButton.onClick.AddListener(ToggleVolume);
		promptRestartButton.onClick.AddListener(PromptRestart);
		yesRestartButton.onClick.AddListener(RestartGame);
		noRestartButton.onClick.AddListener(CancelRestart);
		endRestartButton.onClick.AddListener(RestartGame);
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<GameStateEvents.StartGame>(StartGameHandler);
		eventBroker.Unsubscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Unsubscribe<UIEvents.UpdateEndUI>(UpdateEndUIHandler);

		infoButton.onClick.RemoveListener(ToggleInfo);
		volumeButton.onClick.RemoveListener(ToggleVolume);
		promptRestartButton.onClick.RemoveListener(PromptRestart);
		yesRestartButton.onClick.RemoveListener(RestartGame);
		noRestartButton.onClick.RemoveListener(CancelRestart);
		endRestartButton.onClick.RemoveListener(RestartGame);
	}
}
