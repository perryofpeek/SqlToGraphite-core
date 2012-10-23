using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using SqlToGraphite;

using SqlToGraphiteInterfaces;

namespace Configurator.code
{
    public class BuilderBase
    {

        public event EventHandler AddedJobEvent;

        public void OnAddedJobEvent(EventArgs e)
        {
            EventHandler handler = this.AddedJobEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected Panel panel;

        protected DefaultJobProperties defaultJobProperties;

        protected Controller controller;

        protected AssemblyResolver assemblyResolver;

        protected readonly DataGridView resultGrid;

        protected int nextTop;

        private ISqlClient client;

        public BuilderBase(Panel panel, DefaultJobProperties defaultJobProperties, Controller controller, AssemblyResolver assemblyResolver, DataGridView resultGrid)
        {
            this.panel = panel;
            this.defaultJobProperties = defaultJobProperties;
            this.controller = controller;
            this.assemblyResolver = assemblyResolver;
            this.resultGrid = resultGrid;
        }

        protected int GetNextTop()
        {
            var rtn = this.nextTop;
            this.IncNextTop();
            return rtn;
        }

        protected void IncNextTop()
        {
            this.nextTop = this.nextTop + this.defaultJobProperties.DefaultHeight;
        }

        protected string GetPropertyValue(ISqlClient typedJob, PropertyInfo property)
        {
            string rtn;
            if (property == null)
            {
                rtn = string.Empty;
            }
            else
            {
                object obj = null;
                if(this.IsEncrypted(property))
                {
                    Encryption encryption = new Encryption();
                    var value = property.GetValue(typedJob, null).ToString();
                    if (value != string.Empty)
                    {
                        obj = encryption.Decrypt(property.GetValue(typedJob, null).ToString());                    
                    }                        
                }
                else
                {
                    obj = property.GetValue(typedJob, null);       
                }
                
                rtn = obj == null ? string.Empty : obj.ToString();
            }
            return rtn;
        }

        protected Panel CreateNewPanel()
        {
            return new Panel { Top = this.nextTop, Width = this.panel.Width, Height = this.panel.Height - this.nextTop };
        }

        protected void AddPair(string name, string value)
        {
            var lbl = new Label { Text = name, Top = this.nextTop, Width = this.defaultJobProperties.DefaultLabelWidth };
            var txb = new TextBox
            {
                Text = value,
                Top = this.GetNextTop(),
                Left = this.defaultJobProperties.DefaultLabelWidth + this.defaultJobProperties.DefaultSpace,
                Width = defaultJobProperties.TextWidth,
                Name = name
            };
            panel.Controls.Add(lbl);
            panel.Controls.Add(txb);
        }

        private void AddButton(ISqlClient c, string name, EventHandler eventHandler)
        {
            client = c;
            var button = new Button { Text = name, Top = this.GetNextTop(), Left = 0 };
            button.Click += eventHandler;
            panel.Controls.Add(button);
        }

        private void TestButtonClick(object sender, EventArgs e)
        {
            try
            {
                this.WireUpTheClientObjectWithUiValues();
                var results = client.Get();
                resultGrid.DataSource = results;
            }
            catch (Exception ex)
            {
                var s = ex.Message;
            }
        }

        public void DisplayEmptyJob(string name)
        {
            Type type = assemblyResolver.ResolveType(name);
            client = (ISqlClient)Activator.CreateInstance(type);

            foreach (var property in client.GetType().GetProperties())
            {
                if (property.Name != "ClientName" && property.Name != "Type")
                {
                    var value = GetPropertyValue(client, property);
                    this.AddPair(property.Name, value);
                }
            }
            this.AddButton(client, "Test", this.TestButtonClick);
            this.AddButton(client, "Add", this.AddJobButtonClick);
        }

        private void AddJobButtonClick(object sender, EventArgs e)
        {
            this.WireUpTheClientObjectWithUiValues();

            this.controller.AddJob(client);
            OnAddedJobEvent(EventArgs.Empty);
        }

        private void WireUpTheClientObjectWithUiValues()
        {
            foreach (PropertyInfo property in this.client.GetType().GetProperties())
            {
                string value = string.Empty;
                if (property.Name == "Type")
                {
                    value = this.client.GetType().FullName;
                }
                else if (property.Name == "ClientName")
                {
                    value = this.GetComboValue(property.Name);
                }
                else
                {
                    value = this.GetTextBoxValue(property.Name);
                }
                //This is a rubbish implementation , but it will do for now 
                //TODO:Make this more generic
                if (property.PropertyType.Name == typeof(Int32).Name)
                {
                    property.SetValue(this.client, Convert.ToInt32(value), null);
                }                

                if (property.PropertyType.Name == typeof(string).Name)
                {
                    if(IsEncrypted(property))
                    {
                        var encryption = new Encryption();
                        property.SetValue(this.client, encryption.Encrypt(value), null);
                    }
                    else
                    {
                        property.SetValue(this.client, value, null);    
                    }                    
                }

                if (property.PropertyType.Name == typeof(bool).Name)
                {
                    property.SetValue(this.client, Convert.ToBoolean(value), null);
                }
            }
        }

        private bool IsEncrypted(PropertyInfo property)
        {
            PropertyInfo[] properties = property.GetType().GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(EncryptedAttribute)) as EncryptedAttribute;
                if (attribute != null) 
                {
                    return true;
                }
            }
            return false;
        }

        private string GetTextBoxValue(string name)
        {
            foreach (var control in this.panel.Controls)
            {
                if (control.GetType().FullName == typeof(TextBox).FullName)
                {
                    var txb = (TextBox)control;
                    if (txb.Name == name)
                    {
                        return txb.Text;
                    }
                }
            }

            return string.Empty;
        }

        private string GetComboValue(string name)
        {
            foreach (var control in this.panel.Controls)
            {
                if (control.GetType().FullName == typeof(ComboBox).FullName)
                {
                    var txb = (ComboBox)control;
                    if (txb.Name == name)
                    {
                        return txb.Text;
                    }
                }
            }

            return string.Empty;
        }

        public void DisplayJob(string name)
        {
            var typedJob = controller.GetTypedJob(name);
            foreach (var property in typedJob.GetType().GetProperties())
            {
                var value = GetPropertyValue(typedJob, property);
                this.AddPair(property.Name, value);
            }
            this.AddButton(typedJob, "Test", this.TestButtonClick);
            this.DisplayResultsPannel();
        }

        private Panel resultsPanel;

        private void DisplayResultsPannel()
        {
            resultsPanel = this.CreateNewPanel();
            resultsPanel.BorderStyle = BorderStyle.Fixed3D;
            resultsPanel.Width = panel.Width;
            this.AddPanel(resultsPanel);
        }

        private void AddPanel(Panel p)
        {
            panel.Controls.Add(p);
        }
    }
}