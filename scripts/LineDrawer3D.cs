using Godot;
using System.Collections.Generic;

namespace Utility
{
    class LineDrawer3D : ImmediateGeometry
    {
        List<Vector3> points = new List<Vector3>();

        public void addLine(Vector3 p1, Vector3 p2)
        {
            points.Add(p1);
            points.Add(p2);
            
        }

        public void clearLines() 
        {
            points.Clear();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            Clear();
            Begin(Mesh.PrimitiveType.Lines);
            for (int i = 0; i < points.Count; ++i)
                AddVertex(points[i]);
            End();
        }
    }
}
