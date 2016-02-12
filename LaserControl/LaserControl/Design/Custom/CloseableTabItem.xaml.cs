using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Xml;

using LaserControl.ScriptV2;
using Microsoft.Win32;
using LaserControl.ScriptV2.Exceptions;
using ICSharpCode.AvalonEdit.Rendering;

namespace LaserControl.Design.Custom
{
    public class CloseableTabItemDataHandler : INotifyPropertyChanged
    {
        protected string _Code;
        public string Code
        {
            get { return _Code; }
            set
            {
                this._Code = value;
                this.NotifyPropertyChanged("Code");
            }
        }

        protected Visibility _IsSavedVis;
        public Visibility IsSavedVis
        {
            get { return _IsSavedVis; }
            set
            {
                _IsSavedVis = value;
                this.NotifyPropertyChanged("IsSavedVis");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    public class LineBackgroundRenderer : IBackgroundRenderer
    {
        protected Pen ErrorPen;// = new Pen(Brushes.Black , 0.0); 
        protected SolidColorBrush ErrorBackground; // = Brushes.Aquamarine;

        protected Pen CurrentLinePen;
        protected SolidColorBrush CurrentLineBackground;

        //public List<int> LinesToHighLight = new List<int>();
       
        public int CurrentLine
        {
            get;
            set;
        }
        public List<int> ErrorLines;

        public LineBackgroundRenderer()
        {
            ErrorPen = new Pen(Brushes.Wheat, 0.0);
            ErrorBackground = new SolidColorBrush(Color.FromArgb(64, 200, 0, 0));

            CurrentLinePen = new Pen(new SolidColorBrush(Color.FromArgb(255, 144, 238, 144)), 1.0);
            CurrentLineBackground = new SolidColorBrush(Color.FromArgb(255, 144, 238, 144));

            CurrentLine = -1;
            ErrorLines = new List<int>();
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            int cl = CurrentLine + 1;
            foreach (VisualLine v in textView.VisualLines)
            {                
                //CurrentLine
                if (cl == v.FirstDocumentLine.LineNumber)
                {
                    Rect rc = BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, 1000).First();
                    drawingContext.DrawRectangle(CurrentLineBackground, CurrentLinePen, new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
                }

                //Error Lines
                if (ErrorLines.Contains(v.FirstDocumentLine.LineNumber))
                {
                    Rect rc = BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, v, 0, 1000).First();                    
                    drawingContext.DrawRectangle(ErrorBackground, ErrorPen, new Rect(0, rc.Top, textView.ActualWidth, rc.Height));
                }
            }
        }

        public void Reset()
        {
            CurrentLine = -1;
            ErrorLines.Clear();
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }
    }

    /// <summary>
    /// Interaktionslogik für CloseableTabItem.xaml
    /// </summary>
    public partial class CloseableTabItem : TabItem
    {
        public static readonly RoutedEvent CloseTabEvent = EventManager.RegisterRoutedEvent("CloseTab", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(CloseableTabItem));

        public event RoutedEventHandler CloseTab
        {
            add
            {
                this.AddHandler(CloseTabEvent, value);
            }
            remove
            {
                this.RemoveHandler(CloseTabEvent, value);
            }
        }

        public static readonly DependencyProperty CodeSourceProperty = DependencyProperty.Register(
            "Code",
            typeof(string),
            typeof(CloseableTabItem),
            new UIPropertyMetadata("", new PropertyChangedCallback(CodeSourcePropertyChange))
            );

        protected static IHighlightingDefinition _LaserScriptHighlighter = null;
        protected static IHighlightingDefinition LaserScriptHighlighter
        {
            get
            {
                if (_LaserScriptHighlighter == null)
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceName = Properties.Resources.LaserScriptStyle;
                    StringReader sr = new StringReader(resourceName);
                    using (XmlTextReader reader = new XmlTextReader(sr))
                    {
                        _LaserScriptHighlighter = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
                return _LaserScriptHighlighter;
            }
        }

        public string Code
        {
            get
            {
                //return (string)GetValue(CodeSourceProperty);
                return InternTextEdit.Text;
            }
            set
            {
                SetValue(CodeSourceProperty, value);
            }
        }

        protected CloseableTabItemDataHandler Data;

        public CloseableTabItem()
        {
            InitializeComponent();
            Data = FindResource("datahandler") as CloseableTabItemDataHandler;

            this.InternTextEdit.SyntaxHighlighting = LaserScriptHighlighter;
             

            this.HighlightedLine = new LineBackgroundRenderer();
            this.InternTextEdit.TextArea.TextView.BackgroundRenderers.Add(this.HighlightedLine);

            Path = string.Empty;
            IsSaved = true;
        }

        public CloseableTabItem(string header) : this()
        {
            this.Header = header;            
        }

        public CloseableTabItem(string header, bool open)
            : this(header)
        {
            if (open)
            {
                this.Open();
            }
        }

        protected static void CodeSourcePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d.GetType() == typeof(CloseableTabItem))
            {
                try
                {
                    CloseableTabItem cti = ((CloseableTabItem)d);
                    cti.InternTextEdit.Text = (string)e.NewValue;
                }
                catch
                {

                }
            }
        }

        private void PART_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Code = "";
            RoutedEventArgs r = new RoutedEventArgs(CloseTabEvent);
            RaiseEvent(r);
            e.Handled = true;
        }

        #region Properties for Menu Actions

        protected string _Path;
        public string Path
        {
            get{return _Path;}
            protected set{
                _Path = value;
                // Data.Path _Path;
            }
        }

        protected bool _IsSaved;
        public bool IsSaved
        {
            get
            {
                return _IsSaved;
            }
            set
            {
                _IsSaved = value;
                Data.IsSavedVis = value ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public bool CanUndo
        {
            get
            {
                return this.InternTextEdit.CanUndo;
            }
        }

        public bool CanRedo
        {
            get
            {
                return this.InternTextEdit.CanRedo;
            }
        }

        #endregion Properties for Menu Actions

        #region Properties for Highlighting

        protected LineBackgroundRenderer HighlightedLine;

        #endregion //Properties for Highlighting

        #region External Menu Actions

        public void Open()
        {
            OpenFileDialog ofd1 = new OpenFileDialog();
            ofd1.Filter = "Laser Script File|*.lsf|Text File|*.txt";
            ofd1.Title = "Open an Laser Script File";
            if (ofd1.ShowDialog() != true)
            {
                return;
            }

            string p = ofd1.FileName;
            string fname = System.IO.Path.GetFileName(p);
            if (!string.IsNullOrEmpty(p))
            {
                if (File.Exists(p))
                {
                    Code = File.ReadAllText(p);
                    Path = p;
                    Header = fname;
                    IsSaved = true;
                }
            }
        }

        public void Redo()
        {
            this.InternTextEdit.Redo();            
        }

        public void Run()
        {
            this.HighlightedLine.Reset();
            ScriptHandler.SetCodeThread1(this.Code, this.OnParserException, this.OnNewLine);
        }

        public void Save()
        {
            if (!IsSaved)
            {
                //Gibt es schon einen Pfad.
                if (string.IsNullOrEmpty(Path))
                {
                    this.SaveTo();
                    return;
                }
                using (StreamWriter sw = new StreamWriter(Path, false))
                {
                    sw.Write(Code);
                    sw.Flush();
                }
                IsSaved = true;
            }
        }

        public void SaveTo()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Laser Script File|*.lsf|Text File|*.txt";
            saveFileDialog1.Title = "Save an Laser Script File";
            if (!string.IsNullOrEmpty(Path))
            {
                saveFileDialog1.FileName = Path;
            }
            if (saveFileDialog1.ShowDialog() != true)
            {
                return;
            }

            string p = saveFileDialog1.FileName;
            string fname = System.IO.Path.GetFileName(p);

            if (!string.IsNullOrEmpty(p))
            {                                
                using (StreamWriter sw = new StreamWriter(p, false))
                {
                    sw.Write(Code);
                    sw.Flush();
                }
                Path = p;
                Header = fname;
                IsSaved = true;
            }
        }

        public void Undo()
        {
            this.InternTextEdit.Undo();
        }

        #endregion //External Menu Actions

        private void InternTextEdit_TextChanged(object sender, EventArgs e)
        {
            this.IsSaved = false;
        }

        #region Parser Exceptions

        protected void OnParserException(ParserException pEx)
        {
            //Change color of text : http://stackoverflow.com/questions/29008274/how-to-change-text-color-at-icsharpcode-avalonedit-texteditor

            this.HighlightedLine.ErrorLines.Add(pEx.ParserErrorLine + 1);
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.InternTextEdit.TextArea.Caret.Line = pEx.ParserErrorLine + 1;
                this.InternTextEdit.TextArea.TextView.Redraw();                
            }));

            MessageBox.Show(pEx.Message);
        }

        #endregion //Parser Exception

        #region Lines

        protected void OnNewLine(int newLine)
        {
            this.HighlightedLine.CurrentLine = newLine;
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.InternTextEdit.TextArea.TextView.Redraw();
            }));
        }

        #endregion // Lines
    }
}
