//------------------------------------------------------------------------------
// <copyright file="DebuggingConfigControl.xaml.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace FastDebuggingConfig
{
    using EnvDTE;
    using EnvDTE80;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.VCProjectEngine;
    using System;
    using System.Timers;
    using System.Windows.Controls;


    public partial class DebuggingConfigControl : UserControl
    {
        private System.Timers.Timer aTimer = new System.Timers.Timer();
        private bool isWD = false;
        private string currentWriting = "";
        public DebuggingConfigControl()
        {
            this.InitializeComponent();

            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if(isWD)
            {
                SetWorkingDirectory(currentWriting);
            }
            else
            {
                SetCommandArguments(currentWriting);
            }

            aTimer.Stop();
        }

        private void TextBox_TextChanged_WD(object sender, TextChangedEventArgs e)
        {
            isWD = true;
            currentWriting = textboxwd.Text;
            aTimer.Start();
        }

        private void TextBox_TextChanged_CLA(object sender, TextChangedEventArgs e)
        {
            isWD = false;
            currentWriting = textboxcla.Text;
            aTimer.Start();
        }

        private Project GetStartUpProject()
        {
            DTE2 dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            SolutionBuild2 sb = (SolutionBuild2)dte2.Solution.SolutionBuild;
            if(sb.StartupProjects != null)
            {
                string name = "";
                foreach (String s in (Array)sb.StartupProjects)
                {
                    name += s;
                }

                if(string.IsNullOrEmpty(name))
                {
                    return null;
                }

                return dte2.Solution.Item(name);
            }

            return null;
        }

        private void SetCommandArguments(string value)
        {
            Project startupProj = GetStartUpProject();
            if (startupProj != null)
            {
                VCProject vcProj = startupProj.Object as VCProject;
                if (vcProj != null)
                {
                    foreach (VCConfiguration config in vcProj.Configurations)
                    {
                        VCDebugSettings debugSetting = config.DebugSettings;
                        debugSetting.CommandArguments = value;
                    }
                }
                else
                {
                    Configuration configuration = startupProj.ConfigurationManager.ActiveConfiguration;
                    configuration.Properties.Item("StartArguments").Value = value;
                }
            }
        }

        private void SetWorkingDirectory(string value)
        {
            Project startupProj = GetStartUpProject();
            if (startupProj != null)
            {
                VCProject vcProj = startupProj.Object as VCProject;
                if (vcProj != null)
                {
                    foreach (VCConfiguration config in vcProj.Configurations)
                    {
                        VCDebugSettings debugSetting = config.DebugSettings;
                        debugSetting.WorkingDirectory = value;
                    }
                }
                else
                {
                    Configuration configuration = startupProj.ConfigurationManager.ActiveConfiguration;
                    configuration.Properties.Item("StartWorkingDirectory").Value = value;
                }
            }
        }
    }
}