namespace PetSalon.Models.Authorization;

public static class PermissionCodes
{
    public const string Login = "auth.login";
    public const string ManageSystemSettings = "system.settings.manage";
    public const string ManageAccounts = "accounts.manage";
    public const string ManagePermissions = "permissions.manage";
    public const string PermanentlyDeleteFiles = "files.delete.permanent";
    public const string ReadFinancialData = "finance.read";

    public static IReadOnlyList<string> Baseline { get; } =
    [
        Login,
        ManageSystemSettings,
        ManageAccounts,
        ManagePermissions,
        PermanentlyDeleteFiles,
        ReadFinancialData
    ];
}
