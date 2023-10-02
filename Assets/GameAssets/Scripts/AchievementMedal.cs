using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AchievementMedal : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private string achievement;

	private bool isHovering = false;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (isHovering) return;

		isHovering = true;
		eventBroker.Publish(this, new UIEvents.ShowAchievementDescription(achievement, transform.position));
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!isHovering) return;

		isHovering = false;
		eventBroker.Publish(this, new UIEvents.HideAchievementDescription());
	}
}
