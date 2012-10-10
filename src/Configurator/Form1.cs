using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Configurator.code;

using SqlToGraphite;
using SqlToGraphite.Conf;
using SqlToGraphite.Config;

using SqlToGraphiteInterfaces;

using log4net;

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

        private void SetUpDialogues()
        {
            ofgConfig.Multiselect = false;
            ofgConfig.FileName = @"C:\git\perryOfPeek\SqlToGraphite\src\Configurator\bin\Debug\config.xml";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            selectedJob = string.Empty;
            controller = new code.Controller();
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
            selectedJob =  listControl.SelectedItem.ToString();
            this.RefreshForm();
        }


        private void DisplayJob(string name)
        {
            jobDisplay.Controls.Clear();
            if(!string.IsNullOrEmpty(name))
            {                             
                //for (int i = 0; i < jobDisplay.Controls.Count; i++)
                //{
                //    jobDisplay.Controls.RemoveAt(i);
                //}
                foreach (var job in controller.GetJobs())
                {
                    if (job.Name == name)
                    {
                        var typedJob = controller.GetTypedJob(name);
                        //create an instance of the type. 
                        var defaultJobProperties = new DefaultJobProperties(jobDisplay.Width);
                        var b = new Builder(jobDisplay, defaultJobProperties);
                        foreach (var property in typedJob.GetType().GetProperties())
                        {
                            var value = GetPropertyValue(typedJob, property);
                            b.AddPair(property.Name, value);
                        }
                    }
                }

                this.Refresh();
                
            }

        }

        private string GetPropertyValue(ISqlClient typedJob, PropertyInfo property)
        {
            string rtn;
            if (property == null)
            {
                rtn = string.Empty;
            }
            else
            {
                var obj = property.GetValue(typedJob, null);
                rtn = obj == null ? string.Empty : obj.ToString();
            }
            return rtn;
        }

        private Object GetPropValue(String name, Object obj)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.RefreshForm();
        }
    }
}
