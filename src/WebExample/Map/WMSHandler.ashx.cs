using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MapAround.Caching;
using MapAround.DataProviders;
using MapAround.Geometry;
using MapAround.Mapping;
using MapAround.Web;
using MapAround.Web.Wms;

namespace WebExample.Map
{
    /// <summary>
    /// Summary description for WMSHandler
    /// </summary>
    public class WMSHandler : IHttpHandler
    {

        public const string SessionWorkspaceKeyPrefix = "maphandling_mapworkspace_";
        public const string TileCacheKeyPrefix = "DemoTileCache";
        private  string MapFolder = "";
        
        private MapWorkspace mapWs = null;
      

        private HttpContext httpContext;

        public void ProcessRequest(HttpContext context)
        {
            httpContext = context;
            var WorkspaceFilePath = System.IO.Path.Combine(httpContext.Server.MapPath("~"), "workspace.xml");
            MapFolder = System.IO.Path.Combine(httpContext.Server.MapPath("~"), "..", "..", "data");
            string mapId = "";
            


            
                mapWs = new MapWorkspace();
                mapWs.XmlRepresentation = File.ReadAllText(WorkspaceFilePath);
                mapWs.Map.RenderingSettings.AntiAliasGeometry = true;
                mapWs.Map.RenderingSettings.AntiAliasText = true;
          

            //    mapWs.Map.CoodrinateSystemWKT = "PROJCS[\"CS_WGS_1984_Major_Auxiliary_Sphere\",GEOGCS[\"WGS_1984_Major_Auxiliary_Sphere\",DATUM[\"WGS_1984_Major_Auxiliary_Sphere\",SPHEROID[\"WGS84\", 6378137, 0]],PRIMEM[\"Greenwich\", 0],UNIT[\"Degree\", 0.0174532925199433]],PROJECTION[\"Mercator\"],PARAMETER[\"latitude_of_origin\", 0],PARAMETER[\"central_meridian\", 0],PARAMETER[\"false_easting\", 0],PARAMETER[\"false_northing\", 0],UNIT[\"metre\", 1, AUTHORITY[\"EPSG\", \"9001\"]],AXIS[\"East\", EAST], AXIS[\"North\", NORTH],AUTHORITY[\"EPSG\", \"102100\"]];";

            foreach (LayerBase l in mapWs.Map.Layers)
            {
               
                    FeatureLayer fl = l as FeatureLayer;
                    if (fl != null)
                    {
                        fl.DataSourceNeeded += LayerDataSourceNeeded;
                        fl.DataSourceReadyToRelease += LayerDataSourceReadyToRelease;                       

                        if (!fl.AreFeaturesAutoLoadable)
                            fl.LoadFeatures();
                    }

                   
               
            }

            mapWs.Map.FeatureRenderer.SelectionColor = ColorTranslator.FromHtml("#0000FF");
            BoundingRectangle bbox = new BoundingRectangle();
            if (context.Request.Params.AllKeys.Contains("BBOX",StringComparer.InvariantCultureIgnoreCase))
            {
                bbox = QueryStringDataExtractor.GetBBox(httpContext.Request.QueryString["BBOX"]);
            }
            Size displaySize;
            double mapScale = 0;
            if (context.Request.Params.AllKeys.Contains("WIDTH", StringComparer.InvariantCultureIgnoreCase) && context.Request.Params.AllKeys.Contains("HEIGHT", StringComparer.InvariantCultureIgnoreCase))
            {
                displaySize = QueryStringDataExtractor.GetDisplaySize(httpContext.Request.QueryString["WIDTH"], httpContext.Request.QueryString["HEIGHT"]);
                mapScale = displaySize.Width / bbox.Width;
            }



            if (context.Request.Params.AllKeys.Contains("LAYERS", StringComparer.InvariantCultureIgnoreCase))
            {
                string[] layersAliases = context.Request.Params["LAYERS"].Split(',');
                foreach (LayerBase l in mapWs.Map.Layers)
                    l.Visible = layersAliases.Contains(l.Alias);
            }

            

            SetSpecialScaleStyles(mapWs, mapScale);

            IMapServer server = new WMSServer(new WmsServiceDescription("MapAround Demo", ""));
            server.ImageQuality = 95;        
            server.Map = mapWs.Map;
            server.GutterSize = 180;
            server.BeforeRenderNewImage += server_BeforeRenderNewImage;


           
               
                FileTileCacheAccessor tileCacheAccessor =
                    new FileTileCacheAccessor(Path.Combine(HttpContext.Current.Server.MapPath("~"), "map\\cache"));
                tileCacheAccessor.Prefix = TileCacheKeyPrefix;
                //  tileCacheAccessor.IsHierarchic = true;
                server.TileCacheAccessor = tileCacheAccessor;


            string mime = string.Empty;
            context.Response.Clear();

            server.GetResponse(context.Request.Params, context.Response.OutputStream, out mime);


            context.Response.ContentType = mime;
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(new TimeSpan(24, 0, 0));

           
         
            server.BeforeRenderNewImage -= server_BeforeRenderNewImage;

            context.Response.End();
        }

        private void SetSpecialScaleStyles(MapWorkspace mapWs, double scale)
        {
        }

       

        private void LayerDataSourceNeeded(object sender, MapAround.Mapping.FeatureDataSourceEventArgs e)
        {
            FeatureLayer l = sender as FeatureLayer;
            string featuresFilePath = string.Empty;
            switch (l.DataProviderRegName)
            {
              

                case "MapAround.DataProviders.ShapeFileSpatialDataProvider":
                    l.AreFeaturesAutoLoadable = true;
                    ShapeFileSpatialDataProvider shapeP = new ShapeFileSpatialDataProvider();
                    shapeP.AttributesEncoding = Encoding.UTF8;
                    shapeP.FileName = GetFeaturesFilePath(l.DataProviderParameters["file_name"]);
                    shapeP.ProcessAttributes = true;                    
                    e.Provider = shapeP;
                    break;
                default:
                    throw new Exception("Map data provider not found: \"" + l.DataProviderRegName + "\"");
            }
        }

        private string GetFeaturesFilePath(string fileNameParameter)
        {
            string featuresFilePath = string.Empty;

            if (string.IsNullOrEmpty(fileNameParameter))
                throw new ArgumentException("Empty or null", "fileNameParameter");

            if (!fileNameParameter.Contains("\\"))
                featuresFilePath = Path.Combine(MapFolder, Path.GetFileName(fileNameParameter));
            else
                featuresFilePath = fileNameParameter;

            if (!File.Exists(featuresFilePath))
                throw new Exception("File not found: " + featuresFilePath);

            return featuresFilePath;
        }


        private void LayerDataSourceReadyToRelease(object sender, MapAround.Mapping.FeatureDataSourceEventArgs e)
        {
            FeatureLayer l = sender as FeatureLayer;

            switch (l.DataProviderRegName)
            {
               

                case "MapAround.DataProviders.ShapeFileSpatialDataProvider":
                    break;

               

                default:
                    throw new Exception("Map data provider not found: \"" + l.DataProviderRegName + "\"");
            }
        }

       

        private void server_BeforeRenderNewImage(object sender, RenderNewImageEventArgs e)
        {
            BoundingRectangle bbox = QueryStringDataExtractor.GetBBox(httpContext.Request.QueryString["BBOX"]);
            Size displaySize = QueryStringDataExtractor.GetDisplaySize(httpContext.Request.QueryString["WIDTH"], httpContext.Request.QueryString["HEIGHT"]);
            double mapScale = displaySize.Width / bbox.Width;

            mapWs.Map.LoadFeatures(mapScale, mapWs.Map.MapViewBoxFromPresentationViewBox(e.BboxWithGutters));
            mapWs.Map.LoadRasters(mapScale, mapWs.Map.MapViewBoxFromPresentationViewBox(e.BboxWithGutters));
        }

       

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class QueryStringDataExtractor
    {
        public static BoundingRectangle GetBBox(string bboxStr)
        {
            if (string.IsNullOrEmpty(bboxStr))
                throw new Exception("Empty bbox");

            double x1 = 0;
            double y1 = 0;
            double x2 = 0;
            double y2 = 0;
            string[] points = bboxStr.Split(',');
            if (
                    points.Length < 4
                    ||
                    !double.TryParse(points[0], NumberStyles.Any, CultureInfo.InvariantCulture, out x1)
                    ||
                    !double.TryParse(points[1], NumberStyles.Any, CultureInfo.InvariantCulture, out y1)
                    ||
                    !double.TryParse(points[2], NumberStyles.Any, CultureInfo.InvariantCulture, out x2)
                    ||
                    !double.TryParse(points[3], NumberStyles.Any, CultureInfo.InvariantCulture, out y2)
                )
                throw new Exception("bbox = \"" + bboxStr + "\"");

            return new BoundingRectangle(Math.Min(x1, x2), Math.Min(y1, y2), Math.Max(x1, x2), Math.Max(y1, y2));
        }

        public static Size GetDisplaySize(string wStr, string hStr)
        {
            if (string.IsNullOrEmpty(wStr))
                throw new Exception("Empty display map width");

            if (string.IsNullOrEmpty(hStr))
                throw new Exception("Empty display map height");

            int w = 0;
            int h = 0;

            if (
                    !int.TryParse(wStr, out w)
                    ||
                    !int.TryParse(hStr, out h)
                )
                throw new NotImplementedException();

            return new Size(w, h);
        }
    }
}