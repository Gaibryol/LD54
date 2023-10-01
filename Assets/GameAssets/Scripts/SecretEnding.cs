using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretEnding : MonoBehaviour
{
	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<PlayerController>() == null) return;

		eventBroker.Publish(this, new GameStateEvents.SecretEnding());
		eventBroker.Publish(this, new AchievementEvents.EarnAchievement(Constants.Achievements.Escaped));
	}
}
