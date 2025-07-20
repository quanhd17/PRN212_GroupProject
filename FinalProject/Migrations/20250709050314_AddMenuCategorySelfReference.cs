using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuCategorySelfReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentCategoryId",
                table: "MenuCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCategories_ParentCategoryId",
                table: "MenuCategories",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuCategories_MenuCategories_ParentCategoryId",
                table: "MenuCategories",
                column: "ParentCategoryId",
                principalTable: "MenuCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuCategories_MenuCategories_ParentCategoryId",
                table: "MenuCategories");

            migrationBuilder.DropIndex(
                name: "IX_MenuCategories_ParentCategoryId",
                table: "MenuCategories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "MenuCategories");
        }
    }
}
