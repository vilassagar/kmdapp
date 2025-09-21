using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Entities
{
    public class ApplicationPermission:BaseEntity
    {
        public string? PermissionType { get; set; }
        public string? Description { get; set; }
        public bool? Create { get; set; }
        public bool? Read { get; set; }
        public bool? Update { get; set; }
        public bool? Delete { get; set; }
    }

    

    public class PermissionValidator : AbstractValidator<ApplicationPermission>
    {
        public PermissionValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Description).Length(0, 50);
            RuleFor(x => x.PermissionType).MinimumLength(2);

        }
    }
}
