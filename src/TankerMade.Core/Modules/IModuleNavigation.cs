namespace TankerMade.Core.Modules;

public interface IModuleNavigation
{
    string NavigationLabel { get; }
    string NavigationRoute { get; }
    int NavigationOrder { get; }
}
