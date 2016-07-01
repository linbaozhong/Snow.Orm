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
        /// 获取单行数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DataRow Row(BaseEntity model)
        {
            DataSet ds = this.Find(model, 1);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ds.Tables[0].Rows[0];
        }

        public DataRow[] Rows(BaseEntity model)
        {
            DataSet ds = this.Find(model);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ds.Tables[0].Select();
        }
    }
}
