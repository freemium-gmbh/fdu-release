namespace DriverUtilites.InfoCollecting
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
			this.lblResult = new System.Windows.Forms.Label();
			this.prbMain = new System.Windows.Forms.ProgressBar();
			this.SuspendLayout();
			// 
			// lblResult
			// 
			this.lblResult.AutoSize = true;
			this.lblResult.Location = new System.Drawing.Point(12, 9);
			this.lblResult.Name = "lblResult";
			this.lblResult.Size = new System.Drawing.Size(86, 13);
			this.lblResult.TabIndex = 1;
			this.lblResult.Text = "Collecting data...";
			// 
			// prbMain
			// 
			this.prbMain.Location = new System.Drawing.Point(12, 34);
			this.prbMain.Name = "prbMain";
			this.prbMain.Size = new System.Drawing.Size(390, 23);
			this.prbMain.TabIndex = 2;
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(417, 72);
			this.Controls.Add(this.prbMain);
			this.Controls.Add(this.lblResult);
			this.Name = "frmMain";
			this.Text = "DriverUtilites.InfoCollecting";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblResult;
		private System.Windows.Forms.ProgressBar prbMain;
	}
}

