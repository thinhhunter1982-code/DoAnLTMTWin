using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnLTMTWin.Migrations
{
    /// <inheritdoc />
    public partial class ThemBangTaiKhoanQuyen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaiKhoanQuyens",
                columns: table => new
                {
                    TaiKhoanQuyenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    ChucNang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuocTruyCap = table.Column<bool>(type: "bit", nullable: false),
                    DuocSua = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoanQuyens", x => x.TaiKhoanQuyenId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaiKhoanQuyens");
        }
    }
}
