using KMDRecociliationApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;

namespace KMDRecociliationApp.Domain.Results
{
    public class AnnouncementResult: BaseResult<Announcement, AnnouncementResult>
    { 
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? AnnouncementText { get; set; }
            public Delta<AnnouncementResult> GetDelta()
            {
                Delta<AnnouncementResult> delta = new Delta<AnnouncementResult>();
                if (Id > 0) delta.TrySetPropertyValue("Id", Id);
                if (!string.IsNullOrEmpty(Name)) delta.TrySetPropertyValue("Name", Name);
                return delta;

            }

            public override AnnouncementResult CopyPolicyData(Announcement x)
            {
                this.Name = x.Name;
                this.Id = x.Id;
                this.AnnouncementText = x.AnnouncementText;
                return this;
            }
        }
}
