//  MapAround - .NET tools for developing web and desktop mapping applications 

//  Copyright (coffee) 2009-2012 OOO "GKR"
//  This program is free software; you can redistribute it and/or 
//  modify it under the terms of the GNU General Public License 
//   as published by the Free Software Foundation; either version 3 
//  of the License, or (at your option) any later version. 
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program; If not, see <http://www.gnu.org/licenses/>



﻿/*===========================================================================
** 
** File: GDALRasterProvider.cs 
** 
** Copyright (c) Complex Solution Group. 
**
** Description: GDAL raster provider.
**
=============================================================================*/

namespace MapAround.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using OSGeo.GDAL;
    using MapAround.Geometry;
    using System.Drawing;
    using System.Drawing.Imaging;
    using MapAround.CoordinateSystems;


    /// <summary>
    /// Provides access to rasters via GDAL library.
    /// <p>
    /// GDAL library is distributed under the terms of  X11/ MIT License: 
    /// </p>
    /// <code>
    /// Copyright (c) 2000, Frank Warmerdam
    /// Permission is hereby granted, free of charge, to any person obtaining a
    /// copy of this software and associated documentation files (the "Software"),
    /// to deal in the Software without restriction, including without limitation
    /// the rights to use, copy, modify, merge, publish, distribute, sublicense,
    /// and/or sell copies of the Software, and to permit persons to whom the
    /// Software is furnished to do so, subject to the following conditions:
    ///
    /// The above copyright notice and this permission notice shall be included
    /// in all copies or substantial portions of the Software.
    ///
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
    /// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
    /// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    /// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
    /// DEALINGS IN THE SOFTWARE.
    /// </code>
    /// </summary>
    public class GDALRasterProvider : IDisposable, IRasterProvider
    {
        private string _fileName = string.Empty;
        private string _strProjection = string.Empty;
        private bool _disposed = false;
        private Size _imagesize;
        private Dataset _gdalDataset;
        private int _bandCount;
        private int _bitDepth = 8;
        //private bool _showClip = false;

        private bool _displayCIR = false;
        private bool _displayIR = false;

        private Color _transparentColor = Color.Empty;

        /// <summary>
        /// Disposes the GdalRasterProvider.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    if (_gdalDataset != null)
                    {
                        try
                        {
                            _gdalDataset.Dispose();
                        }
                        finally
                        {
                            _gdalDataset = null;
                        }
                    }
                _disposed = true;
            }
        }


        /// <summary>
        /// Gets a name of file containing raster.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }

        /// <summary>
        /// Initializes a new instance of the MapAround.DataProviders.GDALRasterProvider.
        /// </summary>
        /// <param name="fileName">Image file name</param>
        public GDALRasterProvider(string fileName)
        {
            _fileName = fileName;

            Gdal.AllRegister();

            try
            {
                _gdalDataset = Gdal.OpenShared(_fileName, Access.GA_ReadOnly);

                // have gdal read the projection
                _strProjection = _gdalDataset.GetProjectionRef();

                // no projection info found in the image...check for a prj
                if (string.IsNullOrEmpty(_strProjection) &&
                    File.Exists(_fileName.Substring(0, _fileName.LastIndexOf(".")) + ".prj"))
                {
                    _strProjection = File.ReadAllText(_fileName.Substring(0, _fileName.LastIndexOf(".")) + ".prj");
                }

                _imagesize = new Size(_gdalDataset.RasterXSize, _gdalDataset.RasterYSize);
                _bandCount = _gdalDataset.RasterCount;
            }
            catch (Exception ex)
            {
                _gdalDataset = null;
                throw new Exception("Failed loading " + _fileName + "\n" + ex.Message + ex.InnerException);
            }
        }

        //private BoundingRectangle GetExtent()
        //{
        //    if (_gdalDataset != null)
        //    {
        //        double right = 0, left = 0, top = 0, bottom = 0;
        //        double dblW, dblH;

        //        double[] geoTrans = new double[6];

        //        _gdalDataset.GetGeoTransform(geoTrans);

        //        // no rotation...use default transform
        //        //if (!bUseRotation && !bHaveSpot || (geoTrans[0] == 0 && geoTrans[3] == 0))
        //        geoTrans = new double[] { 999.5, 1, 0, 1000.5, 0, -1 };

        //        GeoTransform GT = new GeoTransform(geoTrans);

        //        // image pixels
        //        dblW = _imagesize.Width;
        //        dblH = _imagesize.Height;

        //        left = GT.EnvelopeLeft(dblW, dblH);
        //        right = GT.EnvelopeRight(dblW, dblH);
        //        top = GT.EnvelopeTop(dblW, dblH);
        //        bottom = GT.EnvelopeBottom(dblW, dblH);

        //        return new BoundingRectangle(left, bottom, right, top);
        //    }

        //    return null;
        //}

        private unsafe void writePixel(int x, double[] intVal, int iPixelSize, int[] ch, byte* row)
        {
            // write out pixels
            // black and white
            if (_bandCount == 1 && _bitDepth != 32)
            {
                //if (_showClip)
                //{
                //    if (intVal[0] == 0)
                //    {
                //        row[(int)Math.Round(x) * iPixelSize] = 255;
                //        row[(int)Math.Round(x) * iPixelSize + 1] = 0;
                //        row[(int)Math.Round(x) * iPixelSize + 2] = 0;
                //    }
                //    else if (intVal[0] == 255)
                //    {
                //        row[(int)Math.Round(x) * iPixelSize] = 0;
                //        row[(int)Math.Round(x) * iPixelSize + 1] = 0;
                //        row[(int)Math.Round(x) * iPixelSize + 2] = 255;
                //    }
                //    else
                //    {
                //        row[(int)Math.Round(x) * iPixelSize] = (byte)intVal[0];
                //        row[(int)Math.Round(x) * iPixelSize + 1] = (byte)intVal[0];
                //        row[(int)Math.Round(x) * iPixelSize + 2] = (byte)intVal[0];
                //    }
                //}
                //else
                {
                    row[x * iPixelSize] = (byte)intVal[0];
                    row[x * iPixelSize + 1] = (byte)intVal[0];
                    row[x * iPixelSize + 2] = (byte)intVal[0];
                }
            }
            // IR grayscale
            else if (_displayIR && _bandCount == 4)
            {
                for (int i = 0; i < _bandCount; i++)
                {
                    if (ch[i] == 3)
                    {
                        //if (_showClip)
                        //{
                        //    if (intVal[3] == 0)
                        //    {
                        //        row[(int)Math.Round(x) * iPixelSize] = 255;
                        //        row[(int)Math.Round(x) * iPixelSize + 1] = 0;
                        //        row[(int)Math.Round(x) * iPixelSize + 2] = 0;
                        //    }
                        //    else if (intVal[3] == 255)
                        //    {
                        //        row[(int)Math.Round(x) * iPixelSize] = 0;
                        //        row[(int)Math.Round(x) * iPixelSize + 1] = 0;
                        //        row[(int)Math.Round(x) * iPixelSize + 2] = 255;
                        //    }
                        //    else
                        //    {
                        //        row[(int)Math.Round(x) * iPixelSize] = (byte)intVal[i];
                        //        row[(int)Math.Round(x) * iPixelSize + 1] = (byte)intVal[i];
                        //        row[(int)Math.Round(x) * iPixelSize + 2] = (byte)intVal[i];
                        //    }
                        //}
                        //else
                        {
                            row[x * iPixelSize] = (byte)intVal[i];
                            row[x * iPixelSize + 1] = (byte)intVal[i];
                            row[x * iPixelSize + 2] = (byte)intVal[i];
                        }
                    }
                    else
                        continue;
                }
            }
            // CIR
            else if (_displayCIR && _bandCount == 4)
            {
                //if (_showClip)
                //{
                //    if (intVal[0] == 0 && intVal[1] == 0 && intVal[3] == 0)
                //    {
                //        intVal[3] = intVal[0] = 0;
                //        intVal[1] = 255;
                //    }
                //    else if (intVal[0] == 255 && intVal[1] == 255 && intVal[3] == 255)
                //        intVal[1] = intVal[0] = 0;
                //}

                for (int i = 0; i < _bandCount; i++)
                {
                    if (ch[i] != 0 && ch[i] != -1)
                        row[x * iPixelSize + ch[i] - 1] = (byte)intVal[i];
                }
            }
            // RGB
            else
            {
                //if (_showClip)
                //{
                //    if (intVal[0] == 0 && intVal[1] == 0 && intVal[2] == 0)
                //    {
                //        intVal[0] = intVal[1] = 0;
                //        intVal[2] = 255;
                //    }
                //    else if (intVal[0] == 255 && intVal[1] == 255 && intVal[2] == 255)
                //        intVal[1] = intVal[2] = 0;
                //}

                for (int i = 0; i < _bandCount; i++)
                {
                    if (ch[i] != 3 && ch[i] != -1)
                        row[x * iPixelSize + ch[i]] = (byte)intVal[i];
                }
            }
        }

        #region IRasterProvider Members

        /// <summary>
        /// Retreives a chunk of the raster.
        /// </summary>
        /// <param name="srcX">A minimum X coordinate of the querying area</param>
        /// <param name="srcY">A minimum Y coordinate of the querying area</param>
        /// <param name="srcWidth">A width of the querying area</param>
        /// <param name="srcHeight">A height of the querying area</param>
        /// <param name="maxDestWidth">A maximum width in pixels of the resulting raster</param>
        /// <param name="maxDestHeight">A maximum height in pixels of the resulting raster</param>
        /// <param name="bounds">A bounds of querying area on the map</param>
        /// <param name="receiver">An object receiving raster</param>
        public void QueryRaster(int srcX, int srcY, int srcWidth, int srcHeight, int maxDestWidth, int maxDestHeight, MapAround.Geometry.BoundingRectangle bounds, MapAround.Mapping.IRasterReceiver receiver)
        {
            if (srcX < 0)
                throw new ArgumentException("Minimum X of the querying area should not be negative", "srcX");

            if (srcY < 0)
                throw new ArgumentException("Minimum Y of the querying area should not be negative", "srcY");

            if (srcWidth <= 0)
                throw new ArgumentException("A width of the querying area should be positive", "srcWidth");

            if (srcHeight <= 0)
                throw new ArgumentException("A height of the querying area should be positive", "srcHeight");

            Bitmap bitmap = null;
            BitmapData bitmapData = null;
            double[] intVal = new double[_bandCount];
            int pIndex;
            double bitScalar = 1.0;

            // scale
            if (_bitDepth == 12)
                bitScalar = 16.0;
            else if (_bitDepth == 16)
                bitScalar = 256.0;
            else if (_bitDepth == 32)
                bitScalar = 16777216.0;

            int iPixelSize = 3;

            try
            {
                bitmap = new Bitmap(Math.Max(maxDestWidth, 1), Math.Max(maxDestHeight, 1), PixelFormat.Format24bppRgb);
                bitmapData =
                    bitmap.LockBits(
                        new Rectangle(0, 0, Math.Max(maxDestWidth, 1), Math.Max(maxDestHeight, 1)), ImageLockMode.ReadWrite, bitmap.PixelFormat);

                unsafe
                {
                    double[][] buffer = new double[_bandCount][];
                    Band[] band = new Band[_bandCount];
                    int[] ch = new int[_bandCount];

                    // get data from image
                    for (int i = 0; i < _bandCount; i++)
                    {
                        buffer[i] = new double[maxDestWidth * maxDestHeight];
                        band[i] = _gdalDataset.GetRasterBand(i + 1);

                        band[i].ReadRaster(
                            srcX,          // xOff
                            srcY,          // yOff
                            srcWidth,      // xSize
                            srcHeight,     // ySize
                            buffer[i],     // buffer 
                            maxDestWidth,  // buf_xSize 
                            maxDestHeight, // buf_ySize
                            0,             // pixelSpace
                            0);            // lineSpace

                        if (band[i].GetRasterColorInterpretation() == ColorInterp.GCI_BlueBand) ch[i] = 0;
                        else if (band[i].GetRasterColorInterpretation() == ColorInterp.GCI_GreenBand) ch[i] = 1;
                        else if (band[i].GetRasterColorInterpretation() == ColorInterp.GCI_RedBand) ch[i] = 2;
                        else if (band[i].GetRasterColorInterpretation() == ColorInterp.GCI_Undefined)
                            ch[i] = 3; // infrared
                        else if (band[i].GetRasterColorInterpretation() == ColorInterp.GCI_GrayIndex) ch[i] = 4;
                        else ch[i] = -1;
                    }

                    if (_bitDepth == 32)
                        ch = new int[] { 0, 1, 2 };

                    pIndex = 0;
                    for (int y = 0; y < maxDestHeight; y++)
                    {
                        byte* row = (byte*)bitmapData.Scan0 + (y * bitmapData.Stride);
                        for (int x = 0; x < maxDestWidth; x++, pIndex++)
                        {
                            for (int i = 0; i < _bandCount; i++)
                            {
                                intVal[i] = buffer[i][pIndex] / bitScalar;

                                if (intVal[i] > 255)
                                    intVal[i] = 255;
                            }

                            writePixel(x, intVal, iPixelSize, ch, row);
                        }
                    }
                }
            }
            finally
            {
                if (bitmapData != null)
                    bitmap.UnlockBits(bitmapData);
            }

            receiver.AddRasterPreview(bitmap, bounds, Width, Height);
        }

        /// <summary>
        /// Gets a width of the raster in pixels.
        /// </summary>
        public int Width
        {
            get
            {
                return _imagesize.Width;
            }
        }

        /// <summary>
        /// Gets a height of the raster in pixels.
        /// </summary>
        public int Height
        {
            get
            {
                return _imagesize.Height;
            }
        }

        #endregion
    }

    /// <summary>
    /// GDAL raster provider holder.
    /// </summary>
    public class GDALRasterProviderHolder : RasterProviderHolderBase
    {
        private static string[] _parameterNames = { "file_name" };
        private Dictionary<string, string> _parameters = null;

        /// <summary>
        /// Sets the parameter values.
        /// </summary>
        /// <param name="parameters">Parameter values</param>
        public override void SetParameters(Dictionary<string, string> parameters)
        {
            string missingField = "Parameter \"{0}\" missimg";
            foreach (string s in _parameterNames)
                if (!parameters.ContainsKey("file_name"))
                    throw new ArgumentException(string.Format(missingField, s));

            _parameters = parameters;
        }

        /// <summary>
        /// Gets a list containing the names of parameters.
        /// </summary>
        /// <returns>List containing the names of parameters</returns>
        public override string[] GetParameterNames()
        {
            return _parameterNames;
        }

        private IRasterProvider createProviderInstance()
        {
            if (_parameters == null)
                throw new InvalidOperationException("Parameter values not set");

            GDALRasterProvider provider = null; 

            if (File.Exists(_parameters["file_name"]))
                provider = new GDALRasterProvider(_parameters["file_name"]);
            else
                throw new FileNotFoundException(_parameters["file_name"]);

            return provider;
        }

        /// <summary>
        /// Performs a finalization procedure for the raster provider.
        /// This implementation call Dispose method of the provider.
        /// </summary>
        /// <param name="provider">Raster provider instance</param>
        public override void ReleaseProviderIfNeeded(IRasterProvider provider)
        {
            GDALRasterProvider p = provider as GDALRasterProvider;
            if (p != null)
                p.Dispose();
        }

        /// <summary>
        /// Initializes a new instance of the MapAround.DataProviders.GDALRasterProvider.
        /// </summary>
        public GDALRasterProviderHolder()
            : base("MapAround.DataProviders.GDALRasterProvider")
        {
            GetProviderMethod = createProviderInstance;
        }
    }
}
