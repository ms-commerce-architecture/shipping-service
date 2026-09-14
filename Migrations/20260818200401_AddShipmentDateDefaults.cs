using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shipping_service_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddShipmentDateDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trackingevents_shipments_ShipmentId",
                table: "trackingevents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_trackingevents",
                table: "trackingevents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_shipments",
                table: "shipments");

            migrationBuilder.RenameTable(
                name: "trackingevents",
                newName: "TrackingEvents");

            migrationBuilder.RenameTable(
                name: "shipments",
                newName: "Shipments");

            migrationBuilder.RenameColumn(
                name: "ShipmentId",
                table: "TrackingEvents",
                newName: "shipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_trackingevents_ShipmentId",
                table: "TrackingEvents",
                newName: "IX_TrackingEvents_shipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_shipments_OrderNumber",
                table: "Shipments",
                newName: "IX_Shipments_OrderNumber");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "TrackingEvents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "Shipments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Shipments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrackingEvents",
                table: "TrackingEvents",
                column: "EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shipments",
                table: "Shipments",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_TrackingNumber",
                table: "Shipments",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackingEvents_Shipments_shipmentId",
                table: "TrackingEvents",
                column: "shipmentId",
                principalTable: "Shipments",
                principalColumn: "ShipmentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackingEvents_Shipments_shipmentId",
                table: "TrackingEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrackingEvents",
                table: "TrackingEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shipments",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_TrackingNumber",
                table: "Shipments");

            migrationBuilder.RenameTable(
                name: "TrackingEvents",
                newName: "trackingevents");

            migrationBuilder.RenameTable(
                name: "Shipments",
                newName: "shipments");

            migrationBuilder.RenameColumn(
                name: "shipmentId",
                table: "trackingevents",
                newName: "ShipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_TrackingEvents_shipmentId",
                table: "trackingevents",
                newName: "IX_trackingevents_ShipmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Shipments_OrderNumber",
                table: "shipments",
                newName: "IX_shipments_OrderNumber");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "trackingevents",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "TrackingNumber",
                table: "shipments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "shipments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_trackingevents",
                table: "trackingevents",
                column: "EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_shipments",
                table: "shipments",
                column: "ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_trackingevents_shipments_ShipmentId",
                table: "trackingevents",
                column: "ShipmentId",
                principalTable: "shipments",
                principalColumn: "ShipmentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
