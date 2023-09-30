using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rbody;
	private CapsuleCollider2D coll;

	private PlayerControls playerControls;
	private InputAction move;
	private InputAction jump;

	private bool grounded;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	[SerializeField] private float moveSpeed;
	[SerializeField] private float jumpStrength;

	private void Awake()
	{
		rbody = GetComponent<Rigidbody2D>();
		coll = GetComponent<CapsuleCollider2D>();
		playerControls = new PlayerControls();

		grounded = false;
	}

	private void Jump(InputAction.CallbackContext context)
	{
		if (grounded)
		{
			rbody.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
			return;
		}
	}

	private void OnEnable()
	{
		move = playerControls.Player.Move;
		move.Enable();

		jump = playerControls.Player.Jump;
		jump.performed += Jump;
		jump.Enable();
	}

	private void OnDisable()
	{
		move.Disable();

		jump.performed -= Jump;
		jump.Disable();
	}

	private void FixedUpdate()
	{
		Vector2 moveDirection = move.ReadValue<Vector2>();
		rbody.velocity = new Vector2(moveDirection.x * moveSpeed, rbody.velocity.y);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.transform.tag == Constants.GroundTag && Mathf.Abs(collision.GetContact(0).point.y - (transform.position.y - (coll.bounds.size.y * 0.5f))) <= Constants.Player.AllowedCollisionMargin)
		{
			grounded = true;
		}
		else if (Mathf.Abs(collision.GetContact(0).point.y - (transform.position.y + (coll.bounds.size.y * 0.5f))) <= Constants.Player.AllowedCollisionMargin)
		{
			eventBroker.Publish(this, new GameStateEvents.EndGame());
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.transform.tag == Constants.GroundTag)
		{
			grounded = false;
		}
	}
}
