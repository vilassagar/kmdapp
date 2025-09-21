

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using KMDRecociliationApp.Domain.Entities;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.ReportDataModels;
using EntityFrameworkCore.UseRowNumberForPaging;
using KMDRecociliationApp.Domain.BulkUpload;
namespace KMDRecociliationApp.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options
            , IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<ApplicationUser> Applicationuser { get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }
        public DbSet<ApplicationPermission> ApplicationPermission { get; set; }
        public DbSet<ApplicationRolePermission> ApplicationRolePermission { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRole { get; set; }
        public DbSet<AddressState> AddressState { get; set; }
        public DbSet<AddressCountry> AddressCountry { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Association> Association { get; set; }
        public DbSet<AssociationOnePayDetails> AssociationOnePayDetails { get; set; }
        public DbSet<AssociationBankDetails> AssociationBankDetails { get; set; }
        public DbSet<AssociationContactDetails> AssociationContactDetails { get; set; }
        public DbSet<AssociationMessageDetails> AssociationMessageDetails { get; set; }
        public DbSet<Audit> AuditTrail { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Campaigns> Campaigns { get; set; }
        public DbSet<CampaignAssociations> CampaignAssociations { get; set; }
        public DbSet<CampaignProducts> CampaignProducts { get; set; }
        public DbSet<ExecuteCampaign> ExecuteCampaign { get; set; }
        public DbSet<CampaignMembersDetails> CampaignMembersDetails { get; set; }
        public DbSet<ProductPremimum> ProductPremimumChart { get; set; }
        public DbSet<PolicyHeader> PolicyHeader { get; set; }
        public DbSet<PolicyProductDetails> PolicyProductDetails { get; set; }
        public DbSet<PolicyProductPremimum> PolicyProductPremimum { get; set; }
        public DbSet<BeneficiaryDetails> BeneficiaryDetails { get; set; }
        public DbSet<BeneficiaryPerson> BeneficiaryPerson { get; set; }    
        public DbSet<PaymentModeUPI> PaymentModeUPI { get; set; }   
        public DbSet<PaymentModeNEFT> PaymentModeNEFT { get; set; }
        public DbSet<PaymentModeGateway> PaymentModeGateway { get; set; }
        public DbSet<PaymentModeCheque> PaymentModeCheque { get; set; }
        public DbSet<PaymentHeader> PaymentHeader { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<RefundRequest> RefundRequest { get; set; }
        public DbSet<RefundPaymentModeNEFT> RefundPaymentModeNEFT { get; set; }
        public DbSet<RefundPaymentModeUPI> RefundPaymentModeUPI { get; set; }
        public DbSet<RefundPaymentModeCheque> RefundPaymentModeCheque { get; set; }
        public DbSet<PaymentDetails> Report { get; set; }
        public DbSet<ExceptionLog> ExceptionLog { get; set; }
        public DbSet<AppLogs> AppLogs { get; set; }
        public DbSet<DailyCountAssociationWiseReportDataModels> DailyCountAssociationWiseReportDataModels { get; set; }
        public DbSet<AssociationWisePaymentDetailsDataModels> AssociationWisePaymentDetailsDataModels { get; set; }
        public DbSet<CompletedFormsDataModels> CompletedFormsDataModels { get; set; }
        public DbSet<IncompleteTransactionDataModels> IncompleteTransactionDataModels { get; set; }
        public DbSet<CorrectionReportDataModels> CorrectionReportDataModels { get; set; }
        public DbSet<GetRefundReportsDataModels> GetRefundReportsDataModels { get; set; }
        public DbSet<ExceptionLog> ExceptionLogs { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<ApplicationPage>ApplicationPages { get; set; }
        public DbSet<UserSpouseDetail> UserSpouseDetail { get; set; }
        public DbSet<UserNomineeDetail> UserNomineeDetail { get; set; }
        public DbSet<UserChild1> UserChild1 { get; set; }
        public DbSet<UserChild2> UserChild2 { get; set; }
        public DbSet<FinancialYear> FinancialYear {  get; set; }
        public DbSet<AgeBandPremiumRate> AgeBandPremiumRate { get; set; }

        //Insurance Policy Applicant classes
        public DbSet<ApplicantInsurancePolicy> ApplicantInsurancePolicies { get; set; }
        public DbSet<ApplicantBankDetails> ApplicantBankDetails { get; set; }
        public DbSet<ApplicantDependent> ApplicantDependents { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptions)
       => dbContextOptions
            .UseSqlServer(_configuration.GetConnectionString("constr")
           , builder => builder.UseRowNumberForPaging()).EnableSensitiveDataLogging();

        public virtual async Task<int> SaveChangesAsync(int userId, string email = null, string name = null, string comment = null)
        {
            OnBeforeSaveChanges(name, email, userId, comment);
            var result = await base.SaveChangesAsync();
            ChangeTracker.Clear();
            return result;
        }

        public virtual int SaveChanges(int userId, string email = null, string name = null, string comment = null)
        {
            OnBeforeSaveChanges(name, email, userId, comment);
            var result = base.SaveChanges();
            ChangeTracker.Clear();
            return result;

        }
        public override int SaveChanges()
        {
            // OnBeforeSaveChanges(name, email, userId, comment);
            var result = base.SaveChanges();
            ChangeTracker.Clear();
            return result;

        }

        private void OnBeforeSaveChanges(string name, string email, int userId, string comment)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached
                    || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserId = userId;
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = Domain.Enum.AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = Domain.Enum.AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = Domain.Enum.AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }
            foreach (var auditEntry in auditEntries)
            {
                AuditTrail.Add(auditEntry.ToAudit());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Defaly data load while migrating ef.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            // Configuring the one-to-many relationship between Product and ProductPremium
            modelBuilder.Entity<Product>()
               .HasOne(pp => pp.BasePolicy)
               .WithMany(p => p.ChildProducts)
               .HasForeignKey(pp => pp.BasePolicyId);

            modelBuilder.Entity<ProductPremimum>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.ProductPremiums)
                .HasForeignKey(pp => pp.ProductId);



            // Configure the self-referencing relationship
            modelBuilder.Entity<ProductPremimum>()
                .HasOne(p => p.ParentProductPremimum)
                .WithMany(p => p.TopUpOptions)
                .HasForeignKey(p => p.ParentProductPremimumId);

            modelBuilder.Entity<CampaignAssociations>()
               .HasOne(p => p.Campaign)
               .WithMany(p => p.CampaignAssociations)
               .HasForeignKey(p => p.CampaignId);

            modelBuilder.Entity<CampaignProducts>()
              .HasOne(p => p.Campaign)
              .WithMany(p => p.CampaignProducts)
              .HasForeignKey(p => p.CampaignId);

            modelBuilder.Entity<CampaignMembersDetails>()
             .HasOne(p => p.ExecuteCampaign)
             .WithMany(p => p.CampaignMembersDetail)
             .HasForeignKey(p => p.ExecuteCampaignId);
                
            modelBuilder.Entity<RefundRequest>()
                .HasOne(p => p.User)
                .WithMany(p => p.RefundRequest)
                .HasForeignKey(p => p.RetireeId);
            // Configure the unique constraint for IdCardNumber
            modelBuilder.Entity<ApplicantInsurancePolicy>()
                .HasIndex(p => p.IdCardNumber)
                .IsUnique();


            // Configure the unique constraint for UniqueIdentifier
            modelBuilder.Entity<ApplicantInsurancePolicy>()
                .HasIndex(p => p.UniqueIdentifier)
                .IsUnique();

            // Configure one-to-one relationship between Applicant and BankDetails
            modelBuilder.Entity<ApplicantInsurancePolicy>()
                .HasOne(a => a.BankDetails)
                .WithOne(b => b.Applicant)
                .HasForeignKey<ApplicantBankDetails>(b => b.ApplicantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship between Applicant and Dependents
            modelBuilder.Entity<ApplicantInsurancePolicy>()
                .HasMany(a => a.Dependents)
                .WithOne(d => d.Applicant)
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.Cascade);




        }
    }
}
