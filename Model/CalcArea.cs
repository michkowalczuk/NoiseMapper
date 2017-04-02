using esriGeometry = ESRI.ArcGIS.Client.Geometry;

namespace NoiseMapper.Model
{
    class CalcArea : ModelObject
    {
        public CalcArea(esriGeometry.Polygon polygon)
            : base(polygon) { }


        #region PROPERTIES

        public esriGeometry.Polygon Polygon
        {
            get
            {
                return base.Geometry as esriGeometry.Polygon;
            }
        }

        #endregion
    }
}
