using esriGeometry = ESRI.ArcGIS.Client.Geometry;

namespace NoiseMapper.Model
{
    class PointSource : PointModelObject
    {
        #region CONSTRUCTOR

        public PointSource(esriGeometry.MapPoint mapPoint)
            : base(mapPoint) { }

        #endregion
                

        #region PROPERTIES

        public double LWA { get; set; }

        #endregion
    }
}
