using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetArticle.Database.Migrations
{
    public partial class UpdateArticleWithSourceUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullText",
                table: "Articles",
                newName: "SourceUrl");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "SourceUrl",
                table: "Articles",
                newName: "FullText");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
