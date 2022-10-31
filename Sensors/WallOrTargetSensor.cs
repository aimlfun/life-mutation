using LifeMutation.AI;
using LifeMutation.Configuration;
using LifeMutation.LifeFormAndManager;
using LifeMutation.Utilities;
using System.Drawing.Drawing2D;

namespace LifeMutation.Sensors;

/// <summary>
/// Sensor used to detect walls / lines / squares etc.
/// </summary>
internal class WallOrTargetSensor
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
    /// Detects any walls or targets within the sensor.
    /// </summary>
    /// <param name="angleLifeFormIsPointing"></param>
    /// <param name="lifeFormLocation"></param>
    /// <param name="sensorRegionsOutput"></param>
    /// <returns></returns>
    internal double[] Read(double angleLifeFormIsPointing, PointF lifeFormLocation, out double[] sensorRegionsOutput)
    {
        sensorTriangleTargetIsInPolygonsInDeviceCoordinates.Clear();
        sensorSweepTrianglePolygonsInDeviceCoordinates.Clear();

        sensorRegionsOutput = new double[Config.WallOrTargetSamplePoints];

        // e.g 
        // input to the neural network
        //   _ \ | / _   
        //   0 1 2 3 4 
        //        
        double fieldOfVisionStartInDegrees = 0;

        //   _ \ | / _   
        //   0 1 2 3 4
        //   [-] this
        double sensorVisionAngleInDegrees = 360f / Config.WallOrTargetSamplePoints;

        //   _ \ | / _   
        //   0 1 2 3 4
        //   ^ this
        double sensorAngleToCheckInDegrees = fieldOfVisionStartInDegrees - sensorVisionAngleInDegrees / 2 + angleLifeFormIsPointing;

        double DepthOfVisionInPixels = (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.reachCenter ? Config.ReachCentreWallOrTargetDepthOfVisionInPixels : Config.OtherWallOrTargetDepthOfVisionInPixels);

        for (int LIDARangleIndex = 0; LIDARangleIndex < Config.WallOrTargetSamplePoints; LIDARangleIndex++)
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

            sensorSweepTrianglePolygonsInDeviceCoordinates.Add(new PointF[] { lifeFormLocation,
                                                                                  p1,
                                                                                  p2});

            sensorRegionsOutput[LIDARangleIndex] = 99; // no target in this direction

            // check each "target" rectangle and see if it intersects with the sensor.            
            foreach (Rectangle r in GenerationManager.s_targets)
            {
                for (int i = 0; i < 4; i++)
                {
                    PointF point1;
                    PointF point2;

                    switch (i)
                    {
                        case 0: // top 
                            point1 = new PointF(r.X, r.Y);
                            point2 = new PointF(r.Right, r.Y);
                            break;
                        case 1: // bottom
                            point1 = new PointF(r.X, r.Bottom);
                            point2 = new PointF(r.Right, r.Bottom);
                            break;
                        case 2: // left
                            point1 = new PointF(r.X, r.Y);
                            point2 = new PointF(r.X, r.Bottom);
                            break;
                        case 3: // right
                            point1 = new PointF(r.Right, r.Y);
                            point2 = new PointF(r.Right, r.Bottom);
                            break;
                        default: throw new Exception("4 permutations 0..3");
                    }

                    PointF intersection2 = new();
                    PointF intersection3 = new();
                    PointF intersection4 = new();

                    bool detectedWallBetweenLifeFormAndEdgeOfLeftSideOfTriangle = false;
                    bool detectedWallBetweenLifeFormAndEdgeOfRightSideOfTriangle = false;
                    bool detectedWallBetweenFurthestEdgeOfTriangle = false;

                    /*  p1         p2
                     *   +--------+
                     *    \      /
                     *     \   \\ wall    this is our imaginary "wall sensor" triangle
                     *      \  /
                     *       \/
                     *    lifeFormLocation
                     */
                    if (Utils.GetLineIntersection(lifeFormLocation, p1, point1, point2, out intersection2))
                    {
                        detectedWallBetweenLifeFormAndEdgeOfLeftSideOfTriangle = true;
                    }

                    /*  p1         p2
                     *   +--------+
                     *    \      /
                     * wall//   /     this is our imaginary "wall sensor" triangle
                     *      \  /
                     *       \/
                     *    lifeFormLocation
                     */
                    if (Utils.GetLineIntersection(lifeFormLocation, p2, point1, point2, out intersection3))
                    {
                        detectedWallBetweenLifeFormAndEdgeOfRightSideOfTriangle = true;
                    }


                    /*  p1  wall   p2
                     *   +---||---+
                     *    \      /
                     *     \    /     this is our imaginary "wall sensor" triangle
                     *      \  /
                     *       \/
                     *    lifeFormLocation
                     */
                    if (Utils.GetLineIntersection(p1, p2, point1, point2, out intersection4))
                    {
                        detectedWallBetweenFurthestEdgeOfTriangle = true;
                    }

                    if (!detectedWallBetweenLifeFormAndEdgeOfLeftSideOfTriangle &&
                        !detectedWallBetweenLifeFormAndEdgeOfRightSideOfTriangle &&
                        !detectedWallBetweenFurthestEdgeOfTriangle) continue;

                    if (!detectedWallBetweenLifeFormAndEdgeOfRightSideOfTriangle) intersection3 = intersection2;
                    if (!detectedWallBetweenLifeFormAndEdgeOfLeftSideOfTriangle) intersection2 = intersection3;

                    if (detectedWallBetweenFurthestEdgeOfTriangle)
                    {
                        intersection2 = intersection4;
                        intersection3 = intersection2;
                    }

                    PointF intersection = new((intersection2.X + intersection3.X) / 2,
                                              (intersection2.Y + intersection3.Y) / 2 );

                    double dist = Utils.DistanceBetweenTwoPoints(lifeFormLocation, intersection).Clamp(0F, (float)DepthOfVisionInPixels);

                    double mult = dist / DepthOfVisionInPixels;

                    if (mult < sensorRegionsOutput[LIDARangleIndex])
                    {
                        sensorRegionsOutput[LIDARangleIndex] = mult;  // closest
                    }
                }
            }

            if (sensorRegionsOutput[LIDARangleIndex] == 99) sensorRegionsOutput[LIDARangleIndex] = 1;


            if (sensorRegionsOutput[LIDARangleIndex] != 1)
            {
                sensorTriangleTargetIsInPolygonsInDeviceCoordinates.Add(new PointF[] { lifeFormLocation, p1, p2 });
            }

            sensorRegionsOutput[LIDARangleIndex] = 1 - sensorRegionsOutput[LIDARangleIndex];

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
    /// </summary>
    /// <param name="g"></param>
    /// <param name="triangleSweepColour"></param>
    internal void DrawFullSweepOfSensor(Graphics g, Color triangleSweepColour)
    {
        bool showSegmentNumber = false;

        using SolidBrush brushSensor = new(triangleSweepColour);
        using Pen pen = new(Color.FromArgb(60, 100, 100, 100));

        int i = 0;

        foreach (PointF[] point in sensorSweepTrianglePolygonsInDeviceCoordinates)
        {
            g.FillPolygon(brushSensor, point);
            g.DrawPolygon(pen, point);

            if (showSegmentNumber)
            {
                g.DrawString(i.ToString(), new Font("Arial", 7), Brushes.Black, CentreOfPoints(point));
                ++i;
            }
        }
    }

    /// <summary>
    /// Determines the centre of all the points. It's possibly too approximate, 
    /// by taking min/max boundaries and halving.
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private static PointF CentreOfPoints(PointF[] point)
    {
        PointF pointFmin = new(int.MaxValue, int.MaxValue);
        PointF pointFmax = new(-1, -1);

        foreach (PointF p in point)
        {
            if (p.X < pointFmin.X) pointFmin = new PointF(p.X, pointFmin.Y);
            if (p.Y < pointFmin.Y) pointFmin = new PointF(pointFmin.X, p.Y);
            if (p.X > pointFmax.X) pointFmax = new PointF(p.X, pointFmax.Y);
            if (p.Y > pointFmax.Y) pointFmax = new PointF(pointFmax.X, p.Y);
        }

        return new PointF((pointFmin.X + pointFmax.X) / 2, (pointFmin.Y + pointFmax.Y) / 2);
    }

    /// <summary>
    /// Draws the region of the sweep that the target is in.
    /// +---++---+
    ///  \  ||  /
    ///   \ || /     hopefully the center strip
    ///    \||/
    ///     \/
    ///     lifeform
    /// </summary>
    /// <param name="g"></param>
    internal void DrawWhereTargetIsInRespectToSweepOfSensor(Graphics g, SolidBrush sbColor)
    {
        using Pen pen = new(Color.FromArgb(60, 100, 100, 100));
        pen.DashStyle = DashStyle.Dot;

        // draw the sensor
        foreach (PointF[] point in sensorTriangleTargetIsInPolygonsInDeviceCoordinates)
        {
            g.FillPolygon(sbColor, point);
            g.DrawPolygon(pen, point);
        }
    }
}