using Godot;
using System;
using Utility;

public class CreateForcer : RigidBody
{
    public float forceMult = 10;
    public float torqueMult = 10;

    public MeshInstance wheel_front_left;
    public MeshInstance wheel_front_right;
    public MeshInstance wheel_rear_left;
    public MeshInstance wheel_rear_right;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        wheel_front_left = GetNode<Godot.MeshInstance>("car_model/car_body/wheel_front_left");
        wheel_front_right = GetNode<Godot.MeshInstance>("car_model/car_body/wheel_front_right");
        wheel_rear_left = GetNode<Godot.MeshInstance>("car_model/car_body/wheel_rear_left");
        wheel_rear_right = GetNode<Godot.MeshInstance>("car_model/car_body/wheel_rear_right");
        GD.Print("Wheel tansform: " + wheel_front_left.Translation);

        var line3D = new LineDrawer3D();
        line3D.addLine(new Vector3(1, 1, 1), Vector3.One * 10);
        
        AddChild(line3D);
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
    // float forceFactor = Mathf.InverseLerp(10, 0, speed);
    var state = GetWorld().DirectSpaceState;
    var up = Transform.basis.y;
    var dict = state.IntersectRay(Transform.origin + up * 0.2f, Transform.origin - up);
    if (dict.Count > 0) {
        var objCollider = dict["collider"];
    }

    // if (dict.Count > 0) {
    //         GD.Print("Hitting: " + objCollider);
    //     } else {
    //         GD.Print("Not hitting");
    //     }

    AddCentralForce(Transform.basis.z * yInput * forceMult); 
 }
}
