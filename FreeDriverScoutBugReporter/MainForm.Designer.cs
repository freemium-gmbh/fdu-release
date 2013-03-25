namespace BugReporter
{
    partial class frmMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.lblCaption = new System.Windows.Forms.Label();
			this.pbIcon = new System.Windows.Forms.PictureBox();
			this.grpException = new System.Windows.Forms.GroupBox();
			this.txtException = new System.Windows.Forms.TextBox();
			this.txtContext = new System.Windows.Forms.TextBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnDetails = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
			this.grpException.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblCaption
			// 
			this.lblCaption.Location = new System.Drawing.Point(12, 9);
			this.lblCaption.Name = "lblCaption";
			this.lblCaption.Size = new System.Drawing.Size(550, 38);
			this.lblCaption.TabIndex = 0;
			this.lblCaption.Text = "Freemium has encountered a fatal error. Please explain briefly what were you doin" +
    "g when you encountered that error. Thanks for spending time, we appreciate your " +
    "help.";
			// 
			// pbIcon
			// 
			this.pbIcon.Image = ((System.Drawing.Image)(resources.GetObject("pbIcon.Image")));
			this.pbIcon.Location = new System.Drawing.Point(12, 61);
			this.pbIcon.Name = "pbIcon";
			this.pbIcon.Size = new System.Drawing.Size(52, 52);
			this.pbIcon.TabIndex = 2;
			this.pbIcon.TabStop = false;
			// 
			// grpException
			// 
			this.grpException.AutoSize = true;
			this.grpException.Controls.Add(this.txtException);
			this.grpException.Location = new System.Drawing.Point(12, 161);
			this.grpException.Name = "grpException";
			this.grpException.Size = new System.Drawing.Size(553, 132);
			this.grpException.TabIndex = 3;
			this.grpException.TabStop = false;
			this.grpException.Text = "Exception Details";
			// 
			// txtException
			// 
			this.txtException.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtException.Location = new System.Drawing.Point(3, 16);
			this.txtException.Multiline = true;
			this.txtException.Name = "txtException";
			this.txtException.ReadOnly = true;
			this.txtException.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtException.Size = new System.Drawing.Size(547, 113);
			this.txtException.TabIndex = 0;
			this.txtException.TabStop = false;
			// 
			// txtContext
			// 
			this.txtContext.Location = new System.Drawing.Point(73, 50);
			this.txtContext.Multiline = true;
			this.txtContext.Name = "txtContext";
			this.txtContext.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtContext.Size = new System.Drawing.Size(492, 76);
			this.txtContext.TabIndex = 0;
			// 
			// btnSend
			// 
			this.btnSend.Location = new System.Drawing.Point(490, 132);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(75, 23);
			this.btnSend.TabIndex = 5;
			this.btnSend.Text = "&Send Report";
			this.btnSend.UseVisualStyleBackColor = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(409, 132);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnDetails
			// 
			this.btnDetails.Location = new System.Drawing.Point(12, 132);
			this.btnDetails.Name = "btnDetails";
			this.btnDetails.Size = new System.Drawing.Size(75, 23);
			this.btnDetails.TabIndex = 8;
			this.btnDetails.Tag = "Show";
			this.btnDetails.Text = "&Details >>";
			this.btnDetails.UseVisualStyleBackColor = true;
			this.btnDetails.Click += new System.EventHandler(this.detailsBtn_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(577, 162);
			this.Controls.Add(this.btnDetails);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.txtContext);
			this.Controls.Add(this.grpException);
			this.Controls.Add(this.pbIcon);
			this.Controls.Add(this.lblCaption);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmMain";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Freemium Bug Reporter";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.frmMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
			this.grpException.ResumeLayout(false);
			this.grpException.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.GroupBox grpException;
        private System.Windows.Forms.TextBox txtContext;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtException;
        private System.Windows.Forms.Button btnDetails;
    }
}

