using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

using LaserControl.Library;
using System.Threading;

namespace LaserControl.UIElements
{
    public class OneRow
    {
        public static int UID = 0;

        public ImageSource Type
        {
            get;
            set;
        }

        public string ID
        {
            get;
            protected set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Time
        {
            get;
            protected set;
        }

        public string Date
        {
            get;
            protected set;
        }

        public OneRow()
        {
            this.ID = (++UID).ToString();
            this.Time = DateTime.Now.ToString("HH:mm:ss");
            this.Date = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private OneRow(string id)
        {
            this.ID = id;
            this.Time = DateTime.Now.ToString("HH:mm:ss");
            this.Date = DateTime.Now.ToString("dd.MM.yyyy");
        }
    }

    public class UI_ErrorControlDataHandler : INotifyPropertyChanged
    {

        protected List<OneRow> _DGContent;
        public List<OneRow> DGContent
        {
            get { return _DGContent; }
            set
            {
                _DGContent = value;
                NotifyPropertyChanged("DGContent");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
    /// <summary>
    /// Interaktionslogik für UI_ErrorControl.xaml
    /// </summary>
    public partial class UI_ErrorControl : UserControl
    {
        protected UI_ErrorControlDataHandler Data;

        public UI_ErrorControl()
        {
            InitializeComponent();
            Data = FindResource("datahandler") as UI_ErrorControlDataHandler;
            Data.DGContent = new List<OneRow>();

            LaserControl.Library.GlobalEvents.NewInformation += this.NewInformation;

            /*
            List<OneRow> s = new List<OneRow>();
            System.Drawing.Icon ic = (System.Drawing.Icon)LaserControl.Properties.Resources.crit_error_2;
            System.Drawing.Bitmap bmp = ic.ToBitmap();
            s.Add(new OneRow() { Type = BitmapToImageSource(bmp), Description = "Hallo Welt" });
            Data.DGContent = s;
            */
        }

        

        private void NewInformation(string msg, string twhere, int line)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                List<OneRow> l = new List<OneRow>(Data.DGContent);
                l.Add(new OneRow()
                {
                    Type = BitmapToImageSource((System.Drawing.Bitmap)(((System.Drawing.Icon)LaserControl.Properties.Resources.information).ToBitmap())),
                    Description = msg
                });
                Data.DGContent = l;
            }));            

        }

        #region Static Functions
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
        #endregion //Static Functions
    }
}
