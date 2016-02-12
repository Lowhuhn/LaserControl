using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LCSVGViewer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScaleTransform scale;
        public MainWindow()
        {
            InitializeComponent();
            scale = new ScaleTransform();
            TestCanvas.RenderTransform = scale;

            /*Rectangle r = new Rectangle();
            r.Width = 100;
            r.Height = 100;
            r.Fill = Brushes.Bisque;

            TestCanvas.Children.Add(r);*/

            for (int i = 0; i < 1000; ++i)
            {
                Path p = new Path();
                string sData = "M" + (5 + i * 1) + ",20 L" + (5 + i * 1) + ",100";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                p.Data = (Geometry)converter.ConvertFrom(sData);
                p.Stroke = Brushes.Red;
                p.StrokeThickness = 0.1;
                TestCanvas.Children.Add(p);

                Canvas.SetLeft(p, 0);
                Canvas.SetTop(p, 0);
            }

            for (int i = 0; i < 1000; ++i)
            {
                Path p = new Path();
                string sData = "M" + (5 + i * 1) + ",150 L" + (5 + i * 1) + ",300";
                var converter = TypeDescriptor.GetConverter(typeof(Geometry));
                p.Data = (Geometry)converter.ConvertFrom(sData);
                p.Stroke = Brushes.Blue;
                p.StrokeThickness = 0.02;
                TestCanvas.Children.Add(p);

                Canvas.SetLeft(p, 0);
                Canvas.SetTop(p, 0);
            }
        }

        private void TestCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scale.ScaleX += e.Delta*0.001;
            scale.ScaleY += e.Delta*0.001;            
            e.Handled = true;
        }

        private void TestCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
               // Console.WriteLine("Left down");
                /*foreach (UIElement c in TestCanvas.Children)
                {
                    Canvas.SetLeft(c, -50);
                    Canvas.SetTop(c, -50);
                }*/
                
            }
        }

        private bool first = true;
        private double lastX = 0;
        private double lastY = 0;

        private void TestCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) 
            {               
                //Console.WriteLine("Left Down & move: {0} x {1}", e.GetPosition(this.TestCanvas).X, e.GetPosition(this.TestCanvas).Y);
                Point p = e.GetPosition(this.TestCanvas);

                if (!first)                
                {                    
                    double x = lastX - p.X;
                    double y = lastY - p.Y;

                    foreach (UIElement c in TestCanvas.Children)
                    {
                        Canvas.SetLeft(c, Canvas.GetLeft(c) - x);
                        Canvas.SetTop(c, Canvas.GetTop(c) - y);                        
                    }
                }
                else
                {
                    first = false;
                }
                lastX = p.X;
                lastY = p.Y;
                
            }else{
                first = true;
            }
        }
    }
}
