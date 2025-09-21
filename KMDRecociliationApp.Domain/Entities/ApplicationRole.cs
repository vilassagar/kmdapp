using FluentValidation;
using System.Data;
using System.Text.Json.Serialization;

namespace KMDRecociliationApp.Domain.Entities
{
    public class ApplicationRole : BaseEntity
    {
        
        public  string RoleName { get; set; }
        public  string ?Description { get; set; }
        public ICollection<ApplicationUserRole> ApplicationUserRole { get; set; } = new List<ApplicationUserRole>();
    }
    public class RoleValidator : AbstractValidator<ApplicationRole>
    {
        public RoleValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Description).Length(0, 50);
            RuleFor(x => x.RoleName).Length(0, 10);

        }
    }
}