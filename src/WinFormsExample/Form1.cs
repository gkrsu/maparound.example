using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapAround.DataProviders;
using MapAround.Geometry;
using MapAround.Mapping;

namespace WinFormsExample
{
    public partial class Form1 : Form
    {
        private Map _map;

        public Form1()
        {
            _map = new MapAround.Mapping.Map();
            InitializeComponent();
            mapControl.Map = _map;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {

            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "*.shp|*.shp";
                dialog.CheckFileExists = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var layer = new FeatureLayer() { Alias = dialog.FileName, Visible = true };
                    var shape = new ShapeFileSpatialDataProvider {FileName = dialog.FileName};
                    shape.QueryFeatures(layer);
                    _map.AddLayer(layer);
                    SetViewBox();

                }
            }
        }


        private void SetViewBox()
        {
            BoundingRectangle rectangle = _map.CalculateBoundingRectangle();
            if (rectangle.IsEmpty()) return;

            double deltaY = rectangle.Width * mapControl.Height / 2 / mapControl.Width - rectangle.Height / 2;
            

          
                                           
            mapControl.SetViewBox( new BoundingRectangle(rectangle.MinX, rectangle.MinY - deltaY,
                                                       rectangle.MaxX, rectangle.MaxY + deltaY));
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            if (_map.Layers.Count == 0) return;
            
             mapControl.ZoomIn();

        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (_map.Layers.Count == 0) return;

            mapControl.ZoomOut();
        }

        private void btnViewAll_Click(object sender, EventArgs e)
        {
            if (_map.Layers.Count == 0) return;

            SetViewBox();

        }

    }
}
