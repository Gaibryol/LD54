
public partial class Constants
{
    public class Player
	{
		public const string Tag = "Player";
		public const string JumpAnimTrigger = "Jump";
		public const string LandAnimTrigger = "Land";
		public const string MovingAnimBool = "IsMoving";
		public const string GroundedAnimBool = "IsGrounded";
		public const string Highscore = "Highscore";

		public const float AllowedCollisionMargin = 0.025f;
		public const float JumpCooldown = 0.154f;

		public const float RayXOffsetA = 0.12f;
		public const float RayXOffsetB = 0.12f;
		public const float UpRayDistance = 0.28f;
		public const float DownRayDistance = 0.25f;

		public const int LinesCleared1 = 100;
		public const int LinesCleared2 = 250;
		public const int LinesCleared3 = 500;
		public const int LinesCleared4 = 1000;
		public const int SecretEndingPoints = 2500;

		public const float SecretEndingTiming = 5f;

		public const float GravityScale = 1.25f;
	}
}
