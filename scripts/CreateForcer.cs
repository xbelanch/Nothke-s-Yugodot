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

    public float wheelBase = 1.052f;
    public float wheelTrack = 0.644f;
    Vector3[] points = new Vector3[1];
    private LineDrawer3D line;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // create the points
        points[0]= new Vector3(-wheelTrack, 0, wheelBase);

        car_body = GetNode<Godot.MeshInstance>("car_model/car_body");
        
        // points[0] = GetNode<MeshInstance>("car_model/wheel_fl").Translation;
        // points[1] = GetNode<MeshInstance>("car_model/wheel_fr").Translation;
        // points[0] = GetNode<MeshInstance>("car_model/wheel_rl").Translation;
        // points[1] = GetNode<MeshInstance>("car_model/wheel_rr").Translation;

        line = new LineDrawer3D();
        line.addLine(Vector3.One * -10, Vector3.One * 10);
        AddChild(line);

        // Create a material to change line3D color to red
        // var mat = new SpatialMaterial();
        // mat.FlagsUsePointSize = true;
        // mat.VertexColorUseAsAlbedo = true;
        // mat.FlagsUnshaded = true;
        // mat.AlbedoColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        // line3D.MaterialOverride = mat;
        // AddChild(line3D);
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

   
    // AddCentralForce(-LinearVelocity * (1 - delta)); // friction with the ground?

    // Drag shit
    float speed = LinearVelocity.Length();
    // float forceFactor = Mathf.InverseLerp(10, 0, speed);
    var state = GetWorld().DirectSpaceState;
    var up = Transform.basis.y;

    // line3D.clearLines();



        // // Testing intersect ray
        // // Vector3 from = Vector3.One * new Vector3(0, -0f, 0);
        // // Vector3 to = Vector3.One * new Vector3(0, -1f, 0);
        // Vector3 origin = Vector3.Zero + up * 0.2f;
        // Vector3 dest = Vector3.Zero - up;
        // var dict = state.IntersectRay(ToGlobal(origin), ToGlobal(dest));
        // line3D.addLine(origin, dest);
        // // TODO: Why this not work?
        // if (dict.Count > 0) {
        //     wheelsOnGround++;     
        // }

        // if (wheelsOnGround > 0) {        
        //     AddTorque(-Transform.basis.y * xInput * torqueMult);
        //     AddCentralForce(Transform.basis.z * yInput * forceMult);
        // }
        // GD.Print(wheelsOnGround);
        int wheelsOnGround = 0;
        float rayLength = 1;
        // points[0] = GetNode<MeshInstance>("car_model/wheel_fl").GlobalTransform.origin;

        // Vector3 wp = ToGlobal(points[0]);
        // var origin = wp;
        // var dest = origin * new Vector3(0, -3, 0);
        // line3D.addLine(origin, dest);

        // foreach (var p in points)
        //     {
        //         Vector3 wp = ToGlobal(p);
        //         var origin = wp + ToGlobal(up) * 0.2f;
        //         var dest = origin - up * rayLength;
        //         var dict = state.IntersectRay(origin, dest);
        //         // Draw lines gizmos if car not collide?
        //         if (dict.Count > 0)
        //         {
        //             var obj = (Godot.Object)dict["collider"];
        //             Vector3 hit = (Vector3)dict["position"];
        //             Vector3 normal = (Vector3)dict["normal"];
        //             // line3D.addLine(origin,hit);
        //             AddForce(normal * 10, hit - Transform.origin);
        //             wheelsOnGround++;
        //         } else {
        //             line3D.addLine(ToGlobal(origin), ToGlobal(dest));
        //         }
        //     }
        //     GD.Print(wheelsOnGround);
        //     if (wheelsOnGround > 0) {
        //         AddTorque(-Transform.basis.y * xInput * torqueMult);
        //         AddCentralForce(Transform.basis.z * yInput * forceMult);        
        //     }

        AddTorque(-Transform.basis.y * xInput * torqueMult);
        AddCentralForce(Transform.basis.z * yInput * forceMult);       

    }
}
