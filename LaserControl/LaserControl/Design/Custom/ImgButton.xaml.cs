using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaserControl.Design.Custom
{
    public class ImgButtonDataHandler : INotifyPropertyChanged
    {
        protected ImageSource _ImageToDisplay;
        public ImageSource ImageToDisplay
        {
            get { return _ImageToDisplay; }
            set
            {
                this._ImageToDisplay = value;
                this.NotifyPropertyChanged("ImageToDisplay");
            }
        }

        /*protected string _Text;
        public string Text
        {
            get { return _Text; }
            set
            {
                this._Text = value;
                this.NotifyPropertyChanged("Text");
            }
        }*/

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }


    /// <summary>
    /// Interaktionslogik für ImgButton.xaml
    /// </summary>
    public partial class ImgButton : UserControl
    {
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(ImgButton));

        public event RoutedEventHandler Click
        {
            add
            {
                this.AddHandler(ClickEvent, value);
            }
            remove
            {
                this.RemoveHandler(ClickEvent, value);
            }
        }

        public static readonly DependencyProperty ImgToShowSourceProperty = DependencyProperty.Register(
            "ImgToShow",
            typeof(string),
            typeof(ImgButton),
            new UIPropertyMetadata("NONE", new PropertyChangedCallback(ImgToShowSourcePropertyChange))
            );

        public static readonly DependencyProperty TextSourceProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(ImgButton),
            new UIPropertyMetadata("", null)
            );

        public string ImgToShow
        {
            get
            {
                return (string)GetValue(ImgToShowSourceProperty);
            }
            set
            {
                SetValue(ImgToShowSourceProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextSourceProperty);
            }
            set
            {
                SetValue(TextSourceProperty, value);
            }
        }

        protected ImageSource ImgEnabled = null;
        protected ImageSource ImgDisabled = null;

        protected ImgButtonDataHandler Data;

        public ImgButton()
        {
            InitializeComponent();
            Data = FindResource("datahandler") as ImgButtonDataHandler;
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(IsEnableChangedCallback);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            RoutedEventArgs r = new RoutedEventArgs(ClickEvent);
            RaiseEvent(r);
            e.Handled = true;
        }

        protected void IsEnableChangedCallback(object o, DependencyPropertyChangedEventArgs e)
        {
            if (ImgEnabled != null && ImgDisabled != null)
            {
                if (IsEnabled)
                {
                    Data.ImageToDisplay = ImgEnabled;
                }
                else
                {
                    Data.ImageToDisplay = ImgDisabled;
                }
            }
        }

        protected static void ImgToShowSourcePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d.GetType() == typeof(ImgButton))
            {
                try
                {
                    ImgButton ib = d as ImgButton;

//                    Console.WriteLine("Tada: {0} -> {1} ", e.NewValue, ib.IsEnabled);

                    System.Drawing.Icon ic = (System.Drawing.Icon)LaserControl.Properties.Resources.ResourceManager.GetObject(e.NewValue.ToString());
                    System.Drawing.Bitmap bmp = ic.ToBitmap();
                    System.Drawing.Bitmap bmpD = MakeGrayscale3(bmp);

                    ib.ImgEnabled = BitmapToImageSource(bmp);
                    ib.ImgDisabled = BitmapToImageSource(bmpD);

                    if (ib.IsEnabled)
                    {
                        ib.Data.ImageToDisplay = ib.ImgEnabled;
                    }
                    else
                    {
                        ib.Data.ImageToDisplay = ib.ImgDisabled;
                    }

                    ic.Dispose();
                }
                catch
                {
                    //Mache nichts
                }
            }
        }

        private static ImageSource BitmapToImageSource(System.Drawing.Bitmap bmp)
        {
            BitmapImage bitImg = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                bitImg.BeginInit();
                bitImg.StreamSource = new MemoryStream(ms.ToArray());
                bitImg.EndInit();
            }
            bmp.Dispose();
            return bitImg;
        }

        private static System.Drawing.Bitmap MakeGrayscale3(System.Drawing.Bitmap original)
        {
            //create a blank bitmap the same size as original
            System.Drawing.Bitmap newBitmap = new System.Drawing.Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                       new float[][] 
              {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
              });

            //create some image attributes
            System.Drawing.Imaging.ImageAttributes attributes = new System.Drawing.Imaging.ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, System.Drawing.GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }


    }
}
