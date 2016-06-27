using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Snow
{
    public class Direction
    {
        string _field;
        System.Data.ParameterDirection _direction;
        /// <summary>
        /// 存储过程的参数
        /// </summary>
        /// <param name="field">字段名</param>
        /// <param name="direction">参数方向</param>
        public Direction(string field, System.Data.ParameterDirection direction)
        {
            this._field = field;
            this._direction = direction;
        }
        /// <summary>
        /// 字段名
        /// </summary>
        public string field { set { _field = value; } get { return _field; } }
        /// <summary>
        /// 参数方向
        /// </summary>
        public System.Data.ParameterDirection direction { set { _direction = value; } get { return _direction; } }
    }
    public class Page
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public long rowsCount { set; get; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int pages { set; get; }
        /// <summary>
        /// 页码
        /// </summary>
        public int pageIndex { set; get; }
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int pageSize { set; get; }
        /// <summary>
        /// 起始序号
        /// </summary>
        public int startIndex { set; get; }
        /// <summary>
        /// 终止序号
        /// </summary>
        public int endIndex { set; get; }
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

    public abstract class BaseEntity : DictionaryBase
    {

        private string _tablename = string.Empty;

        /// <summary>
        /// 主键名及其类型
        /// </summary>
        protected Dictionary<string, object> _primary = null;

        /// <summary>
        /// 数据表名
        /// </summary>
        public string TableName
        {
            protected set { _tablename = value; }
            get { return _tablename; }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public Dictionary<string, object> PrimaryKey
        {
            protected set
            {
                _primary = value;
            }
            get
            {
                return _primary;
            }
        }

        public object this[string key]
        {
            set { this.Dictionary[key] = value; }
            get { return this.Dictionary[key]; }
        }

        public IDictionary GetEntity
        {
            get
            {
                return this.Dictionary;
            }
        }
        protected void Set<T>(T value, [CallerMemberName]string name = null)
        {
            this.Dictionary[name] = value;
            //属性改变事件
            if (_OnPropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(name);
                _OnPropertyChanged(this, e);
            }
        }

        protected T Get<T>([CallerMemberName]string name = null)
        {
            if (this.Dictionary.Contains(name))
            {
                return (T)this.Dictionary[name];
            }
            return default(T);
        }

        #region Event
        /// <summary>
        /// 属性改变事件处理句柄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected delegate void PropertyChangedHandler(object sender, PropertyChangedEventArgs e);

        /// <summary>
        /// 属性委托处理句柄
        /// </summary>
        private PropertyChangedHandler _OnPropertyChanged = null;

        /// <summary>
        /// 对象属性改变时发生事件
        /// </summary>
        protected event PropertyChangedHandler OnPropertyChanged
        {
            add { _OnPropertyChanged += value; }
            remove { _OnPropertyChanged -= value; }
        }
        #endregion
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
        /// 包含的字段
        /// </summary>
        public List<string> Fields = new List<string>();
        /// <summary>
        /// 排除的字段
        /// </summary>
        public List<string> ExcludeFields = new List<string>();
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
        /// 分页
        /// </summary>
        public Page Page = new Page();
        /// <summary>
        /// 是否主键查询
        /// </summary>
        public bool IsKey = false;
        /// <summary>
        /// 是否分页查询
        /// </summary>
        public bool IsPage = false;
        /// <summary>
        /// 是否原生查询
        /// </summary>
        public bool IsNative = false;
        /// <summary>
        /// 数据库操作命令字符串
        /// </summary>
        public List<string> SqlString = new List<string>();
        /// <summary>
        /// 查询参数
        /// </summary>
        public List<SqlParameter> Params = new List<SqlParameter>();
    }

    class StaticSql
    {
        public string command { set; get; }

        public SqlParameter[] parameters { set; get; }

        public PropertyInfo[] properties { set; get; }
    }

    class Table
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 字段
        /// </summary>
        public Column[] Fields { set; get; }

        public DataColumn[] Columns { set; get; }
    }
    /// <summary>
    /// 表列
    /// </summary>
    class Column
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">列名</param>
        /// <param name="type">类型</param>
        /// <param name="value">初始值</param>
        /// <param name="isDatabaseGenerated">是否数据库自动生成列</param>
        public Column(string name, Type type, object value = null, bool isDatabaseGenerated = false)
        {
            Name = name;
            Type = type;
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
        /// 初始值
        /// </summary>
        public object Value { set; get; }
        /// <summary>
        /// 是否数据库自动生成列
        /// </summary>
        public bool IsDatabaseGenerated { set; get; }
    }

}
