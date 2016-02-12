using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WPFSyntaxHighlighting
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextInput.TextChanged -= TextInput_TextChanged;
            string tokens = "(auto|double|int|struct|break|else|long|switch|case|"+ 
                              "enum|register|typedef|char|extern|return|union|const|"+  
                             "float|short|unsigned|continue|for|signed|void |default|"+
                              "goto|sizeof|volatile|do|if|static|while|using)";  
            Regex rex = new Regex(tokens);  


            if (TextInput.Document == null)
                return;

            TextRange documentRange = new TextRange(TextInput.Document.ContentStart, TextInput.Document.ContentEnd);
            documentRange.ClearAllProperties();

            //Console.WriteLine(documentRange.Text);

            TextPointer position = TextInput.Document.ContentStart;
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    break;
                }
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            MatchCollection mc = rex.Matches(documentRange.Text);
            foreach (Match m in mc)
            {
                //Console.Write("{0}({1}-{2}), ", m.Value, m.Index, m.Index + m.Length);
                TextPointer t1 = GetTextPointAt(position, m.Index);
                TextRange tr = new TextRange(t1, GetTextPointAt(t1, m.Length));
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
            }
            TextInput.TextChanged += TextInput_TextChanged;
            return;
            /*
            int line = 0;
            int b = 0;
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);                  
                    MatchCollection mc = rex.Matches(textRun);
                    foreach (Match m in mc)
                    {
                        //Console.Write("{0}({1}-{2}), ", m.Value, m.Index, m.Index+m.Length);
                        TextPointer t1 = GetTextPointAt(position, m.Index);
                        TextRange tr = new TextRange(t1, GetTextPointAt(t1, m.Length));

                        if ((b % 3) == 0)
                        {
                            tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Red));
                        }
                        else if ((b % 3) == 1)
                        {
                            tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.LightBlue));
                        }
                        else
                        {
                            tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Yellow));
                        }
                        ++b;
                        
                    }
                    // Find the starting index of any substring that matches "word".
                    /*int indexInRun = textRun.IndexOf("void");
                    if (indexInRun >= 0)
                    {
                        Console.WriteLine("Found: {0}, text:\"{1}\" ", indexInRun, textRun);
                        //position = position.GetPositionAtOffset(indexInRun);
                        //break;
                    }
                    position = position.GetLineStartPosition(1);
                    continue;
                }
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }
    */

            /*if (tp1 == null)
                return;
            while (tp1.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.Text)
            {                
                tp1 = tp1.GetNextContextPosition(LogicalDirection.Forward);                
            }
            //Console.WriteLine("");

            foreach (Match m in mc)
            {
                Console.WriteLine("Match: {0} [{1},{2}]", m.Value, m.Index, m.Length);
                //int startIdx = m.Index;
                //int stopIdx = m.Length;
                //documentRange.Select(TextInput.Document.ContentStart, TextInput.Document.ContentStart.GetPositionAtOffset(m.Index + m.Length, LogicalDirection.Forward));
                TextPointer tp2 = tp1.GetPositionAtOffset(m.Index, LogicalDirection.Forward);

                Console.WriteLine(tp2.GetOffsetToPosition(tp2.GetPositionAtOffset(m.Length, LogicalDirection.Forward)));

                TextRange tr = new TextRange(tp2, tp2.GetPositionAtOffset(m.Length, LogicalDirection.Forward));

                tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
            }*/
            

            TextInput.TextChanged += TextInput_TextChanged;
            //TextInput.TextChanged += TextInput_TextChanged;


            
            /*documentRange.ClearAllProperties();

            Console.WriteLine(documentRange.Text);

            TextPointer navigator = TextInput.Document.ContentStart;

            while (navigator.CompareTo(TextInput.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    Console.WriteLine(navigator.GetTextRunLength(LogicalDirection.Forward));
                }

                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }

            Format();
             * */
        }

        private static TextPointer GetTextPointAt(TextPointer from, int pos)
        {
            TextPointer ret = from;
            int i = 0;

            while ((i < pos) && (ret != null))
            {
                if ((ret.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text) || (ret.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.None))
                    i++;

                if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
                    return ret;

                ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
            }

            return ret;
        }

        void Format()
        {
            Console.WriteLine("Format");
            TextPointer tp = TextInput.Document.ContentStart;
            tp = FindNextString(tp);
            
            TextPointer textRangeEnd = tp.GetPositionAtOffset(1, LogicalDirection.Forward);

            TextRange tokenTextRange = new TextRange(tp, tp.GetPositionAtOffset(0,LogicalDirection.Forward));

            tokenTextRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Red);
        }

        private TextPointer FindNextString(TextPointer tp)
        {
            char[] buffer = new char[1];
            tp.GetTextInRun(LogicalDirection.Forward, buffer, 0, 1);
            return tp;
        }
    }
}
