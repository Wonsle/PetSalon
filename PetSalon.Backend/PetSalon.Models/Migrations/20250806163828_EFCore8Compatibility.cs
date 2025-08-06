using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetSalon.Models.Migrations
{
    /// <inheritdoc />
    public partial class EFCore8Compatibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CodeType",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, comment: "代碼類型代碼"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValueSql: "('')", comment: "代碼類型名稱"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, defaultValueSql: "('')", comment: "描述說明"),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "建立者"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "建立時間"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "修改者"),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "修改時間")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CodeType__3214EC270F975522", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ContactPerson",
                columns: table => new
                {
                    ContactPersonID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, comment: "名字"),
                    NickName = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, comment: "暱稱"),
                    ContactNumber = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false, comment: "連絡電話"),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPerson", x => x.ContactPersonID);
                });

            migrationBuilder.CreateTable(
                name: "Pet",
                columns: table => new
                {
                    PetID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "名字"),
                    Gender = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, comment: "性別"),
                    Breed = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, comment: "品種"),
                    BirthDay = table.Column<DateTime>(type: "date", nullable: true, comment: "生日"),
                    NormalPrice = table.Column<decimal>(type: "money", nullable: true, comment: "單次價格"),
                    SubscriptionPrice = table.Column<decimal>(type: "money", nullable: true, comment: "包月價格"),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pet", x => x.PetID);
                });

            migrationBuilder.CreateTable(
                name: "SCRole",
                columns: table => new
                {
                    RoleID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SCRole__8AFACE3A6F25AC8C", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "SCUser",
                columns: table => new
                {
                    SCUserID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    LastLogin = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCUser", x => x.SCUserID);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    ServiceID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "服務項目名稱"),
                    ServiceType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, comment: "服務類型，關聯至SystemCode"),
                    BasePrice = table.Column<decimal>(type: "money", nullable: false, comment: "基礎價格"),
                    Duration = table.Column<int>(type: "int", nullable: false, comment: "服務時長（分鐘）"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "服務描述"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "是否啟用"),
                    Sort = table.Column<int>(type: "int", nullable: false, defaultValue: 0, comment: "顯示排序"),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "建立者"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "建立時間"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "修改者"),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "修改時間")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.ServiceID);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionType",
                columns: table => new
                {
                    SubscriptionTypeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultUsageLimit = table.Column<int>(type: "int", nullable: false),
                    DefaultPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AvailableServiceTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionType", x => x.SubscriptionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SystemCode",
                columns: table => new
                {
                    CodeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeType = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValueSql: "('')"),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, defaultValueSql: "('')"),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Sort = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, defaultValueSql: "('')"),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SystemCo__C6DE2C358B5ECB32", x => x.CodeID);
                });

            migrationBuilder.CreateTable(
                name: "PetRelation",
                columns: table => new
                {
                    PetRelationID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetID = table.Column<long>(type: "bigint", nullable: false),
                    ContactPersonID = table.Column<long>(type: "bigint", nullable: false),
                    RelationshipType = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "OWNER", comment: "關係類型"),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetRelation", x => x.PetRelationID);
                    table.ForeignKey(
                        name: "FK_PetRelation_ContactPerson",
                        column: x => x.ContactPersonID,
                        principalTable: "ContactPerson",
                        principalColumn: "ContactPersonID");
                    table.ForeignKey(
                        name: "FK_PetRelation_Pet",
                        column: x => x.PetID,
                        principalTable: "Pet",
                        principalColumn: "PetID");
                });

            migrationBuilder.CreateTable(
                name: "PetServiceDuration",
                columns: table => new
                {
                    PetServiceDurationID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetID = table.Column<long>(type: "bigint", nullable: false),
                    ServiceID = table.Column<long>(type: "bigint", nullable: false),
                    CustomDuration = table.Column<int>(type: "int", nullable: true, comment: "客製化時長，覆蓋服務預設時長（以分鐘為單位）"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "備註說明"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "是否啟用"),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "建立者"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "建立時間"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "修改者"),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "修改時間")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetServiceDuration", x => x.PetServiceDurationID);
                    table.ForeignKey(
                        name: "FK_PetServiceDuration_Pet",
                        column: x => x.PetID,
                        principalTable: "Pet",
                        principalColumn: "PetID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetServiceDuration_Service",
                        column: x => x.ServiceID,
                        principalTable: "Service",
                        principalColumn: "ServiceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PetServicePrice",
                columns: table => new
                {
                    PetServicePriceID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetID = table.Column<long>(type: "bigint", nullable: false),
                    ServiceID = table.Column<long>(type: "bigint", nullable: false),
                    CustomPrice = table.Column<decimal>(type: "money", nullable: true, comment: "客製化價格，覆蓋服務預設價格"),
                    Duration = table.Column<int>(type: "int", nullable: true, comment: "客製化時長，覆蓋服務預設時長"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "備註說明"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "是否啟用"),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "建立者"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "建立時間"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "修改者"),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "修改時間")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetServicePrice", x => x.PetServicePriceID);
                    table.ForeignKey(
                        name: "FK_PetServicePrice_Pet",
                        column: x => x.PetID,
                        principalTable: "Pet",
                        principalColumn: "PetID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetServicePrice_Service",
                        column: x => x.ServiceID,
                        principalTable: "Service",
                        principalColumn: "ServiceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    SubscriptionID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetID = table.Column<long>(type: "bigint", nullable: false),
                    SubscriptionDate = table.Column<DateTime>(type: "date", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    SubscriptionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionTypeID = table.Column<long>(type: "bigint", nullable: true),
                    TotalUsageLimit = table.Column<int>(type: "int", nullable: false),
                    UsedCount = table.Column<int>(type: "int", nullable: false),
                    ReservedCount = table.Column<int>(type: "int", nullable: false),
                    SubscriptionPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.SubscriptionID);
                    table.ForeignKey(
                        name: "FK_Subscription_Pet",
                        column: x => x.PetID,
                        principalTable: "Pet",
                        principalColumn: "PetID");
                    table.ForeignKey(
                        name: "FK_Subscription_SubscriptionType",
                        column: x => x.SubscriptionTypeID,
                        principalTable: "SubscriptionType",
                        principalColumn: "SubscriptionTypeId");
                });

            migrationBuilder.CreateTable(
                name: "NotificationLog",
                columns: table => new
                {
                    NotificationLogID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    RecipientType = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    RecipientID = table.Column<long>(type: "bigint", nullable: true),
                    RelatedPetID = table.Column<long>(type: "bigint", nullable: true),
                    RelatedSubscriptionID = table.Column<long>(type: "bigint", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SendMethod = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    SendStatus = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    ScheduledTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLog", x => x.NotificationLogID);
                    table.ForeignKey(
                        name: "FK_NotificationLog_ContactPerson",
                        column: x => x.RecipientID,
                        principalTable: "ContactPerson",
                        principalColumn: "ContactPersonID");
                    table.ForeignKey(
                        name: "FK_NotificationLog_Pet",
                        column: x => x.RelatedPetID,
                        principalTable: "Pet",
                        principalColumn: "PetID");
                    table.ForeignKey(
                        name: "FK_NotificationLog_Subscription",
                        column: x => x.RelatedSubscriptionID,
                        principalTable: "Subscription",
                        principalColumn: "SubscriptionID");
                });

            migrationBuilder.CreateTable(
                name: "ReserveRecord",
                columns: table => new
                {
                    ReserveRecordID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetID = table.Column<long>(type: "bigint", nullable: false),
                    SubscriptionID = table.Column<long>(type: "bigint", nullable: true),
                    ReserverDate = table.Column<DateTime>(name: "ReserverDate ", type: "date", nullable: false),
                    ReserverTime = table.Column<TimeSpan>(name: "ReserverTime ", type: "time", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "PENDING"),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0.00m),
                    UseSubscription = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ServiceType = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    SubscriptionDeductionCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Memo = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "('')"),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReserveRecord", x => x.ReserveRecordID);
                    table.ForeignKey(
                        name: "FK_ReserveRecord_Pet_PetID",
                        column: x => x.PetID,
                        principalTable: "Pet",
                        principalColumn: "PetID");
                    table.ForeignKey(
                        name: "FK_ReserveRecord_Subscription_SubscriptionID",
                        column: x => x.SubscriptionID,
                        principalTable: "Subscription",
                        principalColumn: "SubscriptionID");
                });

            migrationBuilder.CreateTable(
                name: "PaymentRecord",
                columns: table => new
                {
                    PaymentRecordID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentCode = table.Column<int>(type: "int", nullable: true),
                    PetID = table.Column<long>(type: "bigint", nullable: true),
                    SubscriptionID = table.Column<long>(type: "bigint", nullable: true),
                    ReserveRecordID = table.Column<long>(type: "bigint", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "SERVICE"),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "CASH"),
                    PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "COMPLETED"),
                    PaymentDate = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceivablePrice = table.Column<decimal>(type: "money", nullable: true),
                    ActualPrice = table.Column<decimal>(type: "money", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentRecord", x => x.PaymentRecordID);
                    table.ForeignKey(
                        name: "FK_PaymentRecord_Pet",
                        column: x => x.PetID,
                        principalTable: "Pet",
                        principalColumn: "PetID");
                    table.ForeignKey(
                        name: "FK_PaymentRecord_ReserveRecord",
                        column: x => x.ReserveRecordID,
                        principalTable: "ReserveRecord",
                        principalColumn: "ReserveRecordID");
                    table.ForeignKey(
                        name: "FK_PaymentRecord_Subscription",
                        column: x => x.SubscriptionID,
                        principalTable: "Subscription",
                        principalColumn: "SubscriptionID");
                });

            migrationBuilder.CreateTable(
                name: "ReservationService",
                columns: table => new
                {
                    ReservationServiceID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReserveRecordID = table.Column<long>(type: "bigint", nullable: false),
                    ServiceID = table.Column<long>(type: "bigint", nullable: false),
                    ServicePrice = table.Column<decimal>(type: "money", nullable: false, comment: "服務價格"),
                    Duration = table.Column<int>(type: "int", nullable: false, comment: "服務時長（分鐘）"),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "備註說明"),
                    CreateUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "建立者"),
                    CreateTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "建立時間"),
                    ModifyUser = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, comment: "修改者"),
                    ModifyTime = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())", comment: "修改時間")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationService", x => x.ReservationServiceID);
                    table.ForeignKey(
                        name: "FK_ReservationService_ReserveRecord",
                        column: x => x.ReserveRecordID,
                        principalTable: "ReserveRecord",
                        principalColumn: "ReserveRecordID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationService_Service",
                        column: x => x.ServiceID,
                        principalTable: "Service",
                        principalColumn: "ServiceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLog_RecipientID",
                table: "NotificationLog",
                column: "RecipientID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLog_RelatedPetID",
                table: "NotificationLog",
                column: "RelatedPetID");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLog_RelatedSubscriptionID",
                table: "NotificationLog",
                column: "RelatedSubscriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRecord_PetID",
                table: "PaymentRecord",
                column: "PetID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRecord_ReserveRecordID",
                table: "PaymentRecord",
                column: "ReserveRecordID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentRecord_SubscriptionID",
                table: "PaymentRecord",
                column: "SubscriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_PetRelation_ContactPersonID",
                table: "PetRelation",
                column: "ContactPersonID");

            migrationBuilder.CreateIndex(
                name: "IX_PetRelation_PetID",
                table: "PetRelation",
                column: "PetID");

            migrationBuilder.CreateIndex(
                name: "IX_PetServiceDuration_PetID",
                table: "PetServiceDuration",
                column: "PetID");

            migrationBuilder.CreateIndex(
                name: "IX_PetServiceDuration_ServiceID",
                table: "PetServiceDuration",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_PetServicePrice_PetID",
                table: "PetServicePrice",
                column: "PetID");

            migrationBuilder.CreateIndex(
                name: "IX_PetServicePrice_ServiceID",
                table: "PetServicePrice",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationService_ReserveRecordID",
                table: "ReservationService",
                column: "ReserveRecordID");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationService_ServiceID",
                table: "ReservationService",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveRecord_PetID",
                table: "ReserveRecord",
                column: "PetID");

            migrationBuilder.CreateIndex(
                name: "IX_ReserveRecord_SubscriptionID",
                table: "ReserveRecord",
                column: "SubscriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_PetID",
                table: "Subscription",
                column: "PetID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SubscriptionTypeID",
                table: "Subscription",
                column: "SubscriptionTypeID");

            migrationBuilder.CreateIndex(
                name: "UQ_CodeType",
                table: "SystemCode",
                columns: new[] { "CodeType", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CodeType");

            migrationBuilder.DropTable(
                name: "NotificationLog");

            migrationBuilder.DropTable(
                name: "PaymentRecord");

            migrationBuilder.DropTable(
                name: "PetRelation");

            migrationBuilder.DropTable(
                name: "PetServiceDuration");

            migrationBuilder.DropTable(
                name: "PetServicePrice");

            migrationBuilder.DropTable(
                name: "ReservationService");

            migrationBuilder.DropTable(
                name: "SCRole");

            migrationBuilder.DropTable(
                name: "SCUser");

            migrationBuilder.DropTable(
                name: "SystemCode");

            migrationBuilder.DropTable(
                name: "ContactPerson");

            migrationBuilder.DropTable(
                name: "ReserveRecord");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "Pet");

            migrationBuilder.DropTable(
                name: "SubscriptionType");
        }
    }
}
