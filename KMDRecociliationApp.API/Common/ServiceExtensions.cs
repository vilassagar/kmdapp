using FluentValidation.AspNetCore;
using KMDRecociliationApp.Data.Repositories;
using KMDRecociliationApp.Domain.Auth;
using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.IISIntegration;
using FluentValidation;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Data.BackgroundTask;
using IEmailService = KMDRecociliationApp.Services.IEmailService;
using EmailService = KMDRecociliationApp.Services.EmailService;
namespace KMDRecociliationApp.API.Common
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {


            //validator

            services.AddSingleton<IValidatorInterceptor, CamelCasePropertyNamesInterceptor>();
            services.AddValidatorsFromAssemblyContaining<UserDTO>();
            services.AddValidatorsFromAssemblyContaining<LoginValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductValidator>();
            //services.AddValidatorsFromAssemblyContaining<AssociationValidator>();
            //Common services
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<ISmsSender, SmsSender>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddScoped<CommonHelperRepository>();
            services.AddScoped<OTPService>();

            //Repository
            services.AddScoped<ApplicationUserRepository>();
            services.AddScoped<ApplicationUserRole>();
            services.AddScoped<RoleRepository>();
            services.AddScoped<PermissionsRepository>();
            services.AddScoped<UserRepository>();
            services.AddScoped<AssociationRepository>();
            services.AddScoped<RolePermissionRepository>();
            services.AddScoped<ProductRepository>();
            services.AddScoped<ProductPolicyRepository>();
            services.AddScoped<CampaignRepository>();
            services.AddScoped<PaymentRepository>();
            services.AddScoped<RefundRequestRepository>();
            services.AddScoped<DashboardRepository>();
            services.AddScoped<ImportDataRepository>();
            services.AddScoped<ReportsRepository>();
            services.AddScoped<AuditRepository>();
            services.AddScoped<DigitReceiptRepository>();
            services.AddScoped<OrganisationRepository>();
            services.AddScoped<BulkReconcilationRepository>();
            services.AddScoped<FinancialYearRepository>();

            //background task
            //services.AddHostedService<BackgroundExcelService>();
            //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            //services.AddScoped<IExcelProcessingService, ExcelProcessingService>();
            //services.AddScoped<IJobTrackingService, JobTrackingService>();
            //services.AddScoped<Data.BackgroundTask.IEmailService, Data.BackgroundTask.EmailService>();
            //services.AddTransient<IHttpClientFactory, HttpClientFactory>();
        }
    }
}
