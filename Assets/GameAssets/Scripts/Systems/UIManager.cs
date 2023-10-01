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
	[SerializeField] private TMP_Text highscoreText;
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

	[SerializeField, Header("Countdown")] private TMP_Text countDownText;

	[SerializeField, Header("End Screen")] private GameObject endScreen;
	[SerializeField] private Sprite gameOverSprite;
	[SerializeField] private Sprite highScoreSprite;
	[SerializeField] private Sprite secretEndingSprite;
	[SerializeField] private TMP_Text finalScoreText;
	[SerializeField] private Button endRestartButton;


	[SerializeField, Header("Secret Ending")] private GameObject splashScreen;

	[SerializeField, Header("Achievements")] private GameObject combo20TimesAchievement;
	[SerializeField] private GameObject earnBigComboAchievement;
	[SerializeField] private GameObject earn1000PointsAchievement;
	[SerializeField] private GameObject earn2000PointsAchievement;
	[SerializeField] private GameObject earn3000PointsAchievement;
	[SerializeField] private GameObject escapedAchievement;
	[SerializeField] private GameObject jump100TimesAchievement;
	[SerializeField] private GameObject rotate50TimesAchievement;

	private bool volumeOn;
	private bool gridOn;
	private bool isPlaying;

	private void Awake()
	{
		volumeOn = true;
		gridOn = false;
	}

	private void Start()
	{
		// Set up achievements
		eventBroker.Publish(this, new AchievementEvents.GetAchievements((achievements) =>
		{
			combo20TimesAchievement.SetActive(achievements[Constants.Achievements.Combo20Times]);
			earnBigComboAchievement.SetActive(achievements[Constants.Achievements.EarnBigCombo]);
			earn1000PointsAchievement.SetActive(achievements[Constants.Achievements.Earn1000Points]);
			earn2000PointsAchievement.SetActive(achievements[Constants.Achievements.Earn2000Points]);
			earn3000PointsAchievement.SetActive(achievements[Constants.Achievements.Earn3000Points]);
			escapedAchievement.SetActive(achievements[Constants.Achievements.Escaped]);
			jump100TimesAchievement.SetActive(achievements[Constants.Achievements.Jump100Times]);
			rotate50TimesAchievement.SetActive(achievements[Constants.Achievements.Rotate50Times]);
		}));

		highscoreText.text = PlayerPrefs.GetInt(Constants.Player.Highscore, 0).ToString();
	}

	private void FixedUpdate()
	{
		if (gameManager == null || !isPlaying) return;
		scoreText.text = ((int)gameManager.Score).ToString();
	}

	private void EarnAchievementHandler(BrokerEvent<AchievementEvents.EarnAchievement> inEvent)
	{
		switch (inEvent.Payload.Achievement)
		{
			case Constants.Achievements.Combo20Times:
				combo20TimesAchievement.SetActive(true);
				break;

			case Constants.Achievements.EarnBigCombo:
				earnBigComboAchievement.SetActive(true);
				break;

			case Constants.Achievements.Escaped:
				escapedAchievement.SetActive(true);
				break;

			case Constants.Achievements.Jump100Times:
				jump100TimesAchievement.SetActive(true);
				break;

			case Constants.Achievements.Earn1000Points:
				earn1000PointsAchievement.SetActive(true);
				break;

			case Constants.Achievements.Earn2000Points:
				earn2000PointsAchievement.SetActive(true);
				break;

			case Constants.Achievements.Earn3000Points:
				earn3000PointsAchievement.SetActive(true);
				break;

			case Constants.Achievements.Rotate50Times:
				rotate50TimesAchievement.SetActive(true);
				break;
		}
	}

	private void StartGameHandler(BrokerEvent<GameStateEvents.StartGame> inEvent)
	{
		countDownText.enabled = false;
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
		StartCoroutine(SecretEndingCoroutine());
	}

	private IEnumerator SecretEndingCoroutine()
	{
		isPlaying = false;

		eventBroker.Publish(this, new PlayerEvents.AdjustGravity(0f));
		Time.timeScale = 0.5f;

		yield return new WaitForSeconds(0.5f);

		Time.timeScale = 1f;
		splashScreen.SetActive(true);
		eventBroker.Publish(this, new PlayerEvents.AdjustGravity(Constants.Player.GravityScale));

		yield return new WaitForSeconds(Constants.Player.SecretEndingTiming);

		splashScreen.SetActive(false);
		endScreen.SetActive(true);
		finalScoreText.text = ((int)gameManager.Score).ToString();
		endScreen.GetComponent<Image>().sprite = secretEndingSprite;
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
		eventBroker.Publish(this, new AudioEvents.ChangeMusicVolume(volumeOn ? Constants.Audio.DefaultAudioLevel : 0));
		eventBroker.Publish(this, new AudioEvents.ChangeSFXVolume(volumeOn ? Constants.Audio.DefaultAudioLevel : 0));
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

    private void UpdateCountDownUIHandler(BrokerEvent<UIEvents.UpdateCountDownUI> inEvent)
    {
		countDownText.enabled = true;
		countDownText.GetComponent<Animator>().Play("Countdown");
        countDownText.text = inEvent.Payload.CountDown.ToString();
    }

    private void OnEnable()
	{
		eventBroker.Subscribe<GameStateEvents.StartGame>(StartGameHandler);
		eventBroker.Subscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Subscribe<GameStateEvents.SecretEnding>(SecretEndingHandler);
		eventBroker.Subscribe<UIEvents.UpdateEndUI>(UpdateEndUIHandler);
		eventBroker.Subscribe<UIEvents.UpdateCountDownUI>(UpdateCountDownUIHandler);
		eventBroker.Subscribe<AchievementEvents.EarnAchievement>(EarnAchievementHandler);

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
        eventBroker.Unsubscribe<UIEvents.UpdateCountDownUI>(UpdateCountDownUIHandler);
		eventBroker.Unsubscribe<AchievementEvents.EarnAchievement>(EarnAchievementHandler);

		infoButton.onClick.RemoveListener(ToggleInfo);
		volumeButton.onClick.RemoveListener(ToggleVolume);
		promptRestartButton.onClick.RemoveListener(PromptRestart);
		yesRestartButton.onClick.RemoveListener(RestartGame);
		noRestartButton.onClick.RemoveListener(CancelRestart);
		gridButton.onClick.RemoveListener(ToggleGrid);
		endRestartButton.onClick.RemoveListener(RestartGame);
	}
}
