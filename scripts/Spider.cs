using Godot;
using System;

public partial class Spider : CharacterBody2D
{
	[Export]
	public CharacterBody2D Target { get; set; }
	[Export]
	public Node2D Home { get; set; }

	public const float Speed = 100.0f;

	private AnimatedSprite2D animatedSprite;
	private NavigationAgent2D _navigationAgent;
	private Timer _attackCooldown;

	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animatedSprite.Play("idle");
		GetNode<Area2D>("AttackArea").BodyEntered += OnBodyEntered;
		_navigationAgent = GetNode<NavigationAgent2D>("Navigation/NavigationAgent2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (animatedSprite.Animation != "bite" && animatedSprite.Animation != "walk")
		{
			if (Velocity != Vector2.Zero)
			{
				animatedSprite.Play("walk");
			}
			else
			{
				animatedSprite.Play("idle");
			}
		}
		
		if (_navigationAgent == null)
			return;
			
		if (Target == null)
		{
			if (Home == null)
			{
				QueueFree();
			}
		}

		Vector2 direction = _navigationAgent.GetNextPathPosition() - GlobalPosition;

		if (direction.Length() > 1.0f)
		{
			Vector2 desiredVelocity = direction.Normalized() * Speed;
			Velocity = Velocity.Lerp(desiredVelocity, 0.1f);
			
			float targetAngle = direction.Angle();
			float turnSpeed = 5f;
			Rotation = Mathf.LerpAngle(Rotation, targetAngle, turnSpeed * (float)delta);
			
			MoveAndSlide();
		}
		else
		{
			Velocity = Vector2.Zero;
		}
	}

	private void OnTimerTimeout()
	{
		if (IsInstanceValid(Target) && Target.IsInsideTree())
		{
			if (_navigationAgent != null && Target != null)
			{
				_navigationAgent.TargetPosition = Target.GlobalPosition;
			}
		}
		else
		{
			_navigationAgent.TargetPosition = Home.GlobalPosition;
		}
	}
	
	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("player"))
		{
			animatedSprite.Play("bite");
			Player player = body as Player;
			if (player != null)
			{
				player.TakeDamage(1);
			}
		}
	}
	
	private void animation_finished()
	{
		if (animatedSprite.Animation == "bite")
		{
			animatedSprite.Play("idle");
		}
	}
}
