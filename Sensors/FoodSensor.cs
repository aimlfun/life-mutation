using LifeMutation.Configuration;
using LifeMutation.Utilities;

namespace LifeMutation.Sensors;

/// <summary>
/// Sensor for detecting food.
/// </summary>
internal class FoodSensor
{
    /// <summary>
    /// Used for showing the sensors sweep.
    /// </summary>
    private readonly List<PointF[]> foodSensorSweepTrianglePolygonsInDeviceCoordinates = new();

    /// <summary>
    /// Used for showing which sensor picked up the food.
    /// </summary>
    private readonly List<PointF[]> foodSensorTriangleTargetIsInPolygonsInDeviceCoordinates = new();

    /// <summary>
    /// Detects any food within the sensor.
    /// </summary>
    /// <param name="angleLifeFormIsPointing"></param>
    /// <param name="lifeFormLocation"></param>
    /// <param name="food"></param>
    /// <param name="sensorRegionsOutput"></param>
    /// <returns></returns>
    internal double[] Read(double angleLifeFormIsPointing, PointF lifeFormLocation, List<PointF> food, out double[] sensorRegionsOutput)
    {
        foodSensorTriangleTargetIsInPolygonsInDeviceCoordinates.Clear();
        foodSensorSweepTrianglePolygonsInDeviceCoordinates.Clear();

        int SamplePoints = (int)(360F / Config.FoodSensorVisionAngleInDegrees);

        sensorRegionsOutput = new double[SamplePoints];

        // e.g 
        // input to the neural network
        //   _ \ | / _   
        //   0 1 2 3 4 
        //        
        double fieldOfVisionStartInDegrees = 0;

        //   _ \ | / _   
        //   0 1 2 3 4
        //   [-] this
        double sensorVisionAngleInDegrees = Config.FoodSensorVisionAngleInDegrees;

        //   _ \ | / _   
        //   0 1 2 3 4
        //   ^ this
        double sensorAngleToCheckInDegrees = fieldOfVisionStartInDegrees - sensorVisionAngleInDegrees / 2 + angleLifeFormIsPointing;

        double DepthOfVisionInPixels = Config.FoodSensorVisionDepthOfVisionInPixels;

        double maxMult = 0;

        for (int LIDARangleIndex = 0; LIDARangleIndex < SamplePoints; LIDARangleIndex++)
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

            foodSensorSweepTrianglePolygonsInDeviceCoordinates.Add(new PointF[] { lifeFormLocation, p1, p2 });

            sensorRegionsOutput[LIDARangleIndex] = -1; // no target in this direction

            foreach (PointF p in food)
            {
                double mult = 0;

                if (Utils.PtInTriangle(p, lifeFormLocation, p1, p2))
                {
                    double dist = Utils.DistanceBetweenTwoPoints(lifeFormLocation, p);

                    mult = 1 - dist / Config.FoodSensorVisionDepthOfVisionInPixels;
                    mult = mult.Clamp(-1, 1);
                }

                if (mult > sensorRegionsOutput[LIDARangleIndex]) sensorRegionsOutput[LIDARangleIndex] = mult;
                if (mult > maxMult) maxMult = mult;
            }

            if (sensorRegionsOutput[LIDARangleIndex] > 0)
            {
                foodSensorTriangleTargetIsInPolygonsInDeviceCoordinates.Add(new PointF[] { lifeFormLocation, p1, p2 });
            }

            //   _ \ | / _         _ \ | / _   
            //   0 1 2 3 4         0 1 2 3 4
            //  [-] from this       [-] to this
            sensorAngleToCheckInDegrees += sensorVisionAngleInDegrees;
        }

        for (int LIDARangleIndex = 0; LIDARangleIndex < SamplePoints; LIDARangleIndex++)
        {
            if (sensorRegionsOutput[LIDARangleIndex] != maxMult)
            {
                sensorRegionsOutput[LIDARangleIndex] = 0;
            }
            else
            {
                sensorRegionsOutput[LIDARangleIndex] = 1;
                maxMult = -99; // only one matches
            }
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
    /// </summary>
    /// <param name="g"></param>
    /// <param name="triangleSweepColour"></param>
    internal void DrawFullSweepOfSensor(Graphics g, Color triangleSweepColour)
    {
        using SolidBrush brushSensor = new(triangleSweepColour);

        foreach (PointF[] point in foodSensorSweepTrianglePolygonsInDeviceCoordinates) g.FillPolygon(brushSensor, point);
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
        // draw the  sensor
        foreach (PointF[] point in foodSensorTriangleTargetIsInPolygonsInDeviceCoordinates) g.FillPolygon(sbColor, point);
    }
}