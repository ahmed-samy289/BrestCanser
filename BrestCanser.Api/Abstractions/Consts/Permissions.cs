namespace BrestCanser.Api.Abstractions.Consts;

public static class Permissions
{
    public static string Type { get; } = "permissions";

    public const string GetProfile = "account:profile";
    public const string UpdateProfile = "account:update-profile";
    public const string ChangePassword = "account:change-password";

    public const string AskChat = "chat:ask";

    public const string RunPrediction = "ml:predict";

    public const string GetNotifications = "notifications:read";
    public const string MarkNotificationAsRead = "notifications:mark-read";
    public const string MarkAllNotificationsAsRead = "notifications:mark-all-read";

    public const string GetPredictionHistory = "prediction-history:read";
    public const string GetPredictionHistoryReport = "prediction-history:report";
    public const string GetPredictionHistoryStatistics = "prediction-history:statistics";
    public const string GetPredictionHistoryWithStatus = "prediction-history:status";

    public const string RiskAssessment = "risk-assessment:create";

    public static IList<string?> GetAllPermissions() =>
        typeof(Permissions)
            .GetFields()
            .Where(x => x.IsLiteral && !x.IsInitOnly)
            .Select(x => x.GetValue(null) as string)
            .ToList();
}