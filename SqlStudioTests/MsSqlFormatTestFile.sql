USE [sD2]
GO

/****** Object:  Table [dbo].[Pick]    Script Date: 25.05.2022 09:54:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pick](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderId] [bigint] NOT NULL,
	[OrderLineId] [bigint] NOT NULL,
	[Amount] [decimal](38, 20) NOT NULL,
	[UnitOfMeasure] [int] NOT NULL,
	[SerialNumber] [nvarchar](50) NULL,
	[BatchLot] [nvarchar](20) NULL,
	[SellBy] [datetime] NULL,
	[PalletNumber] [int] NULL,
	[IsSuggestion] [bit] NOT NULL,
	[ReasonCodeId] [bigint] NULL,
	[ReasonComment] [nvarchar](500) NULL,
	[PickedAt] [datetime] NOT NULL,
	[PickedById] [bigint] NULL,
	[PickedByComment] [nvarchar](50) NULL,
	[CommittedAt] [datetime] NULL,
	[Sscc] [char](18) NULL,
	[PalletContentId] [bigint] NULL,
	[ArticleId] [bigint] NULL,
	[ArticlePackagingTypeId] [bigint] NULL,
	[StoreId] [bigint] NULL,
	[AveragePrice] [money] NULL,
	[Price] [money] NULL,
	[PriceUnit] [nvarchar](15) NULL,
	[FinalisedAt] [datetime] NULL,
	[FinalisedById] [bigint] NULL,
	[ReceivedAt] [datetime] NULL,
	[ReceivedById] [bigint] NULL,
	[SynchronisedAt] [datetime] NULL,
	[NMVSState] [int] NOT NULL,
	[ApprovedIgnoreNMVSStatenAt] [datetime] NULL,
	[ApprovedIgnoreNMVSStateById] [bigint] NULL,
	[SellByNoDayInMonth] [bit] NOT NULL,
 CONSTRAINT [PK_Pick] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Article] ON 

INSERT [dbo].[Article] ([Id], [ArticleId], [ExternalArticleId], [ErpArticleId], [DistUnitArticleName], [DistUnitManufacturer], [DistUnitLayout], [CustUnitsPerDistUnit], [CustUnitWeight], [CustUnitArticleName], [CustUnitLayout], [CustUnitGtin], [DistUnitGtin], [PalletGtin], [PalletArticleName], [PalletManufacturer], [PalletLayout], [DistUnitsPerPallet], [CustUnitsPerPallet], [DistUnitNetWeight], [DistUnitSendWeight], [PalletVolume], [CustUnitCustomField1], [CustUnitCustomField2], [PalletLayoutId], [DistUnitLayoutId], [CustUnitLayoutId], [PalletType], [Gtin], [SendWeight], [NetWeight], [Expiration1Description], [Expiration1Length], [Expiration2Description], [Expiration2Length], [Expiration3Description], [Expiration3Length], [StackingWeight], [IsFood], [ShortName], [TemperatureRequirement], [TemperatureRequirementText], [RequireSerialNumberOnScan], [ArticleName], [ManufacturerName], [UnitVolume], [Layout], [ExpirationType], [WorstSellByOut], [WorstSellByIn], [Longevity], [DoWrap], [DoLabel], [Type], [ArticleType], [IsUseBy], [QuarantineDays], [StoreType], [StoreId], [SerialNumberPrefix], [Department], [AllowOverReceive], [IsAbstract], [Unit], [Description], [InfoUrl], [ImageUrl], [LeadTime], [Unspsc], [TradeCategory], [MinimumOrderSize], [LotSize], [Price], [SpecialPrice1], [SpecialPrice2], [SpecialPrice3], [PriceUnit], [Category], [Assortment], [UnitOfMeasure], [QuarantineOnReceive], [QuarantineOnProduced], [ContentUnitOfMeasure], [ContentAmount], [Flags], [PickPriority], [AccountGroupId], [HomeStoreId], [DefaultWarehouseUnitId], [SupplierId], [LastUpdate], [Company], [MinimumStoreAmount], [ArticleCategoryId], [ReplacingArticleId], [SalesUnitId], [IsPassive], [RequireVerifyEachPackage], [NMVSMode]) VALUES (1, N'74009000', N'32', N'74009000', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'', N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, N'KREDITNOTA', N'', NULL, NULL, 0, 97, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL, NULL, N'          ', NULL, NULL, NULL, 0.0000, 0.0000, 0.0000, 0.0000, NULL, NULL, NULL, 0, 1, 0, 0, NULL, 3, NULL, NULL, NULL, NULL, NULL, CAST(N'2022-05-24T21:32:10.990' AS DateTime), 0, NULL, NULL, NULL, NULL, 0, 0, 0)
INSERT [dbo].[Article] ([Id], [ArticleId], [ExternalArticleId], [ErpArticleId], [DistUnitArticleName], [DistUnitManufacturer], [DistUnitLayout], [CustUnitsPerDistUnit], [CustUnitWeight], [CustUnitArticleName], [CustUnitLayout], [CustUnitGtin], [DistUnitGtin], [PalletGtin], [PalletArticleName], [PalletManufacturer], [PalletLayout], [DistUnitsPerPallet], [CustUnitsPerPallet], [DistUnitNetWeight], [DistUnitSendWeight], [PalletVolume], [CustUnitCustomField1], [CustUnitCustomField2], [PalletLayoutId], [DistUnitLayoutId], [CustUnitLayoutId], [PalletType], [Gtin], [SendWeight], [NetWeight], [Expiration1Description], [Expiration1Length], [Expiration2Description], [Expiration2Length], [Expiration3Description], [Expiration3Length], [StackingWeight], [IsFood], [ShortName], [TemperatureRequirement], [TemperatureRequirementText], [RequireSerialNumberOnScan], [ArticleName], [ManufacturerName], [UnitVolume], [Layout], [ExpirationType], [WorstSellByOut], [WorstSellByIn], [Longevity], [DoWrap], [DoLabel], [Type], [ArticleType], [IsUseBy], [QuarantineDays], [StoreType], [StoreId], [SerialNumberPrefix], [Department], [AllowOverReceive], [IsAbstract], [Unit], [Description], [InfoUrl], [ImageUrl], [LeadTime], [Unspsc], [TradeCategory], [MinimumOrderSize], [LotSize], [Price], [SpecialPrice1], [SpecialPrice2], [SpecialPrice3], [PriceUnit], [Category], [Assortment], [UnitOfMeasure], [QuarantineOnReceive], [QuarantineOnProduced], [ContentUnitOfMeasure], [ContentAmount], [Flags], [PickPriority], [AccountGroupId], [HomeStoreId], [DefaultWarehouseUnitId], [SupplierId], [LastUpdate], [Company], [MinimumStoreAmount], [ArticleCategoryId], [ReplacingArticleId], [SalesUnitId], [IsPassive], [RequireVerifyEachPackage], [NMVSMode]) VALUES (2, N'09310099', N'09319', N'09310099', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'', N'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'GEBYR32', NULL, NULL, 0, N'Ekspedisjonsgebyr', N'', NULL, NULL, 0, 97, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, NULL, NULL, NULL, N'          ', NULL, NULL, NULL, 200.0000, 200.0000, 200.0000, 0.0000, NULL, NULL, NULL, 0, 1, 0, 0, NULL, 3, NULL, NULL, NULL, NULL, NULL, CAST(N'2022-05-24T21:32:04.217' AS DateTime), 0, NULL, NULL, NULL, NULL, 0, 0, 0)
GO
