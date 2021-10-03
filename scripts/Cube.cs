using Godot;
using System;

public class Cube : RigidBody
{
    public float forceMult = 10;
    public float torqueMult = 5;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
            
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
    float xInput = 
        Input.IsKeyPressed((int)KeyList.A) ? -1 :
        (Input.IsKeyPressed((int)KeyList.D) ? 1 : 0);    
    float yInput = 
        Input.IsKeyPressed((int)KeyList.S) ? -1 :
        (Input.IsKeyPressed((int)KeyList.W) ? 1 : 0);

    AddTorque(-Transform.basis.y * xInput * torqueMult);
    // AddCentralForce(-LinearVelocity * (1 - delta)); // friction with the ground?

    // Drag shit
    float speed = LinearVelocity.Length();
    float forceFactor = Mathf.InverseLerp(10, 0, speed);
    AddCentralForce(-Transform.basis.z * yInput * forceMult * forceFactor); 
 }
}
