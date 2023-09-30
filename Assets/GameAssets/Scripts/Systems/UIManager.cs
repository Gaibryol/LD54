using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private GameManager gameManager;

	[SerializeField] private TMP_Text scoreText;

	[SerializeField, Header("End Screen")] private GameObject endScreen;
	[SerializeField] private TMP_Text finalScoreText;
	[SerializeField] private Button restartButton;

	private void Awake()
	{

	}

	private void FixedUpdate()
	{
		scoreText.text = ((int)gameManager.Score).ToString();
	}

	private void StartGameHandler(BrokerEvent<GameStateEvents.StartGame> inEvent)
	{
		gameManager = inEvent.Payload.GameManager;
	}

	private void EndGameHandler(BrokerEvent<GameStateEvents.EndGame> inEvent)
	{
		endScreen.SetActive(true);
		finalScoreText.text = scoreText.text;
	}

	private void RestartGame()
	{
		eventBroker.Publish(this, new GameStateEvents.RestartGame());
		endScreen.SetActive(false);
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<GameStateEvents.StartGame>(StartGameHandler);
		eventBroker.Subscribe<GameStateEvents.EndGame>(EndGameHandler);

		restartButton.onClick.AddListener(RestartGame);
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<GameStateEvents.StartGame>(StartGameHandler);
		eventBroker.Unsubscribe<GameStateEvents.EndGame>(EndGameHandler);

		restartButton.onClick.RemoveListener(RestartGame);
	}
}
