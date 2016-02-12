using LaserControl.HardwareAPI;
using System;
using System.Collections.Generic;
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

using LaserControl.Design.Custom;

namespace LaserControl.UIElements
{
    /// <summary>
    /// Interaktionslogik für UI_IOControl.xaml
    /// </summary>
    public partial class UI_IOControl : UserControl
    {
        protected HardwareController HWC;

        public UI_IOControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Erstellt für alle IOController im HardwareController die GUI Elemente.
        /// </summary>
        /// <param name="hwc">Der Verbundene HardwareController</param>
        public void SetHardwareControllerAndInitGUI(HardwareController hwc)
        {
            HWC = hwc;
            string[] ios = HWC.IOControllerControlIdents;
            int inRow = 0;
            int outRow = 0;
            foreach (var i in ios)
            {                
                IOController ic = HWC.GetIOController(i);
                foreach (var p in ic.All)
                {
                    if (p.PortType == IOPortType.IN || p.PortType == IOPortType.INOUT)
                    {
                        RowDefinition rdin = new RowDefinition();
                        rdin.Height = new GridLength(30);
                        InGrid.RowDefinitions.Add(rdin);

                        Label l = new Label();
                        l.Content = p.InName;
                        Grid.SetRow(l, inRow);
                        InGrid.Children.Add(l);

                        Label l2 = new Label();
                        l2.Content = ic.ControlIdent;
                        Grid.SetRow(l2, inRow);
                        Grid.SetColumn(l2, 1);
                        InGrid.Children.Add(l2);

                        Label l3 = new Label();
                        l3.Content = p.Bit;
                        Grid.SetRow(l3, inRow);
                        Grid.SetColumn(l3, 2);
                        InGrid.Children.Add(l3);

                        ImgButton b = new ImgButton();
                        b.Width = 60;
                        b.ImgToShow = p.InValue ? "bullet_green" : "bullet_white";
                        b.Text = p.InValue ? "ON" : "OFF";
                        Grid.SetRow(b, inRow);
                        Grid.SetColumn(b, 3);
                        InGrid.Children.Add(b);

                        ic.OnInBitValueChange += new IOControllerIntBoolEvent(delegate(IOController iocont, int bit, bool val)
                        {
                            if (bit == p.Bit)
                            {
                                this.Dispatcher.Invoke(new Action(()=>
                                {
                                    b.ImgToShow = val ? "bullet_green" : "bullet_white";
                                    b.Text = val ? "ON" : "OFF";
                                }));
                            }
                        });

                        ++inRow;
                    }

                    if (p.PortType == IOPortType.OUT || p.PortType == IOPortType.INOUT)
                    {
                        RowDefinition rdout = new RowDefinition();
                        rdout.Height = new GridLength(30);
                        OutGrid.RowDefinitions.Add(rdout);

                        Label l = new Label();
                        l.Content = p.OutName;
                        Grid.SetRow(l, outRow);
                        OutGrid.Children.Add(l);

                        Label l2 = new Label();
                        l2.Content = ic.ControlIdent;
                        Grid.SetRow(l2, outRow);
                        Grid.SetColumn(l2, 1);
                        OutGrid.Children.Add(l2);

                        Label l3 = new Label();
                        l3.Content = p.Bit;
                        Grid.SetRow(l3, outRow);
                        Grid.SetColumn(l3, 2);
                        OutGrid.Children.Add(l3);

                        ImgButton b = new ImgButton();
                        b.Width = 60;
                        b.ImgToShow = p.OutValue ? "bullet_green" : "bullet_white";
                        b.Text = p.OutValue ? "ON" : "OFF";
                        ic.OnOutBitValueChange += new IOControllerIntBoolEvent(delegate(IOController iocont, int bit, bool val)
                        {
                            if (bit == p.Bit)
                            {
                                this.Dispatcher.Invoke(new Action(() =>
                                {
                                    b.ImgToShow = val ? "bullet_green" : "bullet_white";
                                    b.Text = val ? "ON" : "OFF";
                                }));
                            }
                        });
                        b.Click += new RoutedEventHandler(delegate(System.Object o, RoutedEventArgs e)
                        {                            
                            ic.WriteOutValue(p.Bit, !p.OutValue);
                        });
                        Grid.SetRow(b, outRow);
                        Grid.SetColumn(b, 3);
                        OutGrid.Children.Add(b);

                        ++outRow;
                    }
                }
            }
        }
    }
}
