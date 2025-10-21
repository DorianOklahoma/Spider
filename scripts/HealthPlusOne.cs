using Godot;
using System;

public partial class HealthPlusOne : RigidBody2D
{
	private GpuParticles2D brokenParticles;
	private GpuParticles2D idleParticles;
	public override void _Ready()
	{
		brokenParticles = GetNode<GpuParticles2D>("BrokenParticles");
		idleParticles = GetNode<GpuParticles2D>("IdleParticles");
	}
	
	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("player"))
		{
			Player player = body as Player;
			player.AddHealth(1);
			idleParticles.Emitting = false;
			brokenParticles.Restart();
			HideObject();
		}
	}
	
	private void HideObject()
	{
		Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
		CollisionShape2D collision = GetNode<CollisionShape2D>("CollisionShape2D");
		Area2D area = GetNode<Area2D>("Area2D");
		sprite.Hide();
		collision.Hide();
		area.Hide();
	}
	
	private void BrokenParticlesFinished()
	{
		QueueFree();
	}
}
