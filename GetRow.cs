using System.Data;

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
            string cacheKey = string.Concat(cmd.TableName, "-Row-",string.Join(" and ",cmd.CacheKey));

            Log.Debug(this.GetType().Name + "_Row",cacheKey);

            object obj = DataCache.Get(cacheKey);

            if (obj == null)
            {
                DataSet ds = _Get(model);

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
        /// <param name="model"></param>
        /// <returns></returns>
        private DataRow[] _Rows(BaseEntity model)
        {
            DataSet ds = _Find(model);

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return null;
            }
            return ds.Tables[0].Select();
        }

    }
}
