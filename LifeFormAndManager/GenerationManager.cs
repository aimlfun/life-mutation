using LifeMutation.AI;
using LifeMutation.Configuration;
using LifeMutation.Utilities;
using System.Security.Cryptography;

namespace LifeMutation.LifeFormAndManager
{
    /// <summary>
    /// Manages the generations of lifeforms.
    /// </summary>
    internal class GenerationManager
    {
        /// <summary>
        /// Countdown of moves. Upon reaching 0, it mutates.
        /// </summary>
        internal static int s_movesBeforeMutation = 0;

        /// <summary>
        /// Tracks the generation/epoch.
        /// </summary>
        internal static int s_generation = 0;

        /// <summary>
        /// Tracks the lifeforms by identifier.
        /// </summary>
        internal readonly static Dictionary<int, LifeForm> s_lifeForms = new();

        /// <summary>
        /// Tracks the food dots (used for all).
        /// </summary>
        internal readonly static List<PointF> s_foodDots = new();

        /// <summary>
        /// Tracks the targets.
        /// </summary>
        internal readonly static List<Rectangle> s_targets = new();

        /// <summary>
        /// The picture box on which the game is drawn.
        /// </summary>
        internal static PictureBox? s_canvas;

        /// <summary>
        /// Initialises the generation manager, with its canvas.
        /// </summary>
        /// <param name="canvas"></param>
        internal static void CreateGenerationManager(PictureBox canvas)
        {
            s_generation = 0;
            s_canvas = canvas;

            s_foodDots.Clear();
            s_lifeForms.Clear();
            s_targets.Clear();

            if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.reachCenter)
            {
                s_targets.Add(new Rectangle(150 - 10, 150 - 10, 20, 20));
            }

            if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.dontTouchRed)
            {
                s_targets.Add(new Rectangle(120, 120, 60, 60));
            }

            if (Config.AddBaffles)
            {
                s_targets.Add(new Rectangle(110, 0, 20, 170)); // left-top wall
                s_targets.Add(new Rectangle(190, 119, 20, 180)); // right-bottom wall

                s_targets.Add(new Rectangle(0, 0, 5, 299)); // left
                s_targets.Add(new Rectangle(293, 0, 5, 299)); // right
                s_targets.Add(new Rectangle(0, 293, 299, 5)); // bottom
                s_targets.Add(new Rectangle(0, 0, 299, 5)); // top
            }

            NewGeneration();

            Draw();
        }

        /// <summary>
        /// Create a new generation of lifeform blobs and food.
        /// </summary>
        internal static void NewGeneration()
        {
            s_movesBeforeMutation = Config.MovesBeforeMutationtationOccurs;

            ++s_generation;

            s_lifeForms.Clear();

            s_foodDots.Clear();

            if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.dontStarve)
            {
                CreateFoodDots();
            }

            for (int i = 0; i < Config.NumberOfLifeformsToCreate; i++)
            {
                s_lifeForms.Add(i, new LifeForm(i, SafeLocationToSpawnLifeForm(), s_foodDots));
            }
        }

        /// <summary>
        /// Creates the food dots.
        /// </summary>
        private static void CreateFoodDots()
        {
            for (int i = 0; i < Config.FoodDotCount; i++)
            {
                Point loc = new(RandomNumberGenerator.GetInt32(15, 285), RandomNumberGenerator.GetInt32(15, 285));

                bool notSafe = false;

                foreach (Rectangle rect in s_targets)
                {
                    if (rect.Contains(new Point(loc.X, loc.Y)))
                    {
                        notSafe = true; break;
                    }
                }

                if (!notSafe) s_foodDots.Add(loc);
            }
        }

        /// <summary>
        /// Check to sse if it is safe for a lifeform to spawn.
        /// </summary>
        /// <returns></returns>
        private static Point SafeLocationToSpawnLifeForm()
        {
            Rectangle bounds = new(5, 5, 290, 290); // default

            switch (Scoring.GameCurrentlyBeingPlayed)
            {
                case Scoring.Games.dontStarve:
                    break;

                case Scoring.Games.reachLeftSide: // rectangle on left 1/3
                    bounds = new(200, 155, 285 - 195, 285 - 160);
                    break;

                case Scoring.Games.reachRightSide: // rectangle on right 1/3
                    bounds = new(10, 5, 189, 290);
                    break;

                case Scoring.Games.dontTouchRed:
                    break;

                case Scoring.Games.reachCenter: // circle
                    bounds = new(20, 20, 260, 260);
                    break;

                case Scoring.Games.reachCorner: // triangles in the corner
                    break;
            }

            while (true)
            {
                Point loc = new(RandomNumberGenerator.GetInt32(bounds.Left, bounds.Right), RandomNumberGenerator.GetInt32(bounds.Top, bounds.Bottom));

                if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.reachCenter && Utils.DistanceBetweenTwoPoints(loc, new PointF(150, 150)) < 50) continue;
                if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.dontTouchRed && (loc.X >= 120 && loc.X <= 180 && loc.Y >= 120 && loc.Y <= 180)) continue;

                // don't touch red, you die!
                if (Scoring.GameCurrentlyBeingPlayed != Scoring.Games.reachCenter)
                {
                    bool notSafe = false;

                    foreach (Rectangle rect in s_targets)
                    {
                        if (rect.Contains(new Point(loc.X, loc.Y)))
                        {
                            notSafe = true; break;
                        }
                    }

                    if (notSafe) continue;
                }
                else
                {
                    if (Utils.DistanceBetweenTwoPoints(loc, new PointF(150, 150)) < 50) continue;
                }

                if (!Config.LifeFormCollisionDetectionEnabled || !LifeFormCollided(-1, loc)) return loc;
            }
        }

        /// <summary>
        /// Moves all the life-forms.
        /// </summary>
        /// <returns>True - able to move at least one.</returns>
        internal static bool Move()
        {
            bool allDead = true;

            for (int i = 0; i < Config.NumberOfLifeformsToCreate; i++)
            {
                s_lifeForms[i].Move();
                allDead &= s_lifeForms[i].IsDead;
            }

            if (--s_movesBeforeMutation < 0) return false; // kill them all, they've played for too long

            // one remaining and, if game is "dontStarve" that we have at least 1 dot remaining
            return !allDead && (Scoring.GameCurrentlyBeingPlayed != Scoring.Games.dontStarve || s_foodDots.Count > 0);
        }

        /// <summary>
        /// Draws the game with lifeforms.
        /// </summary>
        internal static void Draw()
        {
            if (s_canvas is null) throw new Exception("canvas should not be null."); // populated CreateGenerationManager()

            // we're making a "new" bitmap, as updating the existing would cause a huge mess (without lots of complex logic)
            // after painting, we replace the PictureBox image with the new "frame"
            Bitmap b = new(s_canvas.Width, s_canvas.Height);

            using Graphics g = Graphics.FromImage(b);

            // make the "blobs" lets pixellated
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.Clear(Color.White); // background for life-forms

            Scoring.Draw(g); // adds any scoring specific visualisations

            // ask all the life forms to draw themselves
            for (int i = 0; i < Config.NumberOfLifeformsToCreate; i++)
            {
                s_lifeForms[i].Draw(g);
            }

            s_canvas.Image?.Dispose(); // forget this, and every frame will occupy more ram!
            s_canvas.Image = b; // show the new "frame"
        }

        /// <summary>
        /// Test to see if it has collided with another lifeform.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        private static bool LifeFormCollided(int id, PointF location)
        {
            // collided with another life-form?
            foreach (int idOfLifeForm in s_lifeForms.Keys)
            {
                LifeForm f = s_lifeForms[idOfLifeForm];

                if (f.UniqueLifeFormIdentifier == id) continue;

                if (Utils.DistanceBetweenTwoPoints(f.Location, location) < f.DiameterOfBlob)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Collision detection: life-form, food, walls.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        internal static int DetectCollisionWithFoodWallOrLifeform(int id, PointF location, List<PointF> foodDots)
        {
            List<int> removalList = new();

            // don't touch red, you die!
            if (Scoring.GameCurrentlyBeingPlayed != Scoring.Games.reachCenter)
            {
                foreach (Rectangle rect in s_targets)
                {
                    Rectangle r2 = rect;

                    r2.Inflate(4, 4); // because the circle has radius

                    if (r2.Contains(new Point((int)location.X, (int)location.Y))) return 1;
                }
            }

            // collided with another life-form?
            if (LifeFormCollided(id, location))
            {
                return 2;
            }

            if (foodDots.Count > 0)
            {
                float diameter = GenerationManager.s_lifeForms[id].DiameterOfBlob;

                // ran over food?
                for (int i = 0; i < foodDots.Count; i++)
                {
                    if (Utils.DistanceBetweenTwoPoints(foodDots[i], location) <= diameter / 2 + 5) // 3=width of food+2 because the max speed is 3
                    {
                        removalList.Add(i);
                    }
                }

                // ran over food, remove and return food
                if (removalList.Count > 0)
                {
                    removalList.Sort((a, b) => b.CompareTo(a));

                    foreach (int i in removalList)
                    {
                        foodDots.RemoveAt(i);
                    }

                    return 3;
                }
            }

            return 0;
        }

        /// <summary>
        /// Mutate the life-forms, discarding bottom performing 50%.
        /// </summary>
        internal static void Mutate()
        {
            // update networks fitness for each lifeform
            foreach (int id in s_lifeForms.Keys) s_lifeForms[id].UpdateFitness();

            NeuralNetwork.SortNetworkByFitness(); // largest "fitness" (best performing) goes to the bottom

            // sorting is great but index no longer matches the "id".
            // this is because the sort swaps but this misaligns id with the entry            
            
            List<NeuralNetwork> n = new();
            float total = 0;

            foreach (int n2 in NeuralNetwork.s_networks.Keys)
            {
                n.Add(NeuralNetwork.s_networks[n2]);
                total += Math.Abs(NeuralNetwork.s_networks[n2].Fitness > 0 ? NeuralNetwork.s_networks[n2].Fitness : 0);
            }

            // start again, if none meet the training
            if (total == 0)
            {
                NeuralNetwork.s_networks.Clear();
                return;
            }

            NeuralNetwork[] array = n.ToArray();

            // replace the 50% worse offenders with the best, then mutate them.
            // we do this by copying top half (lowest fitness) with top half.
            for (int worstNeuralNetworkIndex = 0; worstNeuralNetworkIndex < s_lifeForms.Keys.Count / 2; worstNeuralNetworkIndex++)
            {
                // 50..100 (in 100 neural networks) are in the top performing
                int neuralNetworkToCloneFromIndex = worstNeuralNetworkIndex + s_lifeForms.Keys.Count / 2; // +50% -> top 50% 

                NeuralNetwork.CopyFromTo(array[neuralNetworkToCloneFromIndex], array[worstNeuralNetworkIndex]); // copy

                array[worstNeuralNetworkIndex].Mutate(50, 0.5F); // mutate
            }

            NeuralNetwork.s_networks[array[0].Id] = new NeuralNetwork(array[0].Id, array[0].Layers, false);


            // unsort, restoring the order of lifeform to neural network i.e [x]=id of "x".
            Dictionary<int, NeuralNetwork> unsortedNetworksDictionary = new();

            for (int index = 0; index < s_lifeForms.Keys.Count; index++)
            {
                var neuralNetwork = NeuralNetwork.s_networks[index];

                unsortedNetworksDictionary[neuralNetwork.Id] = neuralNetwork;
            }


            NeuralNetwork.s_networks = unsortedNetworksDictionary;
        }
    }
}
