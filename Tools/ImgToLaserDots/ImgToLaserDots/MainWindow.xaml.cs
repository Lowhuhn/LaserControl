using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace ImgToLaserDots
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        protected Random random = new Random();

        protected System.Drawing.Bitmap Original;
        protected System.Drawing.Bitmap ProcededOriginal;

        protected int DPP;
        protected int ColorCount;

        public MainWindow()
        {
            InitializeComponent();

            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                AllocConsole();
                //Run alg

                Console.WriteLine("Load original...");
                System.Drawing.Bitmap org = new System.Drawing.Bitmap(openFileDialog1.FileName);
                Original = org.Clone(new System.Drawing.Rectangle(0, 0, org.Width, org.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                //Convert to grayscale
                Console.WriteLine("Convert original to grayscale");
                for (int i = 0; i < Original.Width; i++)
                {
                    for (int x = 0; x < Original.Height; x++)
                    {
                        System.Drawing.Color oc = Original.GetPixel(i, x);
                        int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                        System.Drawing.Color nc = System.Drawing.Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                        Original.SetPixel(i, x, nc);
                    }
                }

                PerformAlgorithm();

                this.ImgOrg.Source = BitmapToImageSource(Original);
               // FreeConsole();
            }
            else
            {
                this.Close();
            }
        }

        protected void PerformAlgorithm()
        {
            Console.Write("Dots per Pixel ( dpp x dpp ): ");
            DPP = int.Parse(Console.ReadLine());
            Console.WriteLine("");

            ColorCount = 255 / (DPP * DPP);

            int w = Original.Width;
            int h = Original.Height;
            int wN = w * DPP;
            int hN = h * DPP;

            Console.WriteLine("Step 1:");
            System.Drawing.Bitmap step1img = new System.Drawing.Bitmap(Original.Width, Original.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for(int x = 0; x < w; ++x){
                for(int y = 0; y < h; ++y){
                    System.Drawing.Color oc = Original.GetPixel(x, y);
                    byte nv = (byte)((oc.R / ColorCount) * ColorCount);
                    System.Drawing.Color nc = System.Drawing.Color.FromArgb(255, nv, nv, nv);
                    step1img.SetPixel(x, y, nc);
                }
            }
            

            Console.WriteLine("Step 2:");
            Console.WriteLine("> Init");
            System.Drawing.Bitmap step2img = new System.Drawing.Bitmap(wN, hN, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int x = 0; x < wN; ++x)
            {
                for (int y = 0; y < hN; ++y)
                {
                    System.Drawing.Color nc = System.Drawing.Color.FromArgb(255, 255, 0, 255);
                    step2img.SetPixel(x, y, nc);
                }
            }

            Console.WriteLine("> Perform");
            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    byte curCol = step1img.GetPixel(x, y).R;
                    //System.Drawing.Color nc = System.Drawing.Color.FromArgb(255, curCol, curCol, curCol);
                    ApplyPixelMatrix(step2img, x * DPP, y * DPP, curCol);                        
                }
            }

            ImgOrgStep1.Source = BitmapToImageSource(step1img);
            ImgOrgStep2.Source = BitmapToImageSource(step2img);

            Console.WriteLine("Step 3: Create LSF");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//Position for Image");
            sb.AppendLine("$X = 0");
            sb.AppendLine("$Y = 0");
            sb.AppendLine("");
            sb.AppendLine("//Time ");
            sb.AppendLine("$TIME = 10");
            sb.AppendLine("$DELAY = 10");

            Console.Write("Spot size: ");
            double spot = double.Parse(Console.ReadLine());

            int count = 0;
            for (int x = 0; x < wN; ++x)
            {
                for (int y = 0; y < hN; ++y)
                {
                    if (step2img.GetPixel(x, y).R > 128)
                    {
                        ++count;
                        sb.AppendFormat("POINT ($X + {0}) ($Y + {1}) ($TIME) ($DELAY)\n", x*spot, y*spot);
                    }
                }
            }

            LSFTextBox.Text = string.Format("// Points to Scribe: {0} \n// Width: {1} [mm]\n// Height: {2} [mm]\n\n", count, spot*wN, spot*hN) + sb.ToString();

            ProcededOriginal = step2img;
        }


        protected void ApplyPixelMatrix(System.Drawing.Bitmap bmp, int x, int y, byte curCol)
        {
            System.Drawing.Color black = System.Drawing.Color.FromArgb(255, 0, 0, 0);
            System.Drawing.Color white = System.Drawing.Color.FromArgb(255, 255, 255, 255);

            curCol = (byte)(curCol / ColorCount);
            if (curCol == 0)
            {
                
                for (int i = 0; i < DPP; ++i)
                {
                    for (int j = 0; j < DPP; ++j)
                    {
                        bmp.SetPixel(x + i, y + j, black);
                    }
                }
            }

            if (curCol != 0 && curCol != (DPP*DPP))
            {
                int[,] mat = createRandomMatrix(DPP, DPP, curCol);
                for (int i = 0; i < DPP; ++i)
                {
                    for (int j = 0; j < DPP; ++j)
                    {
                        if (mat[i, j] == 1)
                        {
                            bmp.SetPixel(x + i, y + j, white);
                        }
                        else
                        {
                            bmp.SetPixel(x + i, y + j, black);
                        }
                    }
                }
            }

            if (curCol == (DPP*DPP))
            {
                
                for (int i = 0; i < DPP; ++i)
                {
                    for (int j = 0; j < DPP; ++j)
                    {
                        bmp.SetPixel(x + i, y + j, white);
                    }
                }
            }
        }

        protected int[,] createRandomMatrix(int w, int h, int c)
        {
            
            int[,] mat = new int[w, h];

            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    mat[i, j] = 0;
                }
            }

            int sum = 0;
            while (sum < c)
            {
                int x = random.Next(0, DPP);
                int y = random.Next(0, DPP);
                if (mat[x, y] == 0)
                {
                    mat[x, y] = 1;
                    sum++;
                }
            }

            return mat;
        }


        #region Static Functions
        private static ImageSource BitmapToImageSource(System.Drawing.Bitmap bmp)
        {
            BitmapImage bitImg = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                bitImg.BeginInit();
                bitImg.StreamSource = new MemoryStream(ms.ToArray());
                bitImg.EndInit();
            }
            //bmp.Dispose();
            return bitImg;
        }
        #endregion //Static Functions

        private void SaveImgbtn_Click(object sender, RoutedEventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                ProcededOriginal.Save(saveFileDialog1.FileName);
            }
        }
    }
}
