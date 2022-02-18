using AdminShellNS;

namespace AasxIntegrationBase
{
    public class Constants
    {
        public const string ManifestSemanticId = "https://example.com/Manifest";
        public const string PlantProjectCodeSemanticId = "https://example.com/PlantProjectCode";
        public const string ContractorCodeSemanticId = "https://example.com/ContractorCode";
        public const string ClientProjectNoSemanticId = "https://example.com/ClientPorjectNo";
        public const string PlantCodeSemanticId = "https://example.com/PlantCode";

        public static readonly AdminShell.Key FromLogoSemanticId = new AdminShell.Key()
        {
            type = "GlobalReference",
            local = false,
            idType = "IRI",
            value = "https://example.com/FromContractorLogo"
        };
        public static readonly AdminShell.Key ToLogoSemanticId = new AdminShell.Key()
        {
            type = "GlobalReference",
            local = false,
            idType = "IRI",
            value = "https://example.com/ToContractorLogo"
        };
    }
}