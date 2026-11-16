using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Engine;

internal static class Program
{
    private static Scene _scene;
    private static TranslateTransform3D _standeeTrans;
    
    [STAThread]
    internal static void Main(string[] args)
    {
        var root = new DockPanel();
        {
            var main = new ContentControl { Content = ConstructMainElement() };
            var stack = new StackPanel();
            {
                stack.Orientation = Orientation.Vertical;
                stack.Children.Add(MakeDebugButton("Reload", () => main.Content = ConstructMainElement()));
                stack.Children.Add(MakeCamRotButton(Scene.Default));
                stack.Children.Add(MakeCamRotButton(Scene.Below));
                stack.Children.Add(MakeCamRotButton(Scene.PlaneDown));
                stack.Children.Add(MakeCamRotButton(Scene.PlaneEast));
                stack.Children.Add(MakeCamRotButton(Scene.PlaneNorth));
                stack.Children.Add(MakeCamRotButton(Scene.PartyTurn));
                stack.Children.Add(MakeCamRotButton(Scene.EnemyTurn));
                stack.Children.Add(MakeDebugButton("AnimateRandom", () =>
                {
                    var d = TimeSpan.FromSeconds(1);
                    _standeeTrans.BeginAnimation(TranslateTransform3D.OffsetXProperty,
                        new DoubleAnimation(Random.Shared.NextDouble() * 2 - 1, d));
                    _standeeTrans.BeginAnimation(TranslateTransform3D.OffsetYProperty,
                        new DoubleAnimation(Random.Shared.NextDouble() * 2 - 1, d));
                }));
                DockPanel.SetDock(stack, Dock.Right);
            }
            root.Children.Add(stack);
            root.Children.Add(main);
        }
        new Application().Run(new Window { Content = root, Background = Brushes.DimGray});
    }

    private static Button MakeCamRotButton(CamPosConfig config, [CallerArgumentExpression(nameof(config))] string? name = null)
    {
        if (name is null)
        {
            name = "Rotation";
        }
        else
        {
            var pretext = nameof(Scene).Length + 1;
            name = name.Substring(pretext, name.Length - pretext);
        }

        return MakeDebugButton(name, () => _scene.AnimateRotation(config));
    }

    private static Button MakeDebugButton(string title, Action action)
    {
        var rv = new Button();
        {
            rv.Content = title;
            rv.Click += (_, _) => action.Invoke();
        }
        return rv;
    }

    internal static object ConstructMainElement()
    {
        _scene = new Scene { Floor = MakePlaneContent() };
        _scene.Children.Add(new ModelVisual3D{ Content = new AmbientLight(Colors.White) });
        var id = _scene.AddStandee((MakePlaneContent(), new Size(0.2, 0.2)), (MakePlaneContent(), new Size(0.2, 0.35)));
        var standee = _scene.GetStandee(id)!;
        _standeeTrans = new TranslateTransform3D();
        standee.Transform = _standeeTrans;
        return _scene;
    }
    
    internal static Grid MakePlaneContent() // This is useful for now, should stay as a debug method.
    {
        var rv = new Grid();
        {
            for (var i = 0; i < 10; i++) rv.ColumnDefinitions.Add(new ColumnDefinition());
            for (var i = 0; i < 10; i++) rv.RowDefinitions.Add(new RowDefinition());
            rv.Background = new SolidColorBrush(Color.FromRgb((byte)Random.Shared.Next(0,255), (byte)Random.Shared.Next(0,255), (byte)Random.Shared.Next(0,255)));
            var bgStr = rv.Background.ToString(CultureInfo.CurrentCulture);
            rv.ShowGridLines = true;
            rv.Width = 512;
            rv.Height = 512;
            var tl = new Rectangle();
            {
                tl.Fill = Brushes.Red;
                Grid.SetColumn(tl, 1);
                Grid.SetRow(tl, 1);
            }
            rv.Children.Add(tl);
            var tr = new Rectangle();
            {
                tr.Fill = Brushes.Blue;
                Grid.SetColumn(tr, 8);
                Grid.SetRow(tr, 1);
            }
            rv.Children.Add(tr);
            var bl = new Rectangle();
            {
                bl.Fill = Brushes.Green;
                Grid.SetColumn(bl, 1);
                Grid.SetRow(bl, 8);
            }
            rv.Children.Add(bl);
            var br = new Rectangle();
            {
                br.Fill = Brushes.Yellow;
                Grid.SetColumn(br, 8);
                Grid.SetRow(br, 8);
            }
            rv.Children.Add(br);
            var button = new Button();
            {
                button.Content = "Button";
                button.Click += (_, _) => Console.WriteLine($"Clicked. Color: {bgStr}");
                Grid.SetColumn(button, 4);
                Grid.SetRow(button, 4);
                Grid.SetColumnSpan(button, 2);
                Grid.SetRowSpan(button, 2);
            }
            rv.Children.Add(button);
        }
        return rv;
    }
}