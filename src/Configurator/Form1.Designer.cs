using System;
using System.Windows.Forms;

namespace Configurator
{
    partial class Form1
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbHosts = new System.Windows.Forms.ListBox();
            this.lblHosts = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.ofgConfig = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpView = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.jobDisplay = new System.Windows.Forms.Panel();
            this.lbJob = new System.Windows.Forms.ListBox();
            this.tpAdd = new System.Windows.Forms.TabPage();
            this.jobAdd = new System.Windows.Forms.Panel();
            this.tpRoles = new System.Windows.Forms.TabPage();
            this.resultView = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tpView.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tpAdd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.lbHosts);
            this.panel1.Controls.Add(this.lblHosts);
            this.panel1.Location = new System.Drawing.Point(12, 92);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(144, 570);
            this.panel1.TabIndex = 3;
            // 
            // lbHosts
            // 
            this.lbHosts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbHosts.FormattingEnabled = true;
            this.lbHosts.Location = new System.Drawing.Point(6, 16);
            this.lbHosts.Name = "lbHosts";
            this.lbHosts.Size = new System.Drawing.Size(127, 524);
            this.lbHosts.TabIndex = 3;
            // 
            // lblHosts
            // 
            this.lblHosts.AutoSize = true;
            this.lblHosts.Location = new System.Drawing.Point(3, 0);
            this.lblHosts.Name = "lblHosts";
            this.lblHosts.Size = new System.Drawing.Size(34, 13);
            this.lblHosts.TabIndex = 0;
            this.lblHosts.Text = "Hosts";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnRefresh);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.btnLoad);
            this.panel2.Location = new System.Drawing.Point(18, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(175, 57);
            this.panel2.TabIndex = 4;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(14, 28);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(95, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(14, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click_1);
            // 
            // ofgConfig
            // 
            this.ofgConfig.FileName = "config.xml";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tpView);
            this.tabControl1.Controls.Add(this.tpAdd);
            this.tabControl1.Controls.Add(this.tpRoles);
            this.tabControl1.Location = new System.Drawing.Point(434, 15);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(650, 494);
            this.tabControl1.TabIndex = 2;
            // 
            // tpView
            // 
            this.tpView.Controls.Add(this.groupBox1);
            this.tpView.Location = new System.Drawing.Point(4, 22);
            this.tpView.Name = "tpView";
            this.tpView.Padding = new System.Windows.Forms.Padding(3);
            this.tpView.Size = new System.Drawing.Size(642, 468);
            this.tpView.TabIndex = 0;
            this.tpView.Text = "View Job";
            this.tpView.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.jobDisplay);
            this.groupBox1.Controls.Add(this.lbJob);
            this.groupBox1.Location = new System.Drawing.Point(6, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(630, 456);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Jobs";
            // 
            // jobDisplay
            // 
            this.jobDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.jobDisplay.Location = new System.Drawing.Point(200, 19);
            this.jobDisplay.Name = "jobDisplay";
            this.jobDisplay.Size = new System.Drawing.Size(424, 426);
            this.jobDisplay.TabIndex = 1;
            // 
            // lbJob
            // 
            this.lbJob.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbJob.FormattingEnabled = true;
            this.lbJob.Location = new System.Drawing.Point(6, 19);
            this.lbJob.Name = "lbJob";
            this.lbJob.Size = new System.Drawing.Size(188, 420);
            this.lbJob.TabIndex = 0;
            this.lbJob.SelectedIndexChanged += new System.EventHandler(this.lbJob_SelectedIndexChanged);
            // 
            // tpAdd
            // 
            this.tpAdd.Controls.Add(this.jobAdd);
            this.tpAdd.Location = new System.Drawing.Point(4, 22);
            this.tpAdd.Name = "tpAdd";
            this.tpAdd.Padding = new System.Windows.Forms.Padding(3);
            this.tpAdd.Size = new System.Drawing.Size(642, 508);
            this.tpAdd.TabIndex = 1;
            this.tpAdd.Text = "Add Job";
            this.tpAdd.UseVisualStyleBackColor = true;
            // 
            // jobAdd
            // 
            this.jobAdd.Location = new System.Drawing.Point(6, 6);
            this.jobAdd.Name = "jobAdd";
            this.jobAdd.Size = new System.Drawing.Size(501, 387);
            this.jobAdd.TabIndex = 0;
            // 
            // tpRoles
            // 
            this.tpRoles.Location = new System.Drawing.Point(4, 22);
            this.tpRoles.Name = "tpRoles";
            this.tpRoles.Size = new System.Drawing.Size(642, 508);
            this.tpRoles.TabIndex = 2;
            this.tpRoles.Text = "Roles";
            this.tpRoles.UseVisualStyleBackColor = true;
            // 
            // resultView
            // 
            this.resultView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultView.Location = new System.Drawing.Point(434, 515);
            this.resultView.Name = "resultView";
            this.resultView.Size = new System.Drawing.Size(646, 150);
            this.resultView.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1088, 674);
            this.Controls.Add(this.resultView);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseHover += new System.EventHandler(this.test);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tpView.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tpAdd.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.resultView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lbHosts;
        private System.Windows.Forms.Label lblHosts;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.OpenFileDialog ofgConfig;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel jobDisplay;
        private System.Windows.Forms.ListBox lbJob;
        private System.Windows.Forms.TabPage tpAdd;
        private System.Windows.Forms.Panel jobAdd;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TabPage tpRoles;
        private DataGridView resultView;
    }
}

