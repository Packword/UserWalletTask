using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UserWallet.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currencies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    username = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    password = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    role = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "deposits",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    currency_id = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    additional_data = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deposits", x => x.id);
                    table.ForeignKey(
                        name: "FK_deposits_currencies_currency_id",
                        column: x => x.currency_id,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_deposits_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userBalances",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    currency_id = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userBalances", x => new { x.user_id, x.currency_id });
                    table.ForeignKey(
                        name: "FK_userBalances_currencies_currency_id",
                        column: x => x.currency_id,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userBalances_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_deposits_currency_id",
                table: "deposits",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "IX_deposits_user_id",
                table: "deposits",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_userBalances_currency_id",
                table: "userBalances",
                column: "currency_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deposits");

            migrationBuilder.DropTable(
                name: "userBalances");

            migrationBuilder.DropTable(
                name: "currencies");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
