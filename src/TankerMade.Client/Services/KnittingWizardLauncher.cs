using Microsoft.AspNetCore.Components;
using MudBlazor;
using TankerMade.Client.Components.Knitting.Wizards;

namespace TankerMade.Client.Services;

public enum KnittingWizardKind
{
    Project,
    Pattern,
    Kit
}

public sealed class KnittingWizardLauncher
{
    private readonly IDialogService _dialogService;
    private readonly NavigationManager _navigation;

    public KnittingWizardLauncher(IDialogService dialogService, NavigationManager navigation)
    {
        _dialogService = dialogService;
        _navigation = navigation;
    }

    public Task OpenAsync(KnittingWizardKind kind) =>
        kind switch
        {
            KnittingWizardKind.Project => OpenProjectWizardAsync(),
            KnittingWizardKind.Pattern => OpenPatternWizardAsync(),
            KnittingWizardKind.Kit => OpenKitWizardAsync(),
            _ => Task.CompletedTask
        };

    public async Task OpenProjectWizardAsync()
    {
        var dialog = await _dialogService.ShowAsync<KnittingProjectWizardDialog>(string.Empty, CreateDialogOptions());
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: Guid projectId })
        {
            _navigation.NavigateTo($"/modules/knitting/projects/{projectId}");
        }
    }

    public async Task OpenPatternWizardAsync()
    {
        var dialog = await _dialogService.ShowAsync<KnittingPatternWizardDialog>(string.Empty, CreateDialogOptions());
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: Guid patternId })
        {
            _navigation.NavigateTo($"/modules/knitting/patterns/{patternId}");
        }
    }

    public async Task OpenKitWizardAsync()
    {
        var dialog = await _dialogService.ShowAsync<KnittingKitWizardDialog>(string.Empty, CreateDialogOptions());
        var result = await dialog.Result;
        if (result is { Canceled: false, Data: KnittingKitWizardResult kitResult })
        {
            var currentPath = _navigation.ToBaseRelativePath(_navigation.Uri).TrimEnd('/');
            var targetPath = _navigation.ToBaseRelativePath(kitResult.NavigateTo).TrimEnd('/');
            var forceReload = string.Equals(currentPath, targetPath, StringComparison.OrdinalIgnoreCase);
            _navigation.NavigateTo(kitResult.NavigateTo, forceReload);
        }
    }

    public static bool TryParseRoute(string relativePath, out KnittingWizardKind kind)
    {
        kind = default;
        var path = relativePath.Trim().Trim('/').ToLowerInvariant();

        return path switch
        {
            "modules/knitting/projects/new" => Set(KnittingWizardKind.Project, out kind),
            "modules/knitting/patterns/new" => Set(KnittingWizardKind.Pattern, out kind),
            "modules/knitting/kits/new" => Set(KnittingWizardKind.Kit, out kind),
            _ => false
        };
    }

    public static string GetListRoute(KnittingWizardKind kind) =>
        kind switch
        {
            KnittingWizardKind.Project => "/modules/knitting/projects",
            KnittingWizardKind.Pattern => "/modules/knitting/patterns",
            KnittingWizardKind.Kit => "/modules/knitting/kits",
            _ => "/modules/knitting"
        };

    private static bool Set(KnittingWizardKind value, out KnittingWizardKind kind)
    {
        kind = value;
        return true;
    }

    private static DialogOptions CreateDialogOptions() =>
        new()
        {
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            CloseButton = true,
            CloseOnEscapeKey = true,
            BackdropClick = true,
            BackgroundClass = "knitting-wizard-dialog-backdrop"
        };
}

public sealed record KnittingKitWizardResult(string NavigateTo);
