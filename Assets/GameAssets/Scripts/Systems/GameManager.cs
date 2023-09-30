using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public float Score;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private bool isPlaying;

	private void Awake()
	{
		Score = 0;
		isPlaying = false;
	}

	private void Start()
	{
		eventBroker.Publish(this, new GameStateEvents.StartGame(this));
		isPlaying = true;
	}

	private void Update()
	{
		// Increment score if playing
		if (isPlaying)
		{
			Score += Time.deltaTime;
		}
	}

	private void HandleHighscore()
	{
		// Check
		int hs = PlayerPrefs.GetInt(Constants.Player.Highscore, 0);

		if (hs != 0)
		{
			if (Score > hs)
			{
				// New highscore, display somewhere

				// Save new highscore
				PlayerPrefs.SetInt(Constants.Player.Highscore, (int)Score);
				PlayerPrefs.Save();
			}
		}
	}

	private void EndGameHandler(BrokerEvent<GameStateEvents.EndGame> inEvent)
	{
		isPlaying = false;

		HandleHighscore();
	}

	private void RestartGameHandler(BrokerEvent<GameStateEvents.RestartGame> inEvent)
	{
		Score = 0;
		isPlaying = true;
		eventBroker.Publish(this, new GameStateEvents.StartGame(this));
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Subscribe<GameStateEvents.RestartGame>(RestartGameHandler);
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Unsubscribe<GameStateEvents.RestartGame>(RestartGameHandler);
	}
}
