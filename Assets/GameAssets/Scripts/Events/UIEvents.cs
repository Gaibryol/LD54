using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvents
{
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
