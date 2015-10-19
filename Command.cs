using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    class Sql
    {
        /// <summary>
        /// 命令
        /// </summary>
        public string Command { set; get; }
        /// <summary>
        /// 表名
        /// </summary>
        public string Table { set; get; }
        /// <summary>
        /// 返回的记录数
        /// </summary>
        public Int64 Top { set; get; }
        /// <summary>
        /// 字段
        /// </summary>
        public List<string> Fields = new List<string>();
        /// <summary>
        /// 查询主键
        /// </summary>
        public string Id { set; get; }
        /// <summary>
        /// 查询条件
        /// </summary>
        public List<string> Where = new List<string>();
        /// <summary>
        /// 排序
        /// </summary>
        public List<string> OrderBy = new List<string>();
        /// <summary>
        /// 参数
        /// </summary>
        public List<SqlParameter> Params = new List<SqlParameter>();
    }
    /// <summary>
    /// Sql Command
    /// </summary>
    class Command
    {
        public static string Insert = "insert";
        public static string Update = "update";
        public static string Delete = "delete";
        public static string Select = "select";
    }
    class Result
    {
        bool _ok = true;
        /// <summary>
        /// 返回成功与否
        /// </summary>
        public bool Ok
        {
            set
            {
                _ok = value;
            }
            get
            {
                return _ok;
            }
        }
        /// <summary>
        /// 返回的消息
        /// </summary>
        public object Data { set; get; }
    }
}
