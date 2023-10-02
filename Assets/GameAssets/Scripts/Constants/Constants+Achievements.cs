
using UnityEngine;

public partial class Constants
{
    public class Achievements
	{
		public const string Combo20Times = "Combo20Times";
		public const string EarnBigCombo = "EarnBigCombo";
		public const string Earn1000Points = "Earn1000Points";
		public const string Earn2000Points = "Earn2000Points";
		public const string Earn3000Points = "Earn3000Points";
		public const string Escaped = "Escaped";
		public const string Jump100Times = "Jump100Times";
		public const string Rotate50Times = "Rotate50Times";

		public const int numTimesToCombo = 20;
		public const int numTimesToJump = 100;
		public const int numTimesToRotate = 50;
		public const int numPointsA = 1000;
		public const int numPointsB = 2000;
		public const int numPointsC = 3000;

		public const string Combo20TimesDescription = "Perform a combo 20 times in a single game";
		public const string EarnBigComboDescription = "Clear 4 lines at once";
		public const string Earn1000PointsDescription = "Earn at least 1000 points in a single game";
		public const string Earn2000PointsDescription = "Earn at least 2000 points in a single game";
		public const string Earn3000PointsDescription = "Earn at least 3000 points in a single game";
		public const string EscapedDescription = "Escape the game";
		public const string Jump100TimesDescription = "Jump at least 100 times in a single game";
		public const string Rotate50TimesDescription = "Rotate blocks at least 100 times in a single game";

		public static readonly Vector2 AchievementBoxOffset = new Vector2(0f, 100);
	}
}
