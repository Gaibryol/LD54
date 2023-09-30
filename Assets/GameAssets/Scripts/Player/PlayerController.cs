using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	private Rigidbody2D rbody;
	private CapsuleCollider2D coll;

	private PlayerControls playerControls;
	private InputAction moveLeft;
	private InputAction moveRight;
	private InputAction jump;

	private void Awake()
	{
		rbody = GetComponent<Rigidbody2D>();
		coll = GetComponent<CapsuleCollider2D>();
		playerControls = new PlayerControls();
	}

	private void MoveLeft(InputAction.CallbackContext context)
	{
		Debug.Log("left");
		rbody.AddForce(new Vector2(-5, 0));
	}

	private void MoveRight(InputAction.CallbackContext context)
	{
		Debug.Log("right");
		rbody.AddForce(new Vector2(5, 0));
	}

	private void Jump(InputAction.CallbackContext context)
	{
		Debug.Log("Jump");
		rbody.AddForce(new Vector2(0, 5));
	}

	private void OnEnable()
	{
		moveLeft = playerControls.Player.Left;
		moveLeft.performed += MoveLeft;
		moveLeft.Enable();

		moveRight = playerControls.Player.Right;
		moveRight.performed += MoveRight;
		moveRight.Enable();

		jump = playerControls.Player.Jump;
		jump.performed += Jump;
		jump.Enable();
	}

	private void OnDisable()
	{
		moveLeft.performed -= MoveLeft;
		moveLeft.Disable();

		moveRight.performed -= MoveRight;
		moveRight.Disable();

		jump.performed -= Jump;
		jump.Disable();
	}
}
