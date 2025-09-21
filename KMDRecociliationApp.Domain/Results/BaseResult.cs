using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace KMDRecociliationApp.Domain.Results
{
    public class BaseResult<T,TR>
    {
        public virtual TR CopyPolicyData(T v)
        {
            var jsonstr = JsonConvert.SerializeObject(v);
            TR jsonobj=(TR) JsonConvert.DeserializeObject(jsonstr,typeof(TR));
            return jsonobj;
        }
        public virtual T CopyTo()
        {
            var jsonstr = JsonConvert.SerializeObject(this);
            T jsonobj = (T)JsonConvert.DeserializeObject(jsonstr, typeof(T));
            return jsonobj;
        }
    }
}
