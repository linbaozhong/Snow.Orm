using Shangshebao.DBUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Snow.Orm
{
    public partial class Db : NativeDb
    {
        #region 公共方法
        /// <summary>
        /// 获取单个数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result Get<T>(T model)
        {
            try
            {
                if (!this._nativeSql)
                {
                    // 读取一条记录
                    cmd.Top = 1;
                    // 查询准备
                    this._query(model);
                    // 构造命令
                    this.createSql();
                }
                //
                this.model(model);
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
            }
            // 调试
            this.trace(this.GetSql());
            return result;
        }
        /// <summary>
        /// 获取数据对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result Find<T>(List<T> model) where T : class,new()
        {
            try
            {
                if (!this._nativeSql)
                {
                    // 查询准备
                    this._query(model);
                    // 构造命令
                    this.createSql();
                }
                //
                DataSet ds = this._dataSet();

                this.DataTable2List(model, ds.Tables[0]);

            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
            }
            // 调试
            this.trace(this.GetSql());

            return result;
        }
        /// <summary>
        /// 返回一个记录集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public DataSet Find<T>(T model)
        {
            try
            {
                if (!this._nativeSql)
                {
                    // 查询准备
                    this._query(model);
                    // 构造命令
                    this.createSql();
                }
                // 调试
                this.trace(this.GetSql());

                return this._dataSet();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 插入一条新的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result Insert<T>(T model)
        {
            try
            {
                if (!this._nativeSql)
                {
                    // 插入
                    cmd.Command = Command.Insert;
                    // 获取对象的类型
                    Type m_type = model.GetType();
                    // 表名
                    cmd.Table = string.IsNullOrWhiteSpace(cmd.Table) ? m_type.Name : cmd.Table;

                    // 如果没有指定返回列，则返回传入参数对象的全部列
                    if (cmd.Fields.Count == 0)
                    {
                        // 获取该对象的全部属性
                        PropertyInfo[] properties = m_type.GetProperties();

                        for (int i = 0; i < properties.Length; i++)
                        {
                            // 忽略数据库自增字段
                            if (properties[i].IsDefined(typeof(DatabaseGeneratedAttribute), false))
                            {
                                continue;
                            }
                            else
                            {
                                cmd.Fields.Add(properties[i].Name);

                                cmd.Params.Add(new SqlParameter("@" + properties[i].Name, properties[i].GetValue(model)));
                            }
                        }
                    }
                    this.createSql();
                }

                DbHelperSQL.ExecuteSql(this._sqlStr, this._parameters);
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
            }
            // 调试
            this.trace(this.GetSql());
            return result;
        }
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result Update<T>(T model)
        {
            try
            {
                if (!this._nativeSql)
                {

                    // 更新
                    cmd.Command = Command.Update;
                    // 获取对象的类型
                    Type m_type = model.GetType();
                    // 表名
                    cmd.Table = string.IsNullOrWhiteSpace(cmd.Table) ? m_type.Name : cmd.Table;



                    // 获取该对象的全部属性
                    PropertyInfo[] properties = m_type.GetProperties();

                    // 如果没有指定更新列，则更新传入参数对象的全部列
                    if (cmd.Fields.Count == 0)
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            cmd.Fields.Add(properties[i].Name);

                            cmd.Params.Add(new SqlParameter("@" + properties[i].Name, properties[i].GetValue(model)));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (cmd.Fields.Contains(properties[i].Name))
                            {
                                cmd.Params.Add(new SqlParameter("@" + properties[i].Name, properties[i].GetValue(model)));
                            }
                        }
                    }
                    this.createSql();
                }

                DbHelperSQL.ExecuteSql(this._sqlStr, this._parameters);
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
            }
            // 调试
            this.trace(this.GetSql());

            return result;

        }
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result Delete<T>(T model)
        {
            try
            {
                if (!this._nativeSql)
                {

                    // 删除
                    cmd.Command = Command.Delete;
                    // 获取对象的类型
                    Type m_type = model.GetType();
                    // 表名
                    cmd.Table = string.IsNullOrWhiteSpace(cmd.Table) ? m_type.Name : cmd.Table;


                    this.createSql();
                }

                DbHelperSQL.ExecuteSql(this._sqlStr, this._parameters);
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
            }
            // 调试
            this.trace(this.GetSql());
            return result;
        }
        /// <summary>
        /// 是否存在符合条件的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Exists<T>(T model)
        {
            try
            {
                if (!this._nativeSql)
                {
                    // 查询准备
                    this._query(model);
                    // 构造命令
                    this.createSql();
                }
                // 调试
                this.trace(this.GetSql());

                return DbHelperSQL.Exists(this._sqlStr, this._parameters);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 统计符合条件的记录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Int64 Count<T>(T model)
        {
            try
            {
                if (!this._nativeSql)
                {
                    this.cmd.Fields.Add("count(1)");
                    // 查询准备
                    this._query(model);
                    // 构造命令
                    this.createSql();
                }
                // 调试
                this.trace(this.GetSql());

                object n = DbHelperSQL.GetSingle(this._sqlStr, this._parameters);

                return Convert.ToInt64(n);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 执行查询，返回首行首列结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public object Single<T>(T model)
        {
            try
            {
                if (!this._nativeSql)
                {

                    // 查询准备
                    this._query(model);
                    // 构造命令
                    this.createSql();
                }
                // 调试
                this.trace(this.GetSql());

                return DbHelperSQL.GetSingle(this._sqlStr, this._parameters);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 执行原生查询，返回受影响的行数
        /// </summary>
        /// <returns></returns>
        public int Exec()
        {
            try
            {
                if (this._nativeSql)
                {
                    // 调试
                    this.trace(this.GetSql());
                    return DbHelperSQL.ExecuteSql(this._sqlStr, this._parameters);
                }
                else
                {
                    throw new Exception("原生查询方法缺乏原生查询命令");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 执行查询，返回受影响的行数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Exec<T>(T model)
        {
            try
            {
                if (!this._nativeSql)
                {
                    // 查询准备
                    this._query(model);
                    // 构造命令
                    this.createSql();
                }
                // 调试
                this.trace(this.GetSql());

                return DbHelperSQL.ExecuteSql(this._sqlStr, this._parameters);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion


        #region 私有方法

        private void _query<T>(T model)
        {
            cmd.Command = Command.Select;

            // 获取对象的类型
            Type m_type = typeof(T);
            // 表名
            cmd.Table = string.IsNullOrWhiteSpace(cmd.Table) ? m_type.Name : cmd.Table;

            // 如果没有指定返回列，则返回传入参数对象的全部列
            if (cmd.Fields.Count == 0)
            {
                // 获取该对象的全部属性
                PropertyInfo[] properties = m_type.GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    cmd.Fields.Add(properties[i].Name);
                }
            }
        }

        /// <summary>
        /// 获取一个记录集
        /// </summary>
        /// <returns></returns>
        private DataSet _dataSet()
        {
            return DbHelperSQL.Query(this._sqlStr, this._parameters);
        }

        /// <summary>
        /// 获取一个对象实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        private void model<T>(T model)
        {
            DataSet ds = this._dataSet();

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow2Model(model, ds.Tables[0].Rows[0]);
            }
        }
        /// <summary>
        /// 构造select命令
        /// </summary>
        private void _select()
        {
            List<string> _cmd = new List<string>();

            // 命令
            _cmd.Add(cmd.Command);

            // Top
            if (cmd.Top > 0)
            {
                _cmd.Add(" top " + cmd.Top);
            }

            // Fields
            if (cmd.Fields.Count > 0)
            {
                _cmd.Add(string.Format(" {0}", string.Join(",", cmd.Fields)));
            }
            else
            {
                _cmd.Add(" *");
            }

            // From
            _cmd.Add(string.Format(" from {0}", cmd.Table));

            // Join
            if (cmd.Join.Count > 0)
            {
                _cmd.Add(string.Join(" ", cmd.Join.ToArray()));
            }

            // 如果是主键查询
            if (_idCondition)
            {
                _cmd.Add(" where " + cmd.Id);
            }
            else
            {
                // Where
                if (cmd.Where.Count > 0)
                {
                    _cmd.Add(string.Format(" where {0}", string.Join(" ", cmd.Where.ToArray())));
                }

                // Order
                if (!_idCondition && cmd.OrderBy.Count > 0)
                {
                    _cmd.Add(string.Format(" order by {0}", string.Join(",", cmd.OrderBy.ToArray())));
                }

                // GroupBy
                if (!string.IsNullOrWhiteSpace(cmd.GroupBy))
                {
                    _cmd.Add(string.Format(" groupby {0}", cmd.GroupBy));
                    //Having
                    if (!string.IsNullOrWhiteSpace(cmd.Having))
                    {
                        _cmd.Add(string.Format(" having {0}", cmd.Having));
                    }
                }
            }

            //
            this._sqlStr = string.Join("", _cmd);
        }
        /// <summary>
        /// 构造insert命令
        /// </summary>
        private void _insert()
        {
            List<string> _cmd = new List<string>();

            // 命令
            _cmd.Add(cmd.Command);
            _cmd.Add(cmd.Table);
            // Fields
            if (cmd.Fields.Count > 0)
            {
                _cmd.Add("(");
                _cmd.Add(string.Format(" {0}", string.Join(",", cmd.Fields)));
                _cmd.Add(")");
            }

            _cmd.Add("values (");

            //参数
            string[] _param = new string[cmd.Params.Count];
            for (int i = 0; i < cmd.Params.Count; i++)
            {
                _param[i] = cmd.Params[i].ParameterName;
            }

            _cmd.Add(string.Join(",", _param));

            _cmd.Add(")");
            //
            this._sqlStr = string.Join(" ", _cmd);
        }
        /// <summary>
        /// 构造update命令
        /// </summary>
        private void _update()
        {
            List<string> _cmd = new List<string>();

            // 命令
            _cmd.Add(cmd.Command);
            _cmd.Add(cmd.Table);
            _cmd.Add("set");

            // Fields
            int _count = cmd.Fields.Count;
            if (_count > 0)
            {
                string[] _set = new string[_count];

                for (int i = 0; i < _count; i++)
                {
                    _set[i] = cmd.Fields[i] + "=@" + cmd.Fields[i];
                }
                _cmd.Add(string.Join(",", _set));
            }
            // Where
            if (_idCondition)
            {
                _cmd.Add("where " + cmd.Id);
            }
            else
            {
                if (cmd.Where.Count > 0)
                {
                    _cmd.Add(string.Format("where {0}", string.Join(" ", cmd.Where.ToArray())));
                }
            }

            //
            this._sqlStr = string.Join(" ", _cmd);
        }
        /// <summary>
        /// 构造delete命令
        /// </summary>
        private void _delete()
        {
            List<string> _cmd = new List<string>();

            // 命令
            _cmd.Add(cmd.Command);
            // From
            _cmd.Add(cmd.Table);
            // Where
            if (_idCondition)
            {
                _cmd.Add("where " + cmd.Id);
            }
            else
            {
                if (cmd.Where.Count > 0)
                {
                    _cmd.Add(string.Format("where {0}", string.Join(" ", cmd.Where.ToArray())));
                }
            }
            //
            this._sqlStr = string.Join(" ", _cmd);
        }
        /// <summary>
        /// table转list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dt"></param>
        private void DataTable2List<T>(List<T> model, DataTable dt) where T : class,new()
        {
            foreach (DataRow row in dt.Rows)
            {
                T m_t = new T();

                this.DataRow2Model(m_t, row);

                model.Add(m_t);
            }
        }
        /// <summary>
        /// datarow转model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="row"></param>
        private void DataRow2Model<T>(T model, DataRow row)
        {
            // 获取对象的类型
            Type m_type = model.GetType();
            // 获取该对象的全部属性
            PropertyInfo[] properties = m_type.GetProperties();

            // 根据查询字段列，填充对象
            try
            {
                foreach (PropertyInfo p in properties)
                {
                    if (Convert.IsDBNull(row[p.Name]) || row[p.Name].ToString() == "")
                    {
                        continue;
                    }
                    else
                    {
                        p.SetValue(model, Convert.ChangeType(row[p.Name], p.PropertyType), null);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            // 动态编译

        }

        #endregion
    }

}
