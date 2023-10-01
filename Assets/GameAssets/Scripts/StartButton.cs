using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private Animator animator;
    private Vector3 startLocation;

    private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        startLocation = transform.position;
    }

    private void OnEnable()
    {
        eventBrokerComponent.Subscribe<GameStateEvents.RestartGame>(RestartGameHandler);
    }

    private void OnDisable()
    {
        eventBrokerComponent.Unsubscribe<GameStateEvents.RestartGame>(RestartGameHandler);
    }

    private void RestartGameHandler(BrokerEvent<GameStateEvents.RestartGame> @event)
    {
        transform.position = startLocation;
    }

    private void Update()
    {
        if (animator == null) return;
        if (!animator.enabled) return;

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            transform.position = new Vector3(100, 100, 0);
            animator.Rebind();
            animator.Update(0);
            animator.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() == null) return;
        animator.enabled = true;
        gameManager.StartGame();
    }
}
