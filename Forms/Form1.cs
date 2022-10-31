using LifeMutation.AI;
using LifeMutation.Configuration;
using LifeMutation.LifeFormAndManager;
using LifeMutation.Utilities;

namespace LifeMutation;

public partial class Form1 : Form
{
    /// <summary>
    /// True - AI learns quietly without drawing anything and therefore much quicker.
    /// </summary>
    private bool inQuietLearningMode = false;

    /// <summary>
    /// If a user clicks on a life form, this is set with the lifeforms' id.
    /// </summary>
    internal static int IdOfLifeformBeingMonitored = -1;

    /// <summary>
    /// Indicates whether the neural network visualiser is on screen or not.
    /// </summary>
    private bool isIsNeuralNetworkVisualiserShown = false;
    
    /// <summary>
    /// Setter/Getter returns whether the neural network visualiser is shown, but also handles
    /// when it changes state by adjusting the screen size.
    /// </summary>
    private bool NeuralNetworkVisualiserShown
    {
        get
        {
            return isIsNeuralNetworkVisualiserShown;
        }
        set
        {
            if (isIsNeuralNetworkVisualiserShown == value) return;

            isIsNeuralNetworkVisualiserShown = value;

            Height = value ? 641 : 411; // make room to show it
        }
    }


    /// <summary>
    /// Constructor.
    /// </summary>
    public Form1()
    {
        InitializeComponent();

        comboBoxGameMode.SelectedIndex = 0;
        checkBoxShowTargetRadius.Checked = Config.DrawTargetSensor;
        checkboxIlluminateInTargetRadius.Checked = Config.DrawTargetPartOfTargetSensor;
        checkboxShowFoodRadius.Checked = Config.DrawFoodSensor;
        checkboxIlluminateInFoodRadius.Checked = Config.DrawTargetPartOfFoodSensor;
        checkboxIlluminateInLifeRadius.Checked = Config.DrawTargetPartOfLifeSensor;
        checkboxShowLifeRadius.Checked = Config.DrawLifeSensor;
        checkBoxBaffles.Checked = Config.AddBaffles;

        pictureBoxCanvas.MouseClick += PictureBoxCanvas_MouseClick;
    }

    /// <summary>
    /// Timer tick moves the lifeforms.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TimerMove_Tick(object sender, EventArgs e)
    {
        // user put it in quiet mode? If so we run in a closed high performant loop (move without paint)
        if (inQuietLearningMode)
        {
            RunInQuietMode();
            return;
        }

        // in visual mode, move and if none left move to next generation
        if (!GenerationManager.Move())
        {
            timerMove.Stop();
            NextGeneration();
            timerMove.Start();

            return;
        }

        // displays the neural in the visualiser if turned on.
        Visualise(IdOfLifeformBeingMonitored);

        // paint (non quiet mode)
        GenerationManager.Draw();
    }

    /// <summary>
    /// Shows the "brains" (neural network) for chosen lifeform.
    /// </summary>
    /// <param name="id"></param>
    private void Visualise(int id)
    {
        if (!NeuralNetworkVisualiserShown || IdOfLifeformBeingMonitored < 0) return;

        // visualiser draws to an image and puts it in the visualiser
        pictureBoxNeuralNetworkVisualiser.Image?.Dispose();
        NeuralNetworkVisualiser.Render(NeuralNetwork.s_networks[id], pictureBoxNeuralNetworkVisualiser);

        pictureBoxNeuralNetworkVisualiser.Visible = true;
    }

    /// <summary>
    /// In quiet mode it whizzes thru generations by avoiding the overhead of drawing on screen.
    /// </summary>
    private void RunInQuietMode()
    {
        timerMove.Stop(); // this is done outside a timer.

        while (inQuietLearningMode)
        {
            while (GenerationManager.Move()) ;

            // move returns false if food runs out (dontStarve game) or all lifeforms died.

            NextGeneration();
        }

        timerMove.Start(); // reinstate timer
    }

    /// <summary>
    /// Decide which are the best, and mutate out the "failures".
    /// </summary>
    private void NextGeneration()
    {
        GenerationManager.Mutate();
        GenerationManager.NewGeneration();

        labelGeneration.Text = GenerationManager.s_generation.ToString();

        int cnt = 0;

        foreach (NeuralNetwork n in NeuralNetwork.s_networks.Values)
        {
            if (n.Fitness > 0) ++cnt;
        }

        // show how many had a non zero fitness in the last generation.
        labelFitnessMet.Text = cnt.ToString();

        Application.DoEvents();
    }

    /// <summary>
    /// User can press keys that impact the games, pause/slow, mutate etc.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.P:
                // "P" pauses the timer (and what's happening)
                timerMove.Enabled = !timerMove.Enabled;
                
                if (timerMove.Enabled)
                {
                    timerMove.Start();
                }
                else
                {
                    timerMove.Stop();
                }
                break;

            case Keys.L:
                // "L" labels mode
                Config.LabelLifeFormWithScore = !Config.LabelLifeFormWithScore;
                break;

            case Keys.S:
                // "S" slow mode
                StepThroughSpeeds();
                break;

            case Keys.Q:
                // "Q" quiet learn mode
                inQuietLearningMode = !inQuietLearningMode;
                break;

            case Keys.M:
                // mutate
                NextGeneration();
                break;
        }
    }

    /// <summary>
    /// User clicked on the play area. Work out which living lifeform is closest, and start monitoring it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PictureBoxCanvas_MouseClick(object? sender, MouseEventArgs e)
    {
        IdOfLifeformBeingMonitored = -1;

        float minDist = int.MaxValue;

        foreach (int id in GenerationManager.s_lifeForms.Keys)
        {
            LifeForm lifeform = GenerationManager.s_lifeForms[id];
            
            if (lifeform.IsDead) continue; // no point monitoring dead ones

            float distBetweenClickedLocationAndLifeForm = Utils.DistanceBetweenTwoPoints(e.Location, lifeform.Location);

            // is this one closer?
            if (distBetweenClickedLocationAndLifeForm < minDist)
            {
                minDist = distBetweenClickedLocationAndLifeForm;
                IdOfLifeformBeingMonitored = id;
            }
        }
    }

    /// <summary>
    /// Pressing "S" steps through animation speeds: 2x slower, 5x slower, 10x slower, 20x slower then back to normal speed.
    /// </summary>
    private void StepThroughSpeeds()
    {
        var newInterval = timerMove.Interval switch
        {
            5 => 10,
            10 => 25,
            25 => 50,
            50 => 100,
            _ => 10,
        };

        timerMove.Interval = newInterval;
    }

    /// <summary>
    /// Form is in the process of closing, we won't close down if in quiet learning mode, so we intercept it and stop learning
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        inQuietLearningMode = false;
    }

    /// <summary>
    /// User clicked [Start]. Use the values provided, and start the AI.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonStart_Click(object sender, EventArgs e)
    {
        buttonStart.Enabled = false; // stop double-clicking

        GenerationManager.s_generation = 1;
        labelGeneration.Text = GenerationManager.s_generation.ToString();

        // stop timer, because we're changing things
        inQuietLearningMode = false;
        timerMove.Stop();
        Application.DoEvents();
        Application.DoEvents();

        // new brains, as we're changing the config
        NeuralNetwork.s_networks.Clear();

        // update the config
        Scoring.GameCurrentlyBeingPlayed = GameFrom(comboBoxGameMode.Items[comboBoxGameMode.SelectedIndex].ToString());
        Config.NumberOfLifeformsToCreate = (int)numericUpDownLifeForms.Value;
        Config.MovesBeforeMutationtationOccurs = (int)numericUpDownMovesPerGeneration.Value;
        Config.FoodDotCount = (int)numericUpDownFoodCount.Value;
        Config.LifeFormCollisionDetectionEnabled = checkBoxPreventCollision.Checked;
        Config.AddBaffles = checkBoxBaffles.Checked;
        
        // these settings are optimal for their respective games
        switch (Scoring.GameCurrentlyBeingPlayed)
        {
            case Scoring.Games.dontStarve:
                Config.MoveToDesiredPoint = true;
                Config.MultipleDirectionOutputNeurons = false;
                break;

            default:
                Config.MoveToDesiredPoint = false;
                break;
        }
        
        IdOfLifeformBeingMonitored = -1;

        // create new generations
        GenerationManager.CreateGenerationManager(pictureBoxCanvas);

        // start the game (a timer moves the life forms)
        timerMove.Enabled = true;
        timerMove.Start();

        buttonStart.Enabled = true;
    }

    /// <summary>
    /// Convert the drop down value to an enum.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private static Scoring.Games GameFrom(string? mode)
    {
        return mode switch
        {
            "Move to LEFT" => Scoring.Games.reachLeftSide,
            "Move to RIGHT" => Scoring.Games.reachRightSide,
            "Move to CENTER" => Scoring.Games.reachCenter,
            "Move to CORNERS" => Scoring.Games.reachCorner,
            "Driven by HUNGER" => Scoring.Games.dontStarve,
            "Don't touch RED" => Scoring.Games.dontTouchRed,
            _ => throw new ArgumentOutOfRangeException(nameof(mode)),
        };
    }

    #region Transfer checkbox values to config
    private void CheckBoxShowTargetRadius_CheckedChanged(object sender, EventArgs e)
    {
        Config.DrawTargetSensor = checkBoxShowTargetRadius.Checked;
    }

    private void CheckboxIlluminateInTargetRadius_CheckedChanged(object sender, EventArgs e)
    {
        Config.DrawTargetPartOfTargetSensor = checkboxIlluminateInTargetRadius.Checked;
    }

    private void CheckboxShowFoodRadius_CheckedChanged(object sender, EventArgs e)
    {
        Config.DrawFoodSensor = checkboxShowFoodRadius.Checked;
    }

    private void CheckboxIlluminateInFoodRadius_CheckedChanged(object sender, EventArgs e)
    {
        Config.DrawTargetPartOfFoodSensor = checkboxIlluminateInFoodRadius.Checked;
    }

    private void CheckboxIlluminateInLifeRadius_CheckedChanged(object sender, EventArgs e)
    {
        Config.DrawTargetPartOfLifeSensor = checkboxIlluminateInLifeRadius.Checked;
    }

    private void CheckboxShowLifeRadius_CheckedChanged(object sender, EventArgs e)
    {
        Config.DrawLifeSensor = checkboxShowLifeRadius.Checked;
    }
    #endregion

    /// <summary>
    /// User clicked [Neural net.] button, so we toggle display of it.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonNeuralNetworkVisualiser_Click(object sender, EventArgs e)
    {
        NeuralNetworkVisualiserShown = !NeuralNetworkVisualiserShown;        
    }

    /// <summary>
    /// Sets some useful defaults.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ComboBoxGameMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (comboBoxGameMode.SelectedIndex < 0) return;

        Scoring.GameCurrentlyBeingPlayed = GameFrom(comboBoxGameMode.Items[comboBoxGameMode.SelectedIndex].ToString());

        switch (Scoring.GameCurrentlyBeingPlayed)
        {
            case Scoring.Games.dontStarve:
                numericUpDownLifeForms.Value = 50;
                numericUpDownFoodCount.Value = 50;
                numericUpDownMovesPerGeneration.Value = 1000;
                break;

            case Scoring.Games.reachCenter:
                numericUpDownLifeForms.Value = 50;
                numericUpDownFoodCount.Value = 50;
                numericUpDownMovesPerGeneration.Value = 500;
                break;

            default:
                Config.MoveToDesiredPoint = false;
                break;
        }

    }
}