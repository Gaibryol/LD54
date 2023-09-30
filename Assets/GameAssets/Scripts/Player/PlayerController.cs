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

	[SerializeField] private float moveSpeed;
	[SerializeField] private float jumpStrength;

	private void Awake()
	{
		rbody = GetComponent<Rigidbody2D>();
		coll = GetComponent<CapsuleCollider2D>();
		playerControls = new PlayerControls();
	}

	private void Jump(InputAction.CallbackContext context)
	{
		rbody.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
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
}
