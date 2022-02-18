using System.Collections.Generic;
using AdminShellNS;

namespace AasxIntegrationBase
{
    public class ManifestViewerOptionsRecord
    {
        public List<AdminShell.Key> AllowSubmodelSemanticId = new List<AdminShell.Key>();
    }
    public class ManifestViewerOptions
    {
        public List<ManifestViewerOptionsRecord> Records { get; set; }
        public static ManifestViewerOptions CreateDefault()
        {
            var options = new ManifestViewerOptions();
            options.Records = new List<ManifestViewerOptionsRecord>()
            {
                new ManifestViewerOptionsRecord()
                {
                    AllowSubmodelSemanticId = new List<AdminShell.Key>()
                    {
                        new AdminShell.Key()
                        {
                            idType = "IRI",
                            local = false,
                            type = "GlobalReference",
                            value = Constants.ManifestSemanticId
                        }
                    }
                }
            };

            return options;
        }
    }
}