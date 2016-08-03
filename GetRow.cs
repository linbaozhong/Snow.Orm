using System.Data;

namespace Snow
{
    public partial class Orm : NativeDb
    {
        /// <summary>
        /// 获取单行数据
        /// </summary>
        /// <returns></returns>
        public DataRow GetRow()
        {
            prepare();
            DataRow row = _Row();
            finish();

            return row;
        }

        /// <summary>
        /// 获取多行数据
        /// </summary>
        /// <returns></returns>
        public DataRow[] GetRows()
        {
            prepare();
            DataRow[] rows = _Rows();
            finish();

            return rows;
        }

        /// <summary>
        /// 获取单行数据
        /// </summary>
        /// <param name="pull">强制从数据库拉取</param>
        /// <returns></returns>
        private DataRow _Row(bool pull = false)
        {
            string cacheKey = getCacheKey();

            Log.Debug(this.GetType().Name + "_Row", cacheKey);

            object obj = DataCache.Get(cacheKey);

            if (obj == null)
            {
                DataSet ds = _Get();

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                obj = ds.Tables[0].Rows[0];
                DataCache.Set(cacheKey, obj);
            }
            return obj as DataRow;
        }
        /// <summary>
        /// 获取多行数据
        /// </summary>
        /// <returns></returns>
        private DataRow[] _Rows()
        {
            DataSet ds = _Find();

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ds.Tables[0].Select();
        }

    }
}
