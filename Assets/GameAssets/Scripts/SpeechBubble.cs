using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] private GameObject startButton;
    [SerializeField] private Vector2 offset;

    private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();

    private Animator animator;
    private PlayerEvents.GetPlayerWorldLocation playerLocation;
    private void Start()
    {
        playerLocation = new PlayerEvents.GetPlayerWorldLocation();
        animator = GetComponent<Animator>();
        startButton.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            float nextTime;
            if (normalizedTime < 1f/3f)
            {
                nextTime = 1f / 3f;
            } else if (normalizedTime < 2f/3f)
            {
                nextTime = 2f / 3f;
            } else
            {
                nextTime = 1f;
            }
            animator.Play("Speech", 0, nextTime);
        }
    }

    private void FixedUpdate()
    {
        eventBrokerComponent.Publish(this, playerLocation);
        transform.position = playerLocation.WorldPosition + (Vector3)offset;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            startButton.SetActive(true);
            Destroy(gameObject);
        }
    }
}
