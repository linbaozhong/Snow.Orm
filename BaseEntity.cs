using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Snow
{

    public abstract class BaseEntity : DictionaryBase
    {

        /// <summary>
        /// 数据表名
        /// </summary>
        public string TableName
        {
            protected set;
            get;
        }

        /// <summary>
        /// 主键
        /// </summary>
        public PrimaryKey PrimaryKey
        {
            protected set;
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
    /// 主键
    /// </summary>
    public struct PrimaryKey
    {
        /// <summary>
        /// 主键字段名，组合主键用逗号分隔
        /// </summary>
        public string Key;
        /// <summary>
        /// 是否数据库自动生成
        /// </summary>
        public bool Type;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">主键字段名，组合主键用逗号分隔</param>
        /// <param name="type">字段值是否数据库自动生成</param>
        public PrimaryKey(string key, bool type = true)
        {
            Key = key;
            Type = type;
        }
    }

}
