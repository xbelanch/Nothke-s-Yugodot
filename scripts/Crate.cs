using Godot;
using System;
using Utility;

public class Crate : RigidBody
{
    private ImmediateGeometry line;
    private MeshInstance mesh;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        line = GetNode<ImmediateGeometry>("draw");
    }

    public override void _PhysicsProcess(float delta)
    {
        var spaceState = GetWorld().DirectSpaceState;
        
        var p1 = line.Transform.origin;
        var p2 = p1 - new Vector3(0, 2, 0);
        line.Clear();
        line.Begin(Mesh.PrimitiveType.LineStrip);
        line.AddVertex((p1));
        line.AddVertex((p2));
        line.End();
        
        var dict =  spaceState.IntersectRay(ToGlobal(p1), ToGlobal(p2));
        if (dict.Count > 0) {
            // GD.Print(dict["collider"]);
            Vector3 normal = (Vector3)dict["normal"];
            AddForce(normal * 5, Vector3.One);
        } else {
            // GD.Print("None");
        }
    }

}
