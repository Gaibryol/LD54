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
		eventBroker.Publish(this, new AudioEvents.PlayMusic(Constants.Audio.Music.MainTrack));
	}

	private void Update()
	{
		// Increment score if playing
		if (isPlaying)
		{
			Score += Time.deltaTime;
		}
	}

	public void StartGame()
	{
        eventBroker.Publish(this, new GameStateEvents.StartGame(this));
        isPlaying = true;
    }

	private void HandleHighscore()
	{
		// Check
		int hs = PlayerPrefs.GetInt(Constants.Player.Highscore, 0);

		if (Score > hs)
		{
			// New highscore, display somewhere
			eventBroker.Publish(this, new UIEvents.UpdateEndUI(true));

			// Save new highscore
			PlayerPrefs.SetInt(Constants.Player.Highscore, (int)Score);
			PlayerPrefs.Save();
		}
		else
		{
			// Game over
			eventBroker.Publish(this, new UIEvents.UpdateEndUI(false));
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
		isPlaying = false;
	}

	private void ClearLinesHandler(BrokerEvent<PlayerEvents.ClearLines> inEvent)
	{
		//Debug.Log("cleared: " + inEvent.Payload.NumCleared);
		switch (inEvent.Payload.NumCleared)
		{
			case 1:
				Score += Constants.Player.LinesCleared1;
				break;

			case 2:
				Score += Constants.Player.LinesCleared2;
				break;

			case 3:
				Score += Constants.Player.LinesCleared3;
				break;

			case 4:
				Score += Constants.Player.LinesCleared4;
				break;
		}
	}

	private void SecretEndingHandler(BrokerEvent<GameStateEvents.SecretEnding> inEvent)
	{
		isPlaying = false;
		Score += Constants.Player.SecretEndingPoints;
		HandleHighscore();
	}


	private void OnEnable()
	{
		eventBroker.Subscribe<PlayerEvents.ClearLines>(ClearLinesHandler);
		eventBroker.Subscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Subscribe<GameStateEvents.RestartGame>(RestartGameHandler);
		eventBroker.Subscribe<GameStateEvents.SecretEnding>(SecretEndingHandler);
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<PlayerEvents.ClearLines>(ClearLinesHandler);
		eventBroker.Unsubscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Unsubscribe<GameStateEvents.RestartGame>(RestartGameHandler);
		eventBroker.Unsubscribe<GameStateEvents.SecretEnding>(SecretEndingHandler);
	}
}
