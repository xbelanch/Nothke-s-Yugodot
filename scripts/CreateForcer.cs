using Godot;
using System;
using Utility;

public class CreateForcer : RigidBody
{
    public float forceMult = 10;
    public float torqueMult = 10;

    public MeshInstance car_body;
    public MeshInstance wheel_front_left;
    public MeshInstance wheel_front_right;
    public MeshInstance wheel_rear_left;
    public MeshInstance wheel_rear_right;

    public float wheelBase = 1.06f;
    public float wheelTrack = 0.667f;
    Vector3[] points = new Vector3[4];


    private LineDrawer3D line3D;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // create the points
        //  = new Vector3(wheelTrack, 0, wheelBase);

        // car_body = GetNode<Godot.MeshInstance>("car/car_body");
        points[0] = GetNode<MeshInstance>("car_model/wheel_front_left").Translation;
        points[1] = GetNode<MeshInstance>("car_model/wheel_front_right").Translation;
        points[2] = GetNode<MeshInstance>("car_model/wheel_rear_left").Translation;
        points[3] = GetNode<MeshInstance>("car_model/wheel_rear_right").Translation;

        line3D = new LineDrawer3D();

        // Create a material to change line3D color to red
        var mat = new SpatialMaterial();
        mat.FlagsUsePointSize = true;
        mat.VertexColorUseAsAlbedo = true;
        mat.FlagsUnshaded = true;
        mat.AlbedoColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        line3D.MaterialOverride = mat;

        
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

    line3D.clearLines();

    // Draw lines gizmos if car not collide?
    var dict = state.IntersectRay(Transform.origin + up * 0.2f, Transform.origin - up);
    if (dict.Count > 0)
    {
        Vector3 hit = (Vector3)dict["position"];
        foreach (var point in points)
        {
            line3D.addLine(point, hit);
            AddCentralForce(Transform.basis.y * 0.9f); // Wow
        }
    } else {
        
    }

    AddCentralForce(Transform.basis.z * yInput * forceMult);
 }
}
