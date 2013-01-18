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
using MapAround.DataProviders;
using MapAround.Geometry;
using MapAround.Mapping;
using Microsoft.Win32;

namespace WPFExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Map _map;

        public MainWindow()
        {
            _map = new MapAround.Mapping.Map();
            InitializeComponent();
            mapControl.Map = _map;
        }


        private void Zoom_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _map.Layers.Count > 0;
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Refresh_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _map.Layers.Count > 0;
        }

        private void Refreshg_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SetViewBox();
        }

        private void SetViewBox()
        {
            BoundingRectangle rectangle = _map.CalculateBoundingRectangle();
            if (rectangle.IsEmpty()) return;

            double deltaY = rectangle.Width * mapControl.ActualHeight / 2 / mapControl.ActualWidth - rectangle.Height / 2;




            mapControl.SetViewBox(new BoundingRectangle(rectangle.MinX, rectangle.MinY - deltaY,
                                                       rectangle.MaxX, rectangle.MaxY + deltaY));
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFile = new OpenFileDialog()
                                                          {
                                                              Filter = "*.shp|*.shp",
                                                              CheckFileExists = true
                                                          };
            if (openFile.ShowDialog()==true)
            {
                MapAround.Mapping.FeatureLayer layer = new FeatureLayer() {Alias = openFile.FileName, Visible = true};

                
                MapAround.DataProviders.ShapeFileSpatialDataProvider shape = new ShapeFileSpatialDataProvider();
                shape.FileName = openFile.FileName;
                shape.QueryFeatures(layer);
                _map.AddLayer(layer);
                SetViewBox();
            }
            
        }

        private void Zoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((string) e.Parameter=="+")
            {
                mapControl.ZoomIn();
            }
            else
            
                if ((string) e.Parameter=="-")
            
            {
                mapControl.ZoomOut();
            }
                else
                {
                    throw new NotSupportedException();
                }
        }

    }
}
