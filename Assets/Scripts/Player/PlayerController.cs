﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ThemeChange))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private float maxSpeed = 0;
	[SerializeField] private float speedToleration = 0;
	[SerializeField] private float accelerationTime = 0;
	[SerializeField] private float slowingTime = 0;
	[SerializeField] private float maxJumpTime = 0;
	[SerializeField] private float jumpSpeed = 0;
	[SerializeField] private float gravityScale = 0;
	[SerializeField] private float groundCheckRadius = 0;
	[SerializeField] private Transform groundCheck = null;
	[SerializeField] private GameMaster master = null;

	private Rigidbody2D Rb { get; set; }
	private float Direction { get; set; }
	private float CurrentJumpTime { get; set; }
	private bool Jump { get; set; }
	private bool IsGrounded { get; set; }
	private ThemeChange ThemeController { get; set; }
	private SpriteRenderer SpriteRenderer { get; set; }

	private void Awake()
	{
		Rb = GetComponent<Rigidbody2D>();
		ThemeController = GetComponent<ThemeChange>();
		SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		Direction = Input.GetAxisRaw("Horizontal");
		if (IsGrounded || Jump)
		{
			Jump = Input.GetButton("Jump");
		}
		SpriteRenderer.flipX = Rb.velocity.x == 0 ? SpriteRenderer.flipX : Rb.velocity.x < 0;
	}

	private void FixedUpdate()
	{
		IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, LayerMask.GetMask(ThemeController.mode == ThemeChange.Mode.Dark ? "Black" : "White", "Neutral"));

		if (IsGrounded)
		{
			CurrentJumpTime = 0;
		}

		Rb.velocity = new Vector2(Mathf.Clamp(Rb.velocity.x, -maxSpeed, maxSpeed), Rb.velocity.y);
		if (Mathf.Abs(Rb.velocity.x) < speedToleration)
		{
			Rb.velocity = new Vector2(0, Rb.velocity.y);
		}
		if (Direction != 0)
		{
			Rb.AddForce(new Vector2(Rb.mass * (maxSpeed / accelerationTime), 0) * Direction);
		}
		else
		{
			Rb.AddForce(new Vector2(-Rb.mass * (Rb.velocity.x / slowingTime), 0));
		}

		if (Jump && CurrentJumpTime <= maxJumpTime - (jumpSpeed / (-Physics2D.gravity.y * Rb.gravityScale) + maxJumpTime / 2.0f))
		{
			Rb.gravityScale = 0;
			Rb.velocity = new Vector2(Rb.velocity.x, jumpSpeed);
			CurrentJumpTime += Time.fixedDeltaTime;
		}
		else
		{
			Rb.gravityScale = gravityScale;
		}
	}

	public void Die()
	{
		transform.position = master.lastCheckPointPos;
	}
}
