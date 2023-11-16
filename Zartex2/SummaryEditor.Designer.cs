namespace Zartex
{
    partial class SummaryEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.Inspector = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(1, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(333, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "Property Inspector";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Inspector
            // 
            this.Inspector.AccessibleName = "inspectorW";
            this.Inspector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Inspector.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Inspector.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.Inspector.Location = new System.Drawing.Point(1, 18);
            this.Inspector.Margin = new System.Windows.Forms.Padding(0);
            this.Inspector.Name = "Inspector";
            this.Inspector.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.Inspector.Size = new System.Drawing.Size(332, 356);
            this.Inspector.TabIndex = 2;
            // 
            // SummaryEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 373);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Inspector);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SummaryEditor";
            this.Text = "Mission Summary Editor";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.PropertyGrid Inspector;
    }
}