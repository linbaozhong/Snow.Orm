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
        public DataRow GetRow(BaseEntity model)
        {
            prepare(model);
            DataRow row = _Row(model);
            finish();

            return row;
        }
        /// <summary>
        /// 获取多行数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DataRow[] GetRows(BaseEntity model)
        {
            prepare(model);
            DataRow[] rows = _Rows(model);
            finish();

            return rows;
        }

        /// <summary>
        /// 获取单行数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private DataRow _Row(BaseEntity model)
        {
            DataSet ds = this._Find(model, 1);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ds.Tables[0].Rows[0];
        }
        /// <summary>
        /// 获取多行数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private DataRow[] _Rows(BaseEntity model)
        {
            DataSet ds = this._Find(model);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ds.Tables[0].Select();
        }

    }
}
