using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow
{
    class Table: DictionaryBase
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            set { this.Dictionary[key] = value; }
            get { return this.Dictionary[key]; }
        }

        /// <summary>
        /// 数据实体
        /// </summary>
        public Dictionary<string, Column> Entity
        {
            get
            {
                return this.Dictionary as Dictionary<string, Column>;
            }
        }
    }
    /// <summary>
    /// 表列
    /// </summary>
    public class Column
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="type">类型</param>
        /// <param name="alias">别名</param>
        /// <param name="value">缺省值</param>
        /// <param name="isDatabaseGenerated">是否数据库自动生成列</param>
        public Column(string name, Type type, string alias = "", object value = null, bool isDatabaseGenerated = false)
        {
            Name = name;
            Type = type;
            Alias = alias;
            IsDatabaseGenerated = isDatabaseGenerated;
        }
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 列类型
        /// </summary>
        public Type Type { set; get; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { set; get; }
        /// <summary>
        /// 缺省值
        /// </summary>
        public object Value { set; get; }
        /// <summary>
        /// 是否数据库自动生成列
        /// </summary>
        public bool IsDatabaseGenerated { set; get; }
    }

}
