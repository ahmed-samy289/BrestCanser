using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrestCanser.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class seedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4f570688-9ed8-474f-9975-1317c87f1f7f", "e566a007-a817-454d-bb8f-10d1f09e5715", false, false, "Admin", "ADMIN" },
                    { "ce197ed8-850f-4272-880f-5b929837ff9e", "e108b4c9-9c16-4396-a91a-39a3b9e9c11d", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "e232de14-def2-4d76-9dcb-05f0eb7619a1", 0, "5abffc5a-7e3a-4963-a4e3-755bdda4e3d5", "admin@breast-cancer.com", true, "Breast Cancer", "Admin", false, null, "ADMIN@BREAST-CANCER.COM", "ADMIN@BREAST-CANCER.COM", "AQAAAAIAAYagAAAAEIhTUhCbYnJnXakFznPpUk/iFzWU4Nc1/liV2gScq58jo1H/oKRb+IgzqeLkH5rJqw==", null, false, "AC6C88E23E674BC0AA53B4B195D91D56", false, "admin@breast-cancer.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "account:profile", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 2, "permissions", "account:update-profile", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 3, "permissions", "account:change-password", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 4, "permissions", "chat:ask", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 5, "permissions", "ml:predict", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 6, "permissions", "notifications:read", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 7, "permissions", "notifications:mark-read", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 8, "permissions", "notifications:mark-all-read", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 9, "permissions", "prediction-history:read", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 10, "permissions", "prediction-history:report", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 11, "permissions", "prediction-history:statistics", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 12, "permissions", "prediction-history:status", "4f570688-9ed8-474f-9975-1317c87f1f7f" },
                    { 13, "permissions", "risk-assessment:create", "4f570688-9ed8-474f-9975-1317c87f1f7f" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "4f570688-9ed8-474f-9975-1317c87f1f7f", "e232de14-def2-4d76-9dcb-05f0eb7619a1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ce197ed8-850f-4272-880f-5b929837ff9e");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4f570688-9ed8-474f-9975-1317c87f1f7f", "e232de14-def2-4d76-9dcb-05f0eb7619a1" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4f570688-9ed8-474f-9975-1317c87f1f7f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "e232de14-def2-4d76-9dcb-05f0eb7619a1");
        }
    }
}
