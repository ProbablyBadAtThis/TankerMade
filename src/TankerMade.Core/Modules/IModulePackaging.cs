namespace TankerMade.Core.Modules;

public interface IModulePackaging
{
    string ManifestVersion { get; }
    string MinHostVersion { get; }
    string? MaxHostVersion { get; }
}
