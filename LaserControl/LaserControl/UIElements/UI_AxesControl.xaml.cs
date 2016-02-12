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

namespace LaserControl.UIElements
{
    /// <summary>
    /// Interaktionslogik für UI_AxesControl.xaml
    /// </summary>
    public partial class UI_AxesControl : UserControl
    {
        #region Readonly Properties

        protected readonly int HeightOfRow = 31;

        #endregion //Readonly Properties

        protected HardwareController HWC;

        public UI_AxesControl()
        {
            InitializeComponent();
#if DEBUG || TEST
            this.SideGrid.Visibility = System.Windows.Visibility.Visible;
#else
            this.SideGrid.Visibility = System.Windows.Visibility.Collapsed;
            this.RightCol.Width = new GridLength(0.0);
            this.DistanceCol.Width = new GridLength(0.0);
#endif
        }

        /// <summary>
        /// Erstellt für alle Achsen im HardwareController die GUI Elemente.
        /// </summary>
        /// <param name="hwc">Der Verbundene HardwareController</param>
        public void SetHardwareControllerAndInitGUI(HardwareController hwc)
        {
            HWC = hwc;

            string[] axes = hwc.AxesControlIdents;
            for (int i = 0; i < axes.Length; ++i)
            {
                MainGrid.RowDefinitions.Add(NewRowDef());

                Axis a = HWC.GetAxis(axes[i]);
                UI_OneAxis one = new UI_OneAxis();
                one.SetAxisAndInitGUI(HWC, a);

                Grid.SetRow(one, i + 1);
                MainGrid.Children.Add(one);
            }
        }

        protected RowDefinition NewRowDef()
        {
            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(HeightOfRow);
            return rd;
        }
    }
}
