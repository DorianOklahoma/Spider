using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public int health { get; set; }
	[Export]
	public bool invulnerable = true;
	
	public const float Speed = 300.0f;
	public const float JumpVelocity = -700.0f;
	
	private AnimatedSprite2D animatedSprite;
	private Timer invulnerabilityTimer;
	private GpuParticles2D bitEffect;
	private Hud HUD;
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		invulnerabilityTimer = GetNode<Timer>("Invulnerability");
		bitEffect = GetNode<GpuParticles2D>("BitEffect");
		HUD = GetNode<Hud>("/root/main/CanvasLayer/HUD");
		HUD.SetHealth(health);
	}
	
	public override void _Process(double delta)
	{
		if (health <= 0)
		{
			Die();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add gravity
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle jump
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		float direction = Input.GetAxis("move_left", "move_right");

		// Horizontal movement
		if (direction != 0)
		{
			velocity.X = direction * Speed;

			// Flip sprite based on direction
			animatedSprite.FlipH = direction < 0;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
		}
		
		if (!IsOnFloor())
		{
			if (velocity.Y < 0 - 80)
			{
				animatedSprite.Play("up");
			}
			else if (velocity.Y < 0 + 80)
			{
				animatedSprite.Play("idle");
			}
			else
			{
				animatedSprite.Play("down");
			}
		}
		else if (direction != 0)
		{
			animatedSprite.Play("walk");
		}
		else
		{
			animatedSprite.Play("idle");
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	
	private void _on_invulnerability_timeout()
	{
		invulnerable = false;
		HUD.invulnerabilityTexture.Hide();
		GD.Print("No longer invulnerabile");
	}
	
	public void TakeDamage(int damage)
	{
		if (invulnerable)
		{
			return;
		}
		health -= damage;
		invulnerable = true;
		invulnerabilityTimer.Start();
		HUD.invulnerabilityTexture.Show();
		bitEffect.Restart();
		HUD.SetHealth(health);
		GD.Print("Player hit! Health: " + health);
	}
	
	public void AddHealth(int HP)
	{
		health += HP;
		HUD.SetHealth(health);
		GD.Print("Player healed! Health: " + health);
	}
	
	private void Die()
	{
		HUD.invulnerabilityTexture.Hide();
		QueueFree();
	}
}
