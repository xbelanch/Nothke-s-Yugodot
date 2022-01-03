using Godot;
using System;
using Utility;

public class CarController : RigidBody
{
    public override void _Ready()
    {

    }

    public float torqueMult = 10;
    public float forceMult = 10;
    
    public override void _PhysicsProcess(float dt)
    {
        float xInput =
            Input.IsKeyPressed((int)KeyList.A) ? -1 :
            (Input.IsKeyPressed((int)KeyList.D) ? 1 : 0);
        float yInput =
            Input.IsKeyPressed((int)KeyList.S) ? -1 :
            (Input.IsKeyPressed((int)KeyList.W) ? 1 : 0);

        // Drag shit:
        AddTorque(-Transform.basis.y * xInput * torqueMult);
        float speed = LinearVelocity.Length();
        // float forceFactor = Math.Lerp(10, 0, speed);
        // float forceFactor = Mathf.InverseLerp(10, 0, speed);
        AddCentralForce(Transform.basis.z * yInput * forceMult);
    }
}
