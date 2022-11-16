using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetArticle.Database.Migrations
{
    public partial class AddToArticleColumnRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rate",
                table: "Articles",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Articles");
        }
    }
}
