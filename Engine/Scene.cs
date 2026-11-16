using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Engine;

public class Scene : Viewport3D
{
    public static readonly CamPosConfig Default = new(0, 0, 0,0,0);
    public static readonly CamPosConfig PlaneDown = new(0, 0, 0,0,-1);
    public static readonly CamPosConfig PlaneEast = new(0, 0, 1,0,0);
    public static readonly CamPosConfig PlaneNorth = new(0, 0, 0,1,0);
    public static readonly CamPosConfig PartyTurn = new(72, 20, -0.4,0,0);
    public static readonly CamPosConfig EnemyTurn = new(72, 200, 0.4,0,0);
    public static readonly CamPosConfig Below = new(120, 0, 0,0,0);

    private readonly AxisAngleRotation3D _pitRot = new(new Vector3D(1, 0, 0), Default.Pitch); 
    private readonly AxisAngleRotation3D _yawRot = new(new Vector3D(0, 0, 1), Default.Yaw);
    private readonly TranslateTransform3D _trans = new(Default.PlaneX, Default.PlaneY, Default.PlaneZ);

    private readonly ContentControl _floorControl = new();
    private readonly ModelVisual3D _content;

    private readonly Dictionary<uint, Visual3D> _standees = new();
    
    public object? Floor
    {
        get => _floorControl.Content;
        set => _floorControl.Content = value;
    }
    
    public Scene()
    {
        Camera = new PerspectiveCamera
        {
            Position = new Point3D(0, 0, 1),
            LookDirection = new Vector3D(0, 0, -1),
            UpDirection = new Vector3D(0, 1, 0),
            FieldOfView = 120,
            Transform = new Transform3DGroup
            {
                Children = [new RotateTransform3D(_pitRot), new RotateTransform3D(_yawRot)]
            }
        };
        _content = new ModelVisual3D();
        {
            _content.Transform = _trans;
            _content.Children.Add(Visual3DFactory.MakeFromVisual(_floorControl, GeometryFactory.MakeHorizontalPlane(new Size(2,2))));
        }
        Children.Add(_content);
    }

    public uint AddStandee((Visual v, Size s)? stand, (Visual v, Size s)? billboard)
    {
        if (stand is null && billboard is null) throw new ArgumentException("At least one part must be not null.");
        var standModel = new ModelVisual3D();
        {
            if (stand is {} s)
            {
                standModel.Children.Add(Visual3DFactory.MakeFromVisual(s.v, GeometryFactory.MakeHorizontalPlane(s.s, 1.0/1024.0)));
            }

            if (billboard is { } b)
            {
                var bb = Visual3DFactory.MakeFromVisual(b.v, GeometryFactory.MakeStandingPlane(b.s));
                {
                    bb.Transform = new RotateTransform3D(_yawRot);
                    BindingOperations.SetBinding(
                        target: bb.Visual,
                        dp: OpacityProperty,
                        binding: new Binding
                        {
                            Source = _pitRot,
                            Path = new PropertyPath(AxisAngleRotation3D.AngleProperty),
                            Converter = new AngleToOpacityConverter()
                        }
                    );
                }
                standModel.Children.Add(bb);
            }
        }

        var key = 0u;
        while (key == 0u || _standees.ContainsKey(key))
        {
            key = (uint)Random.Shared.Next();
        }

        _standees[key] = standModel;
        _content.Children.Add(standModel);
        return key;
    }

    public void RemoveStandee(uint id)
    {
        if (!_standees.Remove(id, out var removed)) return;
        _content.Children.Remove(removed);
    }

    public Visual3D? GetStandee(uint id) => _standees.GetValueOrDefault(id);

    public void AnimateRotation(CamPosConfig config)
    {
        var d = TimeSpan.FromSeconds(1);
        _pitRot.BeginAnimation(AxisAngleRotation3D.AngleProperty, new DoubleAnimation(config.Pitch, d));
        _yawRot.BeginAnimation(AxisAngleRotation3D.AngleProperty, new DoubleAnimation(config.Yaw, d));
        _trans.BeginAnimation(TranslateTransform3D.OffsetXProperty, new DoubleAnimation(config.PlaneX, d));
        _trans.BeginAnimation(TranslateTransform3D.OffsetYProperty, new DoubleAnimation(config.PlaneY, d));
        _trans.BeginAnimation(TranslateTransform3D.OffsetZProperty, new DoubleAnimation(config.PlaneZ, d));
    }
}

public record struct CamPosConfig(double Pitch, double Yaw, double PlaneX, double PlaneY, double PlaneZ);