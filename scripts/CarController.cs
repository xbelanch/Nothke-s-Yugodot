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
    [Export(PropertyHint.ExpEasing)] public float sidewaysTractionEase = 1;
    [Export] public float maxTraction = 30;
    [Export] public float sidewaysTractionMult = 1;

    public float torqueMult = 10;
    public float forceMult = 10;

    public float wheelBase = 1.052f;
    public float wheelTrack = 0.6f;

    Vector3[] wheelPoints = new Vector3[4];

    Spatial wheelRoot;
    MeshInstance[] graphicalWheels = new MeshInstance[4];
    public override void _Ready()
    {
        // create the wheelPoints
        wheelPoints[0] = new Vector3(-wheelTrack, 0, wheelBase);
        wheelPoints[1] = new Vector3(wheelTrack, 0, wheelBase);
        wheelPoints[2] = new Vector3(-wheelTrack, 0, -wheelBase);
        wheelPoints[3] = new Vector3(wheelTrack, 0, -wheelBase);

        graphicalWheels[0] = GetNode<MeshInstance>("car_model/wheel_fl");
        graphicalWheels[1] = GetNode<MeshInstance>("car_model/wheel_fr");
        graphicalWheels[2] = GetNode<MeshInstance>("car_model/wheel_rl");
        graphicalWheels[3] = GetNode<MeshInstance>("car_model/wheel_rr");
        wheelRoot = graphicalWheels[0].GetParent<Spatial>();

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

        float rayLength = 0.5f;

        Vector3 tractionPoint = new Vector3();

        int i = 0;
        foreach (var p in wheelPoints)
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

                line.AddLine(origin, hit, Colors.Cyan);

                float distFromTarget = (dest - hit).Length();

                float spring = springRate * distFromTarget;

                Vector3 veloAtWheel = GetVelocityAtPoint(origin);
                float verticalVeloAtWheel = up.Dot(veloAtWheel);
                float damp = -verticalVeloAtWheel * dampRate;

                AddForce(normal * (spring + damp), hit - Transform.origin);

                wheelsOnGround++;
                tractionPoint += hit;

                graphicalWheels[i].Translation = wheelRoot.ToLocal(hit + up * 0.3f);

                // Rotate the front wheels
                if (i % 2 == 0)
                    graphicalWheels[i].Rotation = new Vector3(0, Mathf.Deg2Rad(180), 0);
                else
                    graphicalWheels[i].Rotation = new Vector3(0, Mathf.Deg2Rad(0), 0);
            
                if (i < 2) 
                    graphicalWheels[i].Rotate(Vector3.Up, -xInput * Mathf.Deg2Rad(30));
            } 
            else
                line.AddLine(origin, dest, Colors.Blue);

            i++;
        }
        // GD.Print(wheelsOnGround);

        line.AddLine(Translation, Translation + LinearVelocity, Colors.Yellow);
        Vector3 right = Transform.basis.x;
        float sidewaysSpeed = right.Dot(LinearVelocity);
        line.AddLine(Translation, Translation + right * sidewaysSpeed, Colors.Red);

        if (wheelsOnGround > 0) {

            float wheelFactor = wheelsOnGround / 4.0f;

            Vector3 midPoint = tractionPoint / wheelsOnGround;

            line.AddLine(midPoint, midPoint + up * 1, Colors.Red);
            line.AddLine(midPoint, midPoint + Vector3.Right * 1, Colors.Red);
            line.AddLine(midPoint, midPoint + Vector3.Forward * 1, Colors.Red);

            float forwardVelocity = forward.Dot(LinearVelocity); 
            float steeringFactor = Mathf.Clamp(Mathf.InverseLerp(0, 2, speed), 0, 1);

            AddTorque(-Transform.basis.y * xInput * torqueMult * steeringFactor);

            float maxSpeed = maxSpeedKmh / 3.6f;
            float tractionMult = 1 - Mathf.Ease(Math.Abs(forwardVelocity) / maxSpeed, tractionEase);
            float tractionForce =  tractionMult * yInput * forceMult * wheelFactor;

            line.AddLine(Vector3.Zero, Vector3.Up * tractionMult, Colors.Red);

            float sidewaysSign = Math.Sign(sidewaysSpeed);
            Vector3 sidewaysTraction = -right * sidewaysTractionMult *
                (Mathf.Ease(Mathf.Abs(sidewaysSpeed) / maxTraction, sidewaysTractionEase) * maxTraction) * sidewaysSign;
            AddForce(
                forward * tractionForce + sidewaysTraction, 
                midPoint - Transform.origin);
        }
    }
}
