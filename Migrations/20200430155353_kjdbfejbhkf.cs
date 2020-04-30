using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyProject.Migrations
{
    public partial class kjdbfejbhkf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Messages_MessageId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_MessageId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Comments");

            migrationBuilder.CreateTable(
                name: "MComments",
                columns: table => new
                {
                    MCommentId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MContent = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    MessageId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MComments", x => x.MCommentId);
                    table.ForeignKey(
                        name: "FK_MComments_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MComments_MessageId",
                table: "MComments",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MComments_UserId",
                table: "MComments",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MComments");

            migrationBuilder.AddColumn<int>(
                name: "MessageId",
                table: "Comments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_MessageId",
                table: "Comments",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Messages_MessageId",
                table: "Comments",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "MessageId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
