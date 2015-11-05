using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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
        /// 联结
        /// </summary>
        public List<string> Join = new List<string>();
        /// <summary>
        /// 查询条件
        /// </summary>
        public List<string> Where = new List<string>();
        /// <summary>
        /// 排序
        /// </summary>
        public List<string> OrderBy = new List<string>();
        /// <summary>
        /// 分组
        /// </summary>
        public string GroupBy = string.Empty;
        /// <summary>
        /// 分组筛选
        /// </summary>
        public string Having = string.Empty;
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
    public class Result
    {
        int _status = 200;
        /// <summary>
        /// 返回成功与否
        /// </summary>
        public int status
        {
            set
            {
                _status = value;
            }
            get
            {
                return _status;
            }
        }
        /// <summary>
        /// 返回的消息
        /// </summary>
        public object data { set; get; }
    }
}
