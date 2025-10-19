﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOTNET_PROJECT.Migrations
{
    /// <inheritdoc />
    public partial class AddGameSaveEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameSaves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerCharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentStoryNodeId = table.Column<int>(type: "INTEGER", nullable: false),
                    SaveName = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSaves", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSaves_Characters_PlayerCharacterId",
                        column: x => x.PlayerCharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameSaves_StoryNodes_CurrentStoryNodeId",
                        column: x => x.CurrentStoryNodeId,
                        principalTable: "StoryNodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameSaves_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameSaves_CurrentStoryNodeId",
                table: "GameSaves",
                column: "CurrentStoryNodeId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaves_PlayerCharacterId",
                table: "GameSaves",
                column: "PlayerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSaves_UserId",
                table: "GameSaves",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSaves");
        }
    }
}
