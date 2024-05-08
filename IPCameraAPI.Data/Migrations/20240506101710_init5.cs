using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPCameraAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class init5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdminUserId",
                table: "SubscriptionPlans",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_AdminUserId",
                table: "SubscriptionPlans",
                column: "AdminUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionPlans_AdminUsers_AdminUserId",
                table: "SubscriptionPlans",
                column: "AdminUserId",
                principalTable: "AdminUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionPlans_AdminUsers_AdminUserId",
                table: "SubscriptionPlans");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionPlans_AdminUserId",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "AdminUserId",
                table: "SubscriptionPlans");
        }
    }
}
