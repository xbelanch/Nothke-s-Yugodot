using Godot;
using System;
using Utility;

public class CreateForcer : RigidBody
{
    public float torqueMult = 10;
    public float forceMult = 10;
    public float wheelBase = 1.052f;
    public float wheelTrack = 0.644f;
    Vector3[] points = new Vector3[4];
    private LineDrawer3D line3D;

    public override void _Ready()
    {
        // create the points
        points[0] = GetNode<MeshInstance>("car_model/wheel_fl").Translation;
        points[1] = GetNode<MeshInstance>("car_model/wheel_fr").Translation;
        points[2] = GetNode<MeshInstance>("car_model/wheel_rl").Translation;
        points[3] = GetNode<MeshInstance>("car_model/wheel_rr").Translation;

        // Create a new line and material
        line3D = new LineDrawer3D();
        var mat = new SpatialMaterial();
        mat.FlagsUsePointSize = true;
        mat.VertexColorUseAsAlbedo = true;
        mat.FlagsUnshaded = true;
        line3D.MaterialOverride = mat;
        AddChild(line3D);
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

        var spaceState = GetWorld().DirectSpaceState;
        float speed = LinearVelocity.Length();

        int wheelsOnGround = 0;
        float rayLength = 0.8f;
        var up = Transform.basis.y;
        line3D.ClearLines();
        foreach(var p in points) {
            var origin = ToGlobal(p) + up  * 0.2f;
            var dest = origin - up * rayLength;
            var dict = spaceState.IntersectRay(origin, dest);
            if (dict.Count > 0) {
                var obj = (Godot.Object)dict["collider"];
                var hit = (Vector3)dict["position"];
                var normal = (Vector3)dict["normal"];

                // Draw 3D line
                line3D.AddLine(origin, hit, Colors.DarkBlue);

                float distFromTarget = (dest - hit).Length();
                float spring = 10 * distFromTarget;

                Vector3 pointVelo = GetVelocityAtPoint(origin);
                line3D.AddLine(origin, origin + pointVelo, Colors.White);
                
                float veloAlongWheel = up.Dot(pointVelo);
                float damp = -veloAlongWheel;

                AddForce(normal * (spring + damp), hit - Transform.origin);
                
                wheelsOnGround++;
            } else {
                // Draw 3D line
                line3D.AddLine(origin, dest, Colors.GreenYellow);
            }
        }

        // GD.Print(wheelsOnGround);
        if (wheelsOnGround > 0) {
            AddTorque(-Transform.basis.y * xInput * torqueMult);
            AddCentralForce(Transform.basis.z * yInput * forceMult);
        }
    }
}
