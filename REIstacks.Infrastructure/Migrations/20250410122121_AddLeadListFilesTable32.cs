using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadListFilesTable32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "marketing_campaigns",
                newName: "TargetCriteria");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "marketing_campaigns",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<decimal>(
                name: "ActualCost",
                table: "marketing_campaigns",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CampaignType",
                table: "marketing_campaigns",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DealsGenerated",
                table: "marketing_campaigns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "marketing_campaigns",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "marketing_campaigns",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AlternatePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Company = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreferredContactMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StreetAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ContactType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LeadSource = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Clicks = table.Column<int>(type: "int", nullable: true),
                    Opens = table.Column<int>(type: "int", nullable: true),
                    SMSResponses = table.Column<int>(type: "int", nullable: true),
                    CallsMade = table.Column<int>(type: "int", nullable: true),
                    MessagesLeft = table.Column<int>(type: "int", nullable: true),
                    LastContacted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConsentTextMessages = table.Column<bool>(type: "bit", nullable: false),
                    ConsentEmailMarketing = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contacts_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaign_contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastContactedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_campaign_contacts_contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_campaign_contacts_marketing_campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "marketing_campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerContactId = table.Column<int>(type: "int", nullable: true),
                    StreetAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    County = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PropertyCondition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Bedrooms = table.Column<int>(type: "int", nullable: true),
                    Bathrooms = table.Column<int>(type: "int", nullable: true),
                    SquareFootage = table.Column<int>(type: "int", nullable: true),
                    LotSize = table.Column<int>(type: "int", nullable: true),
                    YearBuilt = table.Column<int>(type: "int", nullable: true),
                    EstimatedARV = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedRepairCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ListPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SellerAskingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PropertyIssues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcquisitionStrategy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_properties_contacts_OwnerContactId",
                        column: x => x.OwnerContactId,
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_properties_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    SellerContactId = table.Column<int>(type: "int", nullable: true),
                    BuyerContactId = table.Column<int>(type: "int", nullable: true),
                    DealName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DealType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DealStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DealStage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AssignmentFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RehabBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualRehabCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ContractDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualClosingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForSelling = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timeline = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_deals_contacts_BuyerContactId",
                        column: x => x.BuyerContactId,
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_deals_contacts_SellerContactId",
                        column: x => x.SellerContactId,
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_deals_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_deals_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "property_documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BlobUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_documents_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "communications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    DealId = table.Column<int>(type: "int", nullable: true),
                    CommunicationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CommunicationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Direction = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_communications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_communications_contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_communications_deals_DealId",
                        column: x => x.DealId,
                        principalTable: "deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_communications_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deal_documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BlobUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deal_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_deal_documents_deals_DealId",
                        column: x => x.DealId,
                        principalTable: "deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "task_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    PropertyId = table.Column<int>(type: "int", nullable: true),
                    DealId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaskStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_task_items_contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_task_items_deals_DealId",
                        column: x => x.DealId,
                        principalTable: "deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_task_items_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_task_items_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaign_contacts_CampaignId",
                table: "campaign_contacts",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_contacts_ContactId",
                table: "campaign_contacts",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_communications_ContactId",
                table: "communications",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_communications_DealId",
                table: "communications",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_communications_OrganizationId",
                table: "communications",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_contacts_OrganizationId_LastName_FirstName",
                table: "contacts",
                columns: new[] { "OrganizationId", "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_deal_documents_DealId",
                table: "deal_documents",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_deals_BuyerContactId",
                table: "deals",
                column: "BuyerContactId");

            migrationBuilder.CreateIndex(
                name: "IX_deals_OrganizationId_DealStatus",
                table: "deals",
                columns: new[] { "OrganizationId", "DealStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_deals_PropertyId",
                table: "deals",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_deals_SellerContactId",
                table: "deals",
                column: "SellerContactId");

            migrationBuilder.CreateIndex(
                name: "IX_properties_OrganizationId_City_State_ZipCode",
                table: "properties",
                columns: new[] { "OrganizationId", "City", "State", "ZipCode" });

            migrationBuilder.CreateIndex(
                name: "IX_properties_OwnerContactId",
                table: "properties",
                column: "OwnerContactId");

            migrationBuilder.CreateIndex(
                name: "IX_property_documents_PropertyId",
                table: "property_documents",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_task_items_ContactId",
                table: "task_items",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_task_items_DealId",
                table: "task_items",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_task_items_OrganizationId_DueDate_TaskStatus",
                table: "task_items",
                columns: new[] { "OrganizationId", "DueDate", "TaskStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_task_items_PropertyId",
                table: "task_items",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_contacts");

            migrationBuilder.DropTable(
                name: "communications");

            migrationBuilder.DropTable(
                name: "deal_documents");

            migrationBuilder.DropTable(
                name: "property_documents");

            migrationBuilder.DropTable(
                name: "task_items");

            migrationBuilder.DropTable(
                name: "deals");

            migrationBuilder.DropTable(
                name: "properties");

            migrationBuilder.DropTable(
                name: "contacts");

            migrationBuilder.DropColumn(
                name: "ActualCost",
                table: "marketing_campaigns");

            migrationBuilder.DropColumn(
                name: "CampaignType",
                table: "marketing_campaigns");

            migrationBuilder.DropColumn(
                name: "DealsGenerated",
                table: "marketing_campaigns");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "marketing_campaigns");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "marketing_campaigns");

            migrationBuilder.RenameColumn(
                name: "TargetCriteria",
                table: "marketing_campaigns",
                newName: "Type");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "marketing_campaigns",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
