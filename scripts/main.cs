using Godot;
using System;

public class main : Spatial
{
    // Declare member variables here. Examples:
	private string foo = "a simple text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
      System.Console.WriteLine("Hello Godot!");
    }

public override void _Input(InputEvent inputEvent) 
{
  if (inputEvent is InputEventKey keyEvent)
  {
    if (!keyEvent.Pressed) {
      // Switch First Person Camera and Third Person Camera
      if ((KeyList)keyEvent.Scancode == KeyList.C)
      {
          GetNode<Camera>("Car/FP_Camera").Current = GetNode<Camera>("TP_Camera").Current;
          GetNode<Camera>("TP_Camera").Current = !GetNode<Camera>("Car/FP_Camera").Current;
      }
    } else {
      // Quit the game
      if ((KeyList)keyEvent.Scancode == KeyList.Escape)
          GetTree().Quit();
    }
  }

}
// Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }
}
