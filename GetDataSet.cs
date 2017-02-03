using System.Data;

namespace Snow
{
    public partial class Orm : NativeDb
    {
        /// <summary>
        /// 返回记录集
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DataSet GetDataSet(BaseEntity entity)
        {
            prepare(entity);
            DataSet ds = _Find(entity);
            finish();

            return ds;
        }
    }
}
