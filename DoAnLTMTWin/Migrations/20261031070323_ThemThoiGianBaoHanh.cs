using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnLTMTWin.Migrations
{
    /// <inheritdoc />
    public partial class ThemThoiGianBaoHanh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThoiGianBaoHanh",
                table: "DienThoais",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThoiGianBaoHanh",
                table: "DienThoais");
        }
    }
}
