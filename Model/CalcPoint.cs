using esriGeometry = ESRI.ArcGIS.Client.Geometry;

namespace NoiseMapper.Model
{
    /// <summary>
    /// Class for calculation points
    ///</summary>
    public class CalcPoint : PointModelObject
    {
        # region CONSTRUCTOR

        public CalcPoint(esriGeometry.MapPoint mapPoint)
            : base(mapPoint) { }

        #endregion

        #region PEOPERTIES

        public double LDW { get; set; }

        public esriGeometry.Polygon Tile { get; set; }

        #endregion
        

        #region METHODS

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}", Point.X, Point.Y, Point.Z, LDW);
        }

        #endregion
    }
}
