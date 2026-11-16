using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Engine;

internal static class Visual3DFactory
{
    internal static Viewport2DVisual3D MakeFromVisual(Visual visual, MeshGeometry3D geometry)
    {
        var rv = new Viewport2DVisual3D();
        {
            var mat = new DiffuseMaterial();
            {
                mat.Brush = Brushes.White;
                Viewport2DVisual3D.SetIsVisualHostMaterial(mat, true);
            }
            rv.Material = mat;
            rv.Geometry = geometry;
            rv.Visual = visual;
        }
        return rv;
    }
}