namespace ACCIFCConverter.Domain.Models;

public sealed class ExportOptions
{
    public string Preset { get; set; } = "IFC4 Reference";
    public string? JsonConfigPath { get; set; }
    public string? UserDefinedPropertySetsPath { get; set; }
    public string? ParameterMappingPath { get; set; }
    public string? FamilyMappingPath { get; set; }
    public string NamingTemplate { get; set; } = "{Project}_{Model}_{Date}.ifc";

    public bool UseJsonConfig => !string.IsNullOrWhiteSpace(JsonConfigPath);
    public bool EnableUserDefinedPropertySets => !string.IsNullOrWhiteSpace(UserDefinedPropertySetsPath);
    public bool EnableParameterMapping => !string.IsNullOrWhiteSpace(ParameterMappingPath);
    public bool EnableFamilyMapping => !string.IsNullOrWhiteSpace(FamilyMappingPath);
}
