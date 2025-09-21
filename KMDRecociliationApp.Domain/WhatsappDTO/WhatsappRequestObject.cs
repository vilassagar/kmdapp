using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Domain.WhatsappDTO
{
    public class WhatsappRequestObject
    {
        public string url { get; set; }
        public string Authorization { get; set; }
        public string recipient_type { get; set; }
        public string message_type { get; set; }
        public string source { get; set; }
        public string x_apiheader { get; set; }
        public string type_media_templatetype { get; set; }
        public string type_media_templateurl { get; set; }
        public string type_media_templatefilename { get; set; }
        public string MessageTemplateName { get; set; }

    }    
}
