using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAnLTMTWin.Migrations
{
    /// <inheritdoc />
    public partial class DoiEmailThanhChucVu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "NhanViens",
                newName: "ChucVu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChucVu",
                table: "NhanViens",
                newName: "Email");
        }
    }
}
