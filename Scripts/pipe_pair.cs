using Godot;
using System;

public partial class pipe_pair : Node2D
{
	[Export] public float Speed = 160f;    
	[Export] public float OffscreenX = -200f; 
	private Area2D _scoreArea;
	
	 private AudioStreamPlayer2D scoreSound;

	public override void _Ready()
	{
		scoreSound = GetNode<AudioStreamPlayer2D>("ScoreSound");
		
		_scoreArea = GetNode<Area2D>("ScoreArea");
		_scoreArea.AreaEntered += OnScoreAreaAreaEntered;
	}

	public override void _Process(double delta)
	{
		// Move left
		Position += new Vector2(-Speed * (float)delta, 0);

		// If the pipe goes off-screen to the left, free it
		if (Position.X < OffscreenX)
			QueueFree();
	}

	private void OnScoreAreaAreaEntered(Area2D area)
	{
		// Check if the entered area is the Bird
		if (area.Name == "Bird")
		{
			scoreSound.Play();
			
			GD.Print("Bird entered score area! Score should increase now");

			// Call Main.AddScore
			var main = GetTree().Root.GetNodeOrNull<Main>("Main");
			main?.AddScore();

			// Disconnect to prevent double counting
			_scoreArea.AreaEntered -= OnScoreAreaAreaEntered;
		}
	}
}
