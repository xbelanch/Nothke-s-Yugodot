using Godot;
using System;
using Utility;

public class Crate : RigidBody
{
    private LineDrawer3D line;
    private MeshInstance mesh;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        line = new LineDrawer3D(); 
        var mat = new SpatialMaterial();
        mat.FlagsUsePointSize = true;
        mat.VertexColorUseAsAlbedo = true;
        mat.FlagsUnshaded = true;
        mat.AlbedoColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);
        line.MaterialOverride = mat;

        mesh = GetNode<MeshInstance>("cube");
        // mesh.AddChild(line);

        AddChild(line);

        GD.Print(Transform.origin);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        // var up = Transform.basis.y;
        // float rayLength = 1;
        // line.clearLines();
        // // GD.Print(mesh.Translation);
        // Vector3 wp = Translation;
        // // GD.Print(wp);
        // // GD.Print(up);
        // var origin = wp;
        // var dest = wp * 10;
        // line.addLine(origin, dest);

        var spaceState = GetWorld().DirectSpaceState;
        var line3D = GetNode<ImmediateGeometry>("draw");
        var p1 = line3D.Transform.origin;
        var p2 = p1 - new Vector3(0, 0, 5);
        line3D.Clear();
        line3D.Begin(Mesh.PrimitiveType.LineStrip);
        line3D.AddVertex((p1));
        line3D.AddVertex((p2));
        line3D.End();
        
        var dict =  spaceState.IntersectRay(ToGlobal(p1), ToGlobal(p2));
        if (dict.Count > 0) {
            GD.Print(dict["collider"]);
        } else {
            GD.Print("None");
        }
    }

}
