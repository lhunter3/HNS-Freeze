using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;

namespace HNSFreeze
{
    public class SphereEntity
    {

        public Vector CenterPoint { get; set; }
        public int Radius { get; set; } = 1;

        public List<CCSPlayerController> players { get; set; }

        public Vector[] circleOutterPoints { get ; set; }
        public Vector[] circleInnerPoints { get; set; }


        public SphereEntity(Vector center, int radius) 
        {
            CenterPoint = center;
            Radius = radius;
            players = new List<CCSPlayerController>();

            circleOutterPoints = CalculateCirclePoints(CenterPoint, Radius, 360);
            circleInnerPoints = CalculateCirclePoints(CenterPoint, Radius-25, 360);

        }


        public bool colidesWithPlayer(Vector point) 
        {
          var distSrt = Vector3DistanceSquared(CenterPoint, point);
          var radiusSquared = Math.Pow(Radius, 2);

          return distSrt < radiusSquared;

        }


        public static float Vector3DistanceSquared(Vector a, Vector b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            float dz = a.Z - b.Z;

            return dx * dx + dy * dy + dz * dz;
        }


        private static Vector[] CalculateCirclePoints(Vector center, float radius, int numberOfPoints)
        {
            Vector[] points = new Vector[numberOfPoints];

            for (int i = 0; i < numberOfPoints; i++)
            {
                float theta = 2.0f * (float)Math.PI * i / numberOfPoints;
                points[i] = center + new Vector(radius * (float)Math.Cos(theta), radius * (float)Math.Sin(theta), 0.0f);
            }

            return points;
        }


    }
}
