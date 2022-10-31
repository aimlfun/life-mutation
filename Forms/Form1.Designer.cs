namespace LifeMutation;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            this.pictureBoxCanvas = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelGeneration = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.timerMove = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.labelFitnessMet = new System.Windows.Forms.Label();
            this.comboBoxGameMode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxShowTargetRadius = new System.Windows.Forms.CheckBox();
            this.numericUpDownLifeForms = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.checkboxIlluminateInTargetRadius = new System.Windows.Forms.CheckBox();
            this.numericUpDownMovesPerGeneration = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.checkboxIlluminateInFoodRadius = new System.Windows.Forms.CheckBox();
            this.checkboxShowFoodRadius = new System.Windows.Forms.CheckBox();
            this.checkboxIlluminateInLifeRadius = new System.Windows.Forms.CheckBox();
            this.checkboxShowLifeRadius = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownFoodCount = new System.Windows.Forms.NumericUpDown();
            this.checkBoxPreventCollision = new System.Windows.Forms.CheckBox();
            this.checkBoxBaffles = new System.Windows.Forms.CheckBox();
            this.pictureBoxNeuralNetworkVisualiser = new System.Windows.Forms.PictureBox();
            this.buttonNeuralNetworkVisualiser = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanvas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLifeForms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMovesPerGeneration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFoodCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNeuralNetworkVisualiser)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxCanvas
            // 
            this.pictureBoxCanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxCanvas.Location = new System.Drawing.Point(13, 28);
            this.pictureBoxCanvas.Name = "pictureBoxCanvas";
            this.pictureBoxCanvas.Size = new System.Drawing.Size(300, 300);
            this.pictureBoxCanvas.TabIndex = 0;
            this.pictureBoxCanvas.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Generation:";
            // 
            // labelGeneration
            // 
            this.labelGeneration.AutoSize = true;
            this.labelGeneration.Location = new System.Drawing.Point(92, 9);
            this.labelGeneration.Name = "labelGeneration";
            this.labelGeneration.Size = new System.Drawing.Size(13, 15);
            this.labelGeneration.TabIndex = 2;
            this.labelGeneration.Text = "1";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(505, 212);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.ButtonStart_Click);
            // 
            // timerMove
            // 
            this.timerMove.Interval = 5;
            this.timerMove.Tick += new System.EventHandler(this.TimerMove_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(193, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "# Fitness > 0 :";
            // 
            // labelFitnessMet
            // 
            this.labelFitnessMet.AutoSize = true;
            this.labelFitnessMet.Location = new System.Drawing.Point(300, 9);
            this.labelFitnessMet.Name = "labelFitnessMet";
            this.labelFitnessMet.Size = new System.Drawing.Size(13, 15);
            this.labelFitnessMet.TabIndex = 5;
            this.labelFitnessMet.Text = "0";
            // 
            // comboBoxGameMode
            // 
            this.comboBoxGameMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGameMode.FormattingEnabled = true;
            this.comboBoxGameMode.Items.AddRange(new object[] {
            "Move to LEFT",
            "Move to RIGHT",
            "Move to CENTER",
            "Move to CORNERS",
            "Don\'t touch RED",
            "Driven by HUNGER"});
            this.comboBoxGameMode.Location = new System.Drawing.Point(329, 28);
            this.comboBoxGameMode.Name = "comboBoxGameMode";
            this.comboBoxGameMode.Size = new System.Drawing.Size(250, 23);
            this.comboBoxGameMode.TabIndex = 0;
            this.comboBoxGameMode.SelectedIndexChanged += new System.EventHandler(this.ComboBoxGameMode_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.Location = new System.Drawing.Point(326, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Configuration";
            // 
            // checkBoxShowTargetRadius
            // 
            this.checkBoxShowTargetRadius.AutoSize = true;
            this.checkBoxShowTargetRadius.Location = new System.Drawing.Point(338, 253);
            this.checkBoxShowTargetRadius.Name = "checkBoxShowTargetRadius";
            this.checkBoxShowTargetRadius.Size = new System.Drawing.Size(124, 19);
            this.checkBoxShowTargetRadius.TabIndex = 5;
            this.checkBoxShowTargetRadius.Text = "Show target radius";
            this.checkBoxShowTargetRadius.UseVisualStyleBackColor = true;
            this.checkBoxShowTargetRadius.CheckedChanged += new System.EventHandler(this.CheckBoxShowTargetRadius_CheckedChanged);
            // 
            // numericUpDownLifeForms
            // 
            this.numericUpDownLifeForms.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownLifeForms.Location = new System.Drawing.Point(458, 57);
            this.numericUpDownLifeForms.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLifeForms.Name = "numericUpDownLifeForms";
            this.numericUpDownLifeForms.Size = new System.Drawing.Size(120, 23);
            this.numericUpDownLifeForms.TabIndex = 1;
            this.numericUpDownLifeForms.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(329, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "Number of life forms:";
            // 
            // checkboxIlluminateInTargetRadius
            // 
            this.checkboxIlluminateInTargetRadius.AutoSize = true;
            this.checkboxIlluminateInTargetRadius.Location = new System.Drawing.Point(471, 253);
            this.checkboxIlluminateInTargetRadius.Name = "checkboxIlluminateInTargetRadius";
            this.checkboxIlluminateInTargetRadius.Size = new System.Drawing.Size(79, 19);
            this.checkboxIlluminateInTargetRadius.TabIndex = 6;
            this.checkboxIlluminateInTargetRadius.Text = "Illuminate";
            this.checkboxIlluminateInTargetRadius.UseVisualStyleBackColor = true;
            this.checkboxIlluminateInTargetRadius.CheckedChanged += new System.EventHandler(this.CheckboxIlluminateInTargetRadius_CheckedChanged);
            // 
            // numericUpDownMovesPerGeneration
            // 
            this.numericUpDownMovesPerGeneration.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownMovesPerGeneration.Location = new System.Drawing.Point(458, 140);
            this.numericUpDownMovesPerGeneration.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownMovesPerGeneration.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDownMovesPerGeneration.Name = "numericUpDownMovesPerGeneration";
            this.numericUpDownMovesPerGeneration.Size = new System.Drawing.Size(120, 23);
            this.numericUpDownMovesPerGeneration.TabIndex = 2;
            this.numericUpDownMovesPerGeneration.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(328, 144);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(125, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Moves per generation:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(11, 343);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(385, 15);
            this.label6.TabIndex = 15;
            this.label6.Text = "\"P\" Pause | \"S\" Slow | \"Q\" Quiet learn | \"M\" Mutate now | \"L\" Labels";
            // 
            // checkboxIlluminateInFoodRadius
            // 
            this.checkboxIlluminateInFoodRadius.AutoSize = true;
            this.checkboxIlluminateInFoodRadius.Location = new System.Drawing.Point(471, 283);
            this.checkboxIlluminateInFoodRadius.Name = "checkboxIlluminateInFoodRadius";
            this.checkboxIlluminateInFoodRadius.Size = new System.Drawing.Size(79, 19);
            this.checkboxIlluminateInFoodRadius.TabIndex = 8;
            this.checkboxIlluminateInFoodRadius.Text = "Illuminate";
            this.checkboxIlluminateInFoodRadius.UseVisualStyleBackColor = true;
            this.checkboxIlluminateInFoodRadius.CheckedChanged += new System.EventHandler(this.CheckboxIlluminateInFoodRadius_CheckedChanged);
            // 
            // checkboxShowFoodRadius
            // 
            this.checkboxShowFoodRadius.AutoSize = true;
            this.checkboxShowFoodRadius.Location = new System.Drawing.Point(338, 283);
            this.checkboxShowFoodRadius.Name = "checkboxShowFoodRadius";
            this.checkboxShowFoodRadius.Size = new System.Drawing.Size(118, 19);
            this.checkboxShowFoodRadius.TabIndex = 7;
            this.checkboxShowFoodRadius.Text = "Show food radius";
            this.checkboxShowFoodRadius.UseVisualStyleBackColor = true;
            this.checkboxShowFoodRadius.CheckedChanged += new System.EventHandler(this.CheckboxShowFoodRadius_CheckedChanged);
            // 
            // checkboxIlluminateInLifeRadius
            // 
            this.checkboxIlluminateInLifeRadius.AutoSize = true;
            this.checkboxIlluminateInLifeRadius.Location = new System.Drawing.Point(471, 313);
            this.checkboxIlluminateInLifeRadius.Name = "checkboxIlluminateInLifeRadius";
            this.checkboxIlluminateInLifeRadius.Size = new System.Drawing.Size(79, 19);
            this.checkboxIlluminateInLifeRadius.TabIndex = 10;
            this.checkboxIlluminateInLifeRadius.Text = "Illuminate";
            this.checkboxIlluminateInLifeRadius.UseVisualStyleBackColor = true;
            this.checkboxIlluminateInLifeRadius.CheckedChanged += new System.EventHandler(this.CheckboxIlluminateInLifeRadius_CheckedChanged);
            // 
            // checkboxShowLifeRadius
            // 
            this.checkboxShowLifeRadius.AutoSize = true;
            this.checkboxShowLifeRadius.Location = new System.Drawing.Point(338, 313);
            this.checkboxShowLifeRadius.Name = "checkboxShowLifeRadius";
            this.checkboxShowLifeRadius.Size = new System.Drawing.Size(109, 19);
            this.checkboxShowLifeRadius.TabIndex = 9;
            this.checkboxShowLifeRadius.Text = "Show life radius";
            this.checkboxShowLifeRadius.UseVisualStyleBackColor = true;
            this.checkboxShowLifeRadius.CheckedChanged += new System.EventHandler(this.CheckboxShowLifeRadius_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(328, 181);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(96, 15);
            this.label7.TabIndex = 21;
            this.label7.Text = "Amount of food:";
            // 
            // numericUpDownFoodCount
            // 
            this.numericUpDownFoodCount.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownFoodCount.Location = new System.Drawing.Point(458, 176);
            this.numericUpDownFoodCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownFoodCount.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDownFoodCount.Name = "numericUpDownFoodCount";
            this.numericUpDownFoodCount.Size = new System.Drawing.Size(120, 23);
            this.numericUpDownFoodCount.TabIndex = 3;
            this.numericUpDownFoodCount.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // checkBoxPreventCollision
            // 
            this.checkBoxPreventCollision.AutoSize = true;
            this.checkBoxPreventCollision.Location = new System.Drawing.Point(332, 87);
            this.checkBoxPreventCollision.Name = "checkBoxPreventCollision";
            this.checkBoxPreventCollision.Size = new System.Drawing.Size(118, 19);
            this.checkBoxPreventCollision.TabIndex = 22;
            this.checkBoxPreventCollision.Text = "Prevent collisions";
            this.checkBoxPreventCollision.UseVisualStyleBackColor = true;
            // 
            // checkBoxBaffles
            // 
            this.checkBoxBaffles.AutoSize = true;
            this.checkBoxBaffles.Location = new System.Drawing.Point(332, 112);
            this.checkBoxBaffles.Name = "checkBoxBaffles";
            this.checkBoxBaffles.Size = new System.Drawing.Size(86, 19);
            this.checkBoxBaffles.TabIndex = 23;
            this.checkBoxBaffles.Text = "Add Baffles";
            this.checkBoxBaffles.UseVisualStyleBackColor = true;
            // 
            // pictureBoxNeuralNetworkVisualiser
            // 
            this.pictureBoxNeuralNetworkVisualiser.Location = new System.Drawing.Point(13, 377);
            this.pictureBoxNeuralNetworkVisualiser.Name = "pictureBoxNeuralNetworkVisualiser";
            this.pictureBoxNeuralNetworkVisualiser.Size = new System.Drawing.Size(566, 212);
            this.pictureBoxNeuralNetworkVisualiser.TabIndex = 24;
            this.pictureBoxNeuralNetworkVisualiser.TabStop = false;
            // 
            // buttonNeuralNetworkVisualiser
            // 
            this.buttonNeuralNetworkVisualiser.Location = new System.Drawing.Point(329, 212);
            this.buttonNeuralNetworkVisualiser.Name = "buttonNeuralNetworkVisualiser";
            this.buttonNeuralNetworkVisualiser.Size = new System.Drawing.Size(75, 23);
            this.buttonNeuralNetworkVisualiser.TabIndex = 25;
            this.buttonNeuralNetworkVisualiser.Text = "Neural Net.";
            this.buttonNeuralNetworkVisualiser.UseVisualStyleBackColor = true;
            this.buttonNeuralNetworkVisualiser.Click += new System.EventHandler(this.ButtonNeuralNetworkVisualiser_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 372);
            this.Controls.Add(this.buttonNeuralNetworkVisualiser);
            this.Controls.Add(this.pictureBoxNeuralNetworkVisualiser);
            this.Controls.Add(this.checkBoxBaffles);
            this.Controls.Add(this.checkBoxPreventCollision);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.numericUpDownFoodCount);
            this.Controls.Add(this.checkboxIlluminateInLifeRadius);
            this.Controls.Add(this.checkboxShowLifeRadius);
            this.Controls.Add(this.checkboxIlluminateInFoodRadius);
            this.Controls.Add(this.checkboxShowFoodRadius);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDownMovesPerGeneration);
            this.Controls.Add(this.checkboxIlluminateInTargetRadius);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownLifeForms);
            this.Controls.Add(this.checkBoxShowTargetRadius);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxGameMode);
            this.Controls.Add(this.labelFitnessMet);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.labelGeneration);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBoxCanvas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Life Mutation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanvas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLifeForms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMovesPerGeneration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFoodCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNeuralNetworkVisualiser)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private PictureBox pictureBoxCanvas;
    private Label label1;
    private Label labelGeneration;
    private Button buttonStart;
    private System.Windows.Forms.Timer timerMove;
    private Label label2;
    private Label labelFitnessMet;
    private ComboBox comboBoxGameMode;
    private Label label3;
    private CheckBox checkBoxShowTargetRadius;
    private NumericUpDown numericUpDownLifeForms;
    private Label label4;
    private CheckBox checkboxIlluminateInTargetRadius;
    private NumericUpDown numericUpDownMovesPerGeneration;
    private Label label5;
    private Label label6;
    private CheckBox checkboxIlluminateInFoodRadius;
    private CheckBox checkboxShowFoodRadius;
    private CheckBox checkboxIlluminateInLifeRadius;
    private CheckBox checkboxShowLifeRadius;
    private Label label7;
    private NumericUpDown numericUpDownFoodCount;
    private CheckBox checkBoxPreventCollision;
    private CheckBox checkBoxBaffles;
    private PictureBox pictureBoxNeuralNetworkVisualiser;
    private Button buttonNeuralNetworkVisualiser;
}