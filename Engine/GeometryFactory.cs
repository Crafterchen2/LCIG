using System.Windows;
using System.Windows.Media.Media3D;

namespace Engine;

internal static class GeometryFactory
{
    internal static MeshGeometry3D MakePlane(Vector3D widthAxis, Vector3D heightAxis, Point anchor, double normalOffset = 0)
    {
        var normal = Vector3D.CrossProduct(widthAxis, heightAxis);
        normal.Normalize();
        var origin = new Point3D
        (
            x: -anchor.X * widthAxis.X + -anchor.Y * heightAxis.X,
            y: -anchor.X * widthAxis.Y + -anchor.Y * heightAxis.Y,
            z: -anchor.X * widthAxis.Z + -anchor.Y * heightAxis.Z
        );
        origin += normal * normalOffset;
        return new MeshGeometry3D
        {
            Positions = [origin, origin + widthAxis, origin + heightAxis, origin + widthAxis + heightAxis],
            TextureCoordinates = [new Point(0,1), new Point(1,1), new Point(0,0), new Point(1,0)],
            TriangleIndices = [0, 1, 2, 2, 1, 3]
            // Normals are automatically generated and don't have to be specified.
        };
    }

    internal static MeshGeometry3D MakeHorizontalPlane(Size size, double normalOffset = 0, Point? anchor = null) =>
        MakePlane(new Vector3D(size.Width, 0, 0), new Vector3D(0, size.Height, 0), anchor ?? new Point(0.5, 0.5), normalOffset);

    internal static MeshGeometry3D MakeStandingPlane(Size size, double normalOffset = 0, Point? anchor = null) =>
        MakePlane(new Vector3D(size.Width, 0, 0), new Vector3D(0, 0, size.Height), anchor ?? new Point(0.5, 0), normalOffset);
}