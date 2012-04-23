namespace MCSkin3D.Controls
{
	partial class ColorPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.colorTabControl = new System.Windows.Forms.TabControl();
			this.rgbTabPage = new System.Windows.Forms.TabPage();
			this.panel2 = new System.Windows.Forms.Panel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.redNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.greenNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.alphaNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.blueNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.hsvTabPage = new System.Windows.Forms.TabPage();
			this.panel3 = new System.Windows.Forms.Panel();
			this.hueNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label6 = new System.Windows.Forms.Label();
			this.saturationNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.valueNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.swatchTabPage = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.loadingSwatchLabel = new System.Windows.Forms.Label();
			this.alphaColorSlider = new MB.Controls.ColorSlider();
			this.colorPick1 = new MCSkin3D.lemon42.ColorPick();
			this.colorPreview1 = new Paril.Controls.Color.ColorPreview();
			this.colorPreview2 = new Paril.Controls.Color.ColorPreview();
			this.redColorSlider = new MB.Controls.ColorSlider();
			this.blueColorSlider = new MB.Controls.ColorSlider();
			this.greenColorSlider = new MB.Controls.ColorSlider();
			this.hueColorSlider = new MB.Controls.ColorSlider();
			this.valueColorSlider = new MB.Controls.ColorSlider();
			this.saturationColorSlider = new MB.Controls.ColorSlider();
			this.swatchContainer = new MCSkin3D.SwatchContainer();
			this.languageProvider1 = new MCSkin3D.Language.LanguageProvider();
			this.colorTabControl.SuspendLayout();
			this.rgbTabPage.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.redNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.greenNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.alphaNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.blueNumericUpDown)).BeginInit();
			this.hsvTabPage.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.hueNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.saturationNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.valueNumericUpDown)).BeginInit();
			this.swatchTabPage.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// colorTabControl
			// 
			this.colorTabControl.Controls.Add(this.rgbTabPage);
			this.colorTabControl.Controls.Add(this.hsvTabPage);
			this.colorTabControl.Controls.Add(this.swatchTabPage);
			this.colorTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.colorTabControl.Location = new System.Drawing.Point(0, 0);
			this.colorTabControl.Name = "colorTabControl";
			this.colorTabControl.SelectedIndex = 0;
			this.colorTabControl.Size = new System.Drawing.Size(303, 277);
			this.colorTabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
			this.colorTabControl.TabIndex = 2;
			this.colorTabControl.SelectedIndexChanged += new System.EventHandler(this.colorTabControl_SelectedIndexChanged);
			// 
			// rgbTabPage
			// 
			this.rgbTabPage.Controls.Add(this.panel2);
			this.rgbTabPage.Location = new System.Drawing.Point(4, 22);
			this.rgbTabPage.Margin = new System.Windows.Forms.Padding(0);
			this.rgbTabPage.Name = "rgbTabPage";
			this.rgbTabPage.Size = new System.Drawing.Size(295, 251);
			this.rgbTabPage.TabIndex = 1;
			this.rgbTabPage.Text = "RGBA";
			this.rgbTabPage.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.alphaColorSlider);
			this.panel2.Controls.Add(this.colorPick1);
			this.panel2.Controls.Add(this.colorPreview1);
			this.panel2.Controls.Add(this.colorPreview2);
			this.panel2.Controls.Add(this.textBox1);
			this.panel2.Controls.Add(this.redColorSlider);
			this.panel2.Controls.Add(this.label9);
			this.panel2.Controls.Add(this.blueColorSlider);
			this.panel2.Controls.Add(this.redNumericUpDown);
			this.panel2.Controls.Add(this.greenColorSlider);
			this.panel2.Controls.Add(this.label2);
			this.panel2.Controls.Add(this.greenNumericUpDown);
			this.panel2.Controls.Add(this.alphaNumericUpDown);
			this.panel2.Controls.Add(this.label3);
			this.panel2.Controls.Add(this.label5);
			this.panel2.Controls.Add(this.blueNumericUpDown);
			this.panel2.Controls.Add(this.label4);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Margin = new System.Windows.Forms.Padding(0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(295, 251);
			this.panel2.TabIndex = 1;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(230, 141);
			this.textBox1.MaxLength = 9;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(58, 20);
			this.textBox1.TabIndex = 19;
			this.textBox1.Text = "FFFFFFFF";
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(214, 144);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(14, 13);
			this.label9.TabIndex = 18;
			this.label9.Text = "#";
			// 
			// redNumericUpDown
			// 
			this.redNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.redNumericUpDown.Location = new System.Drawing.Point(248, 164);
			this.redNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.redNumericUpDown.Name = "redNumericUpDown";
			this.redNumericUpDown.Size = new System.Drawing.Size(40, 20);
			this.redNumericUpDown.TabIndex = 1;
			this.redNumericUpDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.redNumericUpDown.ValueChanged += new System.EventHandler(this.redNumericUpDown_ValueChanged);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 166);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(15, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "R";
			// 
			// greenNumericUpDown
			// 
			this.greenNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.greenNumericUpDown.Location = new System.Drawing.Point(248, 184);
			this.greenNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.greenNumericUpDown.Name = "greenNumericUpDown";
			this.greenNumericUpDown.Size = new System.Drawing.Size(40, 20);
			this.greenNumericUpDown.TabIndex = 3;
			this.greenNumericUpDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.greenNumericUpDown.ValueChanged += new System.EventHandler(this.greenNumericUpDown_ValueChanged);
			// 
			// alphaNumericUpDown
			// 
			this.alphaNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.alphaNumericUpDown.Location = new System.Drawing.Point(248, 224);
			this.alphaNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.alphaNumericUpDown.Name = "alphaNumericUpDown";
			this.alphaNumericUpDown.Size = new System.Drawing.Size(40, 20);
			this.alphaNumericUpDown.TabIndex = 7;
			this.alphaNumericUpDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.alphaNumericUpDown.ValueChanged += new System.EventHandler(this.alphaNumericUpDown_ValueChanged);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 186);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(15, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "G";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 226);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(14, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "A";
			// 
			// blueNumericUpDown
			// 
			this.blueNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.blueNumericUpDown.Location = new System.Drawing.Point(248, 204);
			this.blueNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.blueNumericUpDown.Name = "blueNumericUpDown";
			this.blueNumericUpDown.Size = new System.Drawing.Size(40, 20);
			this.blueNumericUpDown.TabIndex = 5;
			this.blueNumericUpDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.blueNumericUpDown.ValueChanged += new System.EventHandler(this.blueNumericUpDown_ValueChanged);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 206);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(14, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "B";
			// 
			// hsvTabPage
			// 
			this.hsvTabPage.Controls.Add(this.panel3);
			this.hsvTabPage.Location = new System.Drawing.Point(4, 22);
			this.hsvTabPage.Margin = new System.Windows.Forms.Padding(0);
			this.hsvTabPage.Name = "hsvTabPage";
			this.hsvTabPage.Size = new System.Drawing.Size(295, 251);
			this.hsvTabPage.TabIndex = 2;
			this.hsvTabPage.Text = "HSVA";
			this.hsvTabPage.UseVisualStyleBackColor = true;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.hueNumericUpDown);
			this.panel3.Controls.Add(this.hueColorSlider);
			this.panel3.Controls.Add(this.label6);
			this.panel3.Controls.Add(this.valueColorSlider);
			this.panel3.Controls.Add(this.saturationNumericUpDown);
			this.panel3.Controls.Add(this.saturationColorSlider);
			this.panel3.Controls.Add(this.label7);
			this.panel3.Controls.Add(this.label8);
			this.panel3.Controls.Add(this.valueNumericUpDown);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 0);
			this.panel3.Margin = new System.Windows.Forms.Padding(0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(295, 251);
			this.panel3.TabIndex = 27;
			// 
			// hueNumericUpDown
			// 
			this.hueNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.hueNumericUpDown.Location = new System.Drawing.Point(248, 164);
			this.hueNumericUpDown.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
			this.hueNumericUpDown.Name = "hueNumericUpDown";
			this.hueNumericUpDown.Size = new System.Drawing.Size(40, 20);
			this.hueNumericUpDown.TabIndex = 16;
			this.hueNumericUpDown.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.hueNumericUpDown.ValueChanged += new System.EventHandler(this.hueNumericUpDown_ValueChanged);
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(3, 166);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(15, 13);
			this.label6.TabIndex = 17;
			this.label6.Text = "H";
			// 
			// saturationNumericUpDown
			// 
			this.saturationNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.saturationNumericUpDown.Location = new System.Drawing.Point(248, 184);
			this.saturationNumericUpDown.Name = "saturationNumericUpDown";
			this.saturationNumericUpDown.Size = new System.Drawing.Size(40, 20);
			this.saturationNumericUpDown.TabIndex = 18;
			this.saturationNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.saturationNumericUpDown.ValueChanged += new System.EventHandler(this.saturationNumericUpDown_ValueChanged);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(3, 186);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(14, 13);
			this.label7.TabIndex = 19;
			this.label7.Text = "S";
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(3, 206);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(14, 13);
			this.label8.TabIndex = 21;
			this.label8.Text = "V";
			// 
			// valueNumericUpDown
			// 
			this.valueNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.valueNumericUpDown.Location = new System.Drawing.Point(248, 204);
			this.valueNumericUpDown.Name = "valueNumericUpDown";
			this.valueNumericUpDown.Size = new System.Drawing.Size(40, 20);
			this.valueNumericUpDown.TabIndex = 20;
			this.valueNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.valueNumericUpDown.ValueChanged += new System.EventHandler(this.valueNumericUpDown_ValueChanged);
			// 
			// swatchTabPage
			// 
			this.swatchTabPage.Controls.Add(this.panel1);
			this.swatchTabPage.Location = new System.Drawing.Point(4, 22);
			this.swatchTabPage.Margin = new System.Windows.Forms.Padding(0);
			this.swatchTabPage.Name = "swatchTabPage";
			this.languageProvider1.SetPropertyNames(this.swatchTabPage, "Text");
			this.swatchTabPage.Size = new System.Drawing.Size(295, 251);
			this.swatchTabPage.TabIndex = 0;
			this.swatchTabPage.Text = "T_SWATCHES";
			this.swatchTabPage.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.loadingSwatchLabel);
			this.panel1.Controls.Add(this.swatchContainer);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(295, 251);
			this.panel1.TabIndex = 1;
			// 
			// loadingSwatchLabel
			// 
			this.loadingSwatchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.loadingSwatchLabel.BackColor = System.Drawing.SystemColors.Control;
			this.loadingSwatchLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.loadingSwatchLabel.Location = new System.Drawing.Point(56, 76);
			this.loadingSwatchLabel.Name = "loadingSwatchLabel";
			this.languageProvider1.SetPropertyNames(this.loadingSwatchLabel, "Text");
			this.loadingSwatchLabel.Size = new System.Drawing.Size(183, 98);
			this.loadingSwatchLabel.TabIndex = 1;
			this.loadingSwatchLabel.Text = "M_LOADING";
			this.loadingSwatchLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// alphaColorSlider
			// 
			this.alphaColorSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.alphaColorSlider.BackColor = System.Drawing.Color.Transparent;
			this.alphaColorSlider.LargeChange = ((uint)(5u));
			this.alphaColorSlider.Location = new System.Drawing.Point(24, 224);
			this.alphaColorSlider.Maximum = 255;
			this.alphaColorSlider.Name = "alphaColorSlider";
			this.alphaColorSlider.Size = new System.Drawing.Size(218, 20);
			this.alphaColorSlider.SmallChange = ((uint)(1u));
			this.alphaColorSlider.TabIndex = 22;
			this.alphaColorSlider.Text = "Alpha";
			this.alphaColorSlider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.alphaColorSlider_Scroll);
			// 
			// colorPick1
			// 
			this.colorPick1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.colorPick1.CurrentAlpha = ((short)(255));
			this.colorPick1.CurrentHue = ((short)(0));
			this.colorPick1.CurrentSat = ((short)(0));
			this.colorPick1.CurrentVal = ((short)(0));
			this.colorPick1.Location = new System.Drawing.Point(35, 9);
			this.colorPick1.Name = "colorPick1";
			this.colorPick1.Size = new System.Drawing.Size(147, 150);
			this.colorPick1.TabIndex = 21;
			this.colorPick1.HSVChanged += new System.EventHandler(this.colorPick1_HSVChanged);
			// 
			// colorPreview1
			// 
			this.colorPreview1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.colorPreview1.Location = new System.Drawing.Point(213, 35);
			this.colorPreview1.Name = "colorPreview1";
			this.colorPreview1.Size = new System.Drawing.Size(36, 36);
			this.colorPreview1.TabIndex = 17;
			this.colorPreview1.Text = "colorPreview1";
			this.colorPreview1.Click += new System.EventHandler(this.colorPreview1_Click);
			// 
			// colorPreview2
			// 
			this.colorPreview2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.colorPreview2.Location = new System.Drawing.Point(228, 50);
			this.colorPreview2.Name = "colorPreview2";
			this.colorPreview2.Size = new System.Drawing.Size(36, 36);
			this.colorPreview2.TabIndex = 20;
			this.colorPreview2.Text = "colorPreview2";
			this.colorPreview2.Click += new System.EventHandler(this.colorPreview2_Click);
			// 
			// redColorSlider
			// 
			this.redColorSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.redColorSlider.BackColor = System.Drawing.Color.Transparent;
			this.redColorSlider.LargeChange = ((uint)(5u));
			this.redColorSlider.Location = new System.Drawing.Point(24, 164);
			this.redColorSlider.Maximum = 255;
			this.redColorSlider.Name = "redColorSlider";
			this.redColorSlider.Size = new System.Drawing.Size(218, 20);
			this.redColorSlider.SmallChange = ((uint)(1u));
			this.redColorSlider.TabIndex = 12;
			this.redColorSlider.Text = "colorSlider1";
			this.redColorSlider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.redColorSlider_Scroll);
			// 
			// blueColorSlider
			// 
			this.blueColorSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.blueColorSlider.BackColor = System.Drawing.Color.Transparent;
			this.blueColorSlider.LargeChange = ((uint)(5u));
			this.blueColorSlider.Location = new System.Drawing.Point(24, 204);
			this.blueColorSlider.Maximum = 255;
			this.blueColorSlider.Name = "blueColorSlider";
			this.blueColorSlider.Size = new System.Drawing.Size(218, 20);
			this.blueColorSlider.SmallChange = ((uint)(1u));
			this.blueColorSlider.TabIndex = 13;
			this.blueColorSlider.Text = "colorSlider2";
			this.blueColorSlider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.blueColorSlider_Scroll);
			// 
			// greenColorSlider
			// 
			this.greenColorSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.greenColorSlider.BackColor = System.Drawing.Color.Transparent;
			this.greenColorSlider.LargeChange = ((uint)(5u));
			this.greenColorSlider.Location = new System.Drawing.Point(24, 184);
			this.greenColorSlider.Maximum = 255;
			this.greenColorSlider.Name = "greenColorSlider";
			this.greenColorSlider.Size = new System.Drawing.Size(218, 20);
			this.greenColorSlider.SmallChange = ((uint)(1u));
			this.greenColorSlider.TabIndex = 14;
			this.greenColorSlider.Text = "colorSlider3";
			this.greenColorSlider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.greenColorSlider_Scroll);
			// 
			// hueColorSlider
			// 
			this.hueColorSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.hueColorSlider.BackColor = System.Drawing.Color.Transparent;
			this.hueColorSlider.LargeChange = ((uint)(5u));
			this.hueColorSlider.Location = new System.Drawing.Point(24, 164);
			this.hueColorSlider.Maximum = 360;
			this.hueColorSlider.Name = "hueColorSlider";
			this.hueColorSlider.Size = new System.Drawing.Size(218, 20);
			this.hueColorSlider.SmallChange = ((uint)(1u));
			this.hueColorSlider.TabIndex = 24;
			this.hueColorSlider.Text = "colorSlider1";
			this.hueColorSlider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hueColorSlider_Scroll);
			// 
			// valueColorSlider
			// 
			this.valueColorSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.valueColorSlider.BackColor = System.Drawing.Color.Transparent;
			this.valueColorSlider.LargeChange = ((uint)(5u));
			this.valueColorSlider.Location = new System.Drawing.Point(24, 204);
			this.valueColorSlider.Name = "valueColorSlider";
			this.valueColorSlider.Size = new System.Drawing.Size(218, 20);
			this.valueColorSlider.SmallChange = ((uint)(1u));
			this.valueColorSlider.TabIndex = 25;
			this.valueColorSlider.Text = "colorSlider2";
			this.valueColorSlider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.valueColorSlider_Scroll);
			// 
			// saturationColorSlider
			// 
			this.saturationColorSlider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.saturationColorSlider.BackColor = System.Drawing.Color.Transparent;
			this.saturationColorSlider.LargeChange = ((uint)(5u));
			this.saturationColorSlider.Location = new System.Drawing.Point(24, 184);
			this.saturationColorSlider.Name = "saturationColorSlider";
			this.saturationColorSlider.Size = new System.Drawing.Size(218, 20);
			this.saturationColorSlider.SmallChange = ((uint)(1u));
			this.saturationColorSlider.TabIndex = 26;
			this.saturationColorSlider.Text = "colorSlider3";
			this.saturationColorSlider.Scroll += new System.Windows.Forms.ScrollEventHandler(this.saturationColorSlider_Scroll);
			// 
			// swatchContainer
			// 
			this.swatchContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.swatchContainer.Location = new System.Drawing.Point(0, 0);
			this.swatchContainer.Margin = new System.Windows.Forms.Padding(0);
			this.swatchContainer.Name = "swatchContainer";
			this.swatchContainer.Size = new System.Drawing.Size(295, 251);
			this.swatchContainer.TabIndex = 0;
			this.swatchContainer.SwatchChanged += new System.EventHandler<MCSkin3D.SwatchChangedEventArgs>(this.swatchContainer_SwatchChanged);
			// 
			// ColorPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.colorTabControl);
			this.MinimumSize = new System.Drawing.Size(303, 277);
			this.Name = "ColorPanel";
			this.Size = new System.Drawing.Size(303, 277);
			this.colorTabControl.ResumeLayout(false);
			this.rgbTabPage.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.redNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.greenNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.alphaNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.blueNumericUpDown)).EndInit();
			this.hsvTabPage.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.panel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.hueNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.saturationNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.valueNumericUpDown)).EndInit();
			this.swatchTabPage.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		public Language.LanguageProvider languageProvider1;
		private System.Windows.Forms.TabControl colorTabControl;
		private System.Windows.Forms.TabPage rgbTabPage;
		private System.Windows.Forms.Panel panel2;
		private MB.Controls.ColorSlider alphaColorSlider;
		private lemon42.ColorPick colorPick1;
		private Paril.Controls.Color.ColorPreview colorPreview1;
		private Paril.Controls.Color.ColorPreview colorPreview2;
		private System.Windows.Forms.TextBox textBox1;
		private MB.Controls.ColorSlider redColorSlider;
		private System.Windows.Forms.Label label9;
		private MB.Controls.ColorSlider blueColorSlider;
		private System.Windows.Forms.NumericUpDown redNumericUpDown;
		private MB.Controls.ColorSlider greenColorSlider;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown greenNumericUpDown;
		private System.Windows.Forms.NumericUpDown alphaNumericUpDown;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.NumericUpDown blueNumericUpDown;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TabPage hsvTabPage;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.NumericUpDown hueNumericUpDown;
		private MB.Controls.ColorSlider hueColorSlider;
		private System.Windows.Forms.Label label6;
		private MB.Controls.ColorSlider valueColorSlider;
		private System.Windows.Forms.NumericUpDown saturationNumericUpDown;
		private MB.Controls.ColorSlider saturationColorSlider;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown valueNumericUpDown;
		private System.Windows.Forms.TabPage swatchTabPage;
		private System.Windows.Forms.Panel panel1;
		public System.Windows.Forms.Label loadingSwatchLabel;
		private SwatchContainer swatchContainer;

	}
}
