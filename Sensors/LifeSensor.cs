using LifeMutation.Configuration;
using LifeMutation.LifeFormAndManager;
using LifeMutation.Utilities;

namespace LifeMutation.Sensors;

internal class LifeSensor
{
    /// <summary>
    /// Used for showing the sensors sweep.
    /// </summary>
    private readonly List<PointF[]> sensorSweepTrianglePolygonsInDeviceCoordinates = new();

    /// <summary>
    /// Used for showing which sensor picked up the target/wall.
    /// </summary>
    private readonly List<PointF[]> sensorTriangleTargetIsInPolygonsInDeviceCoordinates = new();

    /// <summary>
    /// Detects any life-forms within the sensor.
    /// </summary>
    /// <param name="angleLifeFormIsPointing"></param>
    /// <param name="lifeFormLocation"></param>
    /// <param name="id"></param>
    /// <param name="sensorRegionsOutput"></param>
    /// <returns></returns>
    internal double[] Read(double angleLifeFormIsPointing, PointF lifeFormLocation, int id, out double[] sensorRegionsOutput)
    {
        sensorTriangleTargetIsInPolygonsInDeviceCoordinates.Clear();
        sensorSweepTrianglePolygonsInDeviceCoordinates.Clear();

        sensorRegionsOutput = new double[Config.LifeFormSamplePoints];

        // e.g 
        // input to the neural network
        //   _ \ | / _   
        //   0 1 2 3 4 
        //        
        double fieldOfVisionStartInDegrees = 0;

        //   _ \ | / _   
        //   0 1 2 3 4
        //   [-] this
        double sensorVisionAngleInDegrees = 45;

        //   _ \ | / _   
        //   0 1 2 3 4
        //   ^ this
        double sensorAngleToCheckInDegrees = fieldOfVisionStartInDegrees - sensorVisionAngleInDegrees / 2 + angleLifeFormIsPointing;

        double DepthOfVisionInPixels = Config.LifeSensorDepthOfVisionInPixels;

        for (int LIDARangleIndex = 0; LIDARangleIndex < Config.LifeFormSamplePoints; LIDARangleIndex++)
        {
            //     -45  0  45
            //  -90 _ \ | / _ 90   <-- relative to direction of lifeform, hence + angle lifeform is pointing
            double LIDARangleToCheckInRadiansMin = Utils.DegreesInRadians(sensorAngleToCheckInDegrees);
            double LIDARangleToCheckInRadiansMax = LIDARangleToCheckInRadiansMin + Utils.DegreesInRadians(sensorVisionAngleInDegrees);

            /*  p1        p2
             *   +--------+
             *    \      /
             *     \    /     this is our imaginary "sensor" triangle
             *      \  /
             *       \/
             *    lifeform
             */
            PointF p1 = new((float)(Math.Sin(LIDARangleToCheckInRadiansMin) * DepthOfVisionInPixels + lifeFormLocation.X),
                            (float)(Math.Cos(LIDARangleToCheckInRadiansMin) * DepthOfVisionInPixels + lifeFormLocation.Y));

            PointF p2 = new((float)(Math.Sin(LIDARangleToCheckInRadiansMax) * DepthOfVisionInPixels + lifeFormLocation.X),
                            (float)(Math.Cos(LIDARangleToCheckInRadiansMax) * DepthOfVisionInPixels + lifeFormLocation.Y));

            sensorSweepTrianglePolygonsInDeviceCoordinates.Add(new PointF[] { lifeFormLocation, p1, p2});

            sensorRegionsOutput[LIDARangleIndex] = 1; // no target in this direction
            
            foreach (int lifeFormId in GenerationManager.s_lifeForms.Keys)
            {
                if (lifeFormId == id) continue; // don't sense yourself!

                LifeForm lf = GenerationManager.s_lifeForms[lifeFormId];
                PointF p = lf.Location;

                if (Utils.PtInTriangle(p, lifeFormLocation, p1, p2))
                {
                    double dist = Utils.DistanceBetweenTwoPoints(lifeFormLocation, p);

                    double mult = 1 - dist / DepthOfVisionInPixels;

                    if (mult < sensorRegionsOutput[LIDARangleIndex])
                    {
                        sensorRegionsOutput[LIDARangleIndex] = mult;  // closest
                    }
                }
            }

            if (sensorRegionsOutput[LIDARangleIndex] != 1)
            {
                sensorTriangleTargetIsInPolygonsInDeviceCoordinates.Add(new PointF[] { lifeFormLocation, p1, p2 });
            }

            //   _ \ | / _         _ \ | / _   
            //   0 1 2 3 4         0 1 2 3 4
            //  [-] from this       [-] to this
            sensorAngleToCheckInDegrees += sensorVisionAngleInDegrees;
        }

        return sensorRegionsOutput;
    }

    /// <summary>
    /// Draws the full triangle sweep range.
    /// +--------+
    ///  \      /
    ///   \    /     this is our imaginary "sensor" triangle
    ///    \  /
    ///     \/
    ///  lifeform
    /// </summary>
    /// <param name="g"></param>
    /// <param name="triangleSweepColour"></param>
    internal void DrawFullSweepOfSensor(Graphics g, Color triangleSweepColour)
    {
        using SolidBrush brushSensor = new(triangleSweepColour);

        foreach (PointF[] point in sensorSweepTrianglePolygonsInDeviceCoordinates) g.FillPolygon(brushSensor, point);
    }

    /// <summary>
    /// Draws the region of the sweep that the target is in.
    /// +---++---+
    ///  \  ||  /
    ///   \ || /     hopefully the center strip
    ///    \||/
    ///     \/
    ///   lifeform
    /// </summary>
    /// <param name="g"></param>
    internal void DrawWhereTargetIsInRespectToSweepOfSensor(Graphics g, SolidBrush sbColor)
    {
        // draw the sensor
        foreach (PointF[] point in sensorTriangleTargetIsInPolygonsInDeviceCoordinates) g.FillPolygon(sbColor, point);
    }
}