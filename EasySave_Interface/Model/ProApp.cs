using System;
using System.Collections.Generic;
using System.Text;
using System.Management;


namespace EasySave_Interface.Model
{
    class ProApp
    {
        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
        }
        private ManagementEventWatcher watcher;
        private Settings settings;
        private bool warn = false;
        public bool stopWatcher = false;

        public ProApp(string name, Settings settings)
        {
            this.settings = settings;
            this.name = name;
            string query = "SELECT *" +
            "  FROM __InstanceOperationEvent " +
            "WITHIN  " + "2" +
            " WHERE TargetInstance ISA 'Win32_Process' " +
            "   AND TargetInstance.Name = '" + $"{name}.exe" + "'";

            this.watcher = new ManagementEventWatcher(
             new WqlEventQuery(query));
            this.watcher.EventArrived += new EventArrivedEventHandler(this.OnEventArrived);
            this.watcher.Start();
        }

        private void OnEventArrived(object sender, EventArrivedEventArgs e)
        {
            string eventName = e.NewEvent.ClassPath.ClassName;


            if (eventName.CompareTo("__InstanceDeletionEvent") == 0)
            {
                this.settings.proAppShutdown(this.name);
                this.warn = false;

                if (this.stopWatcher)
                {
                    this.watcher.Stop();
                }

            } 
            else if(!this.warn)
            {
                
                if (this.stopWatcher)
                {
                    this.watcher.Stop();
                } else
                {
                    this.warn= true;
                    this.settings.proAppRun(this.name);
                }
            }
        }
    }
}
