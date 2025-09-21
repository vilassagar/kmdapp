USE [TestDb]
GO
/****** Object:  UserDefinedTableType [dbo].[ApplicationUserTableType]    Script Date: 09/20/2024 11:58:46 ******/
CREATE TYPE [dbo].[ApplicationUserTableType] AS TABLE(
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[EmpID_PFNo] [nvarchar](50) NULL,
	[Organisation] [nvarchar](100) NULL,
	[Association] [nvarchar](100) NULL,
	[DateOfBirth] [date] NULL,
	[Gender] [nvarchar](10) NULL,
	[Email] [nvarchar](100) NULL,
	[CountryCode] [nvarchar](10) NULL,
	[MobileNumber] [nvarchar](20) NULL,
	[State] [nvarchar](50) NULL,
	[Pincode] [nvarchar](10) NULL,
	[Address] [nvarchar](255) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[AssociationContactTableType]    Script Date: 09/20/2024 11:58:46 ******/
CREATE TYPE [dbo].[AssociationContactTableType] AS TABLE(
	[AssociationName] [nvarchar](255) NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[Phone] [nvarchar](50) NULL,
	[Email] [nvarchar](255) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[AssociationMessageTableType]    Script Date: 09/20/2024 11:58:46 ******/
CREATE TYPE [dbo].[AssociationMessageTableType] AS TABLE(
	[AssociationName] [nvarchar](255) NULL,
	[Name] [nvarchar](255) NULL,
	[Template] [nvarchar](max) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[AssociationTableType]    Script Date: 09/20/2024 11:58:46 ******/
CREATE TYPE [dbo].[AssociationTableType] AS TABLE(
	[AssociationName] [nvarchar](255) NULL,
	[ParentAssociation] [nvarchar](255) NULL,
	[OrganisationName] [nvarchar](255) NULL,
	[AddressLine1] [nvarchar](255) NULL,
	[AddressLine2] [nvarchar](255) NULL,
	[City] [nvarchar](255) NULL,
	[State] [nvarchar](255) NULL,
	[PinCode] [nvarchar](50) NULL,
	[Country] [nvarchar](255) NULL,
	[BankName] [nvarchar](255) NULL,
	[BranchName] [nvarchar](255) NULL,
	[AccountName] [nvarchar](255) NULL,
	[AccountNumber] [nvarchar](50) NULL,
	[IFSCCode] [nvarchar](50) NULL,
	[MICRCode] [nvarchar](50) NULL,
	[AcceptOnepayPayment] [bit] NULL,
	[OnepayID] [nvarchar](50) NULL
)
GO
