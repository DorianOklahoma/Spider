using Godot;
using System;

public partial class Hud : Control
{
	private Label healthText;
	public TextureRect invulnerabilityTexture;
	
	public override void _Ready()
	{
		healthText = GetNode<Label>("Heart/Health");
		invulnerabilityTexture = GetNode<TextureRect>("Invulnerability");
	}

	public void SetHealth(int health)
	{
		healthText.Text = health.ToString();
	}
	
}
