using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow
{
    public partial class Orm : NativeDb
    {
        /// <summary>
        /// 返回记录集
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DataSet GetDataSet(BaseEntity model)
        {
            prepare(model);
            DataSet ds = this._Find(model);
            finish();

            return ds;
        }
    }
}
