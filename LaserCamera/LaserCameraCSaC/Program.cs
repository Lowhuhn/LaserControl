using LaserCameraCSaC.TestUSW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LaserCameraCSaC
{
    class Program
    {
        protected static Form1 MyForm;
        public static long millis = 0;

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("{0}\nFinal version?!\n", CWrapper.Path);
            CWrapper.Load();

            Console.WriteLine("\n");

            Service.CameraService.DispInitMessage();

            Console.WriteLine("\n");

            /*Console.WriteLine("0) WinForms\n1) WPF");
            Console.Write("> ");*/
            string input = "1";// Console.ReadLine();
            if (input == "0")
            {
                /*
                MyForm = new Form1();
                Thread t = new Thread(() =>
                {
                    MyForm.ShowDialog();
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();

                Console.Write("\nChoose Camera:\n0 - Balser Pylon\n1 - Dummy\n2 - Dummy with continuous shot\n3 - Exit\n> ");
                input = Console.ReadLine();
                if (input == "0")
                {
                    if (Camera.Instance.IsConnected)
                    {
                        Console.Write("Press <ENTER> for continuous grab!");
                        Console.ReadLine();
                        Camera.Instance.StartGrab();
                        Camera.Instance.ImageGrabbed += OnNewImage;
                    }
                    else
                    {
                        Console.Write("Can't connect to basler pylon camera! \nPlease restart the program with a connected camera!\n");
                        Console.ReadLine();
                        return;
                    }
                }
                else if (input == "1")
                {
                    Thread t2 = new Thread(DummyCam);
                    t2.IsBackground = true;
                    t2.SetApartmentState(ApartmentState.STA);
                    t2.Start();
                }
                else if (input == "2")
                {
                    Thread t2 = new Thread(DummyCam2);
                    t2.IsBackground = true;
                    t2.SetApartmentState(ApartmentState.STA);
                    t2.Start();
                }
                else
                {
                    return;
                }


                int img = 0, cross = 0;
                while (true)
                {
                    try
                    {
                        Console.Clear();
                        Console.WriteLine("ctrl + c to quit!\n");
                        /*Console.Write("Choose Image and Cross:\n" +
                                          " Image(Selected: {0}):\n 0 - Original\n 1 - LightCorrection\n 2 - Normalized\n 3 - BlackWhite \n 4 - NoiseReduction\n 5 - Inverted \n > ", img);
                        Console.Write("Choose Image and Cross:\n" +
                                      " Image(Selected: {0}):\n 0 - Original\n 1 - LightCorrection+Salt&Pepper\n 2 - Gaussian filter\n 3 - Edges\n 4 - Edges(2)\n > ", img);
                        img = int.Parse(Console.ReadLine()) % 6;
                        Console.Write(" Cross(Selected: {0}):\n 0 - no cross\n 1 - center cross(red)\n 2 - detected cross(green)\n 3 - both (center and detected) cross\n > ", cross);
                        cross = int.Parse(Console.ReadLine()) % 4;
                        CWrapper.SetParameter(img, cross);
                    }
                    catch
                    {
                        //nix
                    }
                }
                //Console.ReadKey();
                */
            }
            else
            {
                FinalWindow fw = new FinalWindow();
                if (!Camera.Instance.IsConnected)
                {
                    Thread t = new Thread(() =>
                    {
                        string pfad = "";
                        OpenFileDialog ofd = new OpenFileDialog();
                        if (ofd.ShowDialog() == DialogResult.OK)
                        {
                            pfad = ofd.FileName;
                        }
                        Bitmap b = Bitmap.FromFile(pfad) as Bitmap;
                        int width = b.Width , height = b.Height ;
                        while (true)
                        {
                            Bitmap c = b.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                            BitmapData data = c.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                            Stopwatch s = new Stopwatch();
                            s.Start();
                            CWrapper.SetImagePointer(data.Scan0, width, height);
                            s.Stop();
                            //Console.Write("{0}\t", s.ElapsedMilliseconds);
                            s.Reset();                           
                            s.Start();
                            CWrapper.ProcessImage();
                            s.Stop();
                            //Console.Write("{0}\n", s.ElapsedMilliseconds);

                            c.UnlockBits(data);
                            fw.NewImage(c);
                            c.Dispose();

                            //Thread.Sleep(100);
                        }
                    });
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                }
                else
                {
                    Camera.Instance.ImageGrabbed += fw.NewImage;
                    Camera.Instance.StartGrab();
                }

                /*Thread cross = new Thread(() =>
                {
                    while (true)
                    {
                        CWrapper.CrossDetection();
                        Thread.Sleep(100);
                    }
                });
                cross.IsBackground = true;
                cross.Start();*/
                fw.ShowDialog();
            }
        }

        private static void DummyCam2(object obj)
        {
            
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string Pfad = ofd.FileName;
                while (true)
                {
                    Bitmap b = Bitmap.FromFile(Pfad) as Bitmap;
                    int width = b.Width, height = b.Height;
                    b = b.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    BitmapData data = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    Stopwatch sp = new Stopwatch();
                    sp.Start();
                    CWrapper.SetImagePointer(data.Scan0, b.Width, b.Height);
                    CWrapper.ProcessImage();
                    Thread.Sleep(1000);
                    sp.Stop();
                    millis = sp.ElapsedMilliseconds;
                    b.UnlockBits(data);
                    OnNewImage(b);
                    b.Dispose();
                }
            }            
        }

        [STAThread]
        static void Main2(string[] args)
        {
            /*Test();
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string Pfad = ofd.FileName;
                Bitmap b = Bitmap.FromFile(Pfad) as Bitmap;
                int width = b.Width, height = b.Height;
                int s = 0;

                MyForm2 = new Form1();
                MyForm2.Text = "Original";
                MyForm2.pictureBox1.Image = b;
                Thread t = new Thread(() =>
                {
                    MyForm2.ShowDialog();
                });
                t.Start();

                for (int i = 0; i < 10; ++i)
                {
                    b = Bitmap.FromFile(Pfad) as Bitmap;
                    Console.WriteLine("Run {0} -----------------------------------------------------------", i);
                    Stopwatch sp = new Stopwatch();
                    sp.Start();
                    b = b.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    if (b.Width > 1000)
                    {
                        b = new Bitmap(ResizePicByWidth(b, width / 2,height/2));
                        b = b.Clone(new Rectangle(0, 0, b.Width, b.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    }
                    sp.Stop();
                    
                    Console.WriteLine("Clone: {0}ms", sp.ElapsedMilliseconds);

                    sp.Restart();
                    BitmapData data = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    sp.Stop();
                    Console.WriteLine("LockBits: {0}ms", sp.ElapsedMilliseconds);

                    sp.Restart();
                    SetImagePointer(data.Scan0, b.Width, b.Height);
                    sp.Stop();
                    Console.WriteLine("SetImagePointer: {0}ms", sp.ElapsedMilliseconds);

                    sp.Restart();
                    ImageProcess_CannyAlgorithm();
                    sp.Stop();
                    Console.WriteLine("Process_GrayScale: {0}ms", sp.ElapsedMilliseconds);

                    sp.Restart();
                    b.UnlockBits(data);
                    sp.Stop();
                    Console.WriteLine("UnlockBits: {0}ms", sp.ElapsedMilliseconds);

                    b.Save("Tada.jpg");
                }
                MyForm = new Form1();
                MyForm.Text = "Processed";
                MyForm.pictureBox1.Image = b;
                t = new Thread(() =>
                {
                    MyForm.ShowDialog();
                });
                t.Start();

            }
            Console.ReadKey();*/
        }


        private static void DummyCam()
        {
            while (true)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string Pfad = ofd.FileName;
                    Bitmap b = Bitmap.FromFile(Pfad) as Bitmap;
                    b = ResizeImage(b, 300, 228); // 25%
                    //b = ResizeImage(b, 600, 456); 
                    int width = b.Width, height = b.Height;
                    b = b.Clone(new Rectangle(0, 0, width, height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    BitmapData data = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    Stopwatch sp = new Stopwatch();
                    sp.Start();
                    CWrapper.SetImagePointer(data.Scan0, b.Width, b.Height);                    
                    CWrapper.ProcessImage();
                    //sp.Stop();
                    millis = sp.ElapsedMilliseconds;
                    b.UnlockBits(data);
                    OnNewImage(b);
                    b.Dispose();
                    
                }
            }
        }

        delegate void NewImage(Bitmap b);
        public static void OnNewImage(Bitmap b)
        {
            if (MyForm.pictureBox1.InvokeRequired)
            {
                NewImage dd = OnNewImage;
                MyForm.Invoke(dd,b);
            }
            else
            {
                if (MyForm.pictureBox1.Image != null)
                    MyForm.pictureBox1.Image.Dispose();
                MyForm.pictureBox1.Image = b.Clone() as Bitmap;
                MyForm.Text = string.Format("Time: {0}ms, Cross found: {1}", millis, CWrapper.GetCrossFound());
            }

        }

        private static Bitmap ResizePicByWidth(Image sourceImage, int newWidth, int newHeight)
        {
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.InterpolationMode = InterpolationMode.Low;
                g.DrawImage(sourceImage, new Rectangle(0, 0, (int)newWidth, (int)newHeight));
            }
            return newImage;
        }


        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
