
using System;
using System.Collections.Generic;

public class AchievementEvents
{
    public class EarnAchievement
	{
		public EarnAchievement(string achievement)
		{
			Achievement = achievement;
		}

		public readonly string Achievement;
	}

	public class GetAchievements
	{
		public GetAchievements(Action<Dictionary<string, bool>> achievements)
		{
			Achievements = achievements;
		}

		public readonly Action<Dictionary<string, bool>> Achievements;
	}
}
