using System;
using System.Collections.Generic;

using esriGeometry = ESRI.ArcGIS.Client.Geometry;
using System.Threading.Tasks;

namespace NoiseMapper.Model
{
    public delegate void ProgressIncrementedDelegate(object sender, int progress);
    public delegate void ModelPreparedDelegate(object sender);
    public delegate void CalcFinishedDelegate(object sender, string t);

    /// <summary>
    /// Class with implemented main noise model according to norm ISO 9613-2 
    ///</summary>
    class NoiseModel
    {
        public NoiseModel(NoiseModelSettings noiseModelSettings)
        {
            this.noiseModelSettings = noiseModelSettings;
        }

        # region DELEGATES

        public ProgressIncrementedDelegate ProgressIncremented { get; set; }
        public CalcFinishedDelegate CalcFinished { get; set; }
        public ModelPreparedDelegate ModelPrepared { get; set; }

        #endregion


        #region FIELDS

        private NoiseModelSettings noiseModelSettings;

        #endregion

        #region PROPERTIES

        public List<PointSource> SourceCollection { get; set; }
        public List<CalcArea> CalcAreaCollection { get; set; }
        public List<CalcPoint> CalcPointCollection { get; set; }
        public List<esriGeometry.Polygon> ResultsGeometry { get; set; }
        
        #endregion

        #region METHODS

        internal void PrepareCalc()
        {
            CalcPointCollection = new List<CalcPoint>();
            esriGeometry.MapPoint calcPointGeometry;
            CalcPoint calcPoint;

            // generating calculation points in CalcArea Envelope
            foreach (var calcArea in CalcAreaCollection)
            {
                // generating calculation area multi-polygon
                esriGeometry.Envelope calcAreaExtent = calcArea.Geometry.Extent;

                double xStart = calcAreaExtent.XMin + noiseModelSettings.GridSize / 2;
                double xEnd = calcAreaExtent.XMax;

                double yStart = calcAreaExtent.YMin + noiseModelSettings.GridSize / 2;
                double yEnd = calcAreaExtent.YMax;

                for (double x = xStart; x < xEnd; x += noiseModelSettings.GridSize)
                {
                    for (double y = yStart; y < yEnd; y += noiseModelSettings.GridSize)
                    {
                        calcPointGeometry = new esriGeometry.MapPoint(x, y, noiseModelSettings.GridHeight, 0, calcArea.Geometry.SpatialReference);
                        calcPoint = new CalcPoint(calcPointGeometry);
                        CalcPointCollection.Add(calcPoint);
                    }
                }
            }

            if (ModelPrepared != null)
            {
                ModelPrepared(this);
            }
        }

        internal void RunCalc()
        {
            this.PrepareCalc();

            // noise calculations
            const double alfa = 1.9;

            int progressInt = 0;

            Parallel.For(0, CalcPointCollection.Count, iCalcPoint =>
            {
                var calcPoint = CalcPointCollection[iCalcPoint];

                double hr, hs, hm;
                double d, dp;
                double Adiv, Aatm, Agr, A;
                double DOmega;

                List<double> ldwCollection = new List<double>(SourceCollection.Count);
                hr = calcPoint.Z;

                for (int iPointSource = 0; iPointSource < SourceCollection.Count; iPointSource++)
                {
                    var pointSource = SourceCollection[iPointSource];
                    hs = pointSource.Z;

                    d = Tools.Geometry.Distance3D(calcPoint.Point, pointSource.Point);
                    dp = Tools.Geometry.Distance2D(calcPoint.Point, pointSource.Point);

                    // Geometrical divergence (Adiv)
                    Adiv = 20 * Math.Log10(d) + 11;

                    // Atmospheric absorption (Aatm)
                    Aatm = d * alfa / 1000;

                    // Ground effect - alternative method (Agr)
                    // mean height of the propagation path above the ground
                    hm = (hs + hr) / 2;

                    // equation (10)
                    Agr = 4.8 - 2 * hm / d * (17 + 300 / d);
                    if (Agr < 0)
                        Agr = 0;

                    // Directivity Omega
                    DOmega = 10 * Math.Log10(1 + Math.Pow(d, 2) / (Math.Pow(dp, 2) + Math.Pow(hs + hr, 2)));

                    // equation (4)
                    A = Adiv + Aatm + Agr;

                    ldwCollection.Add(pointSource.LWA + DOmega - A);
                }

                calcPoint.LDW = Tools.Acoustics.Log10Sum(ldwCollection);

                if (calcPoint.LDW >= noiseModelSettings.MinimumValue)
                {
                    esriGeometry.Polygon resultPolygon = Tools.Geometry.PolygonFromPoint(new esriGeometry.MapPoint(calcPoint.X, calcPoint.Y), noiseModelSettings.GridSize);
                    calcPoint.Tile = resultPolygon;
                }

                int progress = (int)Math.Abs(iCalcPoint * 100.0 / (CalcPointCollection.Count - 1));
                if (progress > progressInt)
                {
                    progressInt++;
                    if (ProgressIncremented != null)
                    {
                        ProgressIncremented(this, progress);
                    }

                }
            }
            );

        #endregion
        }
    }
}
