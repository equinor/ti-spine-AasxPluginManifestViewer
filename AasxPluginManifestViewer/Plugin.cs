/*
Copyright (c) 2018-2021 Festo AG & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AasxPluginManifestViewer;
using AdminShellNS;
using JetBrains.Annotations;

namespace AasxIntegrationBase // the namespace has to be: AasxIntegrationBase
{
    [UsedImplicitlyAttribute]
    // the class names has to be: AasxPlugin and subclassing IAasxPluginInterface
    public class AasxPlugin : IAasxPluginInterface
    {
        public LogInstance Log = new LogInstance();
        private bool stop = false;
        private ManifestViewerOptions options;
        private ManifestViewerViewControl viewControl;
        private PluginEventStack eventStack = new PluginEventStack();


        public string GetPluginName()
        {
            Log.Info("GetPluginName() = {0}", "AasxPluginManifestViewer");
            return "AasxPluginManifestViewer";
        }

        public bool GetPluginCheckForVisualExtension() { return false; }
        public AasxPluginVisualElementExtension CheckForVisualExtension(object referableDisplayed) { return null; }

        public void InitPlugin(string[] args)
        {
            Log.Info("InitPlugin() called with args = {0}", (args == null) ? "" : string.Join(", ", args));
            options = ManifestViewerOptions.CreateDefault();
        }

        public object CheckForLogMessage()
        {
            return Log.PopLastShortTermPrint();
        }

        public AasxPluginActionDescriptionBase[] ListActions()
        {
            Log.Info("ListActions() called");
            var res = new List<AasxPluginActionDescriptionBase>();
            res.Add(new AasxPluginActionDescriptionBase("server-start", "Sample server function doing nothing."));
            res.Add(new AasxPluginActionDescriptionBase("server-stop", "Stops sample server function."));
            res.Add(
                new AasxPluginActionDescriptionBase(
                    "get-check-visual-extension", 
                    "Returns true, if plug-ins checks for visual extension."));
            res.Add(
                new AasxPluginActionDescriptionBase(
                    "fill-panel-visual-extension",
                    "When called, fill given WPF panel with control for graph display."));
            res.Add(
                new AasxPluginActionDescriptionBase(
                    "call-check-visual-extension",
                    "When called with Referable, returns possibly visual extension for it."));
            return res.ToArray();
        }

        public AasxPluginResultBase ActivateAction(string action, params object[] args)
        {

            if (action == "call-check-visual-extension")
            {
                // arguments
                if (args.Length < 1)
                    return null;

                // looking only for Submodels
                var sm = args[0] as AdminShell.Submodel;
                if (sm == null)
                    return null;

                // check for a record in options, that matches Submodel
                var found = false;
                if (this.options != null && this.options.Records != null)
                    foreach (var rec in this.options.Records)
                        if (rec.AllowSubmodelSemanticId != null)
                            foreach (var x in rec.AllowSubmodelSemanticId)
                                if (sm.semanticId != null && sm.semanticId.Matches(x))
                                {
                                    found = true;
                                    break;
                                }
                if (!found)
                    return null;

                // success prepare record
                var cve = new AasxPluginResultVisualExtension("MAN", "Manifest");

                // ok
                return cve;
            }
            if (action == "get-check-visual-extension")
            {
                var cve = new AasxPluginResultBaseObject();
                cve.strType = "True";
                cve.obj = true;
                return cve;
            }
            if (action == "fill-panel-visual-extension" && args != null && args.Length >= 3)
            {
                // access
                var package = args[0] as AdminShellPackageEnv;
                var sm = args[1] as AdminShell.Submodel;
                var master = args[2] as DockPanel;
                if (package == null || sm == null || master == null)
                    return null;

                // the Submodel elements need to have parents
                sm.SetAllParents();

                // create TOP control
                this.viewControl = new ManifestViewerViewControl();
                this.viewControl.Start(package, sm, options, eventStack);
                master.Children.Add(this.viewControl);

                // give object back
                var pluginBase = new AasxPluginResultBaseObject();
                pluginBase.obj = this.viewControl;
                return pluginBase;
            }
            if (action == "server-stop")
                this.stop = true;

            if (action == "server-start")
            {
                this.stop = false;
                Log.Info("This is a (empty) sample server demonstrating the plugin capabilities, only.");
                int i = 0;
                while (true)
                {
                    if (this.stop)
                    {
                        Log.Info("Stopping ...");
                        break;
                    }
                    System.Threading.Thread.Sleep(50);
                    if (i % 20 == 0)
                        Log.Info("Heartbeat {0} ..", i);
                    i++;
                }
                Log.Info("Stopped.");
            }

            var res = new AasxPluginResultBase();
            return res;
        }
    }
}
