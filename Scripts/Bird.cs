using Godot;
using System;

public partial class Bird : Area2D
{
	 [Export] public float Gravity = 900f;         
	[Export] public float FlapStrength = 350f;  
	[Export] public float MaxFallSpeed = 1000f;

	private Vector2 velocity = Vector2.Zero;
	private bool isAlive = true;

 private AudioStreamPlayer2D jumpSound;
 private AudioStreamPlayer2D dieSound;
	 
	private Node? gameManager;

	public override void _Ready()
	{
			jumpSound = GetNode<AudioStreamPlayer2D>("JumpSound");
			dieSound = GetNode<AudioStreamPlayer2D>("DieSound");
		gameManager = GetTree().Root.GetNodeOrNull("Main");
		
		 
		BodyEntered += OnBodyEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isAlive) return;

		float dt = (float)delta;

		 
		if (Input.IsActionJustPressed("Flap"))
		{
			 
			velocity.Y = -FlapStrength;
			  jumpSound.Play();
		}

	 
		velocity.Y += Gravity * dt;

		 
		if (velocity.Y > MaxFallSpeed) velocity.Y = MaxFallSpeed;

		 
		Position += velocity * dt;

		 
		Rotation = Mathf.Clamp(velocity.Y / 600f, -0.6f, 0.7f);
	}

	private void OnBodyEntered(Node body)
	{
		GD.Print("The Bird has Collided with something I think?");
		if (!isAlive) return;

		isAlive = false;

		dieSound.Play();
		 
		if (gameManager != null && gameManager.HasMethod("OnPlayerDied"))
		{
			gameManager.Call("OnPlayerDied");
		}
			 

	 
	}

	 
	public void ResetBird(Vector2 startPosition)
	{
		Position = startPosition;
		velocity = Vector2.Zero;
		Rotation = 0;
		isAlive = true;
	}

	public bool IsAlive() {return isAlive;}
}
