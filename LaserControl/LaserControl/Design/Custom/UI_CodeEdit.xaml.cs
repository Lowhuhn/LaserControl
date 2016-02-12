using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
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

namespace LaserControl.Design.Custom
{
    
    /// <summary>
    /// Interaktionslogik für UI_CodeEdit.xaml
    /// </summary>
    public partial class UI_CodeEdit : UserControl
    {
        #region Syntax Highlighter
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
        #endregion

        public static readonly DependencyProperty CodeSourceProperty = DependencyProperty.Register(
            "Code",
            typeof(string),
            typeof(UI_CodeEdit),
            new UIPropertyMetadata("", new PropertyChangedCallback(CodeSourcePropertyChange))
            );


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

        public UI_CodeEdit()
        {
            InitializeComponent();
            this.InternTextEdit.SyntaxHighlighting = LaserScriptHighlighter;
        }

        /// <summary>
        /// Methode die den Code in die Avalonedit Textarea setzt
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CodeSourcePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d.GetType() == typeof(UI_CodeEdit))
            {
                try
                {
                    UI_CodeEdit ce = ((UI_CodeEdit)d);
                    ce.InternTextEdit.Text = (string)e.NewValue;
                }
                catch
                {

                }
            }
        }
    }
}
