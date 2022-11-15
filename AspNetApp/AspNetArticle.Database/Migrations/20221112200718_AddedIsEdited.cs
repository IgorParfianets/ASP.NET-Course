using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetArticle.Database.Migrations
{
    public partial class AddedIsEdited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Comments");
        }
    }
}
