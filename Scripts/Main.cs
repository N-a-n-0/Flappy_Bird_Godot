using Godot;
using System;

public partial class Main : Node2D
{
	 
	[Export] public PackedScene PipeScene;
	[Export] public float SpawnInterval = 1.6f;
	[Export] public float PipeSpawnX = 600f;
	[Export] public float MinY = -80f;
	[Export] public float MaxY = 80f;
	[Export] public Vector2 BirdStartPosition = new Vector2(-120, 0);

	private Node2D world;
	private Bird bird;
	private Timer spawnTimer;
	private Label scoreLabel;
	private Label startLabel;
	private Control gameOverPanel;

	private enum GameState { StartScreen, Playing, GameOver }
	private GameState state = GameState.StartScreen;

	private int score = 0;

	public override void _Ready()
	{
		 

		world = GetNode<Node2D>("World");
		spawnTimer = GetNode<Timer>("SpawnTimer");
		scoreLabel = GetNode<Label>("UI/ScoreLabel");
		startLabel = GetNode<Label>("UI/StartLabel");
		gameOverPanel = GetNode<Control>("UI/GameOverPanel");
		gameOverPanel.Visible = false;

		 
		var birdPacked = (PackedScene)GD.Load("res://Scenes/Bird.tscn");  
		var birdInstance = birdPacked.Instantiate<Area2D>();
		world.AddChild(birdInstance);
		birdInstance.Position = BirdStartPosition;
		bird = birdInstance;

		 
		spawnTimer.WaitTime = SpawnInterval;
		spawnTimer.OneShot = false;
		spawnTimer.Timeout += OnSpawnTimerTimeout;

		 

		UpdateUI();
	}

	 

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("Flap"))
		{
			if (state == GameState.StartScreen)
			{
				StartGame();
			}
			else if (state == GameState.Playing)
			{
				 //Da Game be happening
			}
			else if (state == GameState.GameOver)
			{
				 
				RestartGame();
			}
		}
	}

	private void StartGame()
	{
		state = GameState.Playing;
		startLabel.Visible = false;
		score = 0;
		UpdateUI();

		// Reset bird and position
		bird.ResetBird(BirdStartPosition);

		// Start spawning
		spawnTimer.Start();
	}

	private void OnSpawnTimerTimeout()
	{
		SpawnPipePair();
	}

	private void SpawnPipePair()
	{
		var pipe = PipeScene.Instantiate<Node2D>();
		
		float y = (float)GD.RandRange(MinY, MaxY);
		pipe.Position = new Vector2(PipeSpawnX, y);

		world.AddChild(pipe);
	}

	 
	public void AddScore()
	{
		if (state != GameState.Playing) return;
		score++;
		UpdateUI();
	}

	private void UpdateUI()
	{
		GD.Print("UI SHOULD BE UPDATE!");
		scoreLabel.Text = score.ToString();
		startLabel.Visible = (state == GameState.StartScreen);
		gameOverPanel.Visible = (state == GameState.GameOver);
	}

	 
	public void OnPlayerDied()
	{
		if (state != GameState.Playing) return;

		state = GameState.GameOver;
		spawnTimer.Stop();
		UpdateUI();
		gameOverPanel.Visible = true;
	}

	 
	public void OnReplayPressed()
	{
		RestartGame();
	}

	private void RestartGame()
	{
		 
		foreach (Node child in world.GetChildren())
		{
			if (child is Node2D node && node != bird)
			{
				node.QueueFree();
			}
		}

		 
		state = GameState.StartScreen;
		startLabel.Visible = true;
		gameOverPanel.Visible = false;

		 
		bird.ResetBird(BirdStartPosition);

		score = 0;
		UpdateUI();
	}
}
