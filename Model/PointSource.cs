using esriGeometry = ESRI.ArcGIS.Client.Geometry;

namespace NoiseMapper.Model
{
    /// <summary>
    /// Class for storing point sources
    ///</summary>
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
