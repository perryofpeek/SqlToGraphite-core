using System;
using System.Drawing;
using System.Windows.Forms;
using Configurator.code;
using SqlToGraphite;

namespace Configurator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private code.Controller controller;

        private string selectedJob;

        private AssemblyResolver assemblyResolver;

        private void SetUpDialogues()
        {
            ofgConfig.Multiselect = false;
            ofgConfig.FileName = @"C:\git\perryOfPeek\SqlToGraphite\src\Configurator\bin\Debug\config.xml";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            selectedJob = string.Empty;
            controller = new code.Controller();
            assemblyResolver = new AssemblyResolver(new DirectoryImpl());
            SetUpDialogues();
            this.LoadTheConfig();
        }

        private void RenderForm()
        {
            DisplayHosts();
            DisplayJobs();
        }

        private void RefreshForm()
        {
            DisplayJob(selectedJob);
            DisplayAddJob();
            DisplayRolesView();
        }

        private void DisplayHosts()
        {
            lbHosts.Items.Clear();
            foreach (var host in controller.GetHosts())
            {
                lbHosts.Items.Add(host.Name);
            }
        }

        private void DisplayJobs()
        {
            lbJob.Items.Clear();            
            foreach (var job in controller.GetJobs())
            {
                lbJob.Items.Add(job.Name);                
            }
        }

        private void btnLoad_Click_1(object sender, EventArgs e)
        {
            var result = ofgConfig.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                this.LoadTheConfig();
            }
        }

        private void LoadTheConfig()
        {
            var newPath = this.ofgConfig.FileName.Replace(@"\", "/");
            var path = string.Format("file:///{0}", newPath);
            this.controller.LoadConfig(path);
            this.RenderForm();
            this.RefreshForm();
        }


        private void lbJob_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listControl = (ListBox)sender;
            selectedJob = listControl.SelectedItem.ToString();
            this.RefreshForm();
        }

        private Builder b;

        private void DisplayJob(string name)
        {
            jobDisplay.Controls.Clear();
          
            if (!string.IsNullOrEmpty(name))
            {
                foreach (var job in controller.GetJobs())
                {
                    if (job.Name == name)
                    {
                        b = new Builder(jobDisplay, new DefaultJobProperties(jobDisplay.Width), controller, assemblyResolver, this.resultView);
                        jobDisplay.Controls.Clear();
                        b.DisplayJob(name);
                        b.AddedJobEvent += BOnAddedJobEvent;                       
                    }
                }

                this.Refresh();
                
            }
        }

        private Builder jobAddObject;

        private void DisplayAddJob()
        {
            jobAddObject = new Builder(jobAdd, new DefaultJobProperties(jobDisplay.Width), controller, assemblyResolver,this.resultView);
            jobAddObject.AddedJobEvent += BOnAddedJobEvent;
            jobAddObject.DisplayJobAdd();
            this.Refresh();
        }

        private void BOnAddedJobEvent(object sender, EventArgs eventArgs)
        {
            RefreshTheForm();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.RefreshForm();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            controller.Save(ofgConfig.FileName);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshTheForm();
        }

        private void RefreshTheForm()
        {
            this.RenderForm();
            this.RefreshForm();
        }

        private void DisplayRolesView()
        {
            var rv = new RolesViewer(controller, new TreeViewer(controller));
            var p = new Position(this.tpRoles.Left, this.tpRoles.Width, this.tpRoles.Top, this.tpRoles.Height);
            var panel = rv.Get(p);
            tpRoles.Controls.Add(panel);
        }

        private void test(object sender, EventArgs e)
        {
            var x = "";
        }
    }
}
