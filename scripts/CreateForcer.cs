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
    private ImmediateGeometry line;
    
    // public MeshInstance car_body;
    // public MeshInstance wheel_front_left;
    // public MeshInstance wheel_front_right;
    // public MeshInstance wheel_rear_left;
    // public MeshInstance wheel_rear_right;
    // private LineDrawer3D line;

    public override void _Ready()
    {
        line = GetNode<ImmediateGeometry>("draw");
        // create the points
        points[0]= new Vector3(-wheelTrack, 0, wheelBase);
        points[1] = new Vector3(wheelTrack, 0, wheelBase);
        points[2] = new Vector3(-wheelTrack, 0, -wheelBase);
        points[3] = new Vector3(wheelTrack, 0, -wheelBase);
        
        // car_body = GetNode<Godot.MeshInstance>("car_model/car_body");
        // points[0] = GetNode<MeshInstance>("car_model/wheel_fl").Translation;
        // points[1] = GetNode<MeshInstance>("car_model/wheel_fr").Translation;
        // points[0] = GetNode<MeshInstance>("car_model/wheel_rl").Translation;
        // points[1] = GetNode<MeshInstance>("car_model/wheel_rr").Translation;
        // line = new LineDrawer3D();
        // line.addLine(Vector3.One * -10, Vector3.One * 10);
        // AddChild(line);
        // Create a material to change line3D color to red
        // var mat = new SpatialMaterial();
        // mat.FlagsUsePointSize = true;
        // mat.VertexColorUseAsAlbedo = true;
        // mat.FlagsUnshaded = true;
        // mat.AlbedoColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        // line3D.MaterialOverride = mat;
        // AddChild(line3D);
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
        float rayLength = 0.6f;
        var up = Transform.basis.y;
        line.Clear();
        foreach(var p in points) {
            var origin = ToGlobal(p) + up  * 0.2f;
            var dest = origin - up * rayLength;
            var dict = spaceState.IntersectRay(origin, dest);
            if (dict.Count > 0) {
                wheelsOnGround++;
                var hit = (Vector3)dict["position"];
                var normal = (Vector3)dict["normal"];
                line.Begin(Mesh.PrimitiveType.Lines);
                line.AddVertex(ToLocal(origin));
                line.AddVertex(ToLocal(hit));
                line.End();

                float distFromTarget = (dest - hit).Length();
                float spring = 10 * distFromTarget;
                AddForce(normal * spring, hit - Transform.origin);
            } else {
                line.Begin(Mesh.PrimitiveType.Lines);
                line.AddVertex(ToLocal(origin));
                line.AddVertex(ToLocal(dest));
                line.End();
            }
        }

        // GD.Print(wheelsOnGround);
        if (wheelsOnGround > 0) {
            AddTorque(-Transform.basis.y * xInput * torqueMult);
            AddCentralForce(Transform.basis.z * yInput * forceMult);
        }
    }
}
