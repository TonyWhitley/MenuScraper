namespace MenuScraper;

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
        buttonScan = new Button();
        textBox1 = new TextBox();
        pictureBox1 = new PictureBox();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // buttonScan
        // 
        buttonScan.Location = new Point(607, 387);
        buttonScan.Name = "buttonScan";
        buttonScan.Size = new Size(155, 49);
        buttonScan.TabIndex = 0;
        buttonScan.Text = "Scan";
        buttonScan.UseVisualStyleBackColor = true;
        buttonScan.Click += buttonScan_Click;
        // 
        // textBox1
        // 
        textBox1.Location = new Point(522, 26);
        textBox1.Multiline = true;
        textBox1.Name = "textBox1";
        textBox1.ReadOnly = true;
        textBox1.Size = new Size(366, 290);
        textBox1.TabIndex = 1;
        // 
        // pictureBox1
        // 
        pictureBox1.Location = new Point(23, 24);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(466, 531);
        pictureBox1.TabIndex = 2;
        pictureBox1.TabStop = false;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(10F, 25F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(918, 606);
        Controls.Add(pictureBox1);
        Controls.Add(textBox1);
        Controls.Add(buttonScan);
        Name = "Form1";
        Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button buttonScan;
    private TextBox textBox1;
    private PictureBox pictureBox1;
}
