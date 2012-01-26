namespace MultiPainter
{
	partial class ColorPicker
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorPicker));
			this.label4 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.numericUpDown7 = new System.Windows.Forms.NumericUpDown();
			this.label8 = new System.Windows.Forms.Label();
			this.saturationSlider1 = new Paril.Controls.Color.SaturationSlider();
			this.colorSquare1 = new Paril.Controls.Color.ColorSquare();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).BeginInit();
			this.SuspendLayout();
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(18, 302);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(31, 13);
			this.label4.TabIndex = 20;
			this.label4.Text = "Color";
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Location = new System.Drawing.Point(5, 256);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(57, 43);
			this.panel1.TabIndex = 19;
			// 
			// numericUpDown3
			// 
			this.numericUpDown3.Location = new System.Drawing.Point(133, 300);
			this.numericUpDown3.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
			this.numericUpDown3.Name = "numericUpDown3";
			this.numericUpDown3.Size = new System.Drawing.Size(41, 20);
			this.numericUpDown3.TabIndex = 17;
			this.numericUpDown3.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
			this.numericUpDown3.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(72, 302);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 13);
			this.label3.TabIndex = 16;
			this.label3.Text = "Lightness";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(72, 280);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 13);
			this.label2.TabIndex = 15;
			this.label2.Text = "Saturation";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(72, 258);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27, 13);
			this.label1.TabIndex = 14;
			this.label1.Text = "Hue";
			// 
			// numericUpDown2
			// 
			this.numericUpDown2.Location = new System.Drawing.Point(133, 278);
			this.numericUpDown2.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
			this.numericUpDown2.Name = "numericUpDown2";
			this.numericUpDown2.Size = new System.Drawing.Size(41, 20);
			this.numericUpDown2.TabIndex = 13;
			this.numericUpDown2.Value = new decimal(new int[] {
            240,
            0,
            0,
            0});
			this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(133, 256);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(41, 20);
			this.numericUpDown1.TabIndex = 12;
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// numericUpDown4
			// 
			this.numericUpDown4.Location = new System.Drawing.Point(223, 322);
			this.numericUpDown4.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown4.Name = "numericUpDown4";
			this.numericUpDown4.Size = new System.Drawing.Size(41, 20);
			this.numericUpDown4.TabIndex = 22;
			this.numericUpDown4.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown4.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(183, 324);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(34, 13);
			this.label5.TabIndex = 21;
			this.label5.Text = "Alpha";
			// 
			// numericUpDown5
			// 
			this.numericUpDown5.Location = new System.Drawing.Point(223, 256);
			this.numericUpDown5.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown5.Name = "numericUpDown5";
			this.numericUpDown5.Size = new System.Drawing.Size(41, 20);
			this.numericUpDown5.TabIndex = 24;
			this.numericUpDown5.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown5.ValueChanged += new System.EventHandler(this.numericUpDown5_ValueChanged);
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(183, 258);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(27, 13);
			this.label6.TabIndex = 23;
			this.label6.Text = "Red";
			// 
			// numericUpDown6
			// 
			this.numericUpDown6.Location = new System.Drawing.Point(223, 278);
			this.numericUpDown6.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown6.Name = "numericUpDown6";
			this.numericUpDown6.Size = new System.Drawing.Size(41, 20);
			this.numericUpDown6.TabIndex = 26;
			this.numericUpDown6.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown6.ValueChanged += new System.EventHandler(this.numericUpDown6_ValueChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(183, 280);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(36, 13);
			this.label7.TabIndex = 25;
			this.label7.Text = "Green";
			// 
			// numericUpDown7
			// 
			this.numericUpDown7.Location = new System.Drawing.Point(223, 300);
			this.numericUpDown7.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown7.Name = "numericUpDown7";
			this.numericUpDown7.Size = new System.Drawing.Size(41, 20);
			this.numericUpDown7.TabIndex = 28;
			this.numericUpDown7.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.numericUpDown7.ValueChanged += new System.EventHandler(this.numericUpDown7_ValueChanged);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(183, 302);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(28, 13);
			this.label8.TabIndex = 27;
			this.label8.Text = "Blue";
			// 
			// saturationSlider1
			// 
			this.saturationSlider1.CurrentLum = 120;
			this.saturationSlider1.Location = new System.Drawing.Point(251, 2);
			this.saturationSlider1.Name = "saturationSlider1";
			this.saturationSlider1.Size = new System.Drawing.Size(21, 256);
			this.saturationSlider1.TabIndex = 18;
			this.saturationSlider1.Text = "saturationSlider1";
			this.saturationSlider1.LumChanged += new System.EventHandler(this.saturationSlider1_LumChanged);
			// 
			// colorSquare1
			// 
			this.colorSquare1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("colorSquare1.BackgroundImage")));
			this.colorSquare1.CurrentHue = 0;
			this.colorSquare1.CurrentSat = 240;
			this.colorSquare1.Location = new System.Drawing.Point(5, 10);
			this.colorSquare1.Name = "colorSquare1";
			this.colorSquare1.Size = new System.Drawing.Size(240, 240);
			this.colorSquare1.TabIndex = 11;
			this.colorSquare1.Text = "colorSquare1";
			this.colorSquare1.HueChanged += new System.EventHandler(this.colorSquare1_HueChanged);
			this.colorSquare1.SatChanged += new System.EventHandler(this.colorSquare1_SatChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(141, 356);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 29;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(60, 356);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 30;
			this.button2.Text = "OK";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// ColorPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(276, 383);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.numericUpDown7);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.numericUpDown6);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.numericUpDown5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.numericUpDown4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.saturationSlider1);
			this.Controls.Add(this.numericUpDown3);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numericUpDown2);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.colorSquare1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ColorPicker";
			this.Load += new System.EventHandler(this.ColorPicker_Load);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel panel1;
		private Paril.Controls.Color.SaturationSlider saturationSlider1;
		private System.Windows.Forms.NumericUpDown numericUpDown3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.NumericUpDown numericUpDown2;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private Paril.Controls.Color.ColorSquare colorSquare1;
		private System.Windows.Forms.NumericUpDown numericUpDown4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown numericUpDown5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numericUpDown6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.NumericUpDown numericUpDown7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
	}
}