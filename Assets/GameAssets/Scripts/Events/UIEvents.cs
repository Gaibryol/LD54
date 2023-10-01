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
}
