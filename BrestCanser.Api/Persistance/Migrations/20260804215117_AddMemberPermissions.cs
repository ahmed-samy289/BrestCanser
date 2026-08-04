using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BrestCanser.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 14, "permissions", "account:profile", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 15, "permissions", "account:update-profile", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 16, "permissions", "account:change-password", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 17, "permissions", "chat:ask", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 18, "permissions", "ml:predict", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 19, "permissions", "notifications:read", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 20, "permissions", "notifications:mark-read", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 21, "permissions", "notifications:mark-all-read", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 22, "permissions", "prediction-history:read", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 23, "permissions", "prediction-history:status", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 24, "permissions", "prediction-history:statistics", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 25, "permissions", "prediction-history:report", "ce197ed8-850f-4272-880f-5b929837ff9e" },
                    { 26, "permissions", "risk-assessment:create", "ce197ed8-850f-4272-880f-5b929837ff9e" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 26);
        }
    }
}
