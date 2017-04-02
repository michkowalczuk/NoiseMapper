using esriGeometry = ESRI.ArcGIS.Client.Geometry;

namespace NoiseMapper.Model
{
    // delegate type
    //public delegate void LDWChangeDelegate(object sender, double ldw);

    public class CalcPoint : PointModelObject
    {
        # region CONSTRUCTOR

        public CalcPoint(esriGeometry.MapPoint mapPoint)
            : base(mapPoint) { }

        #endregion


        #region FIELDS

        //private double ldw;

        #endregion

        // delegates
        //public static LDWChangeDelegate LDWChange { get; set; }

        #region PEOPERTIES

        public double LDW { get; set; }
        //{
        //    get { return ldw; }
        //    set 
        //    {
        //        ldw = value;
        //        if (LDWChange != null)
        //        {
        //            LDWChange(this, ldw);
        //        }
        //    }
        //}

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
