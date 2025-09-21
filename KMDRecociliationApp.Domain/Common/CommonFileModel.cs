using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.Common
{
    public class CommonFileModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public IFormFile? File { get; set; } = null;
        public string? Url  { get; set; }
        public byte[] ?FileData { get; set; }
        public bool isUpdateFile { get; set; } = false;
    }
   
}
