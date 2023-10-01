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
		public UpdateEndUI(bool newHighscore)
		{
			NewHighscore = newHighscore;
		}

		public readonly bool NewHighscore;
	}

	public class UpdateCountDownUI
	{
		public readonly int CountDown;

		public UpdateCountDownUI(int countDown)
		{
			CountDown = countDown;
		}
	}
}
