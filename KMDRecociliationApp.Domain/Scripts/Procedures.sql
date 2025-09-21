USE [TestDb]
GO
/****** Object:  StoredProcedure [dbo].[spGetErrorInfo]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spGetErrorInfo]
as
begin
insert into ExceptionLog(  
ErrorLine, ErrorMessage, ErrorNumber,  
ErrorProcedure, ErrorSeverity, ErrorState,  
DateErrorRaised  
)  
SELECT  
ERROR_LINE () as ErrorLine,  
Error_Message() as ErrorMessage,  
Error_Number() as ErrorNumber,  
Error_Procedure() as 'Proc',  
Error_Severity() as ErrorSeverity,  
Error_State() as ErrorState,  
GETDATE () as DateErrorRaised 
end
GO
/****** Object:  StoredProcedure [dbo].[USPGetAllTemplateAndMetadata]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[USPGetAllTemplateAndMetadata] 
	-- Add the parameters for the stored procedure here
	@TemplateName VARCHAR(200)
AS
BEGIN
BEGIN TRY
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	

SELECT TemplateName, Worksheet, ColumnNumber, Columnkey FROM dbo. TemplateMetadatalookup 
WHERE TemplateName = @TemplateName

END TRY

BEGIN CATCH

EXEC dbo.spGetErrorInfo

END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[uspGetAllLegalEntityMetadata]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

 CREATE  PROCEDURE  [dbo].[uspGetAllLegalEntityMetadata]      
 AS

BEGIN
SET NOCOUNT ON;  
	BEGIN TRY  

		--UserType 0
	    CREATE TABLE #UserType (
			ID INT PRIMARY KEY,
			Name NVARCHAR(50) NULL
		);

		INSERT INTO #UserType (ID, Name)
			 VALUES (1, 'Retiree'), (2, 'InternalUser'), (3, 'Association'), (4, 'Other');

		SELECT Name FROM #UserType
		DROP TABLE #UserType

		--Gender 1
		CREATE TABLE #Gender (
			ID INT PRIMARY KEY,
			Name NVARCHAR(50) NULL
		);

		INSERT INTO #Gender (ID, Name)
			 VALUES (1, 'Male'), (2, 'Female'), (3, 'Unknown');

		SELECT Name FROM #Gender
		DROP TABLE #Gender

		
		--CountryCode 2
		SELECT CountryCode AS Name FROM dbo.AddressCountry
		WHERE  IsActive = 1

		--State 3
		SELECT Name AS Name FROM dbo.AddressState
		WHERE  IsActive = 1

		--Association 4
		SELECT a.AssociationName as Name FROM dbo.Association a
		WHERE  IsActive=1
		
		--Organisations 5
		SELECT Name FROM dbo.Organisations
		WHERE IsActive = 1	

		
	END TRY  
	BEGIN CATCH  
	EXEC dbo.spGetErrorInfo  
	END CATCH 

END
GO
/****** Object:  StoredProcedure [dbo].[uspGetAllAssociationMetadata]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

 create  PROCEDURE  [dbo].[uspGetAllAssociationMetadata]      
 AS

BEGIN
SET NOCOUNT ON;  
	BEGIN TRY  

		--UserType 0
	    CREATE TABLE #yesno (
			ID INT PRIMARY KEY,
			Name NVARCHAR(50) NULL
		);

		INSERT INTO #yesno (ID, Name)
			 VALUES (1, 'YES'), (2, 'NO');

		SELECT Name FROM #yesno
		DROP TABLE #yesno
		
		--State 1
		SELECT Name AS Name FROM dbo.AddressState
		WHERE  IsActive = 1
		
		--Country 2
		SELECT Name AS Name FROM dbo.AddressCountry
		WHERE  IsActive = 1
		
		--Organisations 3
		SELECT Name FROM dbo.Organisations
		WHERE IsActive = 1
		
		--Association 4
		SELECT a.AssociationName as Name FROM dbo.Association a
		WHERE  IsActive=1
		

				
	END TRY  
	BEGIN CATCH  
	EXEC dbo.spGetErrorInfo  
	END CATCH 

END
GO
/****** Object:  StoredProcedure [dbo].[USPGetAllApplicationUserMetadata]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

 Create  PROCEDURE  [dbo].[USPGetAllApplicationUserMetadata]      
 AS

BEGIN
SET NOCOUNT ON;  
	BEGIN TRY  

		--UserType 0
	    CREATE TABLE #UserType (
			ID INT PRIMARY KEY,
			Name NVARCHAR(50) NULL
		);

		INSERT INTO #UserType (ID, Name)
			 VALUES (1, 'Retiree'), (2, 'InternalUser'), (3, 'Association'), (4, 'Other');

		SELECT Name FROM #UserType
		DROP TABLE #UserType

		--Gender 1
		CREATE TABLE #Gender (
			ID INT PRIMARY KEY,
			Name NVARCHAR(50) NULL
		);

		INSERT INTO #Gender (ID, Name)
			 VALUES (1, 'Male'), (2, 'Female'), (3, 'Unknown');

		SELECT Name FROM #Gender
		DROP TABLE #Gender

		
		--CountryCode 2
		SELECT CountryCode AS Name FROM dbo.AddressCountry
		WHERE  IsActive = 1

		--State 3
		SELECT Name AS Name FROM dbo.AddressState
		WHERE  IsActive = 1

		--Association 4
		SELECT a.AssociationName as Name FROM dbo.Association a
		WHERE  IsActive=1
		
		--Organisations 5
		SELECT Name FROM dbo.Organisations
		WHERE IsActive = 1	

		
	END TRY  
	BEGIN CATCH  
	EXEC dbo.spGetErrorInfo  
	END CATCH 

END
GO
/****** Object:  StoredProcedure [dbo].[GetUsersByCreationDate]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetUsersByCreationDate]
    @ReportDate DATE
AS
BEGIN
    SELECT 
        u.[Id],
        u.[FirstName],
        u.[LastName],
        u.[Email],
        u.[CountryCode],
        u.[MobileNumber],
        u.[DOB],
        u.[UserType],
        u.[Gender],
        u.[OrganisationId],
        o.[Name] as Organisation,
        u.[AssociationId],
        a.[AssociationName],
        u.[EMPIDPFNo],
        u.[Address],
        u.[City],
        u.[StateId],
        s.[Name] as State,
        u.[CountryId],
        c.[Name] as Country,
        u.[Pincode],
        u.[OTP],
        u.[OTPExpiration],
        u.[IsProfileComplete],
        u.[CreatedAt]
        
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
    LEFT JOIN 
        [AddressState] s ON u.[StateId] = s.[Id]
    LEFT JOIN 
        [AddressCountry] c ON u.[CountryId] = c.[Id]
    WHERE 
		u.[IsActive]=1 and
        CAST(u.[CreatedAt] AS DATE) = cast(@ReportDate AS DATE)
    ORDER BY
        u.[AssociationId];
END
GO
/****** Object:  StoredProcedure [dbo].[GetReconcileedOnlinePayments]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetReconcileedOnlinePayments] 
	-- Add the parameters for the stored procedure here
	@associationID int=0, 
    @startdate date,
    @enddate date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
        u.[Id],
        u.[FirstName],
        u.[LastName],
        u.[Email],
        u.[CountryCode],
        u.[MobileNumber],
        o.[Name] as OrganisationName,
        a.[AssociationName],
        p.[PaymentDate],
        p.[TotalPremimumAmount] as PremimumAmount,
        p.[AmountPaid] as PaidAmount,
        p.PaymentType as PaymentType,
        p.[PaymentMode],
        p.[PaymentStatus] 
        
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
    LEFT JOIN 
        [AddressState] s ON u.[StateId] = s.[Id]
    LEFT JOIN 
        [AddressCountry] c ON u.[CountryId] = c.[Id]
    INNER JOIN 
        PaymentDetails p ON u.Id = p.UserId
    WHERE 
        (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        AND p.PaymentMode = 1 -- enum PaymentMode IOnline=1
        --AND p.PaymentDate BETWEEN @StartDate AND @EndDate
        and CAST(p.[PaymentDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)

END
GO
/****** Object:  StoredProcedure [dbo].[GetOfflinePayments]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetOfflinePayments] 
	-- Add the parameters for the stored procedure here
	@associationID int=0, 
    @startdate date,
    @enddate date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
        u.[Id],
        u.[FirstName],
        u.[LastName],
        u.[Email],
        u.[CountryCode],
        u.[MobileNumber],
        o.[Name] as OrganisationName,
        a.[AssociationName],
        p.[PaymentDate],
        p.[TotalPremimumAmount] as PremimumAmount,
        p.[AmountPaid] as PaidAmount,
        p.[PaymentType] as PaymentType,
        p.[PaymentMode],
        p.[PaymentStatus] 
        
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
    LEFT JOIN 
        [AddressState] s ON u.[StateId] = s.[Id]
    LEFT JOIN 
        [AddressCountry] c ON u.[CountryId] = c.[Id]
    INNER JOIN 
        PaymentDetails p ON u.Id = p.UserId
    WHERE 
        (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        AND p.PaymentMode = 2 -- enum PaymentMode Offline=2
        --AND p.PaymentDate BETWEEN @StartDate AND @EndDate
        and CAST(p.[PaymentDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)

END
GO
/****** Object:  StoredProcedure [dbo].[GetIncompleteTransaction]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetIncompleteTransaction] 
	-- Add the parameters for the stored procedure here
	@AssociationID int=0
	,@ReportDate date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SELECT 
    u.[Id],
    u.[FirstName],
    u.[LastName],
    u.[Email],
    u.[CountryCode],
    u.[MobileNumber],
    o.[Name] as OrganisationName,
    a.[AssociationName],
    p.[PaymentDate],
    p.[TotalPremimumAmount] as PremimumAmount,
    p.[AmountPaid] as PaidAmount,
    p.[PaymentType] as PaymentType,
    p.[PaymentMode],
    p.[PaymentStatus] 
    
FROM 
    [ApplicationUser] u
INNER JOIN 
    [Association] a ON u.[AssociationId] = a.[Id]
INNER JOIN 
    [Organisations] o ON u.[OrganisationId] = o.[Id]
LEFT JOIN 
    [AddressState] s ON u.[StateId] = s.[Id]
LEFT JOIN 
    [AddressCountry] c ON u.[CountryId] = c.[Id]
INNER JOIN 
    PaymentDetails p ON u.Id = p.UserId
WHERE 
    (@AssociationID = 0 OR u.AssociationId = @AssociationID)
   AND  p.PaymentStatus !=1 
    AND cast(p.[PaymentDate] as DATE) = CAST(@ReportDate AS date)
--and CAST(p.[PaymentDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)

END
GO
/****** Object:  StoredProcedure [dbo].[GetCorrectionReport]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCorrectionReport] 
	-- Add the parameters for the stored procedure here
	@associationID int=0, 
    @startdate date,
    @enddate date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
        u.[Id],
        u.[FirstName],
        u.[LastName],
        u.[Email],
        u.[CountryCode],
        u.[MobileNumber],
        u.[DOB],
        u.[UserType],
        u.[Gender],
        o.[Name] as OrganisationName,
        a.[AssociationName],
        u.[EMPIDPFNo],
        u.[Address],
        u.[City],
        s.[Name] as state,
        c.[Name] as country,
        u.[Pincode],
        u.IsProfileComplete,
        u.CreatedAt,
        u.UpdatedAt,
        u.CreatedBy,
        u.UpdatedBy
         
        
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
    LEFT JOIN 
        [AddressState] s ON u.[StateId] = s.[Id]
    LEFT JOIN 
        [AddressCountry] c ON u.[CountryId] = c.[Id]
    --INNER JOIN 
    --    PaymentDetails p ON u.Id = p.UserId
    WHERE 
        (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        and CAST(u.[createdat] AS date) != CAST(u.[UpdatedAt] AS date)
        AND (
			CAST(u.[UpdatedAt] as date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
			OR CAST(u.[CreatedAt] as date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
		)

        --and CAST(p.[PaymentDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
END
GO
/****** Object:  StoredProcedure [dbo].[GetCompletedForms]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetCompletedForms] 
	-- Add the parameters for the stored procedure here
	@AssociationID int=0,
	@ReportDate date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	 SELECT 
        u.[Id],
        u.[FirstName],
        u.[LastName],
        u.[Email],
        u.[CountryCode],
        u.[MobileNumber],
        u.[DOB],
        u.[UserType],
        u.[Gender],
        o.[Name] as OrganisationName,
        a.[AssociationName],
        u.[EMPIDPFNo],
        u.[Address],
        u.[City],
        s.[Name] as state,
        c.[Name] as country,
        u.[Pincode],
        u.IsProfileComplete
         
        
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
    LEFT JOIN 
        [AddressState] s ON u.[StateId] = s.[Id]
    LEFT JOIN 
        [AddressCountry] c ON u.[CountryId] = c.[Id]
    --INNER JOIN 
    --    PaymentDetails p ON u.Id = p.UserId
    WHERE 
        (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        and CAST(u.[createdat] AS date) = @ReportDate
        and u.IsProfileComplete=1
END
GO
/****** Object:  StoredProcedure [dbo].[GetBouncedPayments]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetBouncedPayments]
	-- Add the parameters for the stored procedure here
	@associationID int=0, 
    @startdate date,
    @enddate date
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
        u.[Id],
        u.[FirstName],
        u.[LastName],
        u.[Email],
        u.[CountryCode],
        u.[MobileNumber],
        o.[Name] as OrganisationName,
        a.[AssociationName],
        p.[PaymentDate],
        p.[TotalPremimumAmount] as PremimumAmount,
        p.[AmountPaid] as PaidAmount,
        p.[PaymentType] as PaymentType,
        p.[PaymentMode],
        p.[PaymentStatus] 
        
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
    LEFT JOIN 
        [AddressState] s ON u.[StateId] = s.[Id]
    LEFT JOIN 
        [AddressCountry] c ON u.[CountryId] = c.[Id]
    INNER JOIN 
        PaymentDetails p ON u.Id = p.UserId
    WHERE 
        (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        AND p.PaymentStatus = 3 -- enum PaymentStatus Initiated = 3
        --AND p.PaymentDate BETWEEN @StartDate AND @EndDate
        and CAST(p.[PaymentDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
END
GO
/****** Object:  StoredProcedure [dbo].[GetAssociationWisePaymentDetails]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

CREATE PROCEDURE [dbo].[GetAssociationWisePaymentDetails] 
    @associationID int=0,
    @paymentStatusId int=0,    
    @startdate date,
    @enddate date
AS
BEGIN
    SELECT 
        u.[Id],
        u.[FirstName],
        u.[LastName],
        u.[Email],
        u.[CountryCode],
        u.[MobileNumber],
        o.[Name] as OrganisationName,
        a.[AssociationName],
        p.[PaymentDate],
        p.[TotalPremimumAmount] as PremimumAmount,
        p.[AmountPaid] as PaidAmount,
        p.[PaymentType] as PaymentType,
        p.[PaymentMode],
        p.[PaymentStatus] 
        
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
    LEFT JOIN 
        [AddressState] s ON u.[StateId] = s.[Id]
    LEFT JOIN 
        [AddressCountry] c ON u.[CountryId] = c.[Id]
    INNER JOIN 
        PaymentDetails p ON u.Id = p.UserId
    WHERE 
        (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        AND (@PaymentStatusId = 0 OR p.PaymentStatus = @PaymentStatusId)
        --AND p.PaymentDate BETWEEN @StartDate AND @EndDate
        and CAST(p.[PaymentDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
    
    ORDER BY
        u.[AssociationId];
END
GO
/****** Object:  StoredProcedure [dbo].[USPGetReconciledOnlinePaymentsReport]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USPGetReconciledOnlinePaymentsReport]
	-- Add the parameters for the stored procedure here
	@associationID int=0, 
    @startdate date,
    @enddate date
AS
BEGIN
	BEGIN TRY
SELECT 
	    a.AssociationName as 'Association Name',
	    o.Name as 'Retiree Organisation Name',
        u.EMPIDPFNo 'PF No/Emp ID',
        isnull(u.[FirstName],'')+ ' '+ISNULL(u.LastName,'') 'Retiree Name',
		(case when  isnull(u.Gender,'')=1 then 'Male'
		when  isnull(u.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),u.DOB,103) as 'Date of Birth',
		isnull(u.Address,'') as Address,
		u.[Email],
		u.MobileNumber as 'Mobile Number'
		,(case when  opt.IsSelfPremiumSelected=1 and opt.IsSelfSpousePremiumSelected=0 then 'Only Self'
		when   opt.IsSelfPremiumSelected=0 and opt.IsSelfSpousePremiumSelected=1 then 'Self+Spouse' end 
		) PolicyOption,
		
		isnull(dep.[Name of Spouse],'') as [Name of Spouse]
		,isnull(dep.[Date of Birth],'') as 'Date of Birth' 
		, isnull(dep.Gender,'') as Gender
		,isnull(dep.[Name of Handicapped Child 1],'') as [Name of Handicapped Child 1]
		,isnull(dep.[Child1 Date of Birth],'') as [Child1 Date of Birth]
		,isnull(dep.[Child1 Gender],'') as [Child1 Gender],

		isnull(dep.[Name of Handicapped Child 2],'') as [Name of Handicapped Child 2]
		,isnull(dep.[Child2 Date of Birth],'') as [Child2 Date of Birth]
		,isnull(dep.[Child2 Gender],'') as [Child2 Gender],

		--dep.[Name of Handicapped Child 2],dep.[Child2 Date of Birth],dep.[Child2 Gender],

		payment.ChildPremium as 'Dependent Child/Children Premium',
		payment.TotalPremimum as 'Total Premium With Handicapped Children/Child',
		case when  payment.PaymentType =1 then 'Cheque' 
		when  payment.PaymentType =2 then 'NEFT' 
		when  payment.PaymentType =3 then 'UPI' 
		when  payment.PaymentType =4 then 'Gateway'  end 'Payment Mode'

		,(case when  payment.PaymentType =1 then cheque.[Bank Name (Amount Transfer From)]
		when  payment.PaymentType =2 then neft.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =3 then upi.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =4 then gate.[Bank Name (Amount Transfer From)]   end ) as 'Bank Name (Amount Transfer From)'

		,(case when  payment.PaymentType =1 then cheque.AccountNumber
		when  payment.PaymentType =2 then neft.AccountNumber
		when  payment.PaymentType =3 then upi.AccountNumber
		when  payment.PaymentType =4 then gate.AccountNumber  end ) as 'Account Number'

		,(case when  payment.PaymentType =1 then cheque.IfscCode
		when  payment.PaymentType =2 then neft.IfscCode
		when  payment.PaymentType =3 then upi.IfscCode
		when  payment.PaymentType =4 then gate.IfscCode  end ) as 'IFSC Code'

			,isnull((case when  payment.PaymentType =1 then cheque.[Transaction Number/Cheque Number]
		when  payment.PaymentType =2 then neft.[Transaction Number/Cheque Number]
		when  payment.PaymentType =3 then upi.[Transaction Number/Cheque Number]
		when  payment.PaymentType =4 then gate.[Transaction Number/Cheque Number]  end ) ,'')as 'Transaction Number/Cheque Number'

			,(case when  payment.PaymentType =1 then cheque.[Payment Date/Cheque Date]
		when  payment.PaymentType =2 then neft.[Payment Date/Cheque Date]
		when  payment.PaymentType =3 then upi.[Payment Date/Cheque Date]
		when  payment.PaymentType =4 then gate.[Payment Date/Cheque Date] end ) as 'Payment Date/Cheque Date'

		,Nominee.[Nominee Name]
		,Nominee.[Nominee Gender]
		,Nominee.[Nominee Date of Birth]
		,Nominee.[Nominee Relation]
		,isnull(cheque.[Cheque Deposit Location],'') as 'Cheque Deposit Location'
		,'I agree to the terms' as Agreement
		,payment.CreatedAt as 'Added Time'
		,payment.UpdatedAt as 'Modified Time'
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
	INNER JOIN 
        PaymentDetails p ON u.Id = p.UserId
	   left join (
	select  distinct  ph.UserId, ppp.PolicyHeaderId,ppp.IsSelfPremiumSelected,ppp.IsSelfSpousePremiumSelected from  PolicyHeader ph	
	inner join  PolicyProductPremimum ppp on ppp.PolicyHeaderId=ph.Id
	inner join PolicyProductDetails ppd on ppp.PolicyHeaderId=ppd.PolicyHeaderId and ppp.ProductPremimumId=ppd.SumInsuredPremimumId
	)opt on opt.UserId=u.Id
    left  join (select 
	distinct 
	isnull(spouse.Name,'') as 'Name of Spouse',
	(case when  isnull(spouse.Gender,'')=1 then 'Male'
		when  isnull(spouse.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),spouse.DateOfBirth,103) as 'Date of Birth',

		isnull(child1.Name,'') as 'Name of Handicapped Child 1',
			CONVERT (varchar(10),child1.DateOfBirth,103) as 'Child1 Date of Birth',
          (case when  isnull(child1.Gender,'')=1 then 'Male'
		when  isnull(child1.Gender,'')=1 then 'Female'
		else '' end) as  'Child1 Gender',
	
	   isnull(child2.Name,'') as 'Name of Handicapped Child 2',
			CONVERT (varchar(10),child2.DateOfBirth,103) as 'Child2 Date of Birth',
          (case when  isnull(child2.Gender,'')=1 then 'Male'
		when  isnull(child2.Gender,'')=1 then 'Female'
		else '' end) as  'Child2 Gender'
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson spouse on spouse.PolicyId=ph.Id and bd.Spouse=spouse.Id
left join BeneficiaryPerson child1 on child1.PolicyId=ph.Id and bd.Child1=child1.Id
left join BeneficiaryPerson child2 on child2.PolicyId=ph.Id and bd.Child2=child2.Id
where (spouse.Name is not null or child1.Name is not null or child2.Name is not null)
)dep on dep.UserId=u.Id
left join 
(select distinct ph.UserId,ph.CreatedAt,ph.UpdatedAt,  ph.TotalPremimum,PaymentType,ph.ChildPremium from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id) payment on payment.UserId=u.Id
left join (
select 
	distinct 
	isnull(Nominee.Name,'') as 'Nominee Name',
	(case when  isnull(Nominee.Gender,'')=1 then 'Male'
		when  isnull(Nominee.Gender,'')=1 then 'Female'
		else '' end) as  'Nominee Gender',
		CONVERT (varchar(10),Nominee.DateOfBirth,103) as 'Nominee Date of Birth'
		,(case when Nominee.NomineeRelation=1 then 'Spouse'
		when Nominee.NomineeRelation=2 then 'Father'
		when Nominee.NomineeRelation=3 then 'Mother'
		when Nominee.NomineeRelation=4 then 'Son'
		when Nominee.NomineeRelation=5 then 'Daughter'
		when Nominee.NomineeRelation=6 then 'FatherInlaw'
		when Nominee.NomineeRelation=7 then 'Motherinlaw'
		when Nominee.NomineeRelation=8 then 'SonInlaw'
		when Nominee.NomineeRelation=9 then 'DaughterInlaw'
		when Nominee.NomineeRelation=10 then 'Brother'
		when Nominee.NomineeRelation=11 then 'Sister'
		when Nominee.NomineeRelation=12 then 'Grandfather'
		when Nominee.NomineeRelation=13 then 'Grandmother'
		when Nominee.NomineeRelation=14 then 'Grandson'
		when Nominee.NomineeRelation=15 then 'Granddaughter'
		when Nominee.NomineeRelation=16 then 'Nephew'
		when Nominee.NomineeRelation=17 then 'Niece'
			when Nominee.NomineeRelation=18 then 'Uncle'
		when Nominee.NomineeRelation=19 then 'Aunt'
		when Nominee.NomineeRelation=20 then 'Cousin'
		when Nominee.NomineeRelation=21 then 'Guardian'
			when Nominee.NomineeRelation=22 then 'Partner'
				when Nominee.NomineeRelation=0 then 'None' end) as 'Nominee Relation' 
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson Nominee  on Nominee.PolicyId=ph.Id and bd.Nominee=Nominee.Id
where (Nominee.Name is not null)
) Nominee on Nominee.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,ch.BankName as 'Bank Name (Amount Transfer From)','' as AccountNumber,'' as IfscCode,ch.ChequeNumber as 'Transaction Number/Cheque Number'
,a.AssociationName as InFavourOf,pd.CreatedAt as 'Payment Date/Cheque Date'
,ch.ChequeDepositLocation [Cheque Deposit Location]
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeCheque ch on ch.PaymentDetailId=pd.Id
inner join Association a on ch.InFavourOfAssociationId=a.Id
) cheque on cheque.UserId=u.Id
left join (
select distinct ph.UserId,PaymentType
,neft.BankName as 'Bank Name (Amount Transfer From)',neft.TransactionId as 'Transaction Number/Cheque Number'
,neft.AccountNumber,neft.IfscCode,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeNEFT neft on neft.PaymentDetailId=pd.Id
)neft on neft.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,upi.TransactionNumber as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeUPI upi on upi.PaymentDetailId=pd.Id
)upi on upi.UserId=u.Id
left join (select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,gate.TransactionId as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeGateway gate on gate.PaymentDetailId=pd.Id
)gate on gate.UserId=u.Id

WHERE 
    (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        AND p.PaymentMode = 1 -- enum PaymentMode IOnline=1
        --AND p.PaymentDate BETWEEN @StartDate AND @EndDate
        and CAST(p.[PaymentDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
     and u.IsProfileComplete=1
    END TRY
    BEGIN CATCH
      
	   EXEC dbo.spGetErrorInfo ;
    
        
    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USPGetIncompleteTransactionReport]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USPGetIncompleteTransactionReport]
	-- Add the parameters for the stored procedure here
	@AssociationID int=0
	,@ReportDate date
AS
BEGIN
	BEGIN TRY
SELECT 
	    a.AssociationName as 'Association Name',
	    o.Name as 'Retiree Organisation Name',
        u.EMPIDPFNo 'PF No/Emp ID',
        isnull(u.[FirstName],'')+ ' '+ISNULL(u.LastName,'') 'Retiree Name',
		(case when  isnull(u.Gender,'')=1 then 'Male'
		when  isnull(u.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),u.DOB,103) as 'Date of Birth',
		isnull(u.Address,'') as Address,
		u.[Email],
		u.MobileNumber as 'Mobile Number'
		,(case when  opt.IsSelfPremiumSelected=1 and opt.IsSelfSpousePremiumSelected=0 then 'Only Self'
		when   opt.IsSelfPremiumSelected=0 and opt.IsSelfSpousePremiumSelected=1 then 'Self+Spouse' end 
		) PolicyOption,
		
		isnull(dep.[Name of Spouse],'') as [Name of Spouse]
		,isnull(dep.[Date of Birth],'') as 'Date of Birth' 
		, isnull(dep.Gender,'') as Gender
		,isnull(dep.[Name of Handicapped Child 1],'') as [Name of Handicapped Child 1]
		,isnull(dep.[Child1 Date of Birth],'') as [Child1 Date of Birth]
		,isnull(dep.[Child1 Gender],'') as [Child1 Gender],

		isnull(dep.[Name of Handicapped Child 2],'') as [Name of Handicapped Child 2]
		,isnull(dep.[Child2 Date of Birth],'') as [Child2 Date of Birth]
		,isnull(dep.[Child2 Gender],'') as [Child2 Gender],

		--dep.[Name of Handicapped Child 2],dep.[Child2 Date of Birth],dep.[Child2 Gender],

		payment.ChildPremium as 'Dependent Child/Children Premium',
		payment.TotalPremimum as 'Total Premium With Handicapped Children/Child',
		case when  payment.PaymentType =1 then 'Cheque' 
		when  payment.PaymentType =2 then 'NEFT' 
		when  payment.PaymentType =3 then 'UPI' 
		when  payment.PaymentType =4 then 'Gateway'  end 'Payment Mode'

		,(case when  payment.PaymentType =1 then cheque.[Bank Name (Amount Transfer From)]
		when  payment.PaymentType =2 then neft.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =3 then upi.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =4 then gate.[Bank Name (Amount Transfer From)]   end ) as 'Bank Name (Amount Transfer From)'

		,(case when  payment.PaymentType =1 then cheque.AccountNumber
		when  payment.PaymentType =2 then neft.AccountNumber
		when  payment.PaymentType =3 then upi.AccountNumber
		when  payment.PaymentType =4 then gate.AccountNumber  end ) as 'Account Number'

		,(case when  payment.PaymentType =1 then cheque.IfscCode
		when  payment.PaymentType =2 then neft.IfscCode
		when  payment.PaymentType =3 then upi.IfscCode
		when  payment.PaymentType =4 then gate.IfscCode  end ) as 'IFSC Code'

			,isnull((case when  payment.PaymentType =1 then cheque.[Transaction Number/Cheque Number]
		when  payment.PaymentType =2 then neft.[Transaction Number/Cheque Number]
		when  payment.PaymentType =3 then upi.[Transaction Number/Cheque Number]
		when  payment.PaymentType =4 then gate.[Transaction Number/Cheque Number]  end ) ,'')as 'Transaction Number/Cheque Number'

			,(case when  payment.PaymentType =1 then cheque.[Payment Date/Cheque Date]
		when  payment.PaymentType =2 then neft.[Payment Date/Cheque Date]
		when  payment.PaymentType =3 then upi.[Payment Date/Cheque Date]
		when  payment.PaymentType =4 then gate.[Payment Date/Cheque Date] end ) as 'Payment Date/Cheque Date'

		,Nominee.[Nominee Name]
		,Nominee.[Nominee Gender]
		,Nominee.[Nominee Date of Birth]
		,Nominee.[Nominee Relation]
		,isnull(cheque.[Cheque Deposit Location],'') as 'Cheque Deposit Location'
		,'I agree to the terms' as Agreement
		,payment.CreatedAt as 'Added Time'
		,payment.UpdatedAt as 'Modified Time'
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
   INNER JOIN 
    PaymentDetails p ON u.Id = p.UserId
	   left join (
	select  distinct  ph.UserId, ppp.PolicyHeaderId,ppp.IsSelfPremiumSelected,ppp.IsSelfSpousePremiumSelected from  PolicyHeader ph	
	inner join  PolicyProductPremimum ppp on ppp.PolicyHeaderId=ph.Id
	inner join PolicyProductDetails ppd on ppp.PolicyHeaderId=ppd.PolicyHeaderId and ppp.ProductPremimumId=ppd.SumInsuredPremimumId
	)opt on opt.UserId=u.Id
    left  join (select 
	distinct 
	isnull(spouse.Name,'') as 'Name of Spouse',
	(case when  isnull(spouse.Gender,'')=1 then 'Male'
		when  isnull(spouse.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),spouse.DateOfBirth,103) as 'Date of Birth',

		isnull(child1.Name,'') as 'Name of Handicapped Child 1',
			CONVERT (varchar(10),child1.DateOfBirth,103) as 'Child1 Date of Birth',
          (case when  isnull(child1.Gender,'')=1 then 'Male'
		when  isnull(child1.Gender,'')=1 then 'Female'
		else '' end) as  'Child1 Gender',
	
	   isnull(child2.Name,'') as 'Name of Handicapped Child 2',
			CONVERT (varchar(10),child2.DateOfBirth,103) as 'Child2 Date of Birth',
          (case when  isnull(child2.Gender,'')=1 then 'Male'
		when  isnull(child2.Gender,'')=1 then 'Female'
		else '' end) as  'Child2 Gender'
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson spouse on spouse.PolicyId=ph.Id and bd.Spouse=spouse.Id
left join BeneficiaryPerson child1 on child1.PolicyId=ph.Id and bd.Child1=child1.Id
left join BeneficiaryPerson child2 on child2.PolicyId=ph.Id and bd.Child2=child2.Id
where (spouse.Name is not null or child1.Name is not null or child2.Name is not null)
)dep on dep.UserId=u.Id
left join 
(select distinct ph.UserId,ph.CreatedAt,ph.UpdatedAt,  ph.TotalPremimum,PaymentType,ph.ChildPremium from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id) payment on payment.UserId=u.Id
left join (
select 
	distinct 
	isnull(Nominee.Name,'') as 'Nominee Name',
	(case when  isnull(Nominee.Gender,'')=1 then 'Male'
		when  isnull(Nominee.Gender,'')=1 then 'Female'
		else '' end) as  'Nominee Gender',
		CONVERT (varchar(10),Nominee.DateOfBirth,103) as 'Nominee Date of Birth'
		,(case when Nominee.NomineeRelation=1 then 'Spouse'
		when Nominee.NomineeRelation=2 then 'Father'
		when Nominee.NomineeRelation=3 then 'Mother'
		when Nominee.NomineeRelation=4 then 'Son'
		when Nominee.NomineeRelation=5 then 'Daughter'
		when Nominee.NomineeRelation=6 then 'FatherInlaw'
		when Nominee.NomineeRelation=7 then 'Motherinlaw'
		when Nominee.NomineeRelation=8 then 'SonInlaw'
		when Nominee.NomineeRelation=9 then 'DaughterInlaw'
		when Nominee.NomineeRelation=10 then 'Brother'
		when Nominee.NomineeRelation=11 then 'Sister'
		when Nominee.NomineeRelation=12 then 'Grandfather'
		when Nominee.NomineeRelation=13 then 'Grandmother'
		when Nominee.NomineeRelation=14 then 'Grandson'
		when Nominee.NomineeRelation=15 then 'Granddaughter'
		when Nominee.NomineeRelation=16 then 'Nephew'
		when Nominee.NomineeRelation=17 then 'Niece'
			when Nominee.NomineeRelation=18 then 'Uncle'
		when Nominee.NomineeRelation=19 then 'Aunt'
		when Nominee.NomineeRelation=20 then 'Cousin'
		when Nominee.NomineeRelation=21 then 'Guardian'
			when Nominee.NomineeRelation=22 then 'Partner'
				when Nominee.NomineeRelation=0 then 'None' end) as 'Nominee Relation' 
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson Nominee  on Nominee.PolicyId=ph.Id and bd.Nominee=Nominee.Id
where (Nominee.Name is not null)
) Nominee on Nominee.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,ch.BankName as 'Bank Name (Amount Transfer From)','' as AccountNumber,'' as IfscCode,ch.ChequeNumber as 'Transaction Number/Cheque Number'
,a.AssociationName as InFavourOf,pd.CreatedAt as 'Payment Date/Cheque Date'
,ch.ChequeDepositLocation [Cheque Deposit Location]
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeCheque ch on ch.PaymentDetailId=pd.Id
inner join Association a on ch.InFavourOfAssociationId=a.Id
) cheque on cheque.UserId=u.Id
left join (
select distinct ph.UserId,PaymentType
,neft.BankName as 'Bank Name (Amount Transfer From)',neft.TransactionId as 'Transaction Number/Cheque Number'
,neft.AccountNumber,neft.IfscCode,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeNEFT neft on neft.PaymentDetailId=pd.Id
)neft on neft.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,upi.TransactionNumber as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeUPI upi on upi.PaymentDetailId=pd.Id
)upi on upi.UserId=u.Id
left join (select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,gate.TransactionId as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeGateway gate on gate.PaymentDetailId=pd.Id
)gate on gate.UserId=u.Id

WHERE 
    (@AssociationID = 0 OR u.AssociationId = @AssociationID)
   AND  p.PaymentStatus !=1 
    AND cast(p.[PaymentDate] as DATE) = CAST(@ReportDate AS date)
     and u.IsProfileComplete=1
    END TRY
    BEGIN CATCH
      
	   EXEC dbo.spGetErrorInfo ;
    
        
    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USPGetCorrectionReport]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USPGetCorrectionReport]
	-- Add the parameters for the stored procedure here
	@associationID int=0, 
    @startdate date,
    @enddate date
AS
BEGIN
	BEGIN TRY
SELECT 
	    a.AssociationName as 'Association Name',
	    o.Name as 'Retiree Organisation Name',
        u.EMPIDPFNo 'PF No/Emp ID',
        isnull(u.[FirstName],'')+ ' '+ISNULL(u.LastName,'') 'Retiree Name',
		(case when  isnull(u.Gender,'')=1 then 'Male'
		when  isnull(u.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),u.DOB,103) as 'Date of Birth',
		isnull(u.Address,'') as Address,
		u.[Email],
		u.MobileNumber as 'Mobile Number'
		,(case when  opt.IsSelfPremiumSelected=1 and opt.IsSelfSpousePremiumSelected=0 then 'Only Self'
		when   opt.IsSelfPremiumSelected=0 and opt.IsSelfSpousePremiumSelected=1 then 'Self+Spouse' end 
		) PolicyOption,
		
		isnull(dep.[Name of Spouse],'') as [Name of Spouse]
		,isnull(dep.[Date of Birth],'') as 'Date of Birth' 
		, isnull(dep.Gender,'') as Gender
		,isnull(dep.[Name of Handicapped Child 1],'') as [Name of Handicapped Child 1]
		,isnull(dep.[Child1 Date of Birth],'') as [Child1 Date of Birth]
		,isnull(dep.[Child1 Gender],'') as [Child1 Gender],

		isnull(dep.[Name of Handicapped Child 2],'') as [Name of Handicapped Child 2]
		,isnull(dep.[Child2 Date of Birth],'') as [Child2 Date of Birth]
		,isnull(dep.[Child2 Gender],'') as [Child2 Gender],

		--dep.[Name of Handicapped Child 2],dep.[Child2 Date of Birth],dep.[Child2 Gender],

		payment.ChildPremium as 'Dependent Child/Children Premium',
		payment.TotalPremimum as 'Total Premium With Handicapped Children/Child',
		case when  payment.PaymentType =1 then 'Cheque' 
		when  payment.PaymentType =2 then 'NEFT' 
		when  payment.PaymentType =3 then 'UPI' 
		when  payment.PaymentType =4 then 'Gateway'  end 'Payment Mode'

		,(case when  payment.PaymentType =1 then cheque.[Bank Name (Amount Transfer From)]
		when  payment.PaymentType =2 then neft.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =3 then upi.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =4 then gate.[Bank Name (Amount Transfer From)]   end ) as 'Bank Name (Amount Transfer From)'

		,(case when  payment.PaymentType =1 then cheque.AccountNumber
		when  payment.PaymentType =2 then neft.AccountNumber
		when  payment.PaymentType =3 then upi.AccountNumber
		when  payment.PaymentType =4 then gate.AccountNumber  end ) as 'Account Number'

		,(case when  payment.PaymentType =1 then cheque.IfscCode
		when  payment.PaymentType =2 then neft.IfscCode
		when  payment.PaymentType =3 then upi.IfscCode
		when  payment.PaymentType =4 then gate.IfscCode  end ) as 'IFSC Code'

			,isnull((case when  payment.PaymentType =1 then cheque.[Transaction Number/Cheque Number]
		when  payment.PaymentType =2 then neft.[Transaction Number/Cheque Number]
		when  payment.PaymentType =3 then upi.[Transaction Number/Cheque Number]
		when  payment.PaymentType =4 then gate.[Transaction Number/Cheque Number]  end ) ,'')as 'Transaction Number/Cheque Number'

			,(case when  payment.PaymentType =1 then cheque.[Payment Date/Cheque Date]
		when  payment.PaymentType =2 then neft.[Payment Date/Cheque Date]
		when  payment.PaymentType =3 then upi.[Payment Date/Cheque Date]
		when  payment.PaymentType =4 then gate.[Payment Date/Cheque Date] end ) as 'Payment Date/Cheque Date'

		,Nominee.[Nominee Name]
		,Nominee.[Nominee Gender]
		,Nominee.[Nominee Date of Birth]
		,Nominee.[Nominee Relation]
		,isnull(cheque.[Cheque Deposit Location],'') as 'Cheque Deposit Location'
		,'I agree to the terms' as Agreement
		,payment.CreatedAt as 'Added Time'
		,payment.UpdatedAt as 'Modified Time'
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
   
	   left join (
	select  distinct  ph.UserId, ppp.PolicyHeaderId,ppp.IsSelfPremiumSelected,ppp.IsSelfSpousePremiumSelected from  PolicyHeader ph	
	inner join  PolicyProductPremimum ppp on ppp.PolicyHeaderId=ph.Id
	inner join PolicyProductDetails ppd on ppp.PolicyHeaderId=ppd.PolicyHeaderId and ppp.ProductPremimumId=ppd.SumInsuredPremimumId
	)opt on opt.UserId=u.Id
    left  join (select 
	distinct 
	isnull(spouse.Name,'') as 'Name of Spouse',
	(case when  isnull(spouse.Gender,'')=1 then 'Male'
		when  isnull(spouse.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),spouse.DateOfBirth,103) as 'Date of Birth',

		isnull(child1.Name,'') as 'Name of Handicapped Child 1',
			CONVERT (varchar(10),child1.DateOfBirth,103) as 'Child1 Date of Birth',
          (case when  isnull(child1.Gender,'')=1 then 'Male'
		when  isnull(child1.Gender,'')=1 then 'Female'
		else '' end) as  'Child1 Gender',
	
	   isnull(child2.Name,'') as 'Name of Handicapped Child 2',
			CONVERT (varchar(10),child2.DateOfBirth,103) as 'Child2 Date of Birth',
          (case when  isnull(child2.Gender,'')=1 then 'Male'
		when  isnull(child2.Gender,'')=1 then 'Female'
		else '' end) as  'Child2 Gender'
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson spouse on spouse.PolicyId=ph.Id and bd.Spouse=spouse.Id
left join BeneficiaryPerson child1 on child1.PolicyId=ph.Id and bd.Child1=child1.Id
left join BeneficiaryPerson child2 on child2.PolicyId=ph.Id and bd.Child2=child2.Id
where (spouse.Name is not null or child1.Name is not null or child2.Name is not null)
)dep on dep.UserId=u.Id
left join 
(select distinct ph.UserId,ph.CreatedAt,ph.UpdatedAt,  ph.TotalPremimum,PaymentType,ph.ChildPremium from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id) payment on payment.UserId=u.Id
left join (
select 
	distinct 
	isnull(Nominee.Name,'') as 'Nominee Name',
	(case when  isnull(Nominee.Gender,'')=1 then 'Male'
		when  isnull(Nominee.Gender,'')=1 then 'Female'
		else '' end) as  'Nominee Gender',
		CONVERT (varchar(10),Nominee.DateOfBirth,103) as 'Nominee Date of Birth'
		,(case when Nominee.NomineeRelation=1 then 'Spouse'
		when Nominee.NomineeRelation=2 then 'Father'
		when Nominee.NomineeRelation=3 then 'Mother'
		when Nominee.NomineeRelation=4 then 'Son'
		when Nominee.NomineeRelation=5 then 'Daughter'
		when Nominee.NomineeRelation=6 then 'FatherInlaw'
		when Nominee.NomineeRelation=7 then 'Motherinlaw'
		when Nominee.NomineeRelation=8 then 'SonInlaw'
		when Nominee.NomineeRelation=9 then 'DaughterInlaw'
		when Nominee.NomineeRelation=10 then 'Brother'
		when Nominee.NomineeRelation=11 then 'Sister'
		when Nominee.NomineeRelation=12 then 'Grandfather'
		when Nominee.NomineeRelation=13 then 'Grandmother'
		when Nominee.NomineeRelation=14 then 'Grandson'
		when Nominee.NomineeRelation=15 then 'Granddaughter'
		when Nominee.NomineeRelation=16 then 'Nephew'
		when Nominee.NomineeRelation=17 then 'Niece'
			when Nominee.NomineeRelation=18 then 'Uncle'
		when Nominee.NomineeRelation=19 then 'Aunt'
		when Nominee.NomineeRelation=20 then 'Cousin'
		when Nominee.NomineeRelation=21 then 'Guardian'
			when Nominee.NomineeRelation=22 then 'Partner'
				when Nominee.NomineeRelation=0 then 'None' end) as 'Nominee Relation' 
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson Nominee  on Nominee.PolicyId=ph.Id and bd.Nominee=Nominee.Id
where (Nominee.Name is not null)
) Nominee on Nominee.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,ch.BankName as 'Bank Name (Amount Transfer From)','' as AccountNumber,'' as IfscCode,ch.ChequeNumber as 'Transaction Number/Cheque Number'
,a.AssociationName as InFavourOf,pd.CreatedAt as 'Payment Date/Cheque Date'
,ch.ChequeDepositLocation [Cheque Deposit Location]
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeCheque ch on ch.PaymentDetailId=pd.Id
inner join Association a on ch.InFavourOfAssociationId=a.Id
) cheque on cheque.UserId=u.Id
left join (
select distinct ph.UserId,PaymentType
,neft.BankName as 'Bank Name (Amount Transfer From)',neft.TransactionId as 'Transaction Number/Cheque Number'
,neft.AccountNumber,neft.IfscCode,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeNEFT neft on neft.PaymentDetailId=pd.Id
)neft on neft.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,upi.TransactionNumber as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeUPI upi on upi.PaymentDetailId=pd.Id
)upi on upi.UserId=u.Id
left join (select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,gate.TransactionId as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeGateway gate on gate.PaymentDetailId=pd.Id
)gate on gate.UserId=u.Id

WHERE 
    (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        and CAST(u.[createdat] AS date) != CAST(u.[UpdatedAt] AS date)
        AND (
			CAST(u.[UpdatedAt] as date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
			OR CAST(u.[CreatedAt] as date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
		)
     and u.IsProfileComplete=1
    END TRY
    BEGIN CATCH
      
	   EXEC dbo.spGetErrorInfo ;
    
        
    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USPGetCompletedFormsReport]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USPGetCompletedFormsReport]
	-- Add the parameters for the stored procedure here
	@associationID int=0 ,
	@ReportDate date
	--,@enddate date
AS
BEGIN
	BEGIN TRY
SELECT 
	    a.AssociationName as 'Association Name',
	    o.Name as 'Retiree Organisation Name',
        u.EMPIDPFNo 'PF No/Emp ID',
        isnull(u.[FirstName],'')+ ' '+ISNULL(u.LastName,'') 'Retiree Name',
		(case when  isnull(u.Gender,'')=1 then 'Male'
		when  isnull(u.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),u.DOB,103) as 'Date of Birth',
		isnull(u.Address,'') as Address,
		u.[Email],
		u.MobileNumber as 'Mobile Number'
		,(case when  opt.IsSelfPremiumSelected=1 and opt.IsSelfSpousePremiumSelected=0 then 'Only Self'
		when   opt.IsSelfPremiumSelected=0 and opt.IsSelfSpousePremiumSelected=1 then 'Self+Spouse' end 
		) PolicyOption,
		
		isnull(dep.[Name of Spouse],'') as [Name of Spouse]
		,isnull(dep.[Date of Birth],'') as 'Date of Birth' 
		, isnull(dep.Gender,'') as Gender
		,isnull(dep.[Name of Handicapped Child 1],'') as [Name of Handicapped Child 1]
		,isnull(dep.[Child1 Date of Birth],'') as [Child1 Date of Birth]
		,isnull(dep.[Child1 Gender],'') as [Child1 Gender],

		isnull(dep.[Name of Handicapped Child 2],'') as [Name of Handicapped Child 2]
		,isnull(dep.[Child2 Date of Birth],'') as [Child2 Date of Birth]
		,isnull(dep.[Child2 Gender],'') as [Child2 Gender],

		--dep.[Name of Handicapped Child 2],dep.[Child2 Date of Birth],dep.[Child2 Gender],

		payment.ChildPremium as 'Dependent Child/Children Premium',
		payment.TotalPremimum as 'Total Premium With Handicapped Children/Child',
		case when  payment.PaymentType =1 then 'Cheque' 
		when  payment.PaymentType =2 then 'NEFT' 
		when  payment.PaymentType =3 then 'UPI' 
		when  payment.PaymentType =4 then 'Gateway'  end 'Payment Mode'

		,(case when  payment.PaymentType =1 then cheque.[Bank Name (Amount Transfer From)]
		when  payment.PaymentType =2 then neft.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =3 then upi.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =4 then gate.[Bank Name (Amount Transfer From)]   end ) as 'Bank Name (Amount Transfer From)'

		,(case when  payment.PaymentType =1 then cheque.AccountNumber
		when  payment.PaymentType =2 then neft.AccountNumber
		when  payment.PaymentType =3 then upi.AccountNumber
		when  payment.PaymentType =4 then gate.AccountNumber  end ) as 'Account Number'

		,(case when  payment.PaymentType =1 then cheque.IfscCode
		when  payment.PaymentType =2 then neft.IfscCode
		when  payment.PaymentType =3 then upi.IfscCode
		when  payment.PaymentType =4 then gate.IfscCode  end ) as 'IFSC Code'

			,isnull((case when  payment.PaymentType =1 then cheque.[Transaction Number/Cheque Number]
		when  payment.PaymentType =2 then neft.[Transaction Number/Cheque Number]
		when  payment.PaymentType =3 then upi.[Transaction Number/Cheque Number]
		when  payment.PaymentType =4 then gate.[Transaction Number/Cheque Number]  end ) ,'')as 'Transaction Number/Cheque Number'

			,(case when  payment.PaymentType =1 then cheque.[Payment Date/Cheque Date]
		when  payment.PaymentType =2 then neft.[Payment Date/Cheque Date]
		when  payment.PaymentType =3 then upi.[Payment Date/Cheque Date]
		when  payment.PaymentType =4 then gate.[Payment Date/Cheque Date] end ) as 'Payment Date/Cheque Date'

		,Nominee.[Nominee Name]
		,Nominee.[Nominee Gender]
		,Nominee.[Nominee Date of Birth]
		,Nominee.[Nominee Relation]
		,isnull(cheque.[Cheque Deposit Location],'') as 'Cheque Deposit Location'
		,'I agree to the terms' as Agreement
		,payment.CreatedAt as 'Added Time'
		,payment.UpdatedAt as 'Modified Time'
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
   
	   left join (
	select  distinct  ph.UserId, ppp.PolicyHeaderId,ppp.IsSelfPremiumSelected,ppp.IsSelfSpousePremiumSelected from  PolicyHeader ph	
	inner join  PolicyProductPremimum ppp on ppp.PolicyHeaderId=ph.Id
	inner join PolicyProductDetails ppd on ppp.PolicyHeaderId=ppd.PolicyHeaderId and ppp.ProductPremimumId=ppd.SumInsuredPremimumId
	)opt on opt.UserId=u.Id
    left  join (select 
	distinct 
	isnull(spouse.Name,'') as 'Name of Spouse',
	(case when  isnull(spouse.Gender,'')=1 then 'Male'
		when  isnull(spouse.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),spouse.DateOfBirth,103) as 'Date of Birth',

		isnull(child1.Name,'') as 'Name of Handicapped Child 1',
			CONVERT (varchar(10),child1.DateOfBirth,103) as 'Child1 Date of Birth',
          (case when  isnull(child1.Gender,'')=1 then 'Male'
		when  isnull(child1.Gender,'')=1 then 'Female'
		else '' end) as  'Child1 Gender',
	
	   isnull(child2.Name,'') as 'Name of Handicapped Child 2',
			CONVERT (varchar(10),child2.DateOfBirth,103) as 'Child2 Date of Birth',
          (case when  isnull(child2.Gender,'')=1 then 'Male'
		when  isnull(child2.Gender,'')=1 then 'Female'
		else '' end) as  'Child2 Gender'
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson spouse on spouse.PolicyId=ph.Id and bd.Spouse=spouse.Id
left join BeneficiaryPerson child1 on child1.PolicyId=ph.Id and bd.Child1=child1.Id
left join BeneficiaryPerson child2 on child2.PolicyId=ph.Id and bd.Child2=child2.Id
where (spouse.Name is not null or child1.Name is not null or child2.Name is not null)
)dep on dep.UserId=u.Id
left join 
(select distinct ph.UserId,ph.CreatedAt,ph.UpdatedAt,  ph.TotalPremimum,PaymentType,ph.ChildPremium from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id) payment on payment.UserId=u.Id
left join (
select 
	distinct 
	isnull(Nominee.Name,'') as 'Nominee Name',
	(case when  isnull(Nominee.Gender,'')=1 then 'Male'
		when  isnull(Nominee.Gender,'')=1 then 'Female'
		else '' end) as  'Nominee Gender',
		CONVERT (varchar(10),Nominee.DateOfBirth,103) as 'Nominee Date of Birth'
		,(case when Nominee.NomineeRelation=1 then 'Spouse'
		when Nominee.NomineeRelation=2 then 'Father'
		when Nominee.NomineeRelation=3 then 'Mother'
		when Nominee.NomineeRelation=4 then 'Son'
		when Nominee.NomineeRelation=5 then 'Daughter'
		when Nominee.NomineeRelation=6 then 'FatherInlaw'
		when Nominee.NomineeRelation=7 then 'Motherinlaw'
		when Nominee.NomineeRelation=8 then 'SonInlaw'
		when Nominee.NomineeRelation=9 then 'DaughterInlaw'
		when Nominee.NomineeRelation=10 then 'Brother'
		when Nominee.NomineeRelation=11 then 'Sister'
		when Nominee.NomineeRelation=12 then 'Grandfather'
		when Nominee.NomineeRelation=13 then 'Grandmother'
		when Nominee.NomineeRelation=14 then 'Grandson'
		when Nominee.NomineeRelation=15 then 'Granddaughter'
		when Nominee.NomineeRelation=16 then 'Nephew'
		when Nominee.NomineeRelation=17 then 'Niece'
			when Nominee.NomineeRelation=18 then 'Uncle'
		when Nominee.NomineeRelation=19 then 'Aunt'
		when Nominee.NomineeRelation=20 then 'Cousin'
		when Nominee.NomineeRelation=21 then 'Guardian'
			when Nominee.NomineeRelation=22 then 'Partner'
				when Nominee.NomineeRelation=0 then 'None' end) as 'Nominee Relation' 
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson Nominee  on Nominee.PolicyId=ph.Id and bd.Nominee=Nominee.Id
where (Nominee.Name is not null)
) Nominee on Nominee.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,ch.BankName as 'Bank Name (Amount Transfer From)','' as AccountNumber,'' as IfscCode,ch.ChequeNumber as 'Transaction Number/Cheque Number'
,a.AssociationName as InFavourOf,pd.CreatedAt as 'Payment Date/Cheque Date'
,ch.ChequeDepositLocation [Cheque Deposit Location]
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeCheque ch on ch.PaymentDetailId=pd.Id
inner join Association a on ch.InFavourOfAssociationId=a.Id
) cheque on cheque.UserId=u.Id
left join (
select distinct ph.UserId,PaymentType
,neft.BankName as 'Bank Name (Amount Transfer From)',neft.TransactionId as 'Transaction Number/Cheque Number'
,neft.AccountNumber,neft.IfscCode,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeNEFT neft on neft.PaymentDetailId=pd.Id
)neft on neft.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,upi.TransactionNumber as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeUPI upi on upi.PaymentDetailId=pd.Id
)upi on upi.UserId=u.Id
left join (select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,gate.TransactionId as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeGateway gate on gate.PaymentDetailId=pd.Id
)gate on gate.UserId=u.Id

WHERE 
    (@AssociationID = 0 OR u.AssociationId = @AssociationID)
     and CAST(u.[createdat] AS date) = @ReportDate
     and u.IsProfileComplete=1
    END TRY
    BEGIN CATCH
      
	   EXEC dbo.spGetErrorInfo ;
    
        
    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USPGetCompletedForms]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[USPGetCompletedForms]
	-- Add the parameters for the stored procedure here
	@associationID int=0 ,
	@ReportDate date
	--,@enddate date
AS
BEGIN
	BEGIN TRY
SELECT 
	    a.AssociationName as 'Association Name',
	    o.Name as 'Retiree Organisation Name',
        u.EMPIDPFNo 'PF No/Emp ID',
        isnull(u.[FirstName],'')+ ' '+ISNULL(u.LastName,'') 'Retiree Name',
		(case when  isnull(u.Gender,'')=1 then 'Male'
		when  isnull(u.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),u.DOB,103) as 'Date of Birth',
		isnull(u.Address,'') as Address,
		u.[Email],
		u.MobileNumber as 'Mobile Number'
		,(case when  opt.IsSelfPremiumSelected=1 and opt.IsSelfSpousePremiumSelected=0 then 'Only Self'
		when   opt.IsSelfPremiumSelected=0 and opt.IsSelfSpousePremiumSelected=1 then 'Self+Spouse' end 
		) PolicyOption,
		
		isnull(dep.[Name of Spouse],'') as [Name of Spouse]
		,isnull(dep.[Date of Birth],'') as 'Date of Birth' 
		, isnull(dep.Gender,'') as Gender
		,isnull(dep.[Name of Handicapped Child 1],'') as [Name of Handicapped Child 1]
		,isnull(dep.[Child1 Date of Birth],'') as [Child1 Date of Birth]
		,isnull(dep.[Child1 Gender],'') as [Child1 Gender],

		isnull(dep.[Name of Handicapped Child 2],'') as [Name of Handicapped Child 2]
		,isnull(dep.[Child2 Date of Birth],'') as [Child2 Date of Birth]
		,isnull(dep.[Child2 Gender],'') as [Child2 Gender],

		--dep.[Name of Handicapped Child 2],dep.[Child2 Date of Birth],dep.[Child2 Gender],

		payment.ChildPremium as 'Dependent Child/Children Premium',
		payment.TotalPremimum as 'Total Premium With Handicapped Children/Child',
		case when  payment.PaymentType =1 then 'Cheque' 
		when  payment.PaymentType =2 then 'NEFT' 
		when  payment.PaymentType =3 then 'UPI' 
		when  payment.PaymentType =4 then 'Gateway'  end 'Payment Mode'

		,(case when  payment.PaymentType =1 then cheque.[Bank Name (Amount Transfer From)]
		when  payment.PaymentType =2 then neft.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =3 then upi.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =4 then gate.[Bank Name (Amount Transfer From)]   end ) as 'Bank Name (Amount Transfer From)'

		,(case when  payment.PaymentType =1 then cheque.AccountNumber
		when  payment.PaymentType =2 then neft.AccountNumber
		when  payment.PaymentType =3 then upi.AccountNumber
		when  payment.PaymentType =4 then gate.AccountNumber  end ) as 'Account Number'

		,(case when  payment.PaymentType =1 then cheque.IfscCode
		when  payment.PaymentType =2 then neft.IfscCode
		when  payment.PaymentType =3 then upi.IfscCode
		when  payment.PaymentType =4 then gate.IfscCode  end ) as 'IFSC Code'

			,isnull((case when  payment.PaymentType =1 then cheque.[Transaction Number/Cheque Number]
		when  payment.PaymentType =2 then neft.[Transaction Number/Cheque Number]
		when  payment.PaymentType =3 then upi.[Transaction Number/Cheque Number]
		when  payment.PaymentType =4 then gate.[Transaction Number/Cheque Number]  end ) ,'')as 'Transaction Number/Cheque Number'

			,(case when  payment.PaymentType =1 then cheque.[Payment Date/Cheque Date]
		when  payment.PaymentType =2 then neft.[Payment Date/Cheque Date]
		when  payment.PaymentType =3 then upi.[Payment Date/Cheque Date]
		when  payment.PaymentType =4 then gate.[Payment Date/Cheque Date] end ) as 'Payment Date/Cheque Date'

		,Nominee.[Nominee Name]
		,Nominee.[Nominee Gender]
		,Nominee.[Nominee Date of Birth]
		,Nominee.[Nominee Relation]
		,isnull(cheque.[Cheque Deposit Location],'') as 'Cheque Deposit Location'
		,'I agree to the terms' as Agreement
		,payment.CreatedAt as 'Added Time'
		,payment.UpdatedAt as 'Modified Time'
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
   
	   left join (
	select  distinct  ph.UserId, ppp.PolicyHeaderId,ppp.IsSelfPremiumSelected,ppp.IsSelfSpousePremiumSelected from  PolicyHeader ph	
	inner join  PolicyProductPremimum ppp on ppp.PolicyHeaderId=ph.Id
	inner join PolicyProductDetails ppd on ppp.PolicyHeaderId=ppd.PolicyHeaderId and ppp.ProductPremimumId=ppd.SumInsuredPremimumId
	)opt on opt.UserId=u.Id
    left  join (select 
	distinct 
	isnull(spouse.Name,'') as 'Name of Spouse',
	(case when  isnull(spouse.Gender,'')=1 then 'Male'
		when  isnull(spouse.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),spouse.DateOfBirth,103) as 'Date of Birth',

		isnull(child1.Name,'') as 'Name of Handicapped Child 1',
			CONVERT (varchar(10),child1.DateOfBirth,103) as 'Child1 Date of Birth',
          (case when  isnull(child1.Gender,'')=1 then 'Male'
		when  isnull(child1.Gender,'')=1 then 'Female'
		else '' end) as  'Child1 Gender',
	
	   isnull(child2.Name,'') as 'Name of Handicapped Child 2',
			CONVERT (varchar(10),child2.DateOfBirth,103) as 'Child2 Date of Birth',
          (case when  isnull(child2.Gender,'')=1 then 'Male'
		when  isnull(child2.Gender,'')=1 then 'Female'
		else '' end) as  'Child2 Gender'
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson spouse on spouse.PolicyId=ph.Id and bd.Spouse=spouse.Id
left join BeneficiaryPerson child1 on child1.PolicyId=ph.Id and bd.Child1=child1.Id
left join BeneficiaryPerson child2 on child2.PolicyId=ph.Id and bd.Child2=child2.Id
where (spouse.Name is not null or child1.Name is not null or child2.Name is not null)
)dep on dep.UserId=u.Id
left join 
(select distinct ph.UserId,ph.CreatedAt,ph.UpdatedAt,  ph.TotalPremimum,PaymentType,ph.ChildPremium from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id) payment on payment.UserId=u.Id
left join (
select 
	distinct 
	isnull(Nominee.Name,'') as 'Nominee Name',
	(case when  isnull(Nominee.Gender,'')=1 then 'Male'
		when  isnull(Nominee.Gender,'')=1 then 'Female'
		else '' end) as  'Nominee Gender',
		CONVERT (varchar(10),Nominee.DateOfBirth,103) as 'Nominee Date of Birth'
		,(case when Nominee.NomineeRelation=1 then 'Spouse'
		when Nominee.NomineeRelation=2 then 'Father'
		when Nominee.NomineeRelation=3 then 'Mother'
		when Nominee.NomineeRelation=4 then 'Son'
		when Nominee.NomineeRelation=5 then 'Daughter'
		when Nominee.NomineeRelation=6 then 'FatherInlaw'
		when Nominee.NomineeRelation=7 then 'Motherinlaw'
		when Nominee.NomineeRelation=8 then 'SonInlaw'
		when Nominee.NomineeRelation=9 then 'DaughterInlaw'
		when Nominee.NomineeRelation=10 then 'Brother'
		when Nominee.NomineeRelation=11 then 'Sister'
		when Nominee.NomineeRelation=12 then 'Grandfather'
		when Nominee.NomineeRelation=13 then 'Grandmother'
		when Nominee.NomineeRelation=14 then 'Grandson'
		when Nominee.NomineeRelation=15 then 'Granddaughter'
		when Nominee.NomineeRelation=16 then 'Nephew'
		when Nominee.NomineeRelation=17 then 'Niece'
			when Nominee.NomineeRelation=18 then 'Uncle'
		when Nominee.NomineeRelation=19 then 'Aunt'
		when Nominee.NomineeRelation=20 then 'Cousin'
		when Nominee.NomineeRelation=21 then 'Guardian'
			when Nominee.NomineeRelation=22 then 'Partner'
				when Nominee.NomineeRelation=0 then 'None' end) as 'Nominee Relation' 
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson Nominee  on Nominee.PolicyId=ph.Id and bd.Nominee=Nominee.Id
where (Nominee.Name is not null)
) Nominee on Nominee.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,ch.BankName as 'Bank Name (Amount Transfer From)','' as AccountNumber,'' as IfscCode,ch.ChequeNumber as 'Transaction Number/Cheque Number'
,a.AssociationName as InFavourOf,pd.CreatedAt as 'Payment Date/Cheque Date'
,ch.ChequeDepositLocation [Cheque Deposit Location]
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeCheque ch on ch.PaymentDetailId=pd.Id
inner join Association a on ch.InFavourOfAssociationId=a.Id
) cheque on cheque.UserId=u.Id
left join (
select distinct ph.UserId,PaymentType
,neft.BankName as 'Bank Name (Amount Transfer From)',neft.TransactionId as 'Transaction Number/Cheque Number'
,neft.AccountNumber,neft.IfscCode,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeNEFT neft on neft.PaymentDetailId=pd.Id
)neft on neft.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,upi.TransactionNumber as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeUPI upi on upi.PaymentDetailId=pd.Id
)upi on upi.UserId=u.Id
left join (select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,gate.TransactionId as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeGateway gate on gate.PaymentDetailId=pd.Id
)gate on gate.UserId=u.Id

WHERE 
    (@AssociationID = 0 OR u.AssociationId = @AssociationID)
     and CAST(u.[createdat] AS date) = @ReportDate
     and u.IsProfileComplete=1
    END TRY
    BEGIN CATCH
      
	   EXEC dbo.spGetErrorInfo ;
    
        
    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USPGetBouncedPaymentsReport]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USPGetBouncedPaymentsReport]
	-- Add the parameters for the stored procedure here
	@associationID int=0, 
    @startdate date,
    @enddate date
AS
BEGIN
	BEGIN TRY
SELECT 
	    a.AssociationName as 'Association Name',
	    o.Name as 'Retiree Organisation Name',
        u.EMPIDPFNo 'PF No/Emp ID',
        isnull(u.[FirstName],'')+ ' '+ISNULL(u.LastName,'') 'Retiree Name',
		(case when  isnull(u.Gender,'')=1 then 'Male'
		when  isnull(u.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),u.DOB,103) as 'Date of Birth',
		isnull(u.Address,'') as Address,
		u.[Email],
		u.MobileNumber as 'Mobile Number'
		,(case when  opt.IsSelfPremiumSelected=1 and opt.IsSelfSpousePremiumSelected=0 then 'Only Self'
		when   opt.IsSelfPremiumSelected=0 and opt.IsSelfSpousePremiumSelected=1 then 'Self+Spouse' end 
		) PolicyOption,
		
		isnull(dep.[Name of Spouse],'') as [Name of Spouse]
		,isnull(dep.[Date of Birth],'') as 'Date of Birth' 
		, isnull(dep.Gender,'') as Gender
		,isnull(dep.[Name of Handicapped Child 1],'') as [Name of Handicapped Child 1]
		,isnull(dep.[Child1 Date of Birth],'') as [Child1 Date of Birth]
		,isnull(dep.[Child1 Gender],'') as [Child1 Gender],

		isnull(dep.[Name of Handicapped Child 2],'') as [Name of Handicapped Child 2]
		,isnull(dep.[Child2 Date of Birth],'') as [Child2 Date of Birth]
		,isnull(dep.[Child2 Gender],'') as [Child2 Gender],

		--dep.[Name of Handicapped Child 2],dep.[Child2 Date of Birth],dep.[Child2 Gender],

		payment.ChildPremium as 'Dependent Child/Children Premium',
		payment.TotalPremimum as 'Total Premium With Handicapped Children/Child',
		case when  payment.PaymentType =1 then 'Cheque' 
		when  payment.PaymentType =2 then 'NEFT' 
		when  payment.PaymentType =3 then 'UPI' 
		when  payment.PaymentType =4 then 'Gateway'  end 'Payment Mode'

		,(case when  payment.PaymentType =1 then cheque.[Bank Name (Amount Transfer From)]
		when  payment.PaymentType =2 then neft.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =3 then upi.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =4 then gate.[Bank Name (Amount Transfer From)]   end ) as 'Bank Name (Amount Transfer From)'

		,(case when  payment.PaymentType =1 then cheque.AccountNumber
		when  payment.PaymentType =2 then neft.AccountNumber
		when  payment.PaymentType =3 then upi.AccountNumber
		when  payment.PaymentType =4 then gate.AccountNumber  end ) as 'Account Number'

		,(case when  payment.PaymentType =1 then cheque.IfscCode
		when  payment.PaymentType =2 then neft.IfscCode
		when  payment.PaymentType =3 then upi.IfscCode
		when  payment.PaymentType =4 then gate.IfscCode  end ) as 'IFSC Code'

			,isnull((case when  payment.PaymentType =1 then cheque.[Transaction Number/Cheque Number]
		when  payment.PaymentType =2 then neft.[Transaction Number/Cheque Number]
		when  payment.PaymentType =3 then upi.[Transaction Number/Cheque Number]
		when  payment.PaymentType =4 then gate.[Transaction Number/Cheque Number]  end ) ,'')as 'Transaction Number/Cheque Number'

			,(case when  payment.PaymentType =1 then cheque.[Payment Date/Cheque Date]
		when  payment.PaymentType =2 then neft.[Payment Date/Cheque Date]
		when  payment.PaymentType =3 then upi.[Payment Date/Cheque Date]
		when  payment.PaymentType =4 then gate.[Payment Date/Cheque Date] end ) as 'Payment Date/Cheque Date'

		,Nominee.[Nominee Name]
		,Nominee.[Nominee Gender]
		,Nominee.[Nominee Date of Birth]
		,Nominee.[Nominee Relation]
		,isnull(cheque.[Cheque Deposit Location],'') as 'Cheque Deposit Location'
		,'I agree to the terms' as Agreement
		,payment.CreatedAt as 'Added Time'
		,payment.UpdatedAt as 'Modified Time'
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
    INNER JOIN 
        PaymentDetails p ON u.Id = p.UserId
   
	   left join (
	select  distinct  ph.UserId, ppp.PolicyHeaderId,ppp.IsSelfPremiumSelected,ppp.IsSelfSpousePremiumSelected from  PolicyHeader ph	
	inner join  PolicyProductPremimum ppp on ppp.PolicyHeaderId=ph.Id
	inner join PolicyProductDetails ppd on ppp.PolicyHeaderId=ppd.PolicyHeaderId and ppp.ProductPremimumId=ppd.SumInsuredPremimumId
	)opt on opt.UserId=u.Id
    left  join (select 
	distinct 
	isnull(spouse.Name,'') as 'Name of Spouse',
	(case when  isnull(spouse.Gender,'')=1 then 'Male'
		when  isnull(spouse.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),spouse.DateOfBirth,103) as 'Date of Birth',

		isnull(child1.Name,'') as 'Name of Handicapped Child 1',
			CONVERT (varchar(10),child1.DateOfBirth,103) as 'Child1 Date of Birth',
          (case when  isnull(child1.Gender,'')=1 then 'Male'
		when  isnull(child1.Gender,'')=1 then 'Female'
		else '' end) as  'Child1 Gender',
	
	   isnull(child2.Name,'') as 'Name of Handicapped Child 2',
			CONVERT (varchar(10),child2.DateOfBirth,103) as 'Child2 Date of Birth',
          (case when  isnull(child2.Gender,'')=1 then 'Male'
		when  isnull(child2.Gender,'')=1 then 'Female'
		else '' end) as  'Child2 Gender'
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson spouse on spouse.PolicyId=ph.Id and bd.Spouse=spouse.Id
left join BeneficiaryPerson child1 on child1.PolicyId=ph.Id and bd.Child1=child1.Id
left join BeneficiaryPerson child2 on child2.PolicyId=ph.Id and bd.Child2=child2.Id
where (spouse.Name is not null or child1.Name is not null or child2.Name is not null)
)dep on dep.UserId=u.Id
left join 
(select distinct ph.UserId,ph.CreatedAt,ph.UpdatedAt,  ph.TotalPremimum,PaymentType,ph.ChildPremium from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id) payment on payment.UserId=u.Id
left join (
select 
	distinct 
	isnull(Nominee.Name,'') as 'Nominee Name',
	(case when  isnull(Nominee.Gender,'')=1 then 'Male'
		when  isnull(Nominee.Gender,'')=1 then 'Female'
		else '' end) as  'Nominee Gender',
		CONVERT (varchar(10),Nominee.DateOfBirth,103) as 'Nominee Date of Birth'
		,(case when Nominee.NomineeRelation=1 then 'Spouse'
		when Nominee.NomineeRelation=2 then 'Father'
		when Nominee.NomineeRelation=3 then 'Mother'
		when Nominee.NomineeRelation=4 then 'Son'
		when Nominee.NomineeRelation=5 then 'Daughter'
		when Nominee.NomineeRelation=6 then 'FatherInlaw'
		when Nominee.NomineeRelation=7 then 'Motherinlaw'
		when Nominee.NomineeRelation=8 then 'SonInlaw'
		when Nominee.NomineeRelation=9 then 'DaughterInlaw'
		when Nominee.NomineeRelation=10 then 'Brother'
		when Nominee.NomineeRelation=11 then 'Sister'
		when Nominee.NomineeRelation=12 then 'Grandfather'
		when Nominee.NomineeRelation=13 then 'Grandmother'
		when Nominee.NomineeRelation=14 then 'Grandson'
		when Nominee.NomineeRelation=15 then 'Granddaughter'
		when Nominee.NomineeRelation=16 then 'Nephew'
		when Nominee.NomineeRelation=17 then 'Niece'
			when Nominee.NomineeRelation=18 then 'Uncle'
		when Nominee.NomineeRelation=19 then 'Aunt'
		when Nominee.NomineeRelation=20 then 'Cousin'
		when Nominee.NomineeRelation=21 then 'Guardian'
			when Nominee.NomineeRelation=22 then 'Partner'
				when Nominee.NomineeRelation=0 then 'None' end) as 'Nominee Relation' 
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson Nominee  on Nominee.PolicyId=ph.Id and bd.Nominee=Nominee.Id

where (Nominee.Name is not null)
) Nominee on Nominee.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,ch.BankName as 'Bank Name (Amount Transfer From)','' as AccountNumber,'' as IfscCode,ch.ChequeNumber as 'Transaction Number/Cheque Number'
,a.AssociationName as InFavourOf,pd.CreatedAt as 'Payment Date/Cheque Date'
,ch.ChequeDepositLocation [Cheque Deposit Location]
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeCheque ch on ch.PaymentDetailId=pd.Id
inner join Association a on ch.InFavourOfAssociationId=a.Id
) cheque on cheque.UserId=u.Id
left join (
select distinct ph.UserId,PaymentType
,neft.BankName as 'Bank Name (Amount Transfer From)',neft.TransactionId as 'Transaction Number/Cheque Number'
,neft.AccountNumber,neft.IfscCode,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeNEFT neft on neft.PaymentDetailId=pd.Id
)neft on neft.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,upi.TransactionNumber as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeUPI upi on upi.PaymentDetailId=pd.Id
)upi on upi.UserId=u.Id
left join (select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,gate.TransactionId as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeGateway gate on gate.PaymentDetailId=pd.Id
)gate on gate.UserId=u.Id

WHERE 
        
     (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        AND p.PaymentStatus = 3 -- enum PaymentStatus Initiated = 3
        --AND p.PaymentDate BETWEEN @StartDate AND @EndDate
        and CAST(p.[PaymentDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
    END TRY
    BEGIN CATCH
      
	   EXEC dbo.spGetErrorInfo ;
    
        
    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USPGetAccountWisePaymentReport]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USPGetAccountWisePaymentReport]
	-- Add the parameters for the stored procedure here
	 @associationID int=0,
    @paymentStatusId int=0,    
    @startdate date,
    @enddate date
AS
BEGIN
	BEGIN TRY
SELECT 
	    a.AssociationName as 'Association Name',
	    o.Name as 'Retiree Organisation Name',
        u.EMPIDPFNo 'PF No/Emp ID',
        isnull(u.[FirstName],'')+ ' '+ISNULL(u.LastName,'') 'Retiree Name',
		(case when  isnull(u.Gender,'')=1 then 'Male'
		when  isnull(u.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),u.DOB,103) as 'Date of Birth',
		isnull(u.Address,'') as Address,
		u.[Email],
		u.MobileNumber as 'Mobile Number'
		,(case when  opt.IsSelfPremiumSelected=1 and opt.IsSelfSpousePremiumSelected=0 then 'Only Self'
		when   opt.IsSelfPremiumSelected=0 and opt.IsSelfSpousePremiumSelected=1 then 'Self+Spouse' end 
		) as PolicyOption,
		
		isnull(dep.[Name of Spouse],'') as [Name of Spouse]
		,isnull(dep.[Date of Birth],'') as 'Date of Birth' 
		, isnull(dep.Gender,'') as Gender
		,isnull(dep.[Name of Handicapped Child 1],'') as [Name of Handicapped Child 1]
		,isnull(dep.[Child1 Date of Birth],'') as [Child1 Date of Birth]
		,isnull(dep.[Child1 Gender],'') as [Child1 Gender],

		isnull(dep.[Name of Handicapped Child 2],'') as [Name of Handicapped Child 2]
		,isnull(dep.[Child2 Date of Birth],'') as [Child2 Date of Birth]
		,isnull(dep.[Child2 Gender],'') as [Child2 Gender],

		--dep.[Name of Handicapped Child 2],dep.[Child2 Date of Birth],dep.[Child2 Gender],

		payment.ChildPremium as 'Dependent Child/Children Premium',
		payment.TotalPremimum as 'Total Premium With Handicapped Children/Child',
		case when  payment.PaymentType =1 then 'Cheque' 
		when  payment.PaymentType =2 then 'NEFT' 
		when  payment.PaymentType =3 then 'UPI' 
		when  payment.PaymentType =4 then 'Gateway'  end 'Payment Mode'

		,(case when  payment.PaymentType =1 then cheque.[Bank Name (Amount Transfer From)]
		when  payment.PaymentType =2 then neft.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =3 then upi.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =4 then gate.[Bank Name (Amount Transfer From)]   end ) as 'Bank Name (Amount Transfer From)'

		,(case when  payment.PaymentType =1 then cheque.AccountNumber
		when  payment.PaymentType =2 then neft.AccountNumber
		when  payment.PaymentType =3 then upi.AccountNumber
		when  payment.PaymentType =4 then gate.AccountNumber  end ) as 'Account Number'

		,(case when  payment.PaymentType =1 then cheque.IfscCode
		when  payment.PaymentType =2 then neft.IfscCode
		when  payment.PaymentType =3 then upi.IfscCode
		when  payment.PaymentType =4 then gate.IfscCode  end ) as 'IFSC Code'

			,isnull((case when  payment.PaymentType =1 then cheque.[Transaction Number/Cheque Number]
		when  payment.PaymentType =2 then neft.[Transaction Number/Cheque Number]
		when  payment.PaymentType =3 then upi.[Transaction Number/Cheque Number]
		when  payment.PaymentType =4 then gate.[Transaction Number/Cheque Number]  end ) ,'')as 'Transaction Number/Cheque Number'

			,(case when  payment.PaymentType =1 then cheque.[Payment Date/Cheque Date]
		when  payment.PaymentType =2 then neft.[Payment Date/Cheque Date]
		when  payment.PaymentType =3 then upi.[Payment Date/Cheque Date]
		when  payment.PaymentType =4 then gate.[Payment Date/Cheque Date] end ) as 'Payment Date/Cheque Date'

		,Nominee.[Nominee Name]
		,Nominee.[Nominee Gender]
		,Nominee.[Nominee Date of Birth]
		,Nominee.[Nominee Relation]
		,isnull(cheque.[Cheque Deposit Location],'') as 'Cheque Deposit Location'
		,'I agree to the terms' as Agreement
		,payment.CreatedAt as 'Added Time'
		,payment.UpdatedAt as 'Modified Time'
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
	INNER JOIN 
        PaymentDetails p ON u.Id = p.UserId
	   left join (
	select  distinct  ph.UserId, ppp.PolicyHeaderId,ppp.IsSelfPremiumSelected,ppp.IsSelfSpousePremiumSelected from  PolicyHeader ph	
	inner join  PolicyProductPremimum ppp on ppp.PolicyHeaderId=ph.Id
	inner join PolicyProductDetails ppd on ppp.PolicyHeaderId=ppd.PolicyHeaderId and ppp.ProductPremimumId=ppd.SumInsuredPremimumId
	)opt on opt.UserId=u.Id
    left  join (select 
	distinct 
	isnull(spouse.Name,'') as 'Name of Spouse',
	(case when  isnull(spouse.Gender,'')=1 then 'Male'
		when  isnull(spouse.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),spouse.DateOfBirth,103) as 'Date of Birth',

		isnull(child1.Name,'') as 'Name of Handicapped Child 1',
			CONVERT (varchar(10),child1.DateOfBirth,103) as 'Child1 Date of Birth',
          (case when  isnull(child1.Gender,'')=1 then 'Male'
		when  isnull(child1.Gender,'')=1 then 'Female'
		else '' end) as  'Child1 Gender',
	
	   isnull(child2.Name,'') as 'Name of Handicapped Child 2',
			CONVERT (varchar(10),child2.DateOfBirth,103) as 'Child2 Date of Birth',
          (case when  isnull(child2.Gender,'')=1 then 'Male'
		when  isnull(child2.Gender,'')=1 then 'Female'
		else '' end) as  'Child2 Gender'
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson spouse on spouse.PolicyId=ph.Id and bd.Spouse=spouse.Id
left join BeneficiaryPerson child1 on child1.PolicyId=ph.Id and bd.Child1=child1.Id
left join BeneficiaryPerson child2 on child2.PolicyId=ph.Id and bd.Child2=child2.Id
where (spouse.Name is not null or child1.Name is not null or child2.Name is not null)
)dep on dep.UserId=u.Id
left join 
(select distinct ph.UserId,ph.CreatedAt,ph.UpdatedAt,  ph.TotalPremimum,PaymentType,ph.ChildPremium from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id) payment on payment.UserId=u.Id
left join (
select 
	distinct 
	isnull(Nominee.Name,'') as 'Nominee Name',
	(case when  isnull(Nominee.Gender,'')=1 then 'Male'
		when  isnull(Nominee.Gender,'')=1 then 'Female'
		else '' end) as  'Nominee Gender',
		CONVERT (varchar(10),Nominee.DateOfBirth,103) as 'Nominee Date of Birth'
		,(case when Nominee.NomineeRelation=1 then 'Spouse'
		when Nominee.NomineeRelation=2 then 'Father'
		when Nominee.NomineeRelation=3 then 'Mother'
		when Nominee.NomineeRelation=4 then 'Son'
		when Nominee.NomineeRelation=5 then 'Daughter'
		when Nominee.NomineeRelation=6 then 'FatherInlaw'
		when Nominee.NomineeRelation=7 then 'Motherinlaw'
		when Nominee.NomineeRelation=8 then 'SonInlaw'
		when Nominee.NomineeRelation=9 then 'DaughterInlaw'
		when Nominee.NomineeRelation=10 then 'Brother'
		when Nominee.NomineeRelation=11 then 'Sister'
		when Nominee.NomineeRelation=12 then 'Grandfather'
		when Nominee.NomineeRelation=13 then 'Grandmother'
		when Nominee.NomineeRelation=14 then 'Grandson'
		when Nominee.NomineeRelation=15 then 'Granddaughter'
		when Nominee.NomineeRelation=16 then 'Nephew'
		when Nominee.NomineeRelation=17 then 'Niece'
			when Nominee.NomineeRelation=18 then 'Uncle'
		when Nominee.NomineeRelation=19 then 'Aunt'
		when Nominee.NomineeRelation=20 then 'Cousin'
		when Nominee.NomineeRelation=21 then 'Guardian'
			when Nominee.NomineeRelation=22 then 'Partner'
				when Nominee.NomineeRelation=0 then 'None' end) as 'Nominee Relation' 
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson Nominee  on Nominee.PolicyId=ph.Id and bd.Nominee=Nominee.Id
where (Nominee.Name is not null)
) Nominee on Nominee.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,ch.BankName as 'Bank Name (Amount Transfer From)','' as AccountNumber,'' as IfscCode,ch.ChequeNumber as 'Transaction Number/Cheque Number'
,a.AssociationName as InFavourOf,pd.CreatedAt as 'Payment Date/Cheque Date'
,ch.ChequeDepositLocation [Cheque Deposit Location]
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeCheque ch on ch.PaymentDetailId=pd.Id
inner join Association a on ch.InFavourOfAssociationId=a.Id
) cheque on cheque.UserId=u.Id
left join (
select distinct ph.UserId,PaymentType
,neft.BankName as 'Bank Name (Amount Transfer From)',neft.TransactionId as 'Transaction Number/Cheque Number'
,neft.AccountNumber,neft.IfscCode,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeNEFT neft on neft.PaymentDetailId=pd.Id
)neft on neft.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,upi.TransactionNumber as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeUPI upi on upi.PaymentDetailId=pd.Id
)upi on upi.UserId=u.Id
left join (select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,gate.TransactionId as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeGateway gate on gate.PaymentDetailId=pd.Id
)gate on gate.UserId=u.Id

WHERE 
       (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        AND (@PaymentStatusId = 0 OR p.PaymentStatus = @PaymentStatusId)
        --AND p.PaymentDate BETWEEN @StartDate AND @EndDate
        and CAST(p.[PaymentDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
        AND u.IsActive = 1;
        
    END TRY
    BEGIN CATCH
      
	   EXEC dbo.spGetErrorInfo ;
    
        
    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USPBulkUploadAssociation]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author Name>
-- Create date: <Create Date>
-- Description:	Bulk upload associations with related details
-- =============================================
create PROCEDURE [dbo].[USPBulkUploadAssociation]
	-- Add the parameters for the stored procedure here
	@UserName NVARCHAR(50),
    @DisplayName NVARCHAR(100),
    @Association dbo.AssociationTableType READONLY,
    @Contact dbo.AssociationContactTableType READONLY,
    @Message dbo.AssociationMessageTableType READONLY
    --,@ReturnValue INT OUTPUT
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Prevent extra result sets from interfering with SELECT statements
        --SET NOCOUNT ON;

        -- Insert into Association table
        INSERT INTO [TestDb].[dbo].[Association]
           ([AssociationName]
           ,[OraganisationId]
           ,[ParentAssociationId]
           ,[Address1]
           ,[Address2]
           ,[City]
           ,[StateId]
           ,[PINCode]
           ,[CountryId]
           ,[AcceptOnePayPayment]
           ,[CreatedAt]
           ,[UpdatedAt]
           ,[CreatedBy]
           ,[UpdatedBy]
           ,[IsActive])
        SELECT 
           a.AssociationName,
           o.Id, -- Assuming OrganisationId is the correct field
           aa.Id, -- Assuming ParentAssociationId should map to AssociationId
           a.AddressLine1,
           a.AddressLine2,
           a.City,
           s.Id, -- Assuming StateId is being inserted
           a.PinCode,
           c.Id, -- Assuming CountryId is being inserted
           CASE 
               WHEN a.AcceptOnepayPayment = 'YES' THEN 1 
               ELSE 0 
           END AS AcceptOnePayPayment, -- Bit value for true or false
           GETDATE(),
           GETDATE(),
           0,
           0,
           1 -- Setting IsActive to true
        FROM @Association a
        LEFT JOIN dbo.Organisations o ON a.OrganisationName = o.Name
        LEFT JOIN dbo.Association aa ON a.ParentAssociation = aa.AssociationName
        LEFT JOIN dbo.AddressState s ON a.State = s.Name
        LEFT JOIN dbo.AddressCountry c ON a.Country = c.Name;
        
        -- Insert into AssociationBankDetails table
        INSERT INTO [TestDb].[dbo].[AssociationBankDetails]
           ([AssociationId]
           ,[BankName]
           ,[BranchName]
           ,[AccountNumber]
           ,[IFSCCode]
           ,[MICRCode]
           ,[AccountName]
           ,[CreatedAt]
           ,[UpdatedAt]
           ,[CreatedBy]
           ,[UpdatedBy]
           ,[IsActive])
        SELECT 
           aa.Id,
           a.BankName,
           a.BranchName,
           a.AccountNumber,
           a.IFSCCode,
           a.MICRCode,
           a.AccountName,
           GETDATE(),
           GETDATE(),
           0,
           0,
           1
        FROM @Association a
        LEFT JOIN dbo.Association aa WITH (NOLOCK) ON a.AssociationName = aa.AssociationName;
        
        -- Insert into AssociationOnePayDetails table
        INSERT INTO [TestDb].[dbo].[AssociationOnePayDetails]
           ([AssociationId]
           ,[OnepayUrl]
           ,[OnePayId]
           ,[CreatedAt]
           ,[UpdatedAt]
           ,[CreatedBy]
           ,[UpdatedBy]
           ,[IsActive])
        SELECT 
           aa.Id,
           NULL, -- Assuming OnepayUrl is not provided and will be handled later
           a.OnepayID,
           GETDATE(),
           GETDATE(),
           0,
           0,
           1
        FROM @Association a
        LEFT JOIN dbo.Association aa WITH (NOLOCK) ON a.AssociationName = aa.AssociationName;

        -- Insert into AssociationContactDetails table
        INSERT INTO [TestDb].[dbo].[AssociationContactDetails]
           ([AssociationId]
           ,[FirstName]
           ,[LastName]
           ,[Phone]
           ,[Email]
           ,[CreatedAt]
           ,[UpdatedAt]
           ,[CreatedBy]
           ,[UpdatedBy]
           ,[IsActive])
        SELECT 
           aa.Id,
           c.FirstName,
           c.LastName,
           c.Phone,
           c.Email,
           GETDATE(),
           GETDATE(),
           0,
           0,
           1
        FROM @Contact c
        LEFT JOIN dbo.Association aa WITH (NOLOCK)  ON c.AssociationName = aa.AssociationName;

        -- Insert into AssociationMessageDetails table
        INSERT INTO [TestDb].[dbo].[AssociationMessageDetails]
           ([AssociationId]
           ,[Name]
           ,[Template]
           ,[IsApproved]
           ,[CreatedAt]
           ,[UpdatedAt]
           ,[CreatedBy]
           ,[UpdatedBy]
           ,[IsActive])
        SELECT 
           aa.Id,
           m.Name,
           m.Template,
           0,
           GETDATE(),
           GETDATE(),
           0,
           0,
           1
        FROM @Message m
        LEFT JOIN dbo.Association aa WITH (NOLOCK) ON m.AssociationName = aa.AssociationName;

        -- Commit the transaction upon success
        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        EXEC dbo.spGetErrorInfo
        -- Rollback the transaction in case of any errors
        ROLLBACK TRANSACTION;

        -- Optionally, log the error or handle it (e.g., using spGetErrorInfo or  EXEC dbo.spGetErrorInfo)
         EXEC dbo.spGetErrorInfo; -- Rethrow the actual error for debugging
        
    END CATCH

END

/****** Object:  StoredProcedure [dbo].[USPBulkUploadApplicationUsers]    Script Date: 09/05/2024 16:02:59 ******/
SET ANSI_NULLS ON
GO
/****** Object:  StoredProcedure [dbo].[USPBulkUploadApplicationUsers]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[USPBulkUploadApplicationUsers]
    @UserName NVARCHAR(50),
    @DisplayName NVARCHAR(100),
    @ApplicationUser dbo.ApplicationUserTableType readonly
    --,@ReturnValue INT OUTPUT
AS
BEGIN
    -- Ensure to use transactions if needed
    BEGIN TRANSACTION;

    BEGIN TRY
     
        -- Example: Insert data from the table type into a target table
        INSERT INTO dbo.ApplicationUser (
    FirstName, LastName, EMPIDPFNo, OrganisationId, AssociationId,
    DOB, Gender, Email, CountryCode, MobileNumber, StateId, Pincode, Address,IsProfileComplete,UserType
)
SELECT 
    au.FirstName,
    au.LastName,
    au.EmpID_PFNo,
    o.Id,
    a.Id,
    au.DateOfBirth,
   CASE 
        WHEN au.Gender = 'Unknown' THEN 0
        WHEN au.Gender = 'Male' THEN 1
        WHEN au.Gender = 'Female' THEN 2
        ELSE NULL -- Handle any unexpected values
    END AS Gender,
    au.Email,
    au.CountryCode,
    au.MobileNumber,
    s.Id,
    au.Pincode,
    au.Address,
    0,
    1
FROM @ApplicationUser au
LEFT JOIN dbo.Organisations o ON au.Organisation = o.Name
LEFT JOIN dbo.Association a ON au.Association = a.AssociationName
LEFT JOIN dbo.AddressState s ON au.State = s.Name;


        -- You might also want to log the action or perform additional operations
        -- Example: INSERT INTO LogTable (UserName, Action, Date) VALUES (@UserName, 'Bulk Upload', GETDATE());

        -- Commit transaction if everything is successful
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
    EXEC dbo.spGetErrorInfo
        -- Rollback transaction in case of an error
        ROLLBACK TRANSACTION;

        -- Optionally, handle the error, log it, or rethrow it
        -- Example:  EXEC dbo.spGetErrorInfo;
    END CATCH
     -- Set the return value
    --SET @ReturnValue = @@ROWCOUNT;
END;
GO
/****** Object:  StoredProcedure [dbo].[USPOfflinePaymentsReport]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USPOfflinePaymentsReport]
	-- Add the parameters for the stored procedure here
	@associationID int=0, 
    @startdate date,
    @enddate date
AS
BEGIN
	BEGIN TRY
SELECT 
	    a.AssociationName as 'Association Name',
	    o.Name as 'Retiree Organisation Name',
        u.EMPIDPFNo 'PF No/Emp ID',
        isnull(u.[FirstName],'')+ ' '+ISNULL(u.LastName,'') 'Retiree Name',
		(case when  isnull(u.Gender,'')=1 then 'Male'
		when  isnull(u.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),u.DOB,103) as 'Date of Birth',
		isnull(u.Address,'') as Address,
		u.[Email],
		u.MobileNumber as 'Mobile Number'
		,(case when  opt.IsSelfPremiumSelected=1 and opt.IsSelfSpousePremiumSelected=0 then 'Only Self'
		when   opt.IsSelfPremiumSelected=0 and opt.IsSelfSpousePremiumSelected=1 then 'Self+Spouse' end 
		) PolicyOption,
		
		isnull(dep.[Name of Spouse],'') as [Name of Spouse]
		,isnull(dep.[Date of Birth],'') as 'Date of Birth' 
		, isnull(dep.Gender,'') as Gender
		,isnull(dep.[Name of Handicapped Child 1],'') as [Name of Handicapped Child 1]
		,isnull(dep.[Child1 Date of Birth],'') as [Child1 Date of Birth]
		,isnull(dep.[Child1 Gender],'') as [Child1 Gender],

		isnull(dep.[Name of Handicapped Child 2],'') as [Name of Handicapped Child 2]
		,isnull(dep.[Child2 Date of Birth],'') as [Child2 Date of Birth]
		,isnull(dep.[Child2 Gender],'') as [Child2 Gender],

		--dep.[Name of Handicapped Child 2],dep.[Child2 Date of Birth],dep.[Child2 Gender],

		payment.ChildPremium as 'Dependent Child/Children Premium',
		payment.TotalPremimum as 'Total Premium With Handicapped Children/Child',
		case when  payment.PaymentType =1 then 'Cheque' 
		when  payment.PaymentType =2 then 'NEFT' 
		when  payment.PaymentType =3 then 'UPI' 
		when  payment.PaymentType =4 then 'Gateway'  end 'Payment Mode'

		,(case when  payment.PaymentType =1 then cheque.[Bank Name (Amount Transfer From)]
		when  payment.PaymentType =2 then neft.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =3 then upi.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =4 then gate.[Bank Name (Amount Transfer From)]   end ) as 'Bank Name (Amount Transfer From)'

		,(case when  payment.PaymentType =1 then cheque.AccountNumber
		when  payment.PaymentType =2 then neft.AccountNumber
		when  payment.PaymentType =3 then upi.AccountNumber
		when  payment.PaymentType =4 then gate.AccountNumber  end ) as 'Account Number'

		,(case when  payment.PaymentType =1 then cheque.IfscCode
		when  payment.PaymentType =2 then neft.IfscCode
		when  payment.PaymentType =3 then upi.IfscCode
		when  payment.PaymentType =4 then gate.IfscCode  end ) as 'IFSC Code'

			,isnull((case when  payment.PaymentType =1 then cheque.[Transaction Number/Cheque Number]
		when  payment.PaymentType =2 then neft.[Transaction Number/Cheque Number]
		when  payment.PaymentType =3 then upi.[Transaction Number/Cheque Number]
		when  payment.PaymentType =4 then gate.[Transaction Number/Cheque Number]  end ) ,'')as 'Transaction Number/Cheque Number'

			,(case when  payment.PaymentType =1 then cheque.[Payment Date/Cheque Date]
		when  payment.PaymentType =2 then neft.[Payment Date/Cheque Date]
		when  payment.PaymentType =3 then upi.[Payment Date/Cheque Date]
		when  payment.PaymentType =4 then gate.[Payment Date/Cheque Date] end ) as 'Payment Date/Cheque Date'

		,Nominee.[Nominee Name]
		,Nominee.[Nominee Gender]
		,Nominee.[Nominee Date of Birth]
		,Nominee.[Nominee Relation]
		,isnull(cheque.[Cheque Deposit Location],'') as 'Cheque Deposit Location'
		,'I agree to the terms' as Agreement
		,payment.CreatedAt as 'Added Time'
		,payment.UpdatedAt as 'Modified Time'
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
    INNER JOIN 
        PaymentDetails p ON u.Id = p.UserId
	   left join (
	select  distinct  ph.UserId, ppp.PolicyHeaderId,ppp.IsSelfPremiumSelected,ppp.IsSelfSpousePremiumSelected from  PolicyHeader ph	
	inner join  PolicyProductPremimum ppp on ppp.PolicyHeaderId=ph.Id
	inner join PolicyProductDetails ppd on ppp.PolicyHeaderId=ppd.PolicyHeaderId and ppp.ProductPremimumId=ppd.SumInsuredPremimumId
	)opt on opt.UserId=u.Id
    left  join (select 
	distinct 
	isnull(spouse.Name,'') as 'Name of Spouse',
	(case when  isnull(spouse.Gender,'')=1 then 'Male'
		when  isnull(spouse.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),spouse.DateOfBirth,103) as 'Date of Birth',

		isnull(child1.Name,'') as 'Name of Handicapped Child 1',
			CONVERT (varchar(10),child1.DateOfBirth,103) as 'Child1 Date of Birth',
          (case when  isnull(child1.Gender,'')=1 then 'Male'
		when  isnull(child1.Gender,'')=1 then 'Female'
		else '' end) as  'Child1 Gender',
	
	   isnull(child2.Name,'') as 'Name of Handicapped Child 2',
			CONVERT (varchar(10),child2.DateOfBirth,103) as 'Child2 Date of Birth',
          (case when  isnull(child2.Gender,'')=1 then 'Male'
		when  isnull(child2.Gender,'')=1 then 'Female'
		else '' end) as  'Child2 Gender'
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson spouse on spouse.PolicyId=ph.Id and bd.Spouse=spouse.Id
left join BeneficiaryPerson child1 on child1.PolicyId=ph.Id and bd.Child1=child1.Id
left join BeneficiaryPerson child2 on child2.PolicyId=ph.Id and bd.Child2=child2.Id
where (spouse.Name is not null or child1.Name is not null or child2.Name is not null)
)dep on dep.UserId=u.Id
left join 
(select distinct ph.UserId,ph.CreatedAt,ph.UpdatedAt,  ph.TotalPremimum,PaymentType,ph.ChildPremium from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id) payment on payment.UserId=u.Id
left join (
select 
	distinct 
	isnull(Nominee.Name,'') as 'Nominee Name',
	(case when  isnull(Nominee.Gender,'')=1 then 'Male'
		when  isnull(Nominee.Gender,'')=1 then 'Female'
		else '' end) as  'Nominee Gender',
		CONVERT (varchar(10),Nominee.DateOfBirth,103) as 'Nominee Date of Birth'
		,(case when Nominee.NomineeRelation=1 then 'Spouse'
		when Nominee.NomineeRelation=2 then 'Father'
		when Nominee.NomineeRelation=3 then 'Mother'
		when Nominee.NomineeRelation=4 then 'Son'
		when Nominee.NomineeRelation=5 then 'Daughter'
		when Nominee.NomineeRelation=6 then 'FatherInlaw'
		when Nominee.NomineeRelation=7 then 'Motherinlaw'
		when Nominee.NomineeRelation=8 then 'SonInlaw'
		when Nominee.NomineeRelation=9 then 'DaughterInlaw'
		when Nominee.NomineeRelation=10 then 'Brother'
		when Nominee.NomineeRelation=11 then 'Sister'
		when Nominee.NomineeRelation=12 then 'Grandfather'
		when Nominee.NomineeRelation=13 then 'Grandmother'
		when Nominee.NomineeRelation=14 then 'Grandson'
		when Nominee.NomineeRelation=15 then 'Granddaughter'
		when Nominee.NomineeRelation=16 then 'Nephew'
		when Nominee.NomineeRelation=17 then 'Niece'
			when Nominee.NomineeRelation=18 then 'Uncle'
		when Nominee.NomineeRelation=19 then 'Aunt'
		when Nominee.NomineeRelation=20 then 'Cousin'
		when Nominee.NomineeRelation=21 then 'Guardian'
			when Nominee.NomineeRelation=22 then 'Partner'
				when Nominee.NomineeRelation=0 then 'None' end) as 'Nominee Relation' 
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson Nominee  on Nominee.PolicyId=ph.Id and bd.Nominee=Nominee.Id
where (Nominee.Name is not null)
) Nominee on Nominee.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,ch.BankName as 'Bank Name (Amount Transfer From)','' as AccountNumber,'' as IfscCode,ch.ChequeNumber as 'Transaction Number/Cheque Number'
,a.AssociationName as InFavourOf,pd.CreatedAt as 'Payment Date/Cheque Date'
,ch.ChequeDepositLocation [Cheque Deposit Location]
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeCheque ch on ch.PaymentDetailId=pd.Id
inner join Association a on ch.InFavourOfAssociationId=a.Id
) cheque on cheque.UserId=u.Id
left join (
select distinct ph.UserId,PaymentType
,neft.BankName as 'Bank Name (Amount Transfer From)',neft.TransactionId as 'Transaction Number/Cheque Number'
,neft.AccountNumber,neft.IfscCode,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeNEFT neft on neft.PaymentDetailId=pd.Id
)neft on neft.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,upi.TransactionNumber as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeUPI upi on upi.PaymentDetailId=pd.Id
)upi on upi.UserId=u.Id
left join (select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,gate.TransactionId as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeGateway gate on gate.PaymentDetailId=pd.Id
)gate on gate.UserId=u.Id

WHERE 
     (@AssociationID = 0 OR u.AssociationId = @AssociationID)
        AND p.PaymentMode = 2 -- enum PaymentMode Offline=2
        --AND p.PaymentDate BETWEEN @StartDate AND @EndDate
        and CAST(p.[PaymentDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)
     and u.IsProfileComplete=1
    END TRY
    BEGIN CATCH
      
	   EXEC dbo.spGetErrorInfo ;
    
        
    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USPInsuranceCompanyReport]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author Name>
-- Create date: <Create Date>
-- Description:	USPInsuranceCompanyReport
-- =============================================
create    PROCEDURE [dbo].[USPInsuranceCompanyReport]
	
   -- Add the parameters for the stored procedure here
@associationID int=0 ,
@OrganisationId int=0,
@startdate date,
@enddate date
AS
BEGIN


    BEGIN TRY
SELECT 
	    a.AssociationName as 'Association Name',
	    o.Name as 'Retiree Organisation Name',
        u.EMPIDPFNo 'PF No/Emp ID',
        isnull(u.[FirstName],'')+ ' '+ISNULL(u.LastName,'') 'Retiree Name',
		(case when  isnull(u.Gender,'')=1 then 'Male'
		when  isnull(u.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),u.DOB,103) as 'Date of Birth',
		isnull(u.Address,'') as Address,
		u.[Email],
		u.MobileNumber as 'Mobile Number'
		,(case when  opt.IsSelfPremiumSelected=1 and opt.IsSelfSpousePremiumSelected=0 then 'Only Self'
		when   opt.IsSelfPremiumSelected=0 and opt.IsSelfSpousePremiumSelected=1 then 'Self+Spouse' end 
		) PolicyOption,
		
		isnull(dep.[Name of Spouse],'') as [Name of Spouse]
		,isnull(dep.[Date of Birth],'') as 'Date of Birth' , isnull(dep.Gender,'') as Gender,
		isnull(dep.[Name of Handicapped Child 1],'') as [Name of Handicapped Child 1]
		,isnull(dep.[Child1 Date of Birth],'') as [Child1 Date of Birth]
		,isnull(dep.[Child1 Gender],'') as [Child1 Gender],

		isnull(dep.[Name of Handicapped Child 2],'') as [Name of Handicapped Child 2]
		,isnull(dep.[Child2 Date of Birth],'') as [Child2 Date of Birth]
		,isnull(dep.[Child2 Gender],'') as [Child2 Gender],

		--dep.[Name of Handicapped Child 2],dep.[Child2 Date of Birth],dep.[Child2 Gender],

		payment.ChildPremium as 'Dependent Child/Children Premium',
		payment.TotalPremimum as 'Total Premium With Handicapped Children/Child',
		case when  payment.PaymentType =1 then 'Cheque' 
		when  payment.PaymentType =2 then 'NEFT' 
		when  payment.PaymentType =3 then 'UPI' 
		when  payment.PaymentType =4 then 'Gateway'  end 'Payment Mode'

		,(case when  payment.PaymentType =1 then cheque.[Bank Name (Amount Transfer From)]
		when  payment.PaymentType =2 then neft.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =3 then upi.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =4 then gate.[Bank Name (Amount Transfer From)]   end ) as 'Bank Name (Amount Transfer From)'

		,(case when  payment.PaymentType =1 then cheque.AccountNumber
		when  payment.PaymentType =2 then neft.AccountNumber
		when  payment.PaymentType =3 then upi.AccountNumber
		when  payment.PaymentType =4 then gate.AccountNumber  end ) as 'Account Number'

		,(case when  payment.PaymentType =1 then cheque.IfscCode
		when  payment.PaymentType =2 then neft.IfscCode
		when  payment.PaymentType =3 then upi.IfscCode
		when  payment.PaymentType =4 then gate.IfscCode  end ) as 'IFSC Code'

			,isnull((case when  payment.PaymentType =1 then cheque.[Transaction Number/Cheque Number]
		when  payment.PaymentType =2 then neft.[Transaction Number/Cheque Number]
		when  payment.PaymentType =3 then upi.[Transaction Number/Cheque Number]
		when  payment.PaymentType =4 then gate.[Transaction Number/Cheque Number]  end ) ,'')as 'Transaction Number/Cheque Number'

			,(case when  payment.PaymentType =1 then cheque.[Payment Date/Cheque Date]
		when  payment.PaymentType =2 then neft.[Payment Date/Cheque Date]
		when  payment.PaymentType =3 then upi.[Payment Date/Cheque Date]
		when  payment.PaymentType =4 then gate.[Payment Date/Cheque Date] end ) as 'Payment Date/Cheque Date'

		,Nominee.[Nominee Name],Nominee.[Nominee Gender],Nominee.[Nominee Date of Birth],Nominee.[Nominee Relation]
		,isnull(cheque.[Cheque Deposit Location],'') as 'Cheque Deposit Location'
		,'I agree to the terms' as Agreement
		,payment.CreatedAt as 'Added Time'
		,payment.UpdatedAt as 'Modified Time'
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
   
	   left join (
	select  distinct  ph.UserId, ppp.PolicyHeaderId,ppp.IsSelfPremiumSelected,ppp.IsSelfSpousePremiumSelected from  PolicyHeader ph	
	inner join  PolicyProductPremimum ppp on ppp.PolicyHeaderId=ph.Id
	inner join PolicyProductDetails ppd on ppp.PolicyHeaderId=ppd.PolicyHeaderId and ppp.ProductPremimumId=ppd.SumInsuredPremimumId
	)opt on opt.UserId=u.Id
    left  join (select 
	distinct 
	isnull(spouse.Name,'') as 'Name of Spouse',
	(case when  isnull(spouse.Gender,'')=1 then 'Male'
		when  isnull(spouse.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),spouse.DateOfBirth,103) as 'Date of Birth',

		isnull(child1.Name,'') as 'Name of Handicapped Child 1',
			CONVERT (varchar(10),child1.DateOfBirth,103) as 'Child1 Date of Birth',
          (case when  isnull(child1.Gender,'')=1 then 'Male'
		when  isnull(child1.Gender,'')=1 then 'Female'
		else '' end) as  'Child1 Gender',
	
	   isnull(child2.Name,'') as 'Name of Handicapped Child 2',
			CONVERT (varchar(10),child2.DateOfBirth,103) as 'Child2 Date of Birth',
          (case when  isnull(child2.Gender,'')=1 then 'Male'
		when  isnull(child2.Gender,'')=1 then 'Female'
		else '' end) as  'Child2 Gender'
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson spouse on spouse.PolicyId=ph.Id and bd.Spouse=spouse.Id
left join BeneficiaryPerson child1 on child1.PolicyId=ph.Id and bd.Child1=child1.Id
left join BeneficiaryPerson child2 on child2.PolicyId=ph.Id and bd.Child2=child2.Id
where (spouse.Name is not null or child1.Name is not null or child2.Name is not null)
)dep on dep.UserId=u.Id
left join 
(select distinct ph.UserId,ph.CreatedAt,ph.UpdatedAt,  ph.TotalPremimum,PaymentType,ph.ChildPremium from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id) payment on payment.UserId=u.Id
left join (
select 
	distinct 
	isnull(Nominee.Name,'') as 'Nominee Name',
	(case when  isnull(Nominee.Gender,'')=1 then 'Male'
		when  isnull(Nominee.Gender,'')=1 then 'Female'
		else '' end) as  'Nominee Gender',
		CONVERT (varchar(10),Nominee.DateOfBirth,103) as 'Nominee Date of Birth'
		,(case when Nominee.NomineeRelation=1 then 'Spouse'
		when Nominee.NomineeRelation=2 then 'Father'
		when Nominee.NomineeRelation=3 then 'Mother'
		when Nominee.NomineeRelation=4 then 'Son'
		when Nominee.NomineeRelation=5 then 'Daughter'
		when Nominee.NomineeRelation=6 then 'FatherInlaw'
		when Nominee.NomineeRelation=7 then 'Motherinlaw'
		when Nominee.NomineeRelation=8 then 'SonInlaw'
		when Nominee.NomineeRelation=9 then 'DaughterInlaw'
		when Nominee.NomineeRelation=10 then 'Brother'
		when Nominee.NomineeRelation=11 then 'Sister'
		when Nominee.NomineeRelation=12 then 'Grandfather'
		when Nominee.NomineeRelation=13 then 'Grandmother'
		when Nominee.NomineeRelation=14 then 'Grandson'
		when Nominee.NomineeRelation=15 then 'Granddaughter'
		when Nominee.NomineeRelation=16 then 'Nephew'
		when Nominee.NomineeRelation=17 then 'Niece'
			when Nominee.NomineeRelation=18 then 'Uncle'
		when Nominee.NomineeRelation=19 then 'Aunt'
		when Nominee.NomineeRelation=20 then 'Cousin'
		when Nominee.NomineeRelation=21 then 'Guardian'
			when Nominee.NomineeRelation=22 then 'Partner'
				when Nominee.NomineeRelation=0 then 'None' end) as 'Nominee Relation' 
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson Nominee  on Nominee.PolicyId=ph.Id and bd.Nominee=Nominee.Id
where (Nominee.Name is not null)
) Nominee on Nominee.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,ch.BankName as 'Bank Name (Amount Transfer From)','' as AccountNumber,'' as IfscCode,ch.ChequeNumber as 'Transaction Number/Cheque Number'
,a.AssociationName as InFavourOf,pd.CreatedAt as 'Payment Date/Cheque Date'
,ch.ChequeDepositLocation [Cheque Deposit Location]
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeCheque ch on ch.PaymentDetailId=pd.Id
inner join Association a on ch.InFavourOfAssociationId=a.Id
) cheque on cheque.UserId=u.Id
left join (
select distinct ph.UserId,PaymentType
,neft.BankName as 'Bank Name (Amount Transfer From)',neft.TransactionId as 'Transaction Number/Cheque Number'
,neft.AccountNumber,neft.IfscCode,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeNEFT neft on neft.PaymentDetailId=pd.Id
)neft on neft.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,upi.TransactionNumber as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeUPI upi on upi.PaymentDetailId=pd.Id
)upi on upi.UserId=u.Id
left join (select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,gate.TransactionId as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeGateway gate on gate.PaymentDetailId=pd.Id
)gate on gate.UserId=u.Id

WHERE 
    (@AssociationID = 0 OR u.AssociationId = @AssociationID)

    END TRY
    BEGIN CATCH
      
	   EXEC dbo.spGetErrorInfo ;
    
        
    END CATCH

END

/****** Object:  StoredProcedure [dbo].[USPBulkUploadApplicationUsers]    Script Date: 09/05/2024 16:02:59 ******/
SET ANSI_NULLS ON
GO
/****** Object:  StoredProcedure [dbo].[USPGetRefundReport]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USPGetRefundReport]
	-- Add the parameters for the stored procedure here
	@AssociationID INT = NULL,
    @OrganisationID INT = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL
AS
BEGIN
	BEGIN TRY
SELECT 
	    a.AssociationName as 'Association Name',
	    o.Name as 'Retiree Organisation Name',
        u.EMPIDPFNo 'PF No/Emp ID',
        isnull(u.[FirstName],'')+ ' '+ISNULL(u.LastName,'') 'Retiree Name',
		(case when  isnull(u.Gender,'')=1 then 'Male'
		when  isnull(u.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),u.DOB,103) as 'Date of Birth',
		isnull(u.Address,'') as Address,
		u.[Email],
		u.MobileNumber as 'Mobile Number'
		,(case when  opt.IsSelfPremiumSelected=1 and opt.IsSelfSpousePremiumSelected=0 then 'Only Self'
		when   opt.IsSelfPremiumSelected=0 and opt.IsSelfSpousePremiumSelected=1 then 'Self+Spouse' end 
		) PolicyOption,
		
		isnull(dep.[Name of Spouse],'') as [Name of Spouse]
		,isnull(dep.[Date of Birth],'') as 'Date of Birth' 
		, isnull(dep.Gender,'') as Gender
		,isnull(dep.[Name of Handicapped Child 1],'') as [Name of Handicapped Child 1]
		,isnull(dep.[Child1 Date of Birth],'') as [Child1 Date of Birth]
		,isnull(dep.[Child1 Gender],'') as [Child1 Gender],

		isnull(dep.[Name of Handicapped Child 2],'') as [Name of Handicapped Child 2]
		,isnull(dep.[Child2 Date of Birth],'') as [Child2 Date of Birth]
		,isnull(dep.[Child2 Gender],'') as [Child2 Gender],

		--dep.[Name of Handicapped Child 2],dep.[Child2 Date of Birth],dep.[Child2 Gender],

		payment.ChildPremium as 'Dependent Child/Children Premium',
		payment.TotalPremimum as 'Total Premium With Handicapped Children/Child',
		case when  payment.PaymentType =1 then 'Cheque' 
		when  payment.PaymentType =2 then 'NEFT' 
		when  payment.PaymentType =3 then 'UPI' 
		when  payment.PaymentType =4 then 'Gateway'  end 'Payment Mode'

		,(case when  payment.PaymentType =1 then cheque.[Bank Name (Amount Transfer From)]
		when  payment.PaymentType =2 then neft.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =3 then upi.[Bank Name (Amount Transfer From)] 
		when  payment.PaymentType =4 then gate.[Bank Name (Amount Transfer From)]   end ) as 'Bank Name (Amount Transfer From)'

		,(case when  payment.PaymentType =1 then cheque.AccountNumber
		when  payment.PaymentType =2 then neft.AccountNumber
		when  payment.PaymentType =3 then upi.AccountNumber
		when  payment.PaymentType =4 then gate.AccountNumber  end ) as 'Account Number'

		,(case when  payment.PaymentType =1 then cheque.IfscCode
		when  payment.PaymentType =2 then neft.IfscCode
		when  payment.PaymentType =3 then upi.IfscCode
		when  payment.PaymentType =4 then gate.IfscCode  end ) as 'IFSC Code'

			,isnull((case when  payment.PaymentType =1 then cheque.[Transaction Number/Cheque Number]
		when  payment.PaymentType =2 then neft.[Transaction Number/Cheque Number]
		when  payment.PaymentType =3 then upi.[Transaction Number/Cheque Number]
		when  payment.PaymentType =4 then gate.[Transaction Number/Cheque Number]  end ) ,'')as 'Transaction Number/Cheque Number'

			,(case when  payment.PaymentType =1 then cheque.[Payment Date/Cheque Date]
		when  payment.PaymentType =2 then neft.[Payment Date/Cheque Date]
		when  payment.PaymentType =3 then upi.[Payment Date/Cheque Date]
		when  payment.PaymentType =4 then gate.[Payment Date/Cheque Date] end ) as 'Payment Date/Cheque Date'

		,Nominee.[Nominee Name]
		,Nominee.[Nominee Gender]
		,Nominee.[Nominee Date of Birth]
		,Nominee.[Nominee Relation]
		,isnull(cheque.[Cheque Deposit Location],'') as 'Cheque Deposit Location'
		,rr.[RefundAmount],
        rr.[RefundRequestDate],
        rr.[Status],
        rr.[Comment],
        rr.[RefundRequstHandledBy],
        --rr.[RefundDocumentName],
        --rr.[RefundDocumentUrl],
        rr.[PaymentType],
		--Cheque = 1, NEFT = 2,UPI=3, Gateway=4
        -- Conditional Payment Details for UPI
        CASE WHEN rr.[PaymentType] = 3 THEN _upi.[TransactionNumber] END AS TransactionNumber,
        CASE WHEN rr.[PaymentType] = 3 THEN _upi.[Amount] END AS UPIAmount,
        CASE WHEN rr.[PaymentType] = 3 THEN _upi.[Date] END AS UPIDate,
        --CASE WHEN rr.[PaymentType] = 3 THEN upi.[UPIReceiptDocumentName] END AS UPIReceiptDocumentName,
        --CASE WHEN rr.[PaymentType] = 3 THEN upi.[UPIReceiptDocumentUrl] END AS UPIReceiptDocumentUrl,

        -- Conditional Payment Details for NEFT
        CASE WHEN rr.[PaymentType] = 2 THEN _neft.[TransactionId] END AS NEFTTransactionId,
        CASE WHEN rr.[PaymentType] = 2 THEN _neft.[Amount] END AS NEFTAmount,
        CASE WHEN rr.[PaymentType] = 2 THEN _neft.[Date] END AS NEFTDate,
        CASE WHEN rr.[PaymentType] = 2 THEN _neft.[BankName] END AS NEFTBankName,
        CASE WHEN rr.[PaymentType] = 2 THEN _neft.[BranchName] END AS NEFTBranchName,
        CASE WHEN rr.[PaymentType] = 2 THEN _neft.[AccountNumber] END AS NEFTAccountNumber,
        CASE WHEN rr.[PaymentType] = 2 THEN _neft.[IfscCode] END AS NEFTIfscCode,
        --CASE WHEN rr.[PaymentType] = 2 THEN neft.[NEFTReceiptDocumentName] END AS NEFTReceiptDocumentName,
        --CASE WHEN rr.[PaymentType] = 2 THEN neft.[NEFTReceiptDocumentUrl] END AS NEFTReceiptDocumentUrl,

        -- Conditional Payment Details for Cheque
        CASE WHEN rr.[PaymentType] = 1 THEN _cheque.[ChequeNumber] END AS ChequeNumber,
        CASE WHEN rr.[PaymentType] = 1 THEN _cheque.[Amount] END AS ChequeAmount,
        CASE WHEN rr.[PaymentType] = 1 THEN _cheque.[Date] END AS ChequeDate,
        CASE WHEN rr.[PaymentType] = 1 THEN _cheque.[BankName] END AS ChequeBankName
        --,CASE WHEN rr.[PaymentType] = 1 THEN cheque.[ChequePhotoDocumentName] END AS ChequePhotoDocumentName,
        --CASE WHEN rr.[PaymentType] = 1 THEN cheque.[ChequePhotoDocumentUrl] END AS ChequePhotoDocumentUrl

		,'I agree to the terms' as Agreement
		,payment.CreatedAt as 'Added Time'
		,payment.UpdatedAt as 'Modified Time'
    FROM 
        [ApplicationUser] u
    INNER JOIN 
        [Association] a ON u.[AssociationId] = a.[Id]
    INNER JOIN 
        [Organisations] o ON u.[OrganisationId] = o.[Id]
	INNER JOIN
		[RefundRequest] rr on u.[Id]=rr.[RetireeId]
	LEFT JOIN 
        RefundPaymentModeUPI _upi ON rr.Id = _upi.RefundId AND rr.PaymentType = 3
    LEFT JOIN 
        RefundPaymentModeNEFT _neft ON rr.Id = _neft.RefundId AND rr.PaymentType = 2
    LEFT JOIN 
        RefundPaymentModeCheque _cheque ON rr.Id = _cheque.RefundId AND rr.PaymentType = 1

	   left join (
	select  distinct  ph.UserId, ppp.PolicyHeaderId,ppp.IsSelfPremiumSelected,ppp.IsSelfSpousePremiumSelected from  PolicyHeader ph	
	inner join  PolicyProductPremimum ppp on ppp.PolicyHeaderId=ph.Id
	inner join PolicyProductDetails ppd on ppp.PolicyHeaderId=ppd.PolicyHeaderId and ppp.ProductPremimumId=ppd.SumInsuredPremimumId
	)opt on opt.UserId=u.Id
    left  join (select 
	distinct 
	isnull(spouse.Name,'') as 'Name of Spouse',
	(case when  isnull(spouse.Gender,'')=1 then 'Male'
		when  isnull(spouse.Gender,'')=1 then 'Female'
		else '' end) as  Gender,
		CONVERT (varchar(10),spouse.DateOfBirth,103) as 'Date of Birth',

		isnull(child1.Name,'') as 'Name of Handicapped Child 1',
			CONVERT (varchar(10),child1.DateOfBirth,103) as 'Child1 Date of Birth',
          (case when  isnull(child1.Gender,'')=1 then 'Male'
		when  isnull(child1.Gender,'')=1 then 'Female'
		else '' end) as  'Child1 Gender',
	
	   isnull(child2.Name,'') as 'Name of Handicapped Child 2',
			CONVERT (varchar(10),child2.DateOfBirth,103) as 'Child2 Date of Birth',
          (case when  isnull(child2.Gender,'')=1 then 'Male'
		when  isnull(child2.Gender,'')=1 then 'Female'
		else '' end) as  'Child2 Gender'
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson spouse on spouse.PolicyId=ph.Id and bd.Spouse=spouse.Id
left join BeneficiaryPerson child1 on child1.PolicyId=ph.Id and bd.Child1=child1.Id
left join BeneficiaryPerson child2 on child2.PolicyId=ph.Id and bd.Child2=child2.Id
where (spouse.Name is not null or child1.Name is not null or child2.Name is not null)
)dep on dep.UserId=u.Id
left join 
(select distinct ph.UserId,ph.CreatedAt,ph.UpdatedAt,  ph.TotalPremimum,PaymentType,ph.ChildPremium from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id) payment on payment.UserId=u.Id
left join (
select 
	distinct 
	isnull(Nominee.Name,'') as 'Nominee Name',
	(case when  isnull(Nominee.Gender,'')=1 then 'Male'
		when  isnull(Nominee.Gender,'')=1 then 'Female'
		else '' end) as  'Nominee Gender',
		CONVERT (varchar(10),Nominee.DateOfBirth,103) as 'Nominee Date of Birth'
		,(case when Nominee.NomineeRelation=1 then 'Spouse'
		when Nominee.NomineeRelation=2 then 'Father'
		when Nominee.NomineeRelation=3 then 'Mother'
		when Nominee.NomineeRelation=4 then 'Son'
		when Nominee.NomineeRelation=5 then 'Daughter'
		when Nominee.NomineeRelation=6 then 'FatherInlaw'
		when Nominee.NomineeRelation=7 then 'Motherinlaw'
		when Nominee.NomineeRelation=8 then 'SonInlaw'
		when Nominee.NomineeRelation=9 then 'DaughterInlaw'
		when Nominee.NomineeRelation=10 then 'Brother'
		when Nominee.NomineeRelation=11 then 'Sister'
		when Nominee.NomineeRelation=12 then 'Grandfather'
		when Nominee.NomineeRelation=13 then 'Grandmother'
		when Nominee.NomineeRelation=14 then 'Grandson'
		when Nominee.NomineeRelation=15 then 'Granddaughter'
		when Nominee.NomineeRelation=16 then 'Nephew'
		when Nominee.NomineeRelation=17 then 'Niece'
			when Nominee.NomineeRelation=18 then 'Uncle'
		when Nominee.NomineeRelation=19 then 'Aunt'
		when Nominee.NomineeRelation=20 then 'Cousin'
		when Nominee.NomineeRelation=21 then 'Guardian'
			when Nominee.NomineeRelation=22 then 'Partner'
				when Nominee.NomineeRelation=0 then 'None' end) as 'Nominee Relation' 
		,ph.UserId
from PolicyHeader ph
left join BeneficiaryDetails bd on bd.PolicyId=ph.Id
left join BeneficiaryPerson Nominee  on Nominee.PolicyId=ph.Id and bd.Nominee=Nominee.Id
where (Nominee.Name is not null)
) Nominee on Nominee.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,ch.BankName as 'Bank Name (Amount Transfer From)','' as AccountNumber,'' as IfscCode,ch.ChequeNumber as 'Transaction Number/Cheque Number'
,a.AssociationName as InFavourOf,pd.CreatedAt as 'Payment Date/Cheque Date'
,ch.ChequeDepositLocation [Cheque Deposit Location]
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeCheque ch on ch.PaymentDetailId=pd.Id
inner join Association a on ch.InFavourOfAssociationId=a.Id
) cheque on cheque.UserId=u.Id
left join (
select distinct ph.UserId,PaymentType
,neft.BankName as 'Bank Name (Amount Transfer From)',neft.TransactionId as 'Transaction Number/Cheque Number'
,neft.AccountNumber,neft.IfscCode,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeNEFT neft on neft.PaymentDetailId=pd.Id
)neft on neft.UserId=u.Id

left join (
select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,upi.TransactionNumber as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeUPI upi on upi.PaymentDetailId=pd.Id
)upi on upi.UserId=u.Id
left join (select distinct ph.UserId,PaymentType
,'' as 'Bank Name (Amount Transfer From)','' as IfscCode,gate.TransactionId as 'Transaction Number/Cheque Number'
,'' AccountNumber,pd.CreatedAt as 'Payment Date/Cheque Date'
from PolicyHeader ph
inner join PaymentDetails pd on pd.PolicyId=ph.Id
inner join PaymentModeGateway gate on gate.PaymentDetailId=pd.Id
)gate on gate.UserId=u.Id

WHERE 
       (u.AssociationId = @AssociationID OR @AssociationID IS NULL)  -- Filter by AssociationID if provided
        AND (u.OrganisationId = @OrganisationID OR @OrganisationID IS NULL) -- Filter by OrganisationID if provided
        And CAST(rr.[RefundRequestDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)-- Filter by date range
        AND rr.IsActive = 1
        AND u.IsActive = 1;
    END TRY
    BEGIN CATCH
      
	   EXEC dbo.spGetErrorInfo ;
    
        
    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[USPDailyCountAssociationWise]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[USPDailyCountAssociationWise]
	-- Add the parameters for the stored procedure here
	@ReportDate DATE
AS
BEGIN
	BEGIN TRY
		SELECT 
			A.AssociationName AS [Association Name], 
			COUNT(DISTINCT U.Id) AS [No of Retirees Form Completed],
			CASE 
				WHEN P.PaymentMode = 1 THEN 'Online' 
				ELSE 'Offline' 
			END AS [Offline/Online Payment],
			COUNT(CASE WHEN P.IsPaymentConfirmed = 1 THEN P.Id END) AS [Payment Received Nos],
			SUM(CASE WHEN P.IsPaymentConfirmed = 1 THEN P.AmountPaid END) AS [Payment Received Amount],
			COUNT(CASE WHEN P.IsPaymentConfirmed = 0 THEN P.Id END) AS [Payment Pending Nos],
			SUM(CASE WHEN P.IsPaymentConfirmed = 0 THEN P.PayableAmount END) AS [Payment Pending Amount],
			COUNT(CASE WHEN P.PaymentStatus = 3 THEN P.Id END) AS [Bounce Payment Count],  -- Assuming PaymentStatus '3' means payment bounced
			COUNT(RR.Id) AS [Refund Request Received]
		FROM 
			[dbo].[Applicationuser] U
		LEFT JOIN 
			[dbo].[PaymentDetails] P ON U.Id = P.UserId
		LEFT JOIN 
			[dbo].[RefundRequest] RR ON U.Id = RR.RetireeId
		LEFT JOIN 
			[dbo].[Association] A ON U.AssociationId = A.Id
		WHERE 
			
			(cast(P.[PaymentDate] as date)=cast(@ReportDate as date))and
			A.IsActive = 1  -- Assuming only active associations should be included
		GROUP BY 
			A.AssociationName, P.PaymentMode

    END TRY
    BEGIN CATCH
      
	 EXEC dbo.spGetErrorInfo
    
        
    END CATCH

END
GO
/****** Object:  StoredProcedure [dbo].[getRefundReports]    Script Date: 09/20/2024 12:00:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[getRefundReports]
    @AssociationID INT = NULL,
    @OrganisationID INT = NULL,
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL
AS
BEGIN
    SELECT 
        rr.Id AS RefundRequestId,
        au.FirstName,
        au.LastName,
        au.Email,
        au.CountryCode,
        au.MobileNumber,
        au.DOB,
        au.UserType,
        au.Gender,
        o.Name as OrganisationName,
        a.AssociationName,
        au.EMPIDPFNo,
        au.Address,
        au.City,
        s.Name as State,
        c.Name as Country,
        au.Pincode,
        rr.[PolicyId],
        rr.[RetireeId],
        rr.[RefundAmount],
        rr.[RefundRequestDate],
        rr.[Status],
        rr.[Comment],
        rr.[RefundRequstHandledBy],
        --rr.[RefundDocumentName],
        --rr.[RefundDocumentUrl],
        rr.[PaymentType],
  --Cheque = 1, NEFT = 2,UPI=3, Gateway=4
        -- Conditional Payment Details for UPI
        CASE WHEN rr.[PaymentType] = 3 THEN upi.[TransactionNumber] END AS TransactionNumber,
        CASE WHEN rr.[PaymentType] = 3 THEN upi.[Amount] END AS UPIAmount,
        CASE WHEN rr.[PaymentType] = 3 THEN upi.[Date] END AS UPIDate,
        --CASE WHEN rr.[PaymentType] = 3 THEN upi.[UPIReceiptDocumentName] END AS UPIReceiptDocumentName,
        --CASE WHEN rr.[PaymentType] = 3 THEN upi.[UPIReceiptDocumentUrl] END AS UPIReceiptDocumentUrl,

        -- Conditional Payment Details for NEFT
        CASE WHEN rr.[PaymentType] = 2 THEN neft.[TransactionId] END AS NEFTTransactionId,
        CASE WHEN rr.[PaymentType] = 2 THEN neft.[Amount] END AS NEFTAmount,
        CASE WHEN rr.[PaymentType] = 2 THEN neft.[Date] END AS NEFTDate,
        CASE WHEN rr.[PaymentType] = 2 THEN neft.[BankName] END AS NEFTBankName,
        CASE WHEN rr.[PaymentType] = 2 THEN neft.[BranchName] END AS NEFTBranchName,
        CASE WHEN rr.[PaymentType] = 2 THEN neft.[AccountNumber] END AS NEFTAccountNumber,
        CASE WHEN rr.[PaymentType] = 2 THEN neft.[IfscCode] END AS NEFTIfscCode,
        --CASE WHEN rr.[PaymentType] = 2 THEN neft.[NEFTReceiptDocumentName] END AS NEFTReceiptDocumentName,
        --CASE WHEN rr.[PaymentType] = 2 THEN neft.[NEFTReceiptDocumentUrl] END AS NEFTReceiptDocumentUrl,

        -- Conditional Payment Details for Cheque
        CASE WHEN rr.[PaymentType] = 1 THEN cheque.[ChequeNumber] END AS ChequeNumber,
        CASE WHEN rr.[PaymentType] = 1 THEN cheque.[Amount] END AS ChequeAmount,
        CASE WHEN rr.[PaymentType] = 1 THEN cheque.[Date] END AS ChequeDate,
        CASE WHEN rr.[PaymentType] = 1 THEN cheque.[BankName] END AS ChequeBankName
        --,CASE WHEN rr.[PaymentType] = 1 THEN cheque.[ChequePhotoDocumentName] END AS ChequePhotoDocumentName,
        --CASE WHEN rr.[PaymentType] = 1 THEN cheque.[ChequePhotoDocumentUrl] END AS ChequePhotoDocumentUrl

    FROM 
        RefundRequest rr
    JOIN 
        ApplicationUser au ON rr.RetireeId = au.Id
    JOIN 
        Organisations o ON au.OrganisationId = o.Id
    JOIN 
        Association a ON au.AssociationId = a.Id
    JOIN 
        AddressCountry c ON au.CountryId = c.Id
    JOIN 
        AddressState s ON au.StateId = s.Id
    JOIN 
        PolicyHeader p ON rr.PolicyId = p.Id

    -- Left joins for conditional payment mode tables
    LEFT JOIN 
        RefundPaymentModeUPI upi ON rr.Id = upi.RefundId AND rr.PaymentType = 3
    LEFT JOIN 
        RefundPaymentModeNEFT neft ON rr.Id = neft.RefundId AND rr.PaymentType = 2
    LEFT JOIN 
        RefundPaymentModeCheque cheque ON rr.Id = cheque.RefundId AND rr.PaymentType = 1

    WHERE
        (au.AssociationId = @AssociationID OR @AssociationID IS NULL)  -- Filter by AssociationID if provided
        AND (au.OrganisationId = @OrganisationID OR @OrganisationID IS NULL) -- Filter by OrganisationID if provided
        And CAST(rr.[RefundRequestDate] AS date) between CAST(@StartDate AS date) and CAST(@EndDate AS date)-- Filter by date range
        AND rr.IsActive = 1
        AND au.IsActive = 1;

END
GO
