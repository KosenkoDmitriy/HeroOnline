USE [LOLGameDB]
GO
ALTER TABLE [dbo].[SkillBase] DROP CONSTRAINT [DF_SkillBase_EffectRange]
GO
ALTER TABLE [dbo].[SkillBase] DROP CONSTRAINT [DF_SkillBase_SkillType]
GO
ALTER TABLE [dbo].[RoleBase] DROP CONSTRAINT [DF_RoleBase_AttackRate]
GO
ALTER TABLE [dbo].[ItemBase] DROP CONSTRAINT [DF_ItemBase_SellPrice]
GO
/****** Object:  Table [dbo].[SkillBase]    Script Date: 1/6/2015 5:12:23 PM ******/
DROP TABLE [dbo].[SkillBase]
GO
/****** Object:  Table [dbo].[Shop]    Script Date: 1/6/2015 5:12:23 PM ******/
DROP TABLE [dbo].[Shop]
GO
/****** Object:  Table [dbo].[RoleExtra]    Script Date: 1/6/2015 5:12:23 PM ******/
DROP TABLE [dbo].[RoleExtra]
GO
/****** Object:  Table [dbo].[RoleBase]    Script Date: 1/6/2015 5:12:23 PM ******/
DROP TABLE [dbo].[RoleBase]
GO
/****** Object:  Table [dbo].[ItemBase]    Script Date: 1/6/2015 5:12:23 PM ******/
DROP TABLE [dbo].[ItemBase]
GO
/****** Object:  Table [dbo].[ItemBase]    Script Date: 1/6/2015 5:12:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemBase](
	[ItemId] [int] NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Desc] [ntext] NOT NULL,
	[Kind] [int] NOT NULL,
	[SubKind] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[Stack] [int] NOT NULL,
	[Attrib01] [real] NOT NULL,
	[Step01] [real] NOT NULL,
	[MinVal01] [real] NOT NULL,
	[MaxVal01] [real] NOT NULL,
	[Attrib02] [real] NOT NULL,
	[Step02] [real] NOT NULL,
	[MinVal02] [real] NOT NULL,
	[MaxVal02] [real] NOT NULL,
	[SellPrice] [int] NOT NULL,
 CONSTRAINT [PK_ItemBase] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoleBase]    Script Date: 1/6/2015 5:12:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RoleBase](
	[RoleId] [int] NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Desc] [ntext] NOT NULL,
	[Type] [int] NOT NULL,
	[Class] [int] NOT NULL,
	[AssetPath] [varchar](128) NOT NULL,
	[Strength] [int] NOT NULL,
	[Agility] [int] NOT NULL,
	[Intelligent] [int] NOT NULL,
	[MoveSpeed] [int] NOT NULL,
	[AttackRate] [int] NOT NULL,
	[Skills] [varchar](64) NULL,
	[Items] [varchar](64) NULL,
 CONSTRAINT [PK_RoleBase] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RoleExtra]    Script Date: 1/6/2015 5:12:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RoleExtra](
	[RoleId] [int] NOT NULL,
	[ElemId] [int] NOT NULL,
	[Skills] [varchar](64) NOT NULL,
 CONSTRAINT [PK_RoleExtra] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[ElemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Shop]    Script Date: 1/6/2015 5:12:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shop](
	[ShopId] [int] NOT NULL,
	[ItemName] [nvarchar](64) NOT NULL,
	[ItemKind] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[UserLevel] [int] NOT NULL,
	[Promotion] [int] NOT NULL,
	[PriceSilver] [int] NOT NULL,
	[PriceGold] [int] NOT NULL,
	[PriceUSD] [real] NOT NULL,
	[PriceVND] [real] NOT NULL,
	[Discount] [int] NOT NULL,
	[StartTime] [smalldatetime] NULL,
	[EndTime] [smalldatetime] NULL,
	[TotalStock] [int] NOT NULL,
 CONSTRAINT [PK_Shop] PRIMARY KEY CLUSTERED 
(
	[ShopId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SkillBase]    Script Date: 1/6/2015 5:12:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SkillBase](
	[SkillId] [int] NOT NULL,
	[Name] [nvarchar](64) NOT NULL,
	[Desc] [ntext] NOT NULL,
	[SkillType] [int] NOT NULL,
	[TargetType] [int] NOT NULL,
	[CostType] [int] NOT NULL,
	[CostValue] [int] NOT NULL,
	[CoolTime] [real] NOT NULL,
	[CastRange] [real] NOT NULL,
	[EffectRange] [real] NOT NULL,
	[EffectType] [int] NOT NULL,
	[EffectMask] [int] NOT NULL,
	[Duration] [real] NOT NULL,
	[SrcAtt01] [int] NOT NULL,
	[DstAtt01] [int] NOT NULL,
	[Value01] [int] NOT NULL,
	[SrcAtt02] [int] NOT NULL,
	[DstAtt02] [int] NOT NULL,
	[Value02] [int] NOT NULL,
	[SrcAtt03] [int] NOT NULL,
	[DstAtt03] [int] NOT NULL,
	[Value03] [int] NOT NULL,
	[SrcAtt04] [int] NOT NULL,
	[DstAtt04] [int] NOT NULL,
	[Value04] [int] NOT NULL,
	[SrcAtt05] [int] NOT NULL,
	[DstAtt05] [int] NOT NULL,
	[Value05] [int] NOT NULL,
 CONSTRAINT [PK_SkillBase] PRIMARY KEY CLUSTERED 
(
	[SkillId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (1, N'Silver ring', N'', 2, 0, 1, 1, 16, 1, 10, 50, 0, 0, 0, 0, 12)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (2, N'Golden ring', N'', 2, 0, 2, 1, 16, 1, 13, 64, 0, 0, 0, 0, 72)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (3, N'Topaz ring', N'', 2, 0, 3, 1, 16, 1, 17, 83, 0, 0, 0, 0, 264)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (4, N'Amethyst ring', N'', 2, 0, 4, 1, 16, 1, 22, 108, 0, 0, 0, 0, 384)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (5, N'Aquamarine ring', N'', 2, 0, 5, 1, 16, 1, 28, 138, 0, 0, 0, 0, 756)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (6, N'Peridot ring', N'', 2, 0, 6, 1, 16, 1, 35, 173, 0, 0, 0, 0, 936)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (7, N'Emerald ring', N'', 2, 0, 7, 1, 16, 1, 43, 213, 0, 0, 0, 0, 1488)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (8, N'Pearl ring', N'', 2, 0, 8, 1, 16, 1, 52, 258, 0, 0, 0, 0, 1728)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (9, N'Ruby ring', N'', 2, 0, 9, 1, 16, 1, 62, 308, 0, 0, 0, 0, 2460)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (10, N'Diamond ring', N'', 2, 0, 10, 1, 16, 1, 73, 364, 0, 0, 0, 0, 2760)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (11, N'Cloak', N'', 3, 0, 1, 1, 23, 1, 5, 27, 0, 0, 0, 0, 12)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (12, N'Cloth armor', N'', 3, 0, 2, 1, 23, 1, 8, 38, 0, 0, 0, 0, 72)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (13, N'Leather armor', N'', 3, 0, 3, 1, 23, 1, 10, 49, 0, 0, 0, 0, 264)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (14, N'Chain armor', N'', 3, 0, 4, 1, 23, 1, 12, 60, 0, 0, 0, 0, 384)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (15, N'Half-plated armor', N'', 3, 0, 5, 1, 23, 1, 14, 71, 0, 0, 0, 0, 756)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (16, N'Plate armor', N'', 3, 0, 6, 1, 23, 1, 16, 82, 0, 0, 0, 0, 936)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (17, N'Steel armor', N'', 3, 0, 7, 1, 23, 1, 18, 92, 0, 0, 0, 0, 1488)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (18, N'Composite armor', N'', 3, 0, 8, 1, 23, 1, 21, 103, 0, 0, 0, 0, 1728)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (19, N'Silver armor', N'', 3, 0, 9, 1, 23, 1, 23, 114, 0, 0, 0, 0, 2460)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (20, N'Golden armor', N'', 3, 0, 10, 1, 23, 1, 25, 125, 0, 0, 0, 0, 2760)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (21, N'Private', N'', 4, 0, 1, 1, 8, 1, 100, 501, 9, 1, 10, 50, 12)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (22, N'Corporal', N'', 4, 0, 2, 1, 8, 1, 128, 642, 9, 1, 16, 78, 72)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (23, N'Sergeant', N'', 4, 0, 3, 1, 8, 1, 167, 834, 9, 1, 23, 117, 264)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (24, N'Lieutenant', N'', 4, 0, 4, 1, 8, 1, 216, 1079, 9, 1, 33, 166, 384)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (25, N'Captain', N'', 4, 0, 5, 1, 8, 1, 275, 1376, 9, 1, 45, 225, 756)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (26, N'Major', N'', 4, 0, 6, 1, 8, 1, 345, 1725, 9, 1, 59, 295, 936)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (27, N'Colonel', N'', 4, 0, 7, 1, 8, 1, 425, 2126, 9, 1, 75, 375, 1488)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (28, N'General', N'', 4, 0, 8, 1, 8, 1, 516, 2579, 9, 1, 93, 466, 1728)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (29, N'Field Marshal', N'', 4, 0, 9, 1, 8, 1, 617, 3084, 9, 1, 113, 567, 2460)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (30, N'Commander', N'', 4, 0, 10, 1, 8, 1, 728, 3642, 9, 1, 136, 678, 2760)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (31, N'Mini HP Pot', N'', 5, 1, 1, 3, 4, 0, 200, 200, 0, 0, 0, 0, 2)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (32, N'Medium HP Pot', N'', 5, 1, 3, 3, 4, 0, 500, 500, 0, 0, 0, 0, 4)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (33, N'Large HP Pot', N'', 5, 1, 5, 3, 4, 0, 900, 900, 0, 0, 0, 0, 6)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (34, N'Greater HP Pot', N'', 5, 1, 7, 3, 4, 0, 1400, 1400, 0, 0, 0, 0, 8)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (35, N'Super HP Pot', N'', 5, 1, 9, 3, 4, 0, 2000, 2000, 0, 0, 0, 0, 10)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (36, N'Mini MP Pot', N'', 5, 2, 1, 3, 5, 0, 30, 30, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (37, N'Medium MP Pot', N'', 5, 2, 3, 3, 5, 0, 60, 60, 0, 0, 0, 0, 2)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (38, N'Large MP Pot', N'', 5, 2, 5, 3, 5, 0, 120, 120, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (39, N'Greater MP Pot', N'', 5, 2, 7, 3, 5, 0, 200, 200, 0, 0, 0, 0, 5)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (40, N'Super MP Pot', N'', 5, 2, 9, 3, 5, 0, 300, 300, 0, 0, 0, 0, 6)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (41, N'Mini Refill Pot', N'', 5, 3, 1, 3, 4, 0, 100, 100, 5, 0, 15, 15, 2)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (42, N'Medium Refill Pot', N'', 5, 3, 3, 3, 4, 0, 260, 260, 5, 0, 30, 30, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (43, N'Large Refill Pot', N'', 5, 3, 5, 3, 4, 0, 460, 460, 5, 0, 60, 60, 5)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (44, N'Greater Refill Pot', N'', 5, 3, 7, 3, 4, 0, 700, 700, 5, 0, 100, 100, 6)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (45, N'Super Refill Pot', N'', 5, 3, 9, 3, 4, 0, 1000, 1000, 5, 0, 150, 150, 8)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (46, N'Minor Energy', N'', 7, 6, 1, 9, 41, 0, 3000, 3000, 0, 0, 0, 0, 0)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (47, N'Medium Energy', N'', 7, 6, 2, 9, 41, 0, 6000, 6000, 0, 0, 0, 0, 0)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (48, N'Full Energy', N'', 7, 6, 3, 9, 41, 0, 10000, 10000, 0, 0, 0, 0, 0)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (49, N'EXP Book 1', N'', 7, 6, 1, 99, 40, 0, 100, 100, 0, 0, 0, 0, 0)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (50, N'EXP Book 2', N'', 7, 6, 2, 99, 40, 0, 500, 500, 0, 0, 0, 0, 0)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (51, N'EXP Book 3', N'', 7, 6, 3, 99, 40, 0, 2500, 2500, 0, 0, 0, 0, 0)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (52, N'Crystal Lv.1', N'', 6, 4, 1, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (53, N'Crystal Lv.2', N'', 6, 4, 2, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (54, N'Crystal Lv.3', N'', 6, 4, 3, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (55, N'Crystal Lv.4', N'', 6, 4, 4, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (56, N'Crystal Lv.5', N'', 6, 4, 5, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (57, N'Crystal Lv.6', N'', 6, 4, 6, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (58, N'Crystal Lv.7', N'', 6, 4, 7, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (59, N'Crystal Lv.8', N'', 6, 4, 8, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (60, N'Crystal Lv.9', N'', 6, 4, 9, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (61, N'Crystal Lv.10', N'', 6, 4, 10, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (62, N'Crystal Lv.11', N'', 6, 4, 1, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (63, N'Crystal Lv.12', N'', 6, 4, 2, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (64, N'Crystal Lv.13', N'', 6, 4, 3, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (65, N'Crystal Lv.14', N'', 6, 4, 4, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (66, N'Crystal Lv.15', N'', 6, 4, 5, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (67, N'Crystal Lv.16', N'', 6, 4, 6, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (68, N'Crystal Lv.17', N'', 6, 4, 7, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (69, N'Crystal Lv.18', N'', 6, 4, 8, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (70, N'Crystal Lv.19', N'', 6, 4, 9, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (71, N'Crystal Lv.20', N'', 6, 4, 10, 99, 0, 0, 0, 0, 0, 0, 0, 0, 1)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (72, N'Holy Radiant', N'', 6, 5, 1, 99, 0, 0, 0, 0, 0, 0, 0, 0, 5)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (73, N'Ghost Sparkle', N'', 6, 5, 2, 99, 0, 0, 0, 0, 0, 0, 0, 0, 10)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (74, N'Spirit Flame', N'', 6, 5, 3, 99, 0, 0, 0, 0, 0, 0, 0, 0, 20)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (75, N'Ancient Herbal', N'', 6, 5, 1, 99, 0, 0, 0, 0, 0, 0, 0, 0, 5)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (76, N'Killer Fungus', N'', 6, 5, 2, 99, 0, 0, 0, 0, 0, 0, 0, 0, 10)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (77, N'Poisonous Flower', N'', 6, 5, 3, 99, 0, 0, 0, 0, 0, 0, 0, 0, 20)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (78, N'Demon Skull', N'', 6, 5, 1, 99, 0, 0, 0, 0, 0, 0, 0, 0, 5)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (79, N'Dragon Skull', N'', 6, 5, 2, 99, 0, 0, 0, 0, 0, 0, 0, 0, 10)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (80, N'Maniac Cranium', N'', 6, 5, 3, 99, 0, 0, 0, 0, 0, 0, 0, 0, 20)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (81, N'Rotten Heart', N'', 6, 5, 2, 99, 0, 0, 0, 0, 0, 0, 0, 0, 5)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (82, N'Dragon Heart', N'', 6, 5, 3, 99, 0, 0, 0, 0, 0, 0, 0, 0, 10)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (83, N'Heart Of Hatred', N'', 6, 5, 2, 99, 0, 0, 0, 0, 0, 0, 0, 0, 20)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (84, N'Orge Eye', N'', 6, 5, 3, 99, 0, 0, 0, 0, 0, 0, 0, 0, 5)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (85, N'Dragon Eye', N'', 6, 5, 1, 99, 0, 0, 0, 0, 0, 0, 0, 0, 10)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (86, N'Eye of Villain', N'', 6, 5, 1, 99, 0, 0, 0, 0, 0, 0, 0, 0, 20)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (87, N'Emerald Lv.1', N'', 6, 4, 1, 99, 0, 0, 0, 0, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (88, N'Emerald Lv.2', N'', 6, 4, 2, 99, 0, 0, 0, 0, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (89, N'Emerald Lv.3', N'', 6, 4, 3, 99, 0, 0, 0, 0, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (90, N'Emerald Lv.4', N'', 6, 4, 4, 99, 0, 0, 0, 0, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (91, N'Emerald Lv.5', N'', 6, 4, 5, 99, 0, 0, 0, 0, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (92, N'Emerald Lv.6', N'', 6, 4, 6, 99, 0, 0, 0, 0, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (93, N'Emerald Lv.7', N'', 6, 4, 7, 99, 0, 0, 0, 0, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (94, N'Emerald Lv.8', N'', 6, 4, 8, 99, 0, 0, 0, 0, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (95, N'Emerald Lv.9', N'', 6, 4, 9, 99, 0, 0, 0, 0, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (96, N'Emerald Lv.10', N'', 6, 4, 10, 99, 0, 0, 0, 0, 0, 0, 0, 0, 3)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (97, N'Sapphire Lv.1', N'', 6, 4, 1, 99, 0, 0, 0, 0, 0, 0, 0, 0, 9)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (98, N'Sapphire Lv.2', N'', 6, 4, 2, 99, 0, 0, 0, 0, 0, 0, 0, 0, 9)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (99, N'Sapphire Lv.3', N'', 6, 4, 3, 99, 0, 0, 0, 0, 0, 0, 0, 0, 9)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (100, N'Sapphire Lv.4', N'', 6, 4, 4, 99, 0, 0, 0, 0, 0, 0, 0, 0, 9)
GO
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (101, N'Sapphire Lv.5', N'', 6, 4, 5, 99, 0, 0, 0, 0, 0, 0, 0, 0, 9)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (102, N'Sapphire Lv.6', N'', 6, 4, 6, 99, 0, 0, 0, 0, 0, 0, 0, 0, 9)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (103, N'Sapphire Lv.7', N'', 6, 4, 7, 99, 0, 0, 0, 0, 0, 0, 0, 0, 9)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (104, N'Sapphire Lv.8', N'', 6, 4, 8, 99, 0, 0, 0, 0, 0, 0, 0, 0, 9)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (105, N'Sapphire Lv.9', N'', 6, 4, 9, 99, 0, 0, 0, 0, 0, 0, 0, 0, 9)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (106, N'Sapphire Lv.10', N'', 6, 4, 10, 99, 0, 0, 0, 0, 0, 0, 0, 0, 9)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (107, N'Topaz Lv.1', N'', 6, 4, 1, 99, 0, 0, 0, 0, 0, 0, 0, 0, 27)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (108, N'Topaz Lv.2', N'', 6, 4, 2, 99, 0, 0, 0, 0, 0, 0, 0, 0, 27)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (109, N'Topaz Lv.3', N'', 6, 4, 3, 99, 0, 0, 0, 0, 0, 0, 0, 0, 27)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (110, N'Topaz Lv.4', N'', 6, 4, 4, 99, 0, 0, 0, 0, 0, 0, 0, 0, 27)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (111, N'Topaz Lv.5', N'', 6, 4, 5, 99, 0, 0, 0, 0, 0, 0, 0, 0, 27)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (112, N'Topaz Lv.6', N'', 6, 4, 6, 99, 0, 0, 0, 0, 0, 0, 0, 0, 27)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (113, N'Topaz Lv.7', N'', 6, 4, 7, 99, 0, 0, 0, 0, 0, 0, 0, 0, 27)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (114, N'Topaz Lv.8', N'', 6, 4, 8, 99, 0, 0, 0, 0, 0, 0, 0, 0, 27)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (115, N'Topaz Lv.9', N'', 6, 4, 9, 99, 0, 0, 0, 0, 0, 0, 0, 0, 27)
INSERT [dbo].[ItemBase] ([ItemId], [Name], [Desc], [Kind], [SubKind], [Level], [Stack], [Attrib01], [Step01], [MinVal01], [MaxVal01], [Attrib02], [Step02], [MinVal02], [MaxVal02], [SellPrice]) VALUES (116, N'Topaz Lv.10', N'', 6, 4, 10, 99, 0, 0, 0, 0, 0, 0, 0, 0, 27)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1, N'Monk', N'Healer', 1, 9, N'Heroes/Monk', 40, 20, 40, 19, 10, N'4', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (2, N'Hulk', N'Warrior', 1, 1, N'Heroes/Hulk', 40, 40, 20, 19, 70, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (3, N'Valkyrie', N'Mage', 1, 7, N'Heroes/Valkyrie', 40, 10, 50, 17, 30, N'3', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (4, N'Hecate', N'Healer', 1, 9, N'Heroes/Hecate', 40, 20, 40, 19, 10, N'4', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (5, N'Golem', N'Tanker', 1, 2, N'Heroes/Earthquake', 60, 20, 20, 14, 60, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (6, N'Lizard', N'Ranger', 1, 4, N'Heroes/Gunner', 40, 50, 10, 20, 30, N'2', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (7, N'WoodElf', N'Healer', 1, 9, N'Heroes/WoodElf', 40, 20, 40, 19, 10, N'4', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (8, N'Songoku', N'Warrior', 1, 1, N'Heroes/Songoku', 40, 40, 20, 19, 30, N'3', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (9, N'Garuda', N'Tanker', 1, 2, N'Heroes/Garuda', 60, 20, 20, 14, 60, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1001, N'Warrior', N'Warrior', 2, 0, N'Mobs/Mob_003', 20, 20, 10, 13, 30, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1002, N'Tanker', N'Tanker', 2, 0, N'Mobs/Mob_008', 30, 10, 10, 13, 30, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1003, N'Ranger', N'Ranger', 2, 0, N'Mobs/Mob_005', 10, 30, 10, 13, 30, N'2', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1004, N'Mage', N'Mage', 2, 0, N'Mobs/Mob_015', 20, 20, 10, 13, 30, N'3', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1005, N'Time Bomber', N'Time Bomber', 2, 0, N'Mobs/Mob_002', 10, 30, 10, 13, 30, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1006, N'Monk', N'Monk', 2, 0, N'Mobs/Mob_011', 20, 20, 10, 13, 30, N'4', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1007, N'Commander', N'Commander', 3, 0, N'Mobs/Mob_012', 30, 30, 10, 13, 30, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1008, N'Butcher', N'Butcher', 3, 0, N'Mobs/Mob_004', 50, 10, 10, 13, 30, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1009, N'Salamander', N'Salamander', 3, 0, N'Mobs/Mob_006', 30, 30, 10, 13, 30, N'2', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1010, N'Dream Catcher', N'Dream Catcher', 3, 0, N'Mobs/Mob_001', 30, 20, 20, 13, 30, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1011, N'Dark Priest', N'Dark Priest', 3, 0, N'Mobs/Mob_011', 40, 20, 10, 13, 30, N'4', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1012, N'Assasin', N'Assasin', 3, 0, N'Mobs/Mob_007', 30, 30, 10, 13, 30, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1013, N'Captain', N'Captain', 3, 0, N'Mobs/Mob_009', 600, 80, 300, 13, 30, N'3,17,13', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1014, N'Rune', N'Rune', 2, 0, N'Mobs/Mob_014', 30, 10, 30, 0, 0, NULL, NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1015, N'Monk Boss', N'Monk Boss', 4, 6, N'Heroes/Monk', 70, 20, 60, 19, 10, N'4', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1016, N'Hulk Boss', N'Hulk Boss', 4, 1, N'Heroes/Hulk', 70, 70, 10, 19, 70, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1017, N'Valkyrie Boss', N'Valkyrie Boss', 4, 5, N'Heroes/Valkyrie', 70, 10, 70, 17, 30, N'3', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1018, N'Hecate Boss', N'Hecate Boss', 4, 6, N'Heroes/Hecate', 70, 20, 60, 19, 10, N'4', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1019, N'Songoku Boss', N'Songoku Boss', 4, 1, N'Heroes/Songoku', 70, 70, 10, 19, 30, N'3', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1020, N'Garuda Boss', N'Garuda Boss', 4, 2, N'Heroes/Garuda', 90, 40, 20, 14, 60, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1021, N'Shadow Guardian', N'Shadow Guardian', 4, 0, N'Mobs/Mob_009', 600, 50, 300, 13, 30, N'1', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1022, N'The Hostage', N'The Hostage', 5, 2, N'Heroes/Monk', 50, 0, 50, 18, 0, NULL, NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1023, N'BlackRune', N'BlackRune', 2, 0, N'Mobs/Mob_014', 600, 0, 300, 0, 0, NULL, NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1024, N'AlQaeda', N'AlQaeda', 3, 0, N'Mobs/Mob_010', 30, 30, 10, 10, 50, N'55', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1025, N'TimeBomber', N'TimeBomber', 3, 0, N'Mobs/Mob_010', 30, 30, 10, 14, 50, N'56', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1026, N'MagicRune1', N'MagicRune1', 2, 0, N'Mobs/Mob_014', 70, 0, 200, 0, 0, N'37', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1027, N'MagicRune2', N'MagicRune2', 2, 0, N'Mobs/Mob_014', 70, 0, 200, 0, 0, N'38', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1028, N'MagicRune3', N'MagicRune3', 2, 0, N'Mobs/Mob_014', 70, 0, 200, 0, 0, N'39', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1029, N'MagicRune4', N'MagicRune4', 2, 0, N'Mobs/Mob_014', 70, 0, 200, 0, 0, N'42', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1030, N'MagicRune5', N'MagicRune5', 2, 0, N'Mobs/Mob_014', 70, 0, 200, 0, 0, N'43', NULL)
INSERT [dbo].[RoleBase] ([RoleId], [Name], [Desc], [Type], [Class], [AssetPath], [Strength], [Agility], [Intelligent], [MoveSpeed], [AttackRate], [Skills], [Items]) VALUES (1031, N'MagicRune8', N'MagicRune8', 2, 0, N'Mobs/Mob_014', 70, 0, 200, 0, 0, N'44', NULL)
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1, 1, N'6,50')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1, 2, N'5,49')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1, 3, N'5,6')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1, 4, N'5,46')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1, 5, N'6,43')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (2, 1, N'38,10')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (2, 2, N'9,54')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (2, 3, N'51,10')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (2, 4, N'9,10')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (2, 5, N'36,10')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (3, 1, N'23,24')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (3, 2, N'23,13')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (3, 3, N'24,13')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (3, 4, N'13,17')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (3, 5, N'41,17')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (4, 1, N'5,7')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (4, 2, N'5,34')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (4, 3, N'7,39')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (4, 4, N'5,42')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (4, 5, N'7,33')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (5, 1, N'11,32')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (5, 2, N'8,45')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (5, 3, N'11,45')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (5, 4, N'9,48')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (5, 5, N'8,11')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (6, 1, N'21,44')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (6, 2, N'21,22')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (6, 3, N'22,53')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (6, 4, N'22,54')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (6, 5, N'21,53')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (7, 1, N'48,25')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (7, 2, N'6,25')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (7, 3, N'52,25')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (7, 4, N'6,47')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (7, 5, N'6,52')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (8, 1, N'26,27')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (8, 2, N'26,48')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (8, 3, N'27,48')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (8, 4, N'27,53')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (8, 5, N'26,53')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (9, 1, N'29,39')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (9, 2, N'28,50')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (9, 3, N'40,37')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (9, 4, N'44,29')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (9, 5, N'28,29')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1015, 1, N'6,50')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1015, 2, N'5,49')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1015, 3, N'5,6')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1015, 4, N'5,46')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1015, 5, N'6,43')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1016, 1, N'36,10')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1016, 2, N'38,10')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1016, 3, N'9,54')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1016, 4, N'51,10')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1016, 5, N'9,10')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1017, 1, N'13,17')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1017, 2, N'41,17')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1017, 3, N'23,24')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1017, 4, N'23,13')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1017, 5, N'24,13')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1018, 1, N'7,39')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1018, 2, N'5,42')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1018, 3, N'7,33')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1018, 4, N'5,7')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1018, 5, N'5,34')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1019, 1, N'26,48')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1019, 2, N'27,48')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1019, 3, N'27,53')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1019, 4, N'26,53')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1019, 5, N'26,27')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1020, 1, N'29,39')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1020, 2, N'28,50')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1020, 3, N'40,37')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1020, 4, N'44,29')
INSERT [dbo].[RoleExtra] ([RoleId], [ElemId], [Skills]) VALUES (1020, 5, N'28,29')
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (1, N'Low Rank Hero', 1, 1, 0, 0, 2000, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (2, N'High Rank Hero', 1, 2, 0, 0, 0, 25, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (3, N'Mini HP Pot', 5, 31, 1, 0, 22, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (4, N'Medium HP Pot', 5, 32, 11, 0, 42, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (5, N'Large HP Pot', 5, 33, 21, 0, 62, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (6, N'Greater HP Pot', 5, 34, 31, 0, 82, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (7, N'Super HP Pot', 5, 35, 41, 0, 102, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (8, N'Mini MP Pot', 5, 36, 1, 0, 13, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (9, N'Medium MP Pot', 5, 37, 11, 0, 20, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (10, N'Large MP Pot', 5, 38, 21, 0, 33, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (11, N'Greater MP Pot', 5, 39, 31, 0, 47, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (12, N'Super MP Pot', 5, 40, 41, 0, 61, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (13, N'Mini Refill Pot', 5, 41, 1, 0, 18, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (14, N'Medium Refill Pot', 5, 42, 11, 0, 31, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (15, N'Large Refill Pot', 5, 43, 21, 0, 48, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (16, N'Greater Refill Pot', 5, 44, 31, 0, 64, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (17, N'Super Refill Pot', 5, 45, 41, 0, 82, 0, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (18, N'Minor Energy', 7, 46, 0, 0, 0, 5, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (19, N'Medium Energy', 7, 47, 0, 0, 0, 9, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (20, N'Full Energy', 7, 48, 0, 0, 0, 14, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (21, N'Crystal Lv.1', 6, 52, 0, 0, 0, 3, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (22, N'Emerald Lv.1', 6, 87, 0, 0, 0, 9, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (23, N'Sapphire Lv.1', 6, 97, 0, 0, 0, 27, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (24, N'Topaz Lv.1', 6, 107, 0, 0, 0, 81, 0, 0, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (25, N'Gold 20', 9, 1000, 0, 0, 0, 20, 2, 21000, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (26, N'Gold 50', 9, 1001, 0, 10, 0, 50, 5, 50000, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (27, N'Gold 100', 9, 1002, 0, 20, 0, 100, 10, 100000, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (28, N'Gold 200', 9, 1003, 0, 50, 0, 200, 20, 200000, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (29, N'Gold 500', 9, 1004, 0, 188, 0, 500, 50, 500000, 0, NULL, NULL, 0)
INSERT [dbo].[Shop] ([ShopId], [ItemName], [ItemKind], [ItemId], [UserLevel], [Promotion], [PriceSilver], [PriceGold], [PriceUSD], [PriceVND], [Discount], [StartTime], [EndTime], [TotalStock]) VALUES (30, N'Silver 25000', 8, 1005, 0, 0, 25000, 0, 10, 100000, 0, NULL, NULL, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (1, N'Attack', N'Damage enemy for 1*(ATK)', 1, 4, 0, 0, 0, 1.5, 0, 0, 23, 0, 16, 4, -100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (2, N'Shoot', N'Damage enemy for 1*(ATK)', 1, 4, 0, 0, 0, 6, 0, 0, 23, 0, 16, 4, -100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (3, N'Bolt', N'Damage enemy for 1*(ATK)', 1, 4, 0, 0, 0, 5, 0, 0, 23, 0, 16, 4, -100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (4, N'Heal', N'Dùng 80% sức Công hồi phục HP cho 01 đồng đội.', 1, 2, 0, 0, 0, 5, 0, 0, 5, 0, 16, 4, 80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (5, N'Group Heal', N'Dùng 110% sức Công hồi phục HP cho tất cả đồng đội', 1, 3, 2, 7, 28, 0, 0, 0, 4, 0, 16, 4, 110, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (6, N'Mega Heal', N'Dùng 150% sức Công hồi phục HP cho 01 đồng đội', 1, 2, 2, 2, 13, 5, 0, 0, 4, 0, 16, 4, 150, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (7, N'Regen', N'Ally target regen for 0.7*(ATK) per sec, in 5 secs.', 1, 2, 2, 17, 12, 6, 0, 0, 0, 5, 16, 12, 70, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (8, N'Stoneskin', N'Giảm 50% tất cả sát thương nhận vào và tăng 20% tốc độ di chuyển trong 6s', 1, 1, 2, 4, 15, 0, 0, 4, 0, 6, 0, 46, 50, 0, 26, 20, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (9, N'Frenzy', N'Tăng 50% tốc độ tấn công trong 6s', 1, 1, 2, 4, 15, 0, 0, 15, 0, 6, 0, 25, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (10, N'Heavy Punch', N'Dùng 120% sức Công đấm 01 mục tiêu và gây 10% sát thương liên tục trong 5s tiếp theo', 1, 4, 2, 4, 14, 1.5, 0, 0, 28, 5, 16, 4, -120, 16, 12, -10, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (11, N'Slam', N'Dùng 120% sức Công sát thương tất cả mục tiêu trong 4m xung quanh và gây Choáng trong 3s', 1, 7, 2, 11, 44, 0, 4, 1, 28, 3, 16, 4, -120, 0, 27, 1, 0, 28, 1, 0, 25, -100, 0, 26, -100)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (12, N'Meteor', N'Damage all enemies for 10+(1.5*ATK)', 1, 6, 2, 18, 25, 5, 3, 0, 24, 0, 0, 4, -10, 16, 4, -150, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (13, N'Fireball', N'Dùng 170% sức Công sát thương mục tiêu và phạm vi 2m xung quanh.', 1, 6, 2, 10, 40, 5, 2, 0, 28, 0, 16, 4, -170, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (14, N'Battle Mage', N'Increase 15% AS and +30% ATK in 8 secs', 1, 1, 2, 8, 12, 0, 0, 13, 24, 8, 0, 17, 30, 0, 25, 15, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (15, N'Bash', N'Damage target 0.3*(ATK) and stun it for 5 sec', 1, 4, 2, 8, 20, 1.5, 0, 1, 24, 5, 16, 4, -30, 0, 27, 1, 0, 28, 1, 0, 25, -100, 0, 26, -100)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (16, N'Freeze', N'Damage target 0.3*(ATK) and freeze target for 5 sec', 1, 4, 2, 15, 20, 5, 0, 5, 24, 5, 16, 4, -30, 0, 26, -100, 0, 25, -100, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (17, N'Frost Nova', N'Dùng 120% sức Công sát thương tất cả mục tiêu và làm chậm 50% tốc độ tấn công và di chuyển trong 5s', 1, 5, 2, 6, 25, 0, 0, 19, 28, 5, 16, 4, -120, 0, 25, -50, 0, 26, -50, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (18, N'Poison Sting', N'Poison 1 enemy, damage 0.7*(ATK) in 5 sec', 1, 4, 2, 8, 18, 6, 0, 6, 24, 5, 16, 4, -70, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (19, N'Furious Strike', N'Damage 0.5*(ATK) and slow target 30% target AS in 5 sec', 1, 4, 2, 10, 15, 1.5, 0, 17, 24, 5, 16, 4, -50, 0, 25, -30, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (20, N'Lion Roar', N'Damage 0.36*(ATK) and slow all targets 30% target AS in 5 sec', 1, 7, 2, 12, 30, 0, 2.5, 17, 24, 5, 16, 4, -36, 0, 25, -30, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (21, N'Stun shot', N'Dùng 120% sức Công sát thương 01 mục tiêu và gây Choáng trong 3s', 1, 4, 2, 4, 17, 6, 0, 1, 28, 3, 16, 4, -120, 0, 27, 1, 0, 28, 1, 0, 25, -100, 0, 26, -100)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (22, N'Calm shot', N'Dùng 120% sức Công sát thương 01 mục tiêu và gây Câm lặng trong 5s', 1, 4, 2, 12, 13, 6, 0, 3, 28, 5, 0, 27, 1, 16, 4, -120, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (23, N'God Power', N'Dùng 30% sức Công gây sát thương liên tục trong 5s và hủy tất cả Buff của 01 mục tiêu.', 1, 4, 2, 4, 14, 6, 0, 0, 28, 5, 16, 12, -30, 0, 42, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (24, N'Lightning Call', N'Dùng 120% sức Công sát thương tất cả mục tiêu trước mặt, đồng thời giảm 20% Khóa đòn và Né của các mục tiêu trong 8s tiếp theo.', 1, 7, 2, 9, 34, 0, 0, 20, 28, 8, 16, 4, -120, 0, 33, -20, 0, 32, -20, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (25, N'Dew of Nature', N'Dùng 100% sức Công hồi phục HP cho tất cả đồng đội và giải trừ tất cả trạng thái xấu.', 1, 3, 2, 7, 28, 6, 0, 12, 16, 0, 16, 4, 120, 0, 43, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (26, N'Kamekameha', N'Dùng 120% sức Công sát thương 01 mục tiêu và gây giảm 50% chính xác trong 5s.', 1, 4, 2, 3, 14, 5, 0, 22, 28, 5, 16, 4, -120, 0, 31, -50, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (27, N'Gravily Ball', N'Dùng 120% sức Công sát thương mục tiêu và phạm vi xung quanh, đồng thời giảm 25% phòng thủ trong 6s tiếp theo', 1, 6, 2, 9, 34, 3, 2.5, 11, 28, 5, 16, 4, -120, 0, 24, -25, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (28, N'Thorn Shield', N'Phản 60% tất cả sát thương trong vòng 12s', 1, 1, 2, 5, 19, 0, 0, 34, 16, 12, 0, 39, 60, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (29, N'Whirlwind', N'Tự hủy 35% Hp tối đa và gây sát thương 25% HP tối đa cho tất cả mục tiêu', 1, 5, 5, 35, 23, 10, 0, 0, 28, 0, 0, 6, -25, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (30, N'Normal Magic', N'Damage enemy for 1*(ATK)', 1, 4, 0, 0, 0, 1.5, 0, 0, 23, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (31, N'Mana Shield', N'Kích hoạt Mana shield có khả năng hấp thụ 80% sát thương', 1, 1, 0, 0, 0, 0, 0, 0, 28, 0, 0, 36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (32, N'G Defense buff', N'Tăng 25% phòng thủ cho tất cả đồng đội trong 12s', 1, 3, 2, 12, 24, 0, 0, 4, 28, 12, 0, 24, 25, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (33, N'G Attack buff', N'Tăng 28% tấn công cho tất cả đồng đội trong 12s', 1, 3, 2, 12, 28, 0, 0, 14, 28, 12, 0, 17, 28, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (34, N'G Crit % buff', N'Tăng 50% tỉ lệ chí mạng cho tất cả đồng đội trong 12s', 1, 3, 2, 12, 28, 0, 0, 23, 28, 12, 0, 34, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (35, N'G Speed buff', N'Tăng 30% tốc độ tấn công và di chuyển cho tất cả đồng đội trong 12s', 1, 3, 2, 12, 32, 0, 0, 16, 28, 12, 0, 25, 30, 0, 26, 30, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (36, N'Vampiric', N'Tăng 75% hút HP trong vòng 15s.', 1, 1, 2, 8, 35, 0, 0, 27, 28, 15, 0, 36, 75, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (37, N'Block Aura', N'Tăng 25% khóa đòn cho tất cả đồng đội', 3, 3, 0, 0, 0, 0, 0, 29, 28, 1, 0, 33, 25, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (38, N'Attack Aura', N'Tăng 20% sát thương cho tất cả đồng đội', 3, 3, 0, 0, 0, 0, 0, 14, 28, 1, 0, 17, 20, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (39, N'Speed Aura', N'Tăng 25% tốc độ tấn công và di chuyển cho tất cả đồng đội', 3, 3, 0, 0, 0, 0, 0, 33, 28, 1, 0, 25, 25, 0, 26, 25, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (40, N'G Hp Regen Buff', N'Hồi phục 10% HP/s cho tất cả đồng đội trong vòng 5s.', 1, 3, 2, 10, 42, 0, 0, 35, 28, 5, 0, 14, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (41, N'MP Regen Aura', N'Tăng 35% hồi phục MP cho tất cả đồng đội', 3, 3, 0, 0, 0, 0, 0, 36, 28, 1, 0, 15, 35, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (42, N'Bleeding Aura', N'Giảm 5 HP/s của tất cả mục tiêu', 3, 5, 0, 0, 0, 0, 0, 8, 28, 1, 0, 12, -5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (43, N'Lost Def Aura', N'Giảm 30% phòng thủ của tất cả mục tiêu', 3, 5, 0, 0, 0, 0, 0, 11, 28, 1, 0, 24, -30, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (44, N'Slow Aura', N'Giảm 28% tốc độ tấn công và di chuyển của tất cả mục tiêu', 3, 5, 0, 0, 0, 0, 0, 19, 28, 1, 0, 25, -28, 0, 26, -28, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (45, N'G MP Drain Buff', N'Tăng 45% hút MP cho tất cả đồng đội trong vòng 16s', 1, 3, 2, 10, 28, 0, 0, 38, 28, 16, 0, 37, 45, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (46, N'Def Debuff', N'Giảm 50% phòng thủ của tất cả mục tiêu trong phạm vi 4m xung quanh bản thân trong vòng 12s', 1, 7, 2, 8, 34, 0, 4, 11, 28, 12, 0, 24, -50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (47, N'Defense buff', N'Tăng 40% phòng thủ trong 12s', 1, 1, 2, 8, 18, 0, 0, 4, 28, 12, 0, 24, 40, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (48, N'Attack buff', N'Tăng 45% tấn công trong 12s', 1, 1, 2, 8, 21, 0, 0, 14, 28, 12, 0, 17, 45, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (49, N'Crit % buff', N'Tăng 75% tỉ lệ chí mạng trong 12s', 1, 1, 2, 8, 21, 0, 0, 23, 28, 12, 0, 34, 75, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (50, N'MP Drain Debuff', N'Giảm 12% Max MP mỗi giây của tất cả mục tiêu trong vòng 6s', 1, 5, 2, 8, 34, 0, 0, 7, 28, 6, 0, 13, -12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (51, N'Evasion buff', N'Tăng 50% Né trong vòng 12s', 1, 1, 2, 8, 24, 0, 0, 31, 28, 12, 0, 32, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (52, N'G Evasion buff', N'Tăng 35% Né cho đồng đội trong vòng 12s', 1, 3, 2, 12, 34, 0, 0, 31, 28, 12, 0, 32, 35, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (53, N'Blood lust', N'Dùng 5% Max HP để tăng 30% tỉ lệ chí mạng và 100% sát thương chí mạng trong vòng 12s', 1, 1, 5, 5, 45, 0, 0, 39, 28, 12, 0, 34, 30, 0, 35, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (54, N'Rage', N'Dùng 8% Max Hp để tăng 50% sát thương và tốc độ tấn công trong 12s', 1, 1, 5, 8, 40, 0, 0, 40, 28, 12, 0, 17, 50, 0, 25, 50, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (55, N'Suicide Bomb', N'Tự nổ gây sát thương 300%*ATK cho tất cả đối thủ trong phạm vi 3m xung quanh', 1, 8, 3, 100, 0, 1.5, 3, 0, 28, 0, 16, 4, -300, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
INSERT [dbo].[SkillBase] ([SkillId], [Name], [Desc], [SkillType], [TargetType], [CostType], [CostValue], [CoolTime], [CastRange], [EffectRange], [EffectType], [EffectMask], [Duration], [SrcAtt01], [DstAtt01], [Value01], [SrcAtt02], [DstAtt02], [Value02], [SrcAtt03], [DstAtt03], [Value03], [SrcAtt04], [DstAtt04], [Value04], [SrcAtt05], [DstAtt05], [Value05]) VALUES (56, N'Detonate!', N'Tự nổ sau 10sec gây sát thương 20% Max HP cho tất cả đối thủ', 1, 9, 3, 100, 0, 10, 0, 0, 28, 0, 0, 6, -20, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
ALTER TABLE [dbo].[ItemBase] ADD  CONSTRAINT [DF_ItemBase_SellPrice]  DEFAULT ((0)) FOR [SellPrice]
GO
ALTER TABLE [dbo].[RoleBase] ADD  CONSTRAINT [DF_RoleBase_AttackRate]  DEFAULT ((0)) FOR [AttackRate]
GO
ALTER TABLE [dbo].[SkillBase] ADD  CONSTRAINT [DF_SkillBase_SkillType]  DEFAULT ((0)) FOR [SkillType]
GO
ALTER TABLE [dbo].[SkillBase] ADD  CONSTRAINT [DF_SkillBase_EffectRange]  DEFAULT ((0)) FOR [EffectRange]
GO
