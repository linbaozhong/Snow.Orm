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
        public DataSet GetDataSet(BaseEntity model)
        {
            prepare(model);
            DataSet ds = _Find(model);
            finish();

            return ds;
        }
    }
}
