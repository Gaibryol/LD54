
public class GameStateEvents
{
	public class StartGame
	{
		public StartGame(GameManager gameManager)
		{
			GameManager = gameManager;
		}

		public readonly GameManager GameManager;
	}

	public class EndGame
	{

	}

	public class RestartGame
	{

	}
}
