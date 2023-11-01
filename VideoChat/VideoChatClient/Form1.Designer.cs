namespace VideoChatClient
{
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
            LocalPicture = new PictureBox();
            button1 = new Button();
            RemotePicture = new PictureBox();
            button3 = new Button();
            Ip_textBox = new TextBox();
            port_textBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            ResizeBtn = new Button();
            RotateBtn = new Button();
            comboBox1 = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)LocalPicture).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RemotePicture).BeginInit();
            SuspendLayout();
            // 
            // LocalPicture
            // 
            LocalPicture.BackColor = SystemColors.ActiveCaption;
            LocalPicture.Location = new Point(460, 216);
            LocalPicture.Name = "LocalPicture";
            LocalPicture.Size = new Size(654, 480);
            LocalPicture.SizeMode = PictureBoxSizeMode.Zoom;
            LocalPicture.TabIndex = 1;
            LocalPicture.TabStop = false;
            // 
            // button1
            // 
            button1.Location = new Point(12, 96);
            button1.Name = "button1";
            button1.Size = new Size(184, 23);
            button1.TabIndex = 2;
            button1.Text = "Connect to server";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // RemotePicture
            // 
            RemotePicture.BackColor = SystemColors.ActiveCaption;
            RemotePicture.Location = new Point(1433, 216);
            RemotePicture.Name = "RemotePicture";
            RemotePicture.Size = new Size(640, 480);
            RemotePicture.SizeMode = PictureBoxSizeMode.Zoom;
            RemotePicture.TabIndex = 5;
            RemotePicture.TabStop = false;
            // 
            // button3
            // 
            button3.Location = new Point(12, 125);
            button3.Name = "button3";
            button3.Size = new Size(184, 23);
            button3.TabIndex = 2;
            button3.Text = "Disconnect";
            button3.TextImageRelation = TextImageRelation.ImageAboveText;
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // Ip_textBox
            // 
            Ip_textBox.Location = new Point(50, 35);
            Ip_textBox.Name = "Ip_textBox";
            Ip_textBox.Size = new Size(146, 23);
            Ip_textBox.TabIndex = 2;
            Ip_textBox.Text = "192.168.1.15";
            Ip_textBox.TextChanged += Ip_textBox_TextChanged;
            // 
            // port_textBox
            // 
            port_textBox.Location = new Point(50, 67);
            port_textBox.Name = "port_textBox";
            port_textBox.Size = new Size(146, 23);
            port_textBox.TabIndex = 2;
            port_textBox.Text = "8080";
            port_textBox.TextChanged += port_textBox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 38);
            label1.Name = "label1";
            label1.Size = new Size(20, 15);
            label1.TabIndex = 2;
            label1.Text = "IP:";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 67);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 2;
            label2.Text = "Port:";
            label2.Click += label2_Click;
            // 
            // ResizeBtn
            // 
            ResizeBtn.Anchor = AnchorStyles.Bottom;
            ResizeBtn.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            ResizeBtn.Location = new Point(980, 843);
            ResizeBtn.Name = "ResizeBtn";
            ResizeBtn.Size = new Size(583, 95);
            ResizeBtn.TabIndex = 11;
            ResizeBtn.Text = "ResizeView";
            ResizeBtn.UseVisualStyleBackColor = true;
            ResizeBtn.Click += ResizeBtn_Click;
            // 
            // RotateBtn
            // 
            RotateBtn.Anchor = AnchorStyles.Bottom;
            RotateBtn.Font = new Font("Segoe UI", 16F, FontStyle.Regular, GraphicsUnit.Point);
            RotateBtn.Location = new Point(1569, 843);
            RotateBtn.Name = "RotateBtn";
            RotateBtn.Size = new Size(99, 95);
            RotateBtn.TabIndex = 12;
            RotateBtn.Text = "Rotate";
            RotateBtn.UseVisualStyleBackColor = true;
            RotateBtn.Click += RotateBtn_Click;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Camera", "Screen" });
            comboBox1.Location = new Point(322, 216);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);
            comboBox1.TabIndex = 13;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2276, 950);
            Controls.Add(comboBox1);
            Controls.Add(RotateBtn);
            Controls.Add(ResizeBtn);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(port_textBox);
            Controls.Add(Ip_textBox);
            Controls.Add(button3);
            Controls.Add(RemotePicture);
            Controls.Add(button1);
            Controls.Add(LocalPicture);
            Name = "Form1";
            Text = "Form1";
            FormClosed += Form1_FormClosed;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)LocalPicture).EndInit();
            ((System.ComponentModel.ISupportInitialize)RemotePicture).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox LocalPicture;
        private Button button1;
        private MenuStrip menuStrip1;
        private PictureBox RemotePicture;
        private Button button3;
        private TextBox Ip_textBox;
        private TextBox port_textBox;
        private Label label1;
        private Label label2;
        private Button ResizeBtn;
        private Button RotateBtn;
        private ComboBox comboBox1;
    }
}