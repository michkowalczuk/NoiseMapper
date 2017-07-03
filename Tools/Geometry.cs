using System;
using esriGeometry = ESRI.ArcGIS.Client.Geometry;

namespace NoiseMapper.Tools
{
    /// <summary>
    /// Class with static methods for geometry calculations
    ///</summary>
    class Geometry
    {
        public static double Distance2D(esriGeometry.MapPoint point1, esriGeometry.MapPoint point2)
        {
            double dist2D = Math.Sqrt(Math.Pow(point1.X - point2.X, 2)+ Math.Pow(point1.Y - point2.Y, 2));
            return dist2D;
        }

        public static double Distance3D(esriGeometry.MapPoint point1, esriGeometry.MapPoint point2)
        {
            double dist3D = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2) + Math.Pow(point1.Z - point2.Z, 2));
            return dist3D;
        }
    
        public static esriGeometry.Polygon PolygonFromPoint(esriGeometry.MapPoint mapPoint, double gridSpacing)
        {
            esriGeometry.Polygon polygon = new esriGeometry.Polygon();
            esriGeometry.PointCollection pointCollection = new esriGeometry.PointCollection();

            double xMin = mapPoint.X - gridSpacing / 2;
            double yMin = mapPoint.Y - gridSpacing / 2;
            double xMax = mapPoint.X + gridSpacing / 2;
            double yMax = mapPoint.Y + gridSpacing / 2;


            esriGeometry.MapPoint mapPoint1 = new esriGeometry.MapPoint(xMin, yMin);
            esriGeometry.MapPoint mapPoint2 = new esriGeometry.MapPoint(xMin, yMax);
            esriGeometry.MapPoint mapPoint3 = new esriGeometry.MapPoint(xMax, yMax);
            esriGeometry.MapPoint mapPoint4 = new esriGeometry.MapPoint(xMax, yMin);

            pointCollection.Add(mapPoint1);
            pointCollection.Add(mapPoint2);
            pointCollection.Add(mapPoint3);
            pointCollection.Add(mapPoint4);

            polygon.Rings.Add(pointCollection);

            return polygon;
        }
    }
}
