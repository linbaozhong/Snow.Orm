﻿using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Snow
{

    public abstract class BaseEntity : DictionaryBase
    {
        public BaseEntity()
        {
            Table = new Table();
        }

        #region 属性

        /// <summary>
        /// 映射数据表属性
        /// </summary>
        public Table Table
        {
            get;
        }
        /// <summary>
        /// 数据字典
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object this[string key]
        {
            set { this.Dictionary[key.ToLower()] = value; }
            get { return this.Dictionary[key.ToLower()]; }
        }


        #endregion

        #region 事件
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

        #region 方法

        /// <summary>
        /// 写入器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        protected void Set<T>(T value, [CallerMemberName]string name = null)
        {
            this.Dictionary[name.ToLower()] = value;
            //属性改变事件
            if (_OnPropertyChanged != null)
            {
                PropertyChangedEventArgs e = new PropertyChangedEventArgs(name);
                _OnPropertyChanged(this, e);
            }
        }
        /// <summary>
        /// 读取器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected T Get<T>([CallerMemberName]string name = null)
        {
            if (this.Dictionary.Contains(name.ToLower()))
            {
                return (T)this.Dictionary[name.ToLower()];
            }
            return default(T);
        }

        #endregion
    }


    public class Table
    {
        public string Name { set; get; }
        public PrimaryKey PrimaryKey { set; get; }
    }
    /// <summary>
    /// 主键
    /// </summary>
    public struct PrimaryKey
    {
        /// <summary>
        /// 主键字段名，组合主键用逗号分隔
        /// </summary>
        public string Key;
        /// <summary>
        /// 是否数据库自动生成(自增字段)
        /// </summary>
        public bool Type;
        /// <summary>
        /// 主键
        /// </summary>
        /// <param name="key">主键字段名，组合主键用逗号分隔</param>
        /// <param name="type">字段值是否数据库自动生成(自增)</param>
        public PrimaryKey(string key, bool type = true)
        {
            Key = key;
            Type = type;
        }
    }
    //public class Column
    //{
    //    private object _value = null;
    //    private ColumnType _columnType = ColumnType.String;
    //    private string _name = string.Empty;
    //    private string _aliax = string.Empty;
    //    public Column(string name, string alias = "", string memo = "", ColumnType columnType = ColumnType.Int)
    //    {
    //        _name = name;
    //        _columnType = columnType;
    //    }
    //    public object Value
    //    {
    //        set
    //        {
    //            _value = value;
    //        }
    //        get
    //        {
    //            switch (_columnType)
    //            {
    //                case ColumnType.Int:
    //                    if (_value == null)
    //                    {
    //                        return 0;
    //                    }
    //                    else
    //                    {
    //                        int result = 0;
    //                        if (int.TryParse(_value.ToString(), out result))
    //                        {
    //                            return result;
    //                        }
    //                        return 0;
    //                    }
    //                case ColumnType.Long:
    //                    if (_value == null)
    //                    {
    //                        return 0;
    //                    }
    //                    else
    //                    {
    //                        long result = 0;
    //                        if (long.TryParse(_value.ToString(), out result))
    //                        {
    //                            return result;
    //                        }
    //                        return 0;
    //                    }
    //                case ColumnType.String:
    //                    return _value;
    //                default:
    //                    return _value;
    //            }
    //        }
    //    }
    //}

    //public enum ColumnType
    //{
    //    Int,
    //    Long,
    //    String
    //}
}
