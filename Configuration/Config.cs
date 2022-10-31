namespace LifeMutation.Configuration
{
    /// <summary>
    /// Configuration.
    /// </summary>
    internal static class Config
    {        
        /// <summary>
        /// Definition of the hidden layers. {} indicates NO hidden layers
        /// </summary>
        internal static int[] AIHiddenLayers = Array.Empty<int>();

        /// <summary>
        /// Speed output of neuron is multiplied by this, enabling speed to be larger than the limited -1/1 TanH output of a neuron.
        /// </summary>
        internal static float SpeedAmplifier = 5;

        /// <summary>
        /// True - if MoveToDesiredPoint is false, then this will use the 2 neurons to decide one of 8 directions.
        /// False - if MoveToDesiredPoint is false, then neurons will be used as delta angle & speed.
        /// </summary>
        internal static bool MultipleDirectionOutputNeurons = false;

        /// <summary>
        /// True - neural network output will be treated as xOffset | yOffset
        /// False - neural network output will be treated as deltaAngle to rotate | speed, or
        /// if MultipleDirectionOutputNeurons is true, it will use the outputs to pick a fixed direction (one of 8).
        /// </summary>
        internal static bool MoveToDesiredPoint = true;

        /// <summary>
        /// True - shows the score as a label next to the lifeform blob.
        /// False - no label is shown.
        /// </summary>
        internal static bool LabelLifeFormWithScore = true;

        #region LIFEFORM SENSOR
        /// <summary>
        /// Defines how far away it detects other lifeforms.
        /// </summary>
        internal static double LifeSensorDepthOfVisionInPixels = 30;

        /// <summary>
        /// How many sensors in an arc/circle are available for detecting lifeforms.
        /// </summary>
        internal static int LifeFormSamplePoints = 8;
        
        /// <summary>
        /// True - collisions between lifeforms are detected (sensor applied).
        /// False - lifeforms go thru each other as if they aren't present.
        /// </summary>
        internal static bool LifeFormCollisionDetectionEnabled = false;

        /// <summary>
        /// True - draws the lifeform sensors for all life-forms.
        /// False - no lifeform sensors drawn.
        /// </summary>
        internal static bool DrawLifeSensor = false;

        /// <summary>
        /// True - shows the triangles where lifeforms were detected.
        /// False - no indication of lifeforms detected is shown. 
        /// </summary>
        internal static bool DrawTargetPartOfLifeSensor = false;
        #endregion

        #region FOOD CONFIGURATION / SENSOR
        /// <summary>
        /// How many food dots are available for the life forms to gobble.
        /// </summary>
        internal static int FoodDotCount = 50;

        /// <summary>
        /// Starting sweep angle of food sensor.
        /// </summary>
        internal static double FoodSensorVisionAngleInDegrees = 11.25F;

        /// <summary>
        /// How far ahead the lifeform can see food. Note that the game is 300px, so 
        /// it is possible for food to be present that it won't chase.
        /// </summary>
        internal static double FoodSensorVisionDepthOfVisionInPixels = 120F;
        
        /// <summary>
        /// True - draws the food sensors for all life-forms (except eat the food game,
        /// for which it draws the sensor for the selected life form).
        /// False - no food sensor drawn.
        /// </summary>
        internal static bool DrawFoodSensor = false;

        /// <summary>
        /// True - shows the triangles where the food sensors detected food for all life-forms 
        /// (except eat the food game, for which it draws the sensor for the selected life form).
        /// False - no indication of food sensor drawn.
        /// </summary>
        internal static bool DrawTargetPartOfFoodSensor = false;
        #endregion

        #region WALL/TARGET SENSOR
        /// <summary>
        /// How many sensors in an arc/circle are available for detecting walls or targets.
        /// </summary>
        internal static int WallOrTargetSamplePoints = 16;

        /// <summary>
        /// How far the sensor detects for the reach centre game.
        /// </summary>
        internal static int ReachCentreWallOrTargetDepthOfVisionInPixels = 220;

        /// <summary>
        /// How far the sensor detects (non reach-centre game).
        /// </summary>
        internal static int OtherWallOrTargetDepthOfVisionInPixels = 20;
        
        /// <summary>
        /// True - draws the wall / target sensors for all life-forms.
        /// False - no wall / target sensors drawn.
        /// </summary>
        internal static bool DrawTargetSensor = false;

        /// <summary>
        /// True - shows the triangles where wall / targets were detected.
        /// False - no indication of wall / targets detected is shown. 
        /// </summary>
        internal static bool DrawTargetPartOfTargetSensor = false;
        #endregion

        /// <summary>
        /// Number of moves lifeforms can make before a forced mutation occurs.
        /// </summary>
        internal static int MovesBeforeMutationtationOccurs = 500;

        /// <summary>
        /// Default number of lifeforms to create.
        /// </summary>
        internal static int NumberOfLifeformsToCreate = 100;
        
        /// <summary>
        /// True - adds the baffles (walls that make it more challenging).
        /// False - no baffles drawn.
        /// </summary>
        internal static bool AddBaffles = false;
    }
}
