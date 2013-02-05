using System;
using System.Drawing;
using System.Windows.Forms;
using Configurator.code;
using SqlToGraphite;

namespace Configurator
{
    using log4net;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            log = LogManager.GetLogger("log");
            log4net.Config.XmlConfigurator.Configure();
            selectedJob = string.Empty;
            controller = new code.Controller();           
            assemblyResolver = new AssemblyResolver(new DirectoryImpl(), log);
        }

        private code.Controller controller;

        private string selectedJob;

        private AssemblyResolver assemblyResolver;

        private Builder b;

        private Builder jobAddObject;

        private RolesViewer rv;

        private HostsViewer hv;

        private RoleTreeViewer roleTreeViewer;

        private HostsTreeViewer hostsTreeViewer;

        private ILog log;

       private void SetUpDialogues()
        {
            ofgConfig.Multiselect = false;
            // ofgConfig.FileName = @"C:\git\perryOfPeek\SqlToGraphite\src\Configurator\bin\Debug\config.xml";
        }

        private void Form1_Load(object sender, EventArgs e)
        {           
            SetUpDialogues();
            //this.LoadTheConfig();
        }

        private void RenderForm()
        {
            DisplayJobs();
        }

        private void RefreshForm()
        {
            DisplayJob(selectedJob);
            DisplayAddJob();
            DisplayRolesView();
            DisplayHostsView();
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

        // This will be called whenever the list changes.
        private void TreeViewerOnChanged(object sender, EventArgs e)
        {
           // RefreshTheForm();
        }

        private void LoadTheConfig()
        {
            var newPath = this.ofgConfig.FileName.Replace(@"\", "/");
            var path = string.Format("file:///{0}", newPath);
            this.controller.LoadConfig(path);
            roleTreeViewer = new RoleTreeViewer(controller);
            hostsTreeViewer = new HostsTreeViewer(controller);
            roleTreeViewer.Changed += this.TreeViewerOnChanged;
            this.rv = new RolesViewer(controller, roleTreeViewer);
            this.hv = new HostsViewer(controller, hostsTreeViewer);
            this.RenderForm();
            this.RefreshForm();
        }

        private void lbJob_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listControl = (ListBox)sender;
            if (listControl.SelectedItem != null)
            {
                selectedJob = listControl.SelectedItem.ToString();
            }
            this.RefreshForm();
        }

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

        private void DisplayAddJob()
        {
            jobAddObject = new Builder(jobAdd, new DefaultJobProperties(jobDisplay.Width), controller, assemblyResolver, this.resultView);
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
            var p = new Position(this.tpRoles.Left, this.tpRoles.Width, this.tpRoles.Top, this.tpRoles.Height - 50);
            var panel = rv.Get(p);
            tpRoles.Controls.Clear();
            tpRoles.Controls.Add(panel);
        }

        private void DisplayHostsView()
        {
            var p = new Position(this.tpRoles.Left, this.tpRoles.Width, this.tpRoles.Top, this.tpRoles.Height - 50);
            var panel = hv.Get(p);
            tpHosts.Controls.Clear();
            tpHosts.Controls.Add(panel);
        }
    }
}
