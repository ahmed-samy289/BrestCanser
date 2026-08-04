using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrestCanser.Api.Persistance.EntitiesConfigurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        var claims = new List<IdentityRoleClaim<string>>();
        var id = 1;

        // Admin -> All Permissions
        foreach (var permission in Permissions.GetAllPermissions())
        {
            claims.Add(new IdentityRoleClaim<string>
            {
                Id = id++,
                RoleId = DefaultRoles.AdminRoleId,
                ClaimType = Permissions.Type,
                ClaimValue = permission
            });
        }

        // Member -> Allowed Permissions
        var memberPermissions = new[]
        {
            Permissions.GetProfile,
            Permissions.UpdateProfile,
            Permissions.ChangePassword,

            Permissions.AskChat,
            Permissions.RunPrediction,

            Permissions.GetNotifications,
            Permissions.MarkNotificationAsRead,
            Permissions.MarkAllNotificationsAsRead,

            Permissions.GetPredictionHistory,
            Permissions.GetPredictionHistoryWithStatus,
            Permissions.GetPredictionHistoryStatistics,
            Permissions.GetPredictionHistoryReport,

            Permissions.RiskAssessment
        };

        foreach (var permission in memberPermissions)
        {
            claims.Add(new IdentityRoleClaim<string>
            {
                Id = id++,
                RoleId = DefaultRoles.MemberRoleId,
                ClaimType = Permissions.Type,
                ClaimValue = permission
            });
        }

        builder.HasData(claims);
    }
}