using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public class BaseModel
    {
        public Dictionary<string, object> Dict = new Dictionary<string, object>();

        public void Set<T>(T value, [CallerMemberName]string name = null)
        {
            Dict[name] = value;
            //属性改变事件
            if (_OnPropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(name);
                _OnPropertyChanged(this, e);
            }
        }

        public T Get<T>([CallerMemberName]string name = null)
        {
            object value;
            if (Dict.TryGetValue(name,out value))
            {

            }
            return (T)value;
        }

        #region Event
        /// <summary>
        /// 属性改变事件处理句柄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void PropertyChangedHandler(object sender, PropertyChangedEventArgs e);

        /// <summary>
        /// 属性委托处理句柄
        /// </summary>
        private PropertyChangedHandler _OnPropertyChanged = null;

        /// <summary>
        /// 对象属性改变时发生事件
        /// </summary>
        public event PropertyChangedHandler OnPropertyChanged
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
        public string[] Fields { set; get; }
    }
}
