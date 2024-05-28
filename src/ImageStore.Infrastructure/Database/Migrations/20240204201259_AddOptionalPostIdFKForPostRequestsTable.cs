using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImageStore.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOptionalPostIdFKForPostRequestsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "PostRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostRequests_PostId",
                table: "PostRequests",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostRequests_Posts_PostId",
                table: "PostRequests",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostRequests_Posts_PostId",
                table: "PostRequests");

            migrationBuilder.DropIndex(
                name: "IX_PostRequests_PostId",
                table: "PostRequests");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "PostRequests");
        }
    }
}
