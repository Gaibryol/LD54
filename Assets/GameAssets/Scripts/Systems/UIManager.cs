using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

	[SerializeField] private Button gridButton;
	[SerializeField] private Sprite gridOnSprite;
	[SerializeField] private Sprite gridOffSprite;
	[SerializeField] private GameObject grid;

	[SerializeField] private Button yesRestartButton;
	[SerializeField] private Button noRestartButton;

	[SerializeField, Header("End Screen")] private GameObject endScreen;
	[SerializeField] private Sprite gameOverSprite;
	[SerializeField] private Sprite highScoreSprite;
	[SerializeField] private TMP_Text finalScoreText;
	[SerializeField] private Button endRestartButton;


	[SerializeField, Header("Secret Ending")] private GameObject splashScreen;
	[SerializeField] private Sprite splash1;
	[SerializeField] private Sprite splash2;

	private bool volumeOn;
	private bool gridOn;
	private bool isPlaying;

	private void Awake()
	{
		volumeOn = true;
		gridOn = false;
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

	private void SecretEndingHandler(BrokerEvent<GameStateEvents.SecretEnding> inEvent)
	{
		// Slow effect?
		// Open splash screen 1
		// splash screen 2
		// Return to game view with congratulatory message

		StartCoroutine(SecretEndingCoroutine());
	}

	private IEnumerator SecretEndingCoroutine()
	{
		splashScreen.SetActive(true);

		yield return new WaitForSeconds(3f);

		splashScreen.GetComponent<Image>().sprite = splash2;

		yield return new WaitForSeconds(3f);

		splashScreen.SetActive(false);
		splashScreen.GetComponent<Image>().sprite = splash1;
	}

	private void ToggleInfo()
	{
		infoPanel.SetActive(!infoPanel.activeSelf);
		infoButton.GetComponent<Image>().sprite = infoPanel.activeSelf ? infoOnSprite : infoOffSprite;
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Button));
	}
	
	private void ToggleVolume()
	{
		volumeOn = !volumeOn;
		eventBroker.Publish(this, new AudioEvents.ChangeMusicVolume(volumeOn ? 0.25f : 0));
		eventBroker.Publish(this, new AudioEvents.ChangeSFXVolume(volumeOn ? 0.25f : 0));
		volumeButton.GetComponent<Image>().sprite = volumeOn ? volumeOnSprite : volumeOffSprite;
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Button));
	}

	private void PromptRestart()
	{
		if (gameManager == null) return;

		Time.timeScale = 0f;
		restartPanel.SetActive(true);
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Button));
	}

	private void RestartGame()
	{
		eventBroker.Publish(this, new GameStateEvents.RestartGame());
		endScreen.SetActive(false);
		restartPanel.SetActive(false);
		Time.timeScale = 1f;
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Button));
	}

	private void CancelRestart()
	{
		Time.timeScale = 1f;
		restartPanel.SetActive(false);
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Button));
	}

	private void ToggleGrid()
	{
		gridOn = !gridOn;
		grid.SetActive(gridOn);
		gridButton.GetComponent<Image>().sprite = gridOn ? gridOnSprite : gridOffSprite;
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Button));
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<GameStateEvents.StartGame>(StartGameHandler);
		eventBroker.Subscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Subscribe<GameStateEvents.SecretEnding>(SecretEndingHandler);
		eventBroker.Subscribe<UIEvents.UpdateEndUI>(UpdateEndUIHandler);

		infoButton.onClick.AddListener(ToggleInfo);
		volumeButton.onClick.AddListener(ToggleVolume);
		promptRestartButton.onClick.AddListener(PromptRestart);
		yesRestartButton.onClick.AddListener(RestartGame);
		noRestartButton.onClick.AddListener(CancelRestart);
		gridButton.onClick.AddListener(ToggleGrid);
		endRestartButton.onClick.AddListener(RestartGame);
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<GameStateEvents.StartGame>(StartGameHandler);
		eventBroker.Unsubscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Unsubscribe<GameStateEvents.SecretEnding>(SecretEndingHandler);
		eventBroker.Unsubscribe<UIEvents.UpdateEndUI>(UpdateEndUIHandler);

		infoButton.onClick.RemoveListener(ToggleInfo);
		volumeButton.onClick.RemoveListener(ToggleVolume);
		promptRestartButton.onClick.RemoveListener(PromptRestart);
		yesRestartButton.onClick.RemoveListener(RestartGame);
		noRestartButton.onClick.RemoveListener(CancelRestart);
		gridButton.onClick.RemoveListener(ToggleGrid);
		endRestartButton.onClick.RemoveListener(RestartGame);
	}
}
