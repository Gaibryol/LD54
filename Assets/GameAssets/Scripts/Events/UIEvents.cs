using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvents
{
	public class UpdateStartUI
	{
		public UpdateStartUI(float volume)
		{
			Volume = volume;
		}

		public readonly float Volume;
	}

    public class UpdateEndUI
	{
		public UpdateEndUI(bool newHighscore, bool secretEnding)
		{
			NewHighscore = newHighscore;
			SecretEnding = secretEnding;
		}

		public readonly bool NewHighscore;
		public readonly bool SecretEnding;
	}

	public class UpdateCountDownUI
	{
		public readonly int CountDown;

		public UpdateCountDownUI(int countDown)
		{
			CountDown = countDown;
		}
	}

	public class ShowAchievementDescription
	{
		public ShowAchievementDescription(string achievement, Vector3 position)
		{
			Achievement = achievement;
			Position = position;
		}

		public readonly string Achievement;
		public readonly Vector3 Position;
	}

	public class HideAchievementDescription
	{
		public HideAchievementDescription() { }
	}
}
