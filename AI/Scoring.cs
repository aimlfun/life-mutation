using LifeMutation.LifeFormAndManager;
using LifeMutation.Utilities;
using System.Drawing.Drawing2D;

namespace LifeMutation.AI
{
    /// <summary>
    /// Class used to score lifeforms. One per game.
    /// </summary>
    internal static class Scoring
    {
        /// <summary>
        /// Supported games (extendable).
        /// </summary>
        internal enum Games { dontStarve, reachLeftSide, reachRightSide, reachCenter, reachCorner, dontTouchRed }

        /// <summary>
        /// Which game the lifeforms are playing.
        /// </summary>
        internal static Games GameCurrentlyBeingPlayed = Games.reachCenter;

        /// <summary>
        /// The centre of the playing area.
        /// </summary>
        private static PointF centerPointOfPlayArea = new(150, 150);

        /// <summary>
        /// Determines the fitness of a life-form.
        /// </summary>
        internal static float GetFitness(LifeForm lifeForm)
        {
            float fitness;

            if (lifeForm.IsDead) return 0; // being dead gets you no points, you failed, lifeform.

            switch (GameCurrentlyBeingPlayed)
            {
                case Games.dontStarve:
                    fitness = lifeForm.AmountOfFoodEaten; // eat as much as you is the goal
                    break;

                case Games.reachLeftSide: // 0..200 gives score (0 most), 201..300 = 0 (centre is at 150px)
                    fitness = lifeForm.Location.X < 180 ? (180 - lifeForm.Location.X) * 1000 : 0;

                    if (lifeForm.Location.X < 110) fitness += 1000000;

                    if (lifeForm.Location.X < 110)
                        fitness += (290 - lifeForm.Location.Y) + 500;
                    else
                        fitness += lifeForm.Location.X >= 110 && lifeForm.Location.X < 190 ? lifeForm.Location.Y * 10 : 0; // extra points for being higher

                    break;

                case Games.reachRightSide: // 200 is 2/3rd of width. Anything less becomes negative
                    fitness = (lifeForm.Location.X - 120) * 1000;

                    if (lifeForm.Location.X > 200) fitness += 1000000;

                    fitness += lifeForm.Location.X > 120 ? 290 - lifeForm.Location.Y : 0; // extra points for being higher
                    break;

                case Games.dontTouchRed:
                    fitness = lifeForm.Location.X >= 120 &&
                              lifeForm.Location.X <= 180 &&
                              lifeForm.Location.Y >= 120 &&
                              lifeForm.Location.Y <= 180 ? 0 : Utils.DistanceBetweenTwoPoints(lifeForm.Location, new(150, 150));
                    break;

                case Games.reachCenter:
                    float distance = Utils.DistanceBetweenTwoPoints(lifeForm.Location, centerPointOfPlayArea);

                    if (distance < 50) fitness = 50 - distance; else fitness = 0;
                    break;

                case Games.reachCorner: // if less than 161 away, they are no way near the corner. (approx)
                    fitness = Utils.DistanceBetweenTwoPoints(lifeForm.Location, centerPointOfPlayArea);
                    if (fitness < 161) fitness = 0; // furthest from center
                    break;

                default:
                    throw new Exception("Implementation missing for scoring Game.");
            }

            return fitness;
        }

        /// <summary>
        /// Draws the regions that impact "score".
        /// </summary>
        /// <param name="g"></param>
        internal static void Draw(Graphics g)
        {
            // targets are either things it will seek or things it should avoid. Either way they are coloured pink.
            foreach (Rectangle r in GenerationManager.s_targets)
            {
                g.FillRectangle(Brushes.Pink, r);
            }

            switch (GameCurrentlyBeingPlayed)
            {
                case Games.dontStarve:
                    List<PointF> food;

                    // when monitoring we show dots of food specific to monitored individual.
                    if (Form1.IdOfLifeformBeingMonitored == -1) food = GenerationManager.s_foodDots; else food = GenerationManager.s_lifeForms[Form1.IdOfLifeformBeingMonitored].FoodDots;

                    // draw the food.
                    foreach (PointF point in food)
                    {
                        g.FillEllipse(Brushes.YellowGreen, new RectangleF(point.X - 3, point.Y - 3, 6, 6));
                    }
                    break;

                case Games.reachLeftSide: // rectangle on left 1/3
                    {
                        using HatchBrush greenBrush = new(HatchStyle.Percent90, Color.FromArgb(70, 0, 255, 0));
                        g.FillRectangle(greenBrush, new Rectangle(0, 0, 100, GenerationManager.s_canvas.Height - 1));
                        break;
                    }

                case Games.reachRightSide: // rectangle on right 1/3
                    {
                        using HatchBrush greenBrush = new(HatchStyle.Percent90, Color.FromArgb(70, 0, 255, 0));
                        g.FillRectangle(greenBrush, new Rectangle(200, 0, GenerationManager.s_canvas.Width - 1, GenerationManager.s_canvas.Height - 1));
                        break;
                    }

                case Games.reachCenter: // circle
                    {
                        using HatchBrush greenBrush = new(HatchStyle.Percent90, Color.FromArgb(70, 0, 255, 0));
                        g.FillEllipse(greenBrush, new Rectangle(150 - 50, 150 - 50, 100, 100));
                        break;
                    }

                case Games.reachCorner: // triangles in the corner
                    {
                        using HatchBrush greenBrush = new(HatchStyle.Percent90, Color.FromArgb(70, 0, 255, 0));
                        g.FillPolygon(greenBrush, new Point[] { new Point(0, 0), new Point(95, 0), new Point(0, 95) });
                        g.FillPolygon(greenBrush, new Point[] { new Point(299, 0), new Point(204, 0), new Point(299, 95) });
                        g.FillPolygon(greenBrush, new Point[] { new Point(299, 299), new Point(204, 299), new Point(299, 204) });
                        g.FillPolygon(greenBrush, new Point[] { new Point(0, 299), new Point(95, 299), new Point(0, 204) });
                        break;
                    }
            }
        }
    }
}