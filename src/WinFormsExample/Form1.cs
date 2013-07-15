using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapAround.CoordinateSystems.Transformations;
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

        

        private void btnOpenFeature_Click(object sender, EventArgs e)
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

        private void btnOpenRaster_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "*.*|*.*";
                dialog.CheckFileExists = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var layer = new RasterLayer() { Alias = dialog.FileName, Visible = true };
                    layer.DataSourceNeeded += new EventHandler<RasterDataSourceEventArgs>(layer_DataSourceNeeded);
                    layer.DataProviderParameters.Add("file_name", dialog.FileName);
                    layer.DataSourceReadyToRelease += new EventHandler<RasterDataSourceEventArgs>(layer_DataSourceReadyToRelease);
                    var width = 0;
                    var height = 0;
                    using (GDALRasterProvider gdal = new GDALRasterProvider(dialog.FileName))
                    {
                        width = gdal.Width;
                        height = gdal.Height;
                    }
                    layer.Binding = new RasterLayer.RasterBinding(0, 0, new Coordinate() { X = 0, Y = height }, 1, 1);
                    layer.LoadRasterPreview(new BoundingRectangle(0, 0, width, height), width/mapControl.ClientSize.Width);
                  
                    _map.AddLayer(layer);
                    SetViewBox();

                }
            }
        }

        void layer_DataSourceReadyToRelease(object sender, RasterDataSourceEventArgs e)
        {
           ((IDisposable) e.Provider).Dispose();
        }

        void layer_DataSourceNeeded(object sender, RasterDataSourceEventArgs e)
        {
            RasterLayer raster = (RasterLayer)(sender);
            e.Provider = new GDALRasterProvider(raster.DataProviderParameters["file_name"]);
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
  

        private void btnFlyTransform_Click(object sender, EventArgs e)
        {
            if (btnFlyTransform.Checked)
            {
                // create an affine transformation 
                Affine transformation = Affine.Rotation(Math.PI/6);

                // assign it to the map on-the-fly transform and redraw the map 
                _map.OnTheFlyTransform = transformation;
                mapControl.RedrawMap();
            }
            else
            {
                _map.OnTheFlyTransform = null;
            }

           SetViewBox();
        }

        private void btnThematic_Click(object sender, EventArgs e)
        {
           
                foreach (var Layer in _map.Layers)
                {
                    FeatureLayer featureLayer = (Layer as FeatureLayer);
                    if (!ReferenceEquals(featureLayer, null))
                    {
                        if (btnThematic.Checked)
                            featureLayer.BeforePolygonRender += featureLayer_BeforePolygonRender;
                        else
                        {
                            featureLayer.BeforePolygonRender -= featureLayer_BeforePolygonRender;
                            polygonFeatureClearStyle(featureLayer);
                        }
                    }
                }
            mapControl.RedrawMap();
            
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (btnSelect.Checked)
                mapControl.DragMode = MapAround.UI.WinForms.MapControl.DraggingMode.Zoom;
            else
                mapControl.DragMode = MapAround.UI.WinForms.MapControl.DraggingMode.Pan;

        }

        private void SetViewBox()
        {
            BoundingRectangle rectangle = _map.CalculateBoundingRectangle();
            if (rectangle.IsEmpty()) return;

            if (!ReferenceEquals(_map.OnTheFlyTransform, null))
            {
                IMathTransform transform = _map.OnTheFlyTransform;
                rectangle = GeometryTransformer.TransformBoundingRectangle(rectangle, transform);
            }

            double deltaY = rectangle.Width * mapControl.Height / 2 / mapControl.Width - rectangle.Height / 2;

            rectangle = new BoundingRectangle(rectangle.MinX, rectangle.MinY - deltaY,
                                              rectangle.MaxX, rectangle.MaxY + deltaY);


            mapControl.SetViewBox(rectangle);
        }

        void polygonFeatureClearStyle(FeatureLayer featureLayer)
        {
            foreach (var feature in featureLayer.Features)
            {
                feature.PolygonStyle = null;
            }
        }

        void featureLayer_BeforePolygonRender(object sender, FeatureRenderEventArgs e)
        {
            // clone layer settings for polygon rendering style 
            PolygonStyle style = (PolygonStyle)e.Feature.Layer.PolygonStyle.Clone();
            style.FillBackColor = Color.Gray;
            // change this settings 
            style.UseHatch = true; 
            int f = e.Feature.Geometry.CoordinateCount % 3; 
            switch (f) 
            { 
                case 0: style.HatchStyle = System.Drawing.Drawing2D.HatchStyle.Horizontal; 
                    break; 
                case 1: style.HatchStyle = System.Drawing.Drawing2D.HatchStyle.Vertical; 
                    break; 
                case 2: style.HatchStyle = System.Drawing.Drawing2D.HatchStyle.Cross; 
                    break; 
            } 

            // style changes may be based on the feature attribute values 
            // for example: 
            // style.FillForeColor = Color.FromName(e.Feature["ColorName"].ToString()); 

            // assign changed style to the feature 
            e.Feature.PolygonStyle = style; 
        }    

        void mapControl_SelectionRectangleDefined(object sender, MapAround.UI.WinForms.ViewBoxEventArgs e)
        {
            // create a list instance for selected features 
            List<Feature> selectedFeatures = new List<Feature>();

            // translate view box to map data coordinates 
            BoundingRectangle br = mapControl.Map.MapViewBoxFromPresentationViewBox(e.ViewBox);
            FeatureLayer l = (FeatureLayer)mapControl.Map.Layers[0];

            // and do primary filter 
            l.SelectObjects(br, selectedFeatures);

            // turn off selection 
            foreach (Feature f in l.Features)
            {
                f.Selected = false;
            }

            Polygon p = null;

            // if the map has on-the-fly transformation, convert initial view box  
            // to polygon and transform its coordinates to map data coordinates 
            if (mapControl.Map.OnTheFlyTransform != null)
            {
                p = e.ViewBox.ToPolygon();
                IMathTransform inverseTransform = mapControl.Map.OnTheFlyTransform.Inverse();
                p = GeometryTransformer.TransformPolygon(p, inverseTransform);
            }

            foreach (Feature f in selectedFeatures)
            {
                if (mapControl.Map.OnTheFlyTransform != null)
                {
                    // do more expensive secondary filter 
                    if (p.Intersects((ISpatialRelative)f.Geometry))
                    {
                        f.Selected = true;
                    }
                }
                else
                {
                    // if our map hasn't on-the-fly transformation, primary filter is fine, 
                    // so, set selection state 
                    f.Selected = true;
                }
            }

         
            mapControl.RedrawMap(); 
        }

        private void mapControl_BeforeMapRender(object sender, MapAround.UI.WinForms.ViewBoxEventArgs e)
        {
            _map.LoadRasters(mapControl.ClientSize.Height / e.ViewBox.Height, e.ViewBox); //Loading raster layer for this viewbox
        }

       

        
    }
}
