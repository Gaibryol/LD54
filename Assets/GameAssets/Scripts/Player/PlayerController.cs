using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rbody;
	private BoxCollider2D coll;
	private Animator anim;

	private PlayerControls playerControls;
	private InputAction move;
	private InputAction jump;

	private bool canPressLeftButton;
	private bool canPressRightButton;

	private bool grounded;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private Vector3 spawnPos;

	[SerializeField] private float moveSpeed;
	[SerializeField] private float jumpStrength;

	private void Awake()
	{
		rbody = GetComponent<Rigidbody2D>();
		coll = GetComponent<BoxCollider2D>();
		anim = GetComponent<Animator>();
		playerControls = new PlayerControls();

		spawnPos = transform.position;

		Init();
	}

	private void Init()
	{
		canPressLeftButton = true;
		canPressRightButton = true;

		grounded = false;

		transform.position = spawnPos;
	}

	private void Jump(InputAction.CallbackContext context)
	{
		if (grounded)
		{
			grounded = false;
			anim.SetTrigger(Constants.Player.JumpAnimTrigger);
			rbody.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
			return;
		}
	}

	private IEnumerator StartLeftButtonCooldown()
	{
		canPressLeftButton = false;
		yield return new WaitForSeconds(Constants.ButtonCooldown);
		canPressLeftButton = true;
	}

	private IEnumerator StartRightButtonCooldown()
	{
		canPressRightButton = false;
		yield return new WaitForSeconds(Constants.ButtonCooldown);
		canPressRightButton = true;
	}

	private void OnEnable()
	{
		move = playerControls.Player.Move;
		move.Enable();

		jump = playerControls.Player.Jump;
		jump.performed += Jump;
		jump.Enable();

		eventBroker.Subscribe<PlayerEvents.GetPlayerWorldLocation>(GetPlayerWorldLocationHandler);
		eventBroker.Subscribe<GameStateEvents.RestartGame>(RestartGameHandler);
	}

	private void OnDisable()
	{
		move.Disable();

		jump.performed -= Jump;
		jump.Disable();

        eventBroker.Unsubscribe<PlayerEvents.GetPlayerWorldLocation>(GetPlayerWorldLocationHandler);
		eventBroker.Unsubscribe<GameStateEvents.RestartGame>(RestartGameHandler);
	}

	private void RestartGameHandler(BrokerEvent<GameStateEvents.RestartGame> inEvent)
	{
		Init();
	}

	private void GetPlayerWorldLocationHandler(BrokerEvent<PlayerEvents.GetPlayerWorldLocation> inEvent)
    {
		inEvent.Payload.WorldPosition = transform.position;
    }

    private void FixedUpdate()
	{
		// Assign player velocity
		Vector2 moveDirection = move.ReadValue<Vector2>();
		rbody.velocity = new Vector2(moveDirection.x * moveSpeed, rbody.velocity.y);
		transform.localScale = new Vector2(rbody.velocity.x > 0 ? -1 : 1, 1);
		anim.SetBool(Constants.Player.MovingAnimBool, (moveDirection.x != 0 && grounded));
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		RaycastHit2D rayUpA = Physics2D.Raycast(new Vector2(transform.position.x - (coll.bounds.size.x / 2f), transform.position.y), Vector2.up, 0.3f, ~LayerMask.GetMask(Constants.Player.Tag));
		RaycastHit2D rayUpB = Physics2D.Raycast(new Vector2(transform.position.x + (coll.bounds.size.x / 2f), transform.position.y), Vector2.up, 0.3f, ~LayerMask.GetMask(Constants.Player.Tag));
		RaycastHit2D rayDownA = Physics2D.Raycast(new Vector2(transform.position.x - (coll.bounds.size.x / 2f), transform.position.y), Vector2.down, 0.3f, ~LayerMask.GetMask(Constants.Player.Tag));
		RaycastHit2D rayDownB = Physics2D.Raycast(new Vector2(transform.position.x + (coll.bounds.size.x / 2f), transform.position.y), Vector2.down, 0.3f, ~LayerMask.GetMask(Constants.Player.Tag));

		if (rayUpA.collider != null)
		{
			if (rayUpA.collider.tag == Constants.GroundTag)
			{
				// Player collided with the bottom of an object while grounded
				eventBroker.Publish(this, new GameStateEvents.EndGame());
			}
		}
		else if (rayUpB.collider != null)
		{
			if (rayUpB.collider.tag == Constants.GroundTag)
			{
				// Player collided with the bottom of an object while grounded
				eventBroker.Publish(this, new GameStateEvents.EndGame());
			}
		}

		if (rayDownA.collider != null)
		{
			if (rayDownA.collider.tag == Constants.GroundTag)
			{
				// Player collided with the top of an object
				anim.SetTrigger(Constants.Player.LandAnimTrigger);
				grounded = true;
			}
		}
		else if (rayDownB.collider != null)
		{
			if (rayDownB.collider.tag == Constants.GroundTag)
			{
				// Player collided with the top of an object
				anim.SetTrigger(Constants.Player.LandAnimTrigger);
				grounded = true;
			}
		}

		if (collision.transform.tag == Constants.LeftButtonTag)
		{
			// Rotate block counter clockwise
			if (canPressLeftButton)
			{
				collision.transform.GetComponent<Animator>().SetTrigger(Constants.ButtonAnimTrigger);
				StartCoroutine(StartLeftButtonCooldown());
				eventBroker.Publish(this, new TetrisEvents.RotatePreviewBlock(false));
			}
		}
		else if (collision.transform.tag == Constants.RightButtonTag)
		{
			// Rotate block clockwise
			if (canPressRightButton)
			{
				collision.transform.GetComponent<Animator>().SetTrigger(Constants.ButtonAnimTrigger);
				StartCoroutine(StartRightButtonCooldown());
                eventBroker.Publish(this, new TetrisEvents.RotatePreviewBlock(true));
            }
        }
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.transform.tag == Constants.GroundTag)
		{
			grounded = true;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.transform.tag == Constants.GroundTag && grounded)
		{
			grounded = false;
		}
	}
}
