using Godot;
using System;
using Utility;

public class CarController : RigidBody
{
    private LineDrawer3D line;

    [Export] public float raycasyHeightOffset = 0;
    [Export] public float springRate = 20;
    [Export] public float dampRate = 2;
    [Export] public int maxSpeedKmh = 60;
    [Export(PropertyHint.ExpEasing)] public float tractionEase = 2;

    public float torqueMult = 10;
    public float forceMult = 10;

    public float wheelBase = 1.052f;
    public float wheelTrack = 0.644f;

    Vector3[] points = new Vector3[4];

    public override void _Ready()
    {
        // create the points
        points[0] = GetNode<MeshInstance>("car_model/wheel_fl").Translation;
        points[1] = GetNode<MeshInstance>("car_model/wheel_fr").Translation;
        points[2] = GetNode<MeshInstance>("car_model/wheel_rl").Translation;
        points[3] = GetNode<MeshInstance>("car_model/wheel_rr").Translation;

        // Create a new line and material
        line = new LineDrawer3D();
        var mat = new SpatialMaterial();
        mat.FlagsUsePointSize = true;
        mat.VertexColorUseAsAlbedo = true;
        mat.FlagsUnshaded = true;
        line.MaterialOverride = mat;
        AddChild(line);
    }

    public Vector3 GetVelocityAtPoint(Vector3 point) {
        return LinearVelocity + AngularVelocity.Cross(point - GlobalTransform.origin);
    }

     public override void _PhysicsProcess(float dt)
    {
        float xInput = 
            Input.IsKeyPressed((int)KeyList.A) ? -1 :
            (Input.IsKeyPressed((int)KeyList.D) ? 1 : 0);    
        float yInput = 
            Input.IsKeyPressed((int)KeyList.S) ? -1 :
            (Input.IsKeyPressed((int)KeyList.W) ? 1 : 0);

        // Drag shit:
        float speed = LinearVelocity.Length();

        var state = GetWorld().DirectSpaceState;
        var up = Transform.basis.y;
        var forward = Transform.basis.z;

        line.ClearLines();

        int wheelsOnGround = 0;

        float rayLength = 0.6f;

        Vector3 tractionPoint = new Vector3();

        foreach (var p in points)
        {
            Vector3 wp = ToGlobal(p);
            
            var origin = wp + up * raycasyHeightOffset;
            var dest = origin - up * rayLength;
            var dict = state.IntersectRay(origin, dest);

            if (dict.Count > 0) 
            {
                var obj = (Godot.Object) dict["collider"];
                Vector3 hit = (Vector3) dict["position"];
                Vector3 normal = (Vector3) dict["normal"];

                line.AddLine(origin, hit, Colors.Red);

                float distFromTarget = (dest - hit).Length();

                float spring = springRate * distFromTarget;

                Vector3 veloAtWheel = GetVelocityAtPoint(origin);
                float verticalVeloAtWheel = up.Dot(veloAtWheel);
                float damp = -verticalVeloAtWheel * dampRate;

                AddForce(normal * (spring + damp), hit - Transform.origin);

                wheelsOnGround++;

                tractionPoint += hit;
            } else {
                line.AddLine(origin, dest, Colors.Blue);
            }
        }

        // foreach(var p in points) {
        //     var origin = ToGlobal(p) + up  * 0.2f;
        //     var dest = origin - up * rayLength;
        //     var dict = spaceState.IntersectRay(origin, dest);
        //     if (dict.Count > 0) {
        //         var obj = (Godot.Object)dict["collider"];
        //         var hit = (Vector3)dict["position"];
        //         var normal = (Vector3)dict["normal"];

        //         // Draw 3D line
        //         line3D.AddLine(origin, hit, Colors.DarkBlue);

        //         float distFromTarget = (dest - hit).Length();
        //         float spring = 10 * distFromTarget;

        //         Vector3 pointVelo = GetVelocityAtPoint(origin);
        //         line3D.AddLine(origin, origin + pointVelo, Colors.White);
                
        //         float veloAlongWheel = up.Dot(pointVelo);
        //         float damp = -veloAlongWheel;

        //         AddForce(normal * (spring + damp), hit - Transform.origin);
                
        //         wheelsOnGround++;
        //     } else {
        //         // Draw 3D line
        //         line3D.AddLine(origin, dest, Colors.GreenYellow);
        //     }
        // }

        // GD.Print(wheelsOnGround);
        if (wheelsOnGround > 0) {

            float wheelFactor = wheelsOnGround / 4.0f;

            Vector3 midPoint = tractionPoint / wheelsOnGround;

            line.AddLine(midPoint, midPoint + up * 1, Colors.Red);
            line.AddLine(midPoint, midPoint + Vector3.Right * 1, Colors.Red);
            line.AddLine(midPoint, midPoint + Vector3.Forward * 1, Colors.Red);

            AddTorque(-Transform.basis.y * xInput * torqueMult);
            float forwardVelocity = forward.Dot(LinearVelocity); 

            float maxSpeed = maxSpeedKmh / 3.6f;
            float tractionMult = 1 - Mathf.Ease(Math.Abs(forwardVelocity) / maxSpeed, tractionEase);
            float tractionForce =  tractionMult * yInput * forceMult * wheelFactor;

            line.AddLine(Vector3.Zero, Vector3.Up * tractionMult, Colors.Red);
            if ( forwardVelocity < 60 / 3.6f)
                AddForce(Transform.basis.z * tractionForce, midPoint - Transform.origin);
        }
    }
}
