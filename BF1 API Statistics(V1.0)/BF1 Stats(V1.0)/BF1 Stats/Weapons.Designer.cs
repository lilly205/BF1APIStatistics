using System.ComponentModel;

namespace BF1_Stats.Resources
{
    public partial class Weapons
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

        public void ResetStats()
        {
            lblWeaponAccuracy.Text = "0";
            lblWeaponKills.Text = "0";
            lblWeaponKPM.Text = "0";
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public string WeaponName
        {
            get { return lblWeaponName.Text; }
            set { lblWeaponName.Text = value;
                lblWeaponName.Location = new System.Drawing.Point((80-(lblWeaponName.Width/2)), lblWeaponName.Location.Y);
                Invalidate();
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public string WeaponKills
        {
            get { return lblWeaponKills.Text; }
            set { lblWeaponKills.Text = value; }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public string WeaponKPM
        {
            get { return lblWeaponKPM.Text; }
            set { lblWeaponKPM.Text = value; }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public string WeaponAccuracy
        {
            get { return lblWeaponAccuracy.Text; }
            set { lblWeaponAccuracy.Text = value; }

        }

        public double WeaponAccuracyDouble
        {
            get {
                double num;
                if (double.TryParse(lblWeaponAccuracy.Text, out num))
                {

                }

                return num; }
        }

        public double WeaponKPMDouble
        {
            get
            {
                double num;
                if (double.TryParse(lblWeaponKPM.Text, out num))
                {

                }

                return num;
            }
        }


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblWeaponName = new System.Windows.Forms.Label();
            this.lblWeaponKills = new System.Windows.Forms.Label();
            this.lblWeaponKPM = new System.Windows.Forms.Label();
            this.lblWeaponAccuracy = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblWeaponName
            // 
            this.lblWeaponName.AutoSize = true;
            this.lblWeaponName.ForeColor = System.Drawing.Color.White;
            this.lblWeaponName.Location = new System.Drawing.Point(74, 44);
            this.lblWeaponName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWeaponName.Name = "lblWeaponName";
            this.lblWeaponName.Size = new System.Drawing.Size(45, 17);
            this.lblWeaponName.TabIndex = 1;
            this.lblWeaponName.Text = "Name";
            // 
            // lblWeaponKills
            // 
            this.lblWeaponKills.AutoSize = true;
            this.lblWeaponKills.ForeColor = System.Drawing.Color.White;
            this.lblWeaponKills.Location = new System.Drawing.Point(260, 44);
            this.lblWeaponKills.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWeaponKills.Name = "lblWeaponKills";
            this.lblWeaponKills.Size = new System.Drawing.Size(16, 17);
            this.lblWeaponKills.TabIndex = 2;
            this.lblWeaponKills.Text = "0";
            // 
            // lblWeaponKPM
            // 
            this.lblWeaponKPM.AutoSize = true;
            this.lblWeaponKPM.ForeColor = System.Drawing.Color.White;
            this.lblWeaponKPM.Location = new System.Drawing.Point(430, 44);
            this.lblWeaponKPM.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWeaponKPM.Name = "lblWeaponKPM";
            this.lblWeaponKPM.Size = new System.Drawing.Size(16, 17);
            this.lblWeaponKPM.TabIndex = 3;
            this.lblWeaponKPM.Text = "0";
            // 
            // lblWeaponAccuracy
            // 
            this.lblWeaponAccuracy.AutoSize = true;
            this.lblWeaponAccuracy.ForeColor = System.Drawing.Color.White;
            this.lblWeaponAccuracy.Location = new System.Drawing.Point(600, 44);
            this.lblWeaponAccuracy.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWeaponAccuracy.Name = "lblWeaponAccuracy";
            this.lblWeaponAccuracy.Size = new System.Drawing.Size(16, 17);
            this.lblWeaponAccuracy.TabIndex = 4;
            this.lblWeaponAccuracy.Text = "0";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel1.Location = new System.Drawing.Point(0, 100);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(685, 5);
            this.panel1.TabIndex = 5;
            // 
            // Weapons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblWeaponAccuracy);
            this.Controls.Add(this.lblWeaponKPM);
            this.Controls.Add(this.lblWeaponKills);
            this.Controls.Add(this.lblWeaponName);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Weapons";
            this.Size = new System.Drawing.Size(685, 110);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblWeaponName;
        private System.Windows.Forms.Label lblWeaponKills;
        private System.Windows.Forms.Label lblWeaponKPM;
        private System.Windows.Forms.Label lblWeaponAccuracy;
        private System.Windows.Forms.Panel panel1;
    }
}
