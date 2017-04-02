using System;
using System.Collections.Generic;

using esriGeometry = ESRI.ArcGIS.Client.Geometry;
using System.Threading.Tasks;

namespace NoiseMapper.Model
{
    public delegate void ProgressIncrementedDelegate(object sender, int progress);
    public delegate void ModelPreparedDelegate(object sender);
    public delegate void CalcFinishedDelegate(object sender, string t);

    //public delegate void CalcPointEvaluatedDelegate(object sender, CalcPoint calcPoint);
    //public delegate void CalcFinishedDelegate2(object sender, List<Graphic> graphicCollection);
    //public delegate void TimeDelegate(object sender, string t); // tests

    class NoiseModel
    {
        //public NoiseModel()
        //{
        //    CalcAreaCollection = new List<CalcArea>();
        //    CalcPointCollection = new List<CalcPoint>();
        //    SourceCollection = new List<PointSource>();
        //    graphicCollection = new List<Graphic>();
        //}

        public NoiseModel(NoiseModelSettings noiseModelSettings)
        {
            //CalcAreaCollection = new List<CalcArea>();
            //CalcPointCollection = new List<CalcPoint>();
            //SourceCollection = new List<PointSource>();
            //graphicCollection = new List<Graphic>();
            this.noiseModelSettings = noiseModelSettings;
            //this.spatialReference = spatialReference;
        }


        # region DELEGATES
        //public LDWChangeDelegate LDWChange { get; set; }
        //public CalcPointEvaluatedDelegate CalcPointEvaluated { get; set; }
        public ProgressIncrementedDelegate ProgressIncremented { get; set; }
        public CalcFinishedDelegate CalcFinished { get; set; }
        //public CalcFinishedDelegate2 CalcFinished2 { get; set; }

        public ModelPreparedDelegate ModelPrepared { get; set; }
        //public TimeDelegate CalcTime { get; set; } // tests

        #endregion


        #region FIELDS

        //private esriGeometry.SpatialReference spatialReference;
        private NoiseModelSettings noiseModelSettings;
        ////private List<Graphic> graphicCollection;

        //private List<PointSource> SourceCollection = new List<PointSource>();
        //private List<CalcArea> CalcAreaCollection = new List<CalcArea>();
        //private List<CalcPoint> CalcPointCollection = new List<CalcPoint>();

        #endregion

        #region PROPERTIES

        public List<PointSource> SourceCollection { get; set; }
        public List<CalcArea> CalcAreaCollection { get; set; }
        public List<CalcPoint> CalcPointCollection { get; set; }
        public List<esriGeometry.Polygon> ResultsGeometry { get; set; }

        ////public List<Graphic> Results { get { return graphicCollection; } }
        
        #endregion


        #region METHODS

        //internal void AddSource(PointSource source)
        //{
        //    if (source is PointSource)
        //        sourceCollection.Add(source);
        //}

        //internal void AddCalcArea(CalcArea calcArea)
        //{
        //    calcAreaCollection.Add(calcArea);
        //}

        internal void PrepareCalc()
        {
            CalcPointCollection = new List<CalcPoint>();
            // 
            //ObservableCollection<esriGeometry.PointCollection> calcAreaRings = new ObservableCollection<esriGeometry.PointCollection>();

            //List<Graphic> calcPointGraphicCollectionInEnvelope = new List<Graphic>();
            //List<esriGeometry.MapPoint> calcPointCollectionInEnvelope = new List<esriGeometry.MapPoint>();

            //Graphic calcPointGraphic;
            esriGeometry.MapPoint calcPointGeometry;
            CalcPoint calcPoint;
            // generating calculation points in CalcArea Envelope
            foreach (var calcArea in CalcAreaCollection)
            {
                // generating calculation area multi-polygon
                //foreach (var ring in calcArea.Polygon.Rings)
                //    calcAreaRings.Add(ring);

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
                        //calcPointGraphic = new Graphic()
                        //{
                        //    Geometry = new esriGeometry.MapPoint(x, y, noiseModelSettings.GridHeight, 0, calcArea.Geometry.SpatialReference)
                        //};
                        //calcPointCollectionInEnvelope.Add(calcPointTemp);
                        calcPoint = new CalcPoint(calcPointGeometry);
                        CalcPointCollection.Add(calcPoint);
                    }
                }
            }

            //esriGeometry.Polygon calcAreaMultiPolygon = new esriGeometry.Polygon();
            //calcAreaMultiPolygon.SpatialReference = this.noiseModelSettings.SpatialReference;

            //calcAreaMultiPolygon.Rings = calcAreaRings;

            // intersection using GeometryService
            //GeometryService geometryService = new GeometryService(
            //    "http://tasks.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer");
            //"http://tasks.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer");
            //"http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer"
            
            // TO DO: dołożyć try i inne rozwiązanie
            //var results = geometryService.Intersect(calcPointGraphicCollectionInEnvelope, CalcAreaCollection[0].Geometry);

            //var results = geometryService.Intersect(calcPointGraphicCollectionInEnvelope, calcAreaMultiPolygon);

            //var results = calcPointCollectionInEnvelope;


            //CalcPoint.LDWChange += OnLDWChange;

            // generating calculation points inside CalcArea
            //foreach (var calcPointGraphicResult in results)
            //{
            //    CalcPoint calcPoint = new CalcPoint(calcPointGraphicResult.Geometry as esriGeometry.MapPoint);

            //    CalcPointCollection.Add(calcPoint);
            //}

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

            //int start = System.Environment.TickCount; //tests
            //List<int> timeList = new List<int>(CalcPointCollection.Count); //tests

            int progressInt = 0;

            //Parallel.ForEach(CalcPointCollection, calcPoint =>
            //foreach (var calcPoint in CalcPointCollection)

            //List<int> indicesToRemove = new List<int>();

            Parallel.For(0, CalcPointCollection.Count, iCalcPoint =>
            //for (int iCalcPoint = 0; iCalcPoint < CalcPointCollection.Count; iCalcPoint++)

            {
                var calcPoint = CalcPointCollection[iCalcPoint];

                double hr, hs, hm;
                double d, dp;
                double Adiv, Aatm, Agr, A;
                double DOmega;

                List<double> ldwCollection = new List<double>(SourceCollection.Count);
                hr = calcPoint.Z;

                for (int iPointSource = 0; iPointSource < SourceCollection.Count; iPointSource++)
                //foreach (var pointSource in SourceCollection)
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

            //indicesToRemove.Sort();
            //indicesToRemove.Reverse();
            //foreach (var index in indicesToRemove)
            //{
            //    CalcPointCollection.RemoveAt(index);
            //}

            //tests
            //int stop = System.Environment.TickCount;

            //string t = (stop - start).ToString() + "ms\n";
            //if (CalcFinished != null)
            //{
            //    CalcFinished(this, t);
            //}

        #endregion
        }
    }
}
