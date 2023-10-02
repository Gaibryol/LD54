using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rbody;
	private CapsuleCollider2D coll;
	private Animator anim;

	private PlayerControls playerControls;
	private InputAction move;
	private InputAction jump;

	private bool canPressLeftButton;
	private bool canPressRightButton;

	private bool grounded;
	private bool aboutToLand;

	private bool playing;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private Vector3 spawnPos;

	private int numJumped;
	private int numRotate;

	[SerializeField] private float moveSpeed;
	[SerializeField] private float jumpStrength;

	private void Awake()
	{
		rbody = GetComponent<Rigidbody2D>();
		coll = GetComponent<CapsuleCollider2D>();
		anim = transform.GetChild(0).GetComponent<Animator>();
		playerControls = new PlayerControls();

		spawnPos = transform.position;

		Init();
	}

	private void Init()
	{
		canPressLeftButton = true;
		canPressRightButton = true;

		grounded = false;
		aboutToLand = true;

		transform.position = spawnPos;

		numJumped = 0;
		numRotate = 0;

		playing = false;
	}

	private void Jump(InputAction.CallbackContext context)
	{
		if (grounded)
		{
			anim.SetTrigger(Constants.Player.JumpAnimTrigger);
			rbody.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
			eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Jump));

			numJumped += 1;
			if (numJumped >= Constants.Achievements.numTimesToJump)
			{
				eventBroker.Publish(this, new AchievementEvents.EarnAchievement(Constants.Achievements.Jump100Times));
			}
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
		eventBroker.Subscribe<PlayerEvents.AdjustGravity>(AdjustGravityHandler);
		eventBroker.Subscribe<GameStateEvents.StartGame>(StartGameHandler);
		eventBroker.Subscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Subscribe<GameStateEvents.RestartGame>(RestartGameHandler);
		eventBroker.Subscribe<GameStateEvents.SecretEnding>(SecretEndingHandler);
	}

	private void OnDisable()
	{
		move.Disable();

		jump.performed -= Jump;
		jump.Disable();

        eventBroker.Unsubscribe<PlayerEvents.GetPlayerWorldLocation>(GetPlayerWorldLocationHandler);
		eventBroker.Unsubscribe<PlayerEvents.AdjustGravity>(AdjustGravityHandler);
		eventBroker.Unsubscribe<GameStateEvents.StartGame>(StartGameHandler);
		eventBroker.Unsubscribe<GameStateEvents.EndGame>(EndGameHandler);
		eventBroker.Unsubscribe<GameStateEvents.RestartGame>(RestartGameHandler);
		eventBroker.Unsubscribe<GameStateEvents.SecretEnding>(SecretEndingHandler);
	}

	private void StartGameHandler(BrokerEvent<GameStateEvents.StartGame> inEvent)
	{
		playing = true;
	}

	private void RestartGameHandler(BrokerEvent<GameStateEvents.RestartGame> inEvent)
	{
		Init();
	}

	private void EndGameHandler(BrokerEvent<GameStateEvents.EndGame> inEvent)
	{
		playing = false;
	}


	private void SecretEndingHandler(BrokerEvent<GameStateEvents.SecretEnding> obj)
	{
		playing = false;
	}

	private void GetPlayerWorldLocationHandler(BrokerEvent<PlayerEvents.GetPlayerWorldLocation> inEvent)
    {
		inEvent.Payload.WorldPosition = transform.position;
    }

	private void AdjustGravityHandler(BrokerEvent<PlayerEvents.AdjustGravity> inEvent)
	{
		rbody.gravityScale = inEvent.Payload.Amount;
	}

	private void Update()
	{
		// Assign player velocity
		Vector2 moveDirection = move.ReadValue<Vector2>();
		rbody.velocity = new Vector2(moveDirection.x * moveSpeed, rbody.velocity.y);
		transform.GetChild(0).localScale = new Vector2(rbody.velocity.x > 0 ? -1 : 1, 1);
		anim.SetBool(Constants.Player.MovingAnimBool, (moveDirection.x != 0 && grounded));

		// Check for hits above and below
		RaycastHit2D rayUpA = Physics2D.Raycast(new Vector2(transform.position.x - Constants.Player.RayXOffsetA, transform.position.y), Vector2.up, Constants.Player.UpRayDistance, ~LayerMask.GetMask(Constants.Player.Tag));
		RaycastHit2D rayUpB = Physics2D.Raycast(new Vector2(transform.position.x + Constants.Player.RayXOffsetB, transform.position.y), Vector2.up, Constants.Player.UpRayDistance, ~LayerMask.GetMask(Constants.Player.Tag));
		RaycastHit2D rayDownA = Physics2D.Raycast(new Vector2(transform.position.x - Constants.Player.RayXOffsetA, transform.position.y), Vector2.down, Constants.Player.DownRayDistance, ~LayerMask.GetMask(Constants.Player.Tag));
		RaycastHit2D rayDownB = Physics2D.Raycast(new Vector2(transform.position.x + Constants.Player.RayXOffsetB, transform.position.y), Vector2.down, Constants.Player.DownRayDistance, ~LayerMask.GetMask(Constants.Player.Tag));

		//Debug.DrawLine(new Vector2(transform.position.x - Constants.Player.RayXOffsetA, transform.position.y), new Vector2(transform.position.x - Constants.Player.RayXOffsetA, transform.position.y + Constants.Player.UpRayDistance), Color.red, 10f);
		//Debug.DrawLine(new Vector2(transform.position.x + Constants.Player.RayXOffsetB, transform.position.y), new Vector2(transform.position.x + Constants.Player.RayXOffsetB, transform.position.y + Constants.Player.UpRayDistance), Color.red, 10f);

		//Debug.DrawLine(new Vector2(transform.position.x - Constants.Player.RayXOffsetA, transform.position.y), new Vector2(transform.position.x - Constants.Player.RayXOffsetA, transform.position.y - Constants.Player.DownRayDistance), Color.cyan, Mathf.Infinity);
		//Debug.DrawLine(new Vector2(transform.position.x + Constants.Player.RayXOffsetB, transform.position.y), new Vector2(transform.position.x + Constants.Player.RayXOffsetB, transform.position.y - Constants.Player.DownRayDistance), Color.cyan, Mathf.Infinity);

		// Grounded logic
		grounded = rayDownA.collider?.tag == Constants.GroundTag || rayDownB.collider?.tag == Constants.GroundTag;
		anim.SetBool(Constants.Player.GroundedAnimBool, grounded);

		if (!grounded)
		{
			aboutToLand = true;
		}

		if (aboutToLand && grounded)
		{
			aboutToLand = false;
			anim.SetTrigger(Constants.Player.LandAnimTrigger);
		}

		if (grounded && (rayUpA.collider?.tag == Constants.GroundTag || rayUpB.collider?.tag == Constants.GroundTag) && (rayUpA.collider?.GetComponent<Block>().isMoving == true || rayUpB.collider?.GetComponent<Block>().isMoving == true))
		{
			// Player collided with the bottom of an object while grounded
			if (playing)
			{
				eventBroker.Publish(this, new GameStateEvents.EndGame());
				eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Death));
				//Debug.Log("Block: " + rayUpA.collider?.gameObject.name + " or " + rayUpB.collider?.gameObject.name);
				//Debug.Log("Moving: " + rayUpA.collider?.GetComponent<Block>().isMoving + " or " + rayUpB.collider?.GetComponent<Block>().isMoving);
				//Debug.Log("Grounded: " + grounded);
				//Debug.DrawLine(new Vector2(transform.position.x - Constants.Player.RayXOffsetA, transform.position.y), new Vector2(transform.position.x - Constants.Player.RayXOffsetA, transform.position.y + Constants.Player.UpRayDistance), Color.green, Mathf.Infinity);
				//Debug.DrawLine(new Vector2(transform.position.x + Constants.Player.RayXOffsetB, transform.position.y), new Vector2(transform.position.x + Constants.Player.RayXOffsetB, transform.position.y + Constants.Player.UpRayDistance), Color.green, Mathf.Infinity);
				//Debug.DrawLine(new Vector2(transform.position.x - Constants.Player.RayXOffsetA, transform.position.y), new Vector2(transform.position.x - Constants.Player.RayXOffsetA, transform.position.y - Constants.Player.DownRayDistance), Color.cyan, Mathf.Infinity);
				//Debug.DrawLine(new Vector2(transform.position.x + Constants.Player.RayXOffsetB, transform.position.y), new Vector2(transform.position.x + Constants.Player.RayXOffsetB, transform.position.y - Constants.Player.DownRayDistance), Color.cyan, Mathf.Infinity);
				//Debug.Break();
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.tag == Constants.LeftButtonTag)
		{
			// Rotate block counter clockwise
			if (canPressLeftButton)
			{
				collision.transform.GetComponent<Animator>().SetTrigger(Constants.ButtonAnimTrigger);
				StartCoroutine(StartLeftButtonCooldown());
				eventBroker.Publish(this, new TetrisEvents.RotatePreviewBlock(false));
				eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.RotateCounterclockwise));

				if (playing)
				{
					numRotate += 1;
					if (numRotate >= Constants.Achievements.numTimesToRotate)
					{
						eventBroker.Publish(this, new AchievementEvents.EarnAchievement(Constants.Achievements.Rotate50Times));
					}
				}
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
				eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.RotateClockwise));

				if (playing)
				{
					numRotate += 1;
					if (numRotate >= Constants.Achievements.numTimesToRotate)
					{
						eventBroker.Publish(this, new AchievementEvents.EarnAchievement(Constants.Achievements.Rotate50Times));
					}
				}
			}
        }
	}
}
