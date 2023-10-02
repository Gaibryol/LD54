using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
	private EventBrokerComponent eventBroker = new EventBrokerComponent();

	private bool Combo20Times = false;
	private bool EarnBigCombo = false;
	private bool Escaped = false;
	private bool Jump100Times = false;
	private bool Earn1000Points = false;
	private bool Earn2000Points = false;
	private bool Earn3000Points = false;
	private bool Rotate50Times = false;

	private void Awake()
	{
		Combo20Times = intToBool(PlayerPrefs.GetInt(Constants.Achievements.Combo20Times, 0));
		EarnBigCombo = intToBool(PlayerPrefs.GetInt(Constants.Achievements.EarnBigCombo, 0));
		Escaped = intToBool(PlayerPrefs.GetInt(Constants.Achievements.Escaped, 0));
		Jump100Times = intToBool(PlayerPrefs.GetInt(Constants.Achievements.Jump100Times, 0));
		Earn1000Points = intToBool(PlayerPrefs.GetInt(Constants.Achievements.Earn1000Points, 0));
		Earn2000Points = intToBool(PlayerPrefs.GetInt(Constants.Achievements.Earn2000Points, 0));
		Earn3000Points = intToBool(PlayerPrefs.GetInt(Constants.Achievements.Earn3000Points, 0));
		Rotate50Times = intToBool(PlayerPrefs.GetInt(Constants.Achievements.Rotate50Times, 0));
	}

	private void EarnAchievementHandler(BrokerEvent<AchievementEvents.EarnAchievement> inEvent)
	{
		switch (inEvent.Payload.Achievement)
		{
			case Constants.Achievements.Combo20Times:
				if (Combo20Times != true)
				{
					eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Achievement));
					Combo20Times = true;
				}
				break;

			case Constants.Achievements.EarnBigCombo:
				if (EarnBigCombo != true)
				{
					eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Achievement));
					EarnBigCombo = true;
				}
				break;

			case Constants.Achievements.Escaped:
				if (Escaped != true)
				{
					eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Achievement));
					Escaped = true;
				}
				break;

			case Constants.Achievements.Jump100Times:
				if (Jump100Times != true)
				{
					eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Achievement));
					Jump100Times = true;
				}
				break;

			case Constants.Achievements.Earn1000Points:
				if (Earn1000Points != true)
				{
					eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Achievement));
					Earn1000Points = true;
				}
				break;

			case Constants.Achievements.Earn2000Points:
				if (Earn2000Points != true)
				{
					eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Achievement));
					Earn2000Points = true;
				}
				break;

			case Constants.Achievements.Earn3000Points:
				if (Earn3000Points != true)
				{
					eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Achievement));
					Earn3000Points = true;
				}
				break;

			case Constants.Achievements.Rotate50Times:
				if (Rotate50Times != true)
				{
					eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Achievement));
					Rotate50Times = true;
				}
				break;
		}

		
		PlayerPrefs.SetInt(inEvent.Payload.Achievement, boolToInt(true));
		PlayerPrefs.Save();
	}


	private void GetAchievementsHandler(BrokerEvent<AchievementEvents.GetAchievements> inEvent)
	{
		Dictionary<string, bool> achievements = new Dictionary<string, bool>();
		achievements.Add(Constants.Achievements.Combo20Times, Combo20Times);
		achievements.Add(Constants.Achievements.EarnBigCombo, EarnBigCombo);
		achievements.Add(Constants.Achievements.Escaped, Escaped);
		achievements.Add(Constants.Achievements.Jump100Times, Jump100Times);
		achievements.Add(Constants.Achievements.Earn1000Points, Earn1000Points);
		achievements.Add(Constants.Achievements.Earn2000Points, Earn2000Points);
		achievements.Add(Constants.Achievements.Earn3000Points, Earn3000Points);
		achievements.Add(Constants.Achievements.Rotate50Times, Rotate50Times);

		inEvent.Payload.Achievements.DynamicInvoke(achievements);
	}

	private int boolToInt(bool input)
	{
		return input ? 1 : 0;
	}

	private bool intToBool(int num)
	{
		return num == 1 ? true : false;
	}

	private void OnEnable()
	{
		eventBroker.Subscribe<AchievementEvents.EarnAchievement>(EarnAchievementHandler);
		eventBroker.Subscribe<AchievementEvents.GetAchievements>(GetAchievementsHandler);
	}

	private void OnDisable()
	{
		eventBroker.Unsubscribe<AchievementEvents.EarnAchievement>(EarnAchievementHandler);
		eventBroker.Unsubscribe<AchievementEvents.GetAchievements>(GetAchievementsHandler);
	}
}
