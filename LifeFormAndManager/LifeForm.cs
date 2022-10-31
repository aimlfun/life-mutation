using LifeMutation.AI;
using LifeMutation.Configuration;
using LifeMutation.Sensors;
using LifeMutation.Utilities;
using System.Security.Cryptography;

namespace LifeMutation.LifeFormAndManager
{
    /// <summary>
    /// Represents a simple life form
    /// </summary>
    internal class LifeForm
    {
        #region CONSTANTS
        
        /// <summary>
        /// Index of neuron that provides speed;
        /// </summary>
        private const int c_outputSpeedNeuron = 0;

        /// <summary>
        /// Index of neuron that provides the angle to rotate.
        /// </summary>
        private const int c_outputRotateNeuron = 1;
        #endregion

        #region Sensors
        /// <summary>
        /// A fully functioning "sensor" that tells the life-form the directions of food with a number of inputs.
        /// From that, the life-form is supposed to rotate and move towards it.
        /// </summary>
        private readonly FoodSensor foodSensor = new();

        /// <summary>
        /// A fully functioning "sensor" that tells the life-form the directions of other life-forms with a number of inputs.
        /// From that, the life-form is supposed to rotate and move away from it.
        /// </summary>
        private readonly LifeSensor lifeSensor = new();

        /// <summary>
        /// A fully functioning "sensor" that tells the life-form the directions of targets (things to avoid or move towards)
        /// with a number of inputs.
        /// From that, the life-form is supposed to either rotate and move towards it or move away from it (depending on game).
        /// </summary>
        private readonly WallOrTargetSensor wallOrTargetSensor = new();
        #endregion

        /// <summary>
        /// Used to uniquely reference an individual life form. 
        /// It's just an ever increasing number assigned to each life-form.
        /// </summary>
        internal int UniqueLifeFormIdentifier;

        /// <summary>
        /// Tracks whether the life form is dead. 
        /// True - dead (doesn't move etc).
        /// False - can move (not dead).
        /// </summary>
        internal bool IsDead
        {
            get;
            private set;
        }

        /// <summary>
        /// Diameter of the life-form.
        /// </summary>
        internal int DiameterOfBlob
        {
            get
            {
                if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.dontStarve)
                    return 4 + AmountOfFoodEaten; // blobs start small and grow as they eat
                else
                    return 10;
            }
        }
      
        /// <summary>
        /// Tracks how much food the life form has eaten, and subsequently used to assign a 
        /// higher fitness for those that ate more.
        /// Applies to: Game.dontStarve
        /// </summary>
        internal int AmountOfFoodEaten = 0;

        /// <summary>
        /// Tracks where the life form is within the play area.
        /// </summary>
        internal PointF Location = new();

        /// <summary>
        /// If the AI is deciding location not rotation/speed it puts it in this.
        /// </summary>
        internal PointF DesiredPosition = new();

        /// <summary>
        /// Indicates direction the lifeform is pointing.
        /// </summary>
        private float AngleInDegrees = 0;

        /// <summary>
        /// Speed the lifeform is moving.
        /// </summary>
        private float speed = 0;

        /// <summary>
        /// Contains a list of positions the life form has visited.
        /// </summary>
        private readonly List<PointF> trail = new();

        /// <summary>
        /// Life forms are multi-colour, this contains the colour
        /// </summary>
        private readonly Color colourToDrawLifeForm = Color.Blue;

        /// <summary>
        /// Each life form keeps a track of the food, which it maintains
        /// deleting when gobbling them.
        /// </summary>
        internal readonly List<PointF> FoodDots = new();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Unique identifier for life-form.</param>
        /// <param name="location">Where the lifeform starts off in the virtual world.</param>
        /// <param name="foodDots">For dontStarve game, contains the positions of food.</param>
        internal LifeForm(int id, Point location, List<PointF> foodDots)
        {
            CreateNeuralNetworkForId(id);

            FoodDots = new(foodDots); // clone the list, don't reference it (othewise multiple lifeforms will update the same!)

            colourToDrawLifeForm = ChooseRandomColourForBlob();
            UniqueLifeFormIdentifier = id;
            Location = location;
            AngleInDegrees = RandomNumberGenerator.GetInt32(0, 359); // point it in a random direction.
            IsDead = false;
        }

        /// <summary>
        /// Creates a neural network with the correct input/outputs.
        /// </summary>
        /// <param name="id">Unique identifier for the network.</param>
        private static void CreateNeuralNetworkForId(int id)
        {
            if (NeuralNetwork.s_networks.ContainsKey(id)) return; // no need, already exists

            int neurons = InputNeuronsRequired();

            // start with an input layer (neuron per sensor)
            List<int> neuronLayers = new()
                {
                    neurons
                };

            // add any hidden layers as specified in config?
            foreach (int i in Config.AIHiddenLayers)
            {
                int hiddenLayer = i;

                if (i == 0) hiddenLayer = neurons; // 0 = same # neurons as input

                neuronLayers.Add(hiddenLayer);
            }

            // add the output neurons
            neuronLayers.Add((Config.MultipleDirectionOutputNeurons ? 5 : 1) + 1); // output neurons

            // create the neural network
            _ = new NeuralNetwork(id, neuronLayers.ToArray(), true);
        }

        /// <summary>
        /// Picks a random colour for the blob, except white.
        /// </summary>
        /// <returns></returns>
        private static Color ChooseRandomColourForBlob()
        {
            // pick a random colour
            Color colourForBlob = Color.FromArgb(180, RandomNumberGenerator.GetInt32(0, 255), RandomNumberGenerator.GetInt32(0, 255), RandomNumberGenerator.GetInt32(0, 255));

            if (colourForBlob.R == 255 && colourForBlob.G == 255 && colourForBlob.B == 255) colourForBlob = Color.Blue;
            
            return colourForBlob;
        }

        /// <summary>
        /// Determine (based on configuration) how many input neurons are required.
        /// </summary>
        /// <returns>The number of neurons required.</returns>
        internal static int InputNeuronsRequired()
        {
            int neurons = 0; // we add all the neurons required for sensors to this.

            // special case: we give the neural network an indicator if it is in the circle
            if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.reachCenter) neurons++;

            // detection of food uses sensors
            if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.dontStarve)
            {
                neurons += (int)(360F / Config.FoodSensorVisionAngleInDegrees);
            }

            // collision detection (with other life-forms) requires a number of sensors
            if (Config.LifeFormCollisionDetectionEnabled) neurons += Config.LifeFormSamplePoints;

            // wall detection or locating targets requires a number of sensors.
            if (GenerationManager.s_targets.Count > 0) neurons += Config.WallOrTargetSamplePoints;

            // if nothing is enabled, we'd end up with a crashing neural network due to 0 inputs.
            if (neurons == 0) neurons = 1; // minimum of "1" but it won't do much

            return neurons;
        }

        /// <summary>
        /// Moves the lifeform.
        /// </summary>
        internal void Move()
        {
            if (IsDead) return; // dead things don't move.

            List<double> sensors = new();

            if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.reachCenter)
            {
                float fitness = Utils.DistanceBetweenTwoPoints(Location, new Point(150, 150));

                sensors.Add(fitness < 40 ? 0 : 1); // in the circle = 0, outside circle = 1
            }

            if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.dontStarve)
            {
                sensors.AddRange(foodSensor.Read(AngleInDegrees, Location, FoodDots, out _)); // detecting food
            }

            // if collision detection is enabled, we need to sense other life-forms
            if (Config.LifeFormCollisionDetectionEnabled) sensors.AddRange(lifeSensor.Read(AngleInDegrees, Location, UniqueLifeFormIdentifier, out _));

            // if we're going to track 1 or more targets, we need a target sensor
            if (GenerationManager.s_targets.Count > 0) sensors.AddRange(wallOrTargetSensor.Read(AngleInDegrees, Location, out _));

            double[] neuralNetworkInput = sensors.ToArray();

            UseAItoMoveLifeForm(neuralNetworkInput);

            // move the blob using basic sin/cos math ->  x = r * cos(theta), y = r * sin(theta)
            // in this instance "r" is the speed output, theta is the angle of the blob.
            double angleLifeFormIsPointingInRadians = Utils.DegreesInRadians(AngleInDegrees);

            PointF locationBeforeMoving = new(Location.X, Location.Y);
            Location.X += (float)Math.Cos(angleLifeFormIsPointingInRadians) * speed;
            Location.Y += (float)Math.Sin(angleLifeFormIsPointingInRadians) * speed;

            // edge walls are fire
            if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.reachCenter)
            {
                if (Location.X < 10 || Location.X > 290 || Location.Y < 10 || Location.Y > 290)
                {
                    IsDead = true;
                    return;
                }
            }

            // box in middle is fire
            if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.dontTouchRed)
            {
                if (Location.X >= 120 && Location.X <= 180 && Location.Y >= 120 && Location.Y <= 180)
                {
                    IsDead = true;
                    return;
                }
            }

            Location.X = Location.X.Clamp(5, 295);
            Location.Y = Location.Y.Clamp(5, 295);

            ApplyCollisionDetection(locationBeforeMoving);

            trail.Add(Location); // store so we can plot the trail

            UpdateFitness();
        }

        /// <summary>
        /// The lifeform may have hit something (food, wall etc), take action.
        /// </summary>
        /// <param name="locationBeforeMoving"></param>
        private void ApplyCollisionDetection(PointF locationBeforeMoving)
        {
            int collideAction = GenerationManager.DetectCollisionWithFoodWallOrLifeform(UniqueLifeFormIdentifier, Location, FoodDots);

            switch (collideAction)
            {
                case 0: // hit nothing of consequence
                    break;

                case 1: // hit object that kills
                    IsDead = true;
                    break;

                case 2: // hit immovable object
                    if (Config.LifeFormCollisionDetectionEnabled) Location = locationBeforeMoving; // put it back to its previous location
                    break;

                case 3: // hit food
                    ++AmountOfFoodEaten;
                    break;
            }
        }

        /// <summary>
        /// Using the inputs provided, ask the neural network to provide speed and direction.
        /// </summary>
        /// <param name="neuralNetworkInput"></param>
        /// <returns></returns>
        private void UseAItoMoveLifeForm(double[] neuralNetworkInput)
        {
            // ask the neural to use the input and decide what to do
            double[] outputFromNeuralNetwork = NeuralNetwork.s_networks[UniqueLifeFormIdentifier].FeedForward(neuralNetworkInput); // process inputs

            // config can use "desired point" or rotate+speed.
            if (!Config.MoveToDesiredPoint)
            {
                // AI decides speed
                speed = (float)outputFromNeuralNetwork[c_outputSpeedNeuron] * Config.SpeedAmplifier;
                speed = speed.Clamp((Scoring.GameCurrentlyBeingPlayed == Scoring.Games.reachCenter ? -3 : 0), 3); // reach center allows backward travel

                // Remember: speed x angle determines where it ends up next
                // we allow the AI to pick a direction, this works better than steering
                if (Config.MultipleDirectionOutputNeurons)
                {
                    MoveBasedOnOneOf8Directions(outputFromNeuralNetwork);
                }
                else
                {
                    AngleInDegrees = (float)outputFromNeuralNetwork[c_outputRotateNeuron] * 360; // 360 = full circle, output of neuron is 0..1
                }
            }
            else
            {
                // ask the AI what to do next? inputs[] => feedforward => outputs[]
                // [0] = x offset from current location 
                // [1] = y offset from current location

                DesiredPosition.X = (float)(Location.X + outputFromNeuralNetwork[0] * 300);
                DesiredPosition.Y = (float)(Location.Y + outputFromNeuralNetwork[1] * 300);

                float angleInDegrees = (float)Utils.RadiansInDegrees((float)Math.Atan2(DesiredPosition.Y - Location.Y, DesiredPosition.X - Location.X));

                float deltaAngle = Math.Abs(angleInDegrees - AngleInDegrees).Clamp(0, 30);

                // quickest way to get from current angle to new angle turning the optimal direction
                float angleInOptimalDirection = (angleInDegrees - AngleInDegrees + 540f) % 360 - 180f;

                // limit max of 30 degrees
                AngleInDegrees = Utils.Clamp360(AngleInDegrees + deltaAngle * Math.Sign(angleInOptimalDirection));

                // close the distance as quickly as possible but without the dog going faster than it should
                speed = Utils.DistanceBetweenTwoPoints(Location, DesiredPosition).Clamp(-2, 2);
            }

            // it'll work even if we violate this, but let's keep it clean 0..359.999 degrees.
            if (AngleInDegrees < 0) AngleInDegrees += 360;
            if (AngleInDegrees >= 360) AngleInDegrees -= 360;
        }

        /// <summary>
        /// With this config, the AI gives us 4 values and we decide a direction from it.
        /// </summary>
        /// <param name="outputFromNeuralNetwork"></param>
        private void MoveBasedOnOneOf8Directions(double[] outputFromNeuralNetwork)
        {
            double x = DeltaBasedOnTwoNNoutputs(outputFromNeuralNetwork[3], outputFromNeuralNetwork[1]);
            double y = DeltaBasedOnTwoNNoutputs(outputFromNeuralNetwork[2], outputFromNeuralNetwork[4]);

            AngleInDegrees = 999; // undecided, not an actual angle

            switch (x)
            {
                case -1:
                    switch (y)
                    {
                        case -1: AngleInDegrees = 135; break;
                        case 0: AngleInDegrees = 180; break;
                        case 1: AngleInDegrees = 0; break;
                    }
                    break;

                case 0:
                    switch (y)
                    {
                        case -1: AngleInDegrees = 90; break;
                        case 0: break;
                        case 1: AngleInDegrees = 270; break;
                    }
                    break;

                case 1:
                    switch (y)
                    {
                        case -1: AngleInDegrees = 225; break;
                        case 0: AngleInDegrees = 270; break;
                        case 1: AngleInDegrees = 315; break;
                    }
                    break;
            }

            // pick a random direction
            if (AngleInDegrees == 999 || Math.Abs(outputFromNeuralNetwork[5]) > 0.9F) AngleInDegrees = RandomNumberGenerator.GetInt32(0, 359);
        }

        /// <summary>
        /// Used to decide which one of the limited positions the cell should go.
        /// -1 if x1<x2
        /// 0 if x1=x2
        /// 1  if x1>x2
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <returns></returns>
        internal static double DeltaBasedOnTwoNNoutputs(double x1, double x2)
        {
            x1 = Math.Round(x1);
            x2 = Math.Round(x2);

            if (x1 == x2) return 0;

            return x1 < x2 ? -1 : 1;
        }

        /// <summary>
        /// Updates the neural network fitness.
        /// </summary>
        internal void UpdateFitness()
        {
            NeuralNetwork.s_networks[UniqueLifeFormIdentifier].Fitness = Scoring.GetFitness(this);
        }

        /// <summary>
        /// Draws the life-form.
        /// </summary>
        /// <param name="g"></param>
        internal void Draw(Graphics g)
        {
            // we draw a trail for all lifeforms, unless the dontStarve game in which case we show it for the selected lifeform.
            if (trail.Count > 1 && (Scoring.GameCurrentlyBeingPlayed != Scoring.Games.dontStarve || Form1.IdOfLifeformBeingMonitored == UniqueLifeFormIdentifier))
            {
                using Pen p = new(Scoring.GameCurrentlyBeingPlayed == Scoring.Games.dontStarve ? Color.FromArgb(120, 255, 0, 0) : Color.FromArgb(120, colourToDrawLifeForm.R, colourToDrawLifeForm.G, colourToDrawLifeForm.B));
                g.DrawLines(p, trail.ToArray());
            }

            int alphaTransparency = 200;

            // for dontStarve when monitoring and this life form is not being monitored they need to be "subtle" so we use an alpha of 40 (barely visible)
            if (Scoring.GameCurrentlyBeingPlayed == Scoring.Games.dontStarve && Form1.IdOfLifeformBeingMonitored != UniqueLifeFormIdentifier)
                alphaTransparency = 40;
            
            using SolidBrush colouredBlobBrush = new(Color.FromArgb(alphaTransparency, colourToDrawLifeForm.R, colourToDrawLifeForm.G, colourToDrawLifeForm.B));

            // the lifeform blob.
            g.FillEllipse(IsDead ? Brushes.Silver : colouredBlobBrush, Location.X - DiameterOfBlob / 2, Location.Y - DiameterOfBlob / 2, DiameterOfBlob, DiameterOfBlob);

            // a red rectangle showing the selected lifeform
            if (Form1.IdOfLifeformBeingMonitored == UniqueLifeFormIdentifier) g.DrawRectangle(Pens.Red, Location.X - DiameterOfBlob / 2, Location.Y - DiameterOfBlob / 2, DiameterOfBlob, DiameterOfBlob);

            if (IsDead) return; // we've drawn the blob, nothing else to draw

            DrawBlobIfPreviouslyHadNonZeroFitness(g);

            DrawScoreLabelIfEnabled(g);

            DrawSensorsIfEnabled(g);
        }

        /// <summary>
        /// Enables you to visually see if this lifeform was a failure (got mutated) or not.
        /// </summary>
        /// <param name="g"></param>
        private void DrawBlobIfPreviouslyHadNonZeroFitness(Graphics g)
        {
            float lastFitness = (NeuralNetwork.s_networks[UniqueLifeFormIdentifier].Fitness / 300F).Clamp(0, 100);

            if (lastFitness < 0) return;

            using Brush f = new SolidBrush(Color.FromArgb((int)lastFitness + 150, 255, 0, 0));
            g.FillEllipse(f, Location.X - 2, Location.Y - 2, 4, 4);
        }

        /// <summary>
        /// If enabled, write the lifeforms score to the right of it.
        /// </summary>
        /// <param name="g"></param>
        private void DrawScoreLabelIfEnabled(Graphics g)
        {
            if (!Config.LabelLifeFormWithScore) return;

            using Font font = new("Arial", 6);

            float score = (float)Math.Round(Scoring.GetFitness(this), 2);
            g.DrawString($"{score}", font, Brushes.Black, new PointF(Location.X + 5, Location.Y - 5));
        }

        /// <summary>
        /// Each sensor is optional. Rather than if x then y, repeatedly in the main draw, we move it out to here.
        /// </summary>
        /// <param name="g"></param>
        private void DrawSensorsIfEnabled(Graphics g)
        {
            // draws all the sensor segments
            if (Config.DrawLifeSensor)
            {
                lifeSensor.DrawFullSweepOfSensor(g, Color.FromArgb(30, 200, 200, 200));
            }

            // draws all the sensor segments
            if (Config.DrawFoodSensor)
            {
                foodSensor.DrawFullSweepOfSensor(g, Color.FromArgb(30, 200, 200, 0));
            }

            // draws all the sensor segments
            if (Config.DrawTargetSensor)
            {
                wallOrTargetSensor.DrawFullSweepOfSensor(g, Color.FromArgb(30, 200, 200, 0));
            }

            // draws the part of the sensor the target is within
            if (Config.DrawTargetPartOfLifeSensor)
            {
                using SolidBrush colorOfTargetLocationSegment = new(Color.FromArgb(40, 255, 0, 0));

                lifeSensor.DrawWhereTargetIsInRespectToSweepOfSensor(g, colorOfTargetLocationSegment);
            }

            // draws the part of the sensor the target is within
            if (Config.DrawTargetPartOfFoodSensor && Scoring.GameCurrentlyBeingPlayed == Scoring.Games.dontStarve && Form1.IdOfLifeformBeingMonitored == UniqueLifeFormIdentifier)
            {
                using SolidBrush colorOfTargetLocationSegment = new(Color.FromArgb(30, 0, 255, 0));

                foodSensor.DrawWhereTargetIsInRespectToSweepOfSensor(g, colorOfTargetLocationSegment);
            }

            // draws the part of the sensor the target is within
            if (Config.DrawTargetPartOfTargetSensor)
            {
                using SolidBrush colorOfTargetLocationSegment = new(Color.FromArgb(30, 0, 0, 255));

                wallOrTargetSensor.DrawWhereTargetIsInRespectToSweepOfSensor(g, colorOfTargetLocationSegment);
            }
        }
    }
}