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
            result = new Result();
            try
            {
                if (!this.cmd.IsNative)
                {
                    // 读取一条记录
                    cmd.Top = 1;
                    // 查询准备
                    this._query<T>();
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
            finally
            {
                // 调试
                this.trace(this.GetSql());
                this._finish();
            }

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
            result = new Result();
            try
            {
                if (!this.cmd.IsNative)
                {
                    // 查询准备
                    this._query<T>();
                    // 构造命令
                    this.createSql();
                }
                // 检查前面的准备是否有错误
                if (result.status == 200)
                {
                    //
                    DataSet ds = this._dataSet();

                    this.DataTable2List(model, ds.Tables[0]);
                }

            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
            }
            finally
            {
                // 调试
                this.trace(this.GetSql());
                this._finish();
            }


            return result;
        }
        /// <summary>
        /// 返回一个记录集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public DataSet Find<T>()
        {
            try
            {
                if (!this.cmd.IsNative)
                {
                    // 查询准备
                    this._query<T>();
                    // 构造命令
                    this.createSql();
                }

                return this._dataSet();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                // 调试
                this.trace(this.GetSql());
                this._finish();
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
            result = new Result();
            try
            {
                if (!this.cmd.IsNative)
                {
                    // 插入
                    cmd.Command = Command.Insert;
                    // 获取对象的类型
                    Type m_type = typeof(T);
                    //Type m_type = model.GetType();
                    // 表名
                    this._getTableName(m_type);

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
                                cmd.Fields.Add(properties[i].Name.ToLower());

                                cmd.Params.Add(new SqlParameter("@" + properties[i].Name.ToLower(), properties[i].GetValue(model)));
                            }
                        }
                    }
                    this.createSql();
                }

                DbHelperSQL.ExecuteSql(string.Join(" ",this.cmd.SqlString), this.cmd.Params.ToArray());
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
            }
            finally
            {
                // 调试
                this.trace(this.GetSql());
                this._finish();
            }

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
            result = new Result();
            try
            {
                if (!this.cmd.IsNative)
                {

                    // 更新
                    cmd.Command = Command.Update;
                    // 获取对象的类型
                    Type m_type = typeof(T);
                    //Type m_type = model.GetType();
                    // 表名
                    this._getTableName(m_type);
                    
                    // 获取该对象的全部属性
                    PropertyInfo[] properties = m_type.GetProperties();

                    // 如果没有指定更新列，则更新传入参数对象的全部列
                    if (cmd.Fields.Count == 0)
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            cmd.Fields.Add(properties[i].Name.ToLower());

                            cmd.Params.Add(new SqlParameter("@" + properties[i].Name.ToLower(), properties[i].GetValue(model)));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            if (cmd.Fields.Contains(properties[i].Name.ToLower()))
                            {
                                cmd.Params.Add(new SqlParameter("@" + properties[i].Name.ToLower(), properties[i].GetValue(model)));
                            }
                        }
                    }
                    this.createSql();
                }

                DbHelperSQL.ExecuteSql(string.Join(" ", this.cmd.SqlString), this.cmd.Params.ToArray());
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
            }
            finally
            {
                // 调试
                this.trace(this.GetSql());
                this._finish();
            }

            return result;

        }
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result Delete<T>()
        {
            result = new Result();
            try
            {
                if (!this.cmd.IsNative)
                {

                    // 删除
                    cmd.Command = Command.Delete;
                    // 获取对象的类型
                    Type m_type = typeof(T);
                    //Type m_type = model.GetType();
                    // 表名
                    this._getTableName(m_type);

                    this.createSql();
                }

                DbHelperSQL.ExecuteSql(string.Join(" ", this.cmd.SqlString), this.cmd.Params.ToArray());
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
            }
            finally
            {
                // 调试
                this.trace(this.GetSql());
                this._finish();
            }

            return result;
        }
        /// <summary>
        /// 是否存在符合条件的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Exists<T>()
        {
            result = new Result();
            try
            {
                if (!this.cmd.IsNative)
                {
                    // 查询准备
                    this._query<T>();
                    // 构造命令
                    this.createSql();
                }
                return DbHelperSQL.Exists(string.Join(" ", this.cmd.SqlString), this.cmd.Params.ToArray());
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
                return false;
            }
            finally
            {
                // 调试
                this.trace(this.GetSql());

                this._finish();
            }
        }
        /// <summary>
        /// 统计符合条件的记录数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Int64 Count<T>()
        {
            result = new Result();
            try
            {
                if (!this.cmd.IsNative)
                {
                    this.cmd.Fields.Add("count(1)");
                    // 查询准备
                    this._query<T>();
                    // 构造命令
                    this.createSql();
                }

                object n = DbHelperSQL.GetSingle(string.Join(" ", this.cmd.SqlString), this.cmd.Params.ToArray());

                return Convert.ToInt64(n);
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
                return 0;
            }
            finally
            {
                // 调试
                this.trace(this.GetSql());
                this._finish();
            }
        }
        /// <summary>
        /// 执行查询，返回首行首列结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public object Single<T>()
        {
            result = new Result();
            try
            {
                if (!this.cmd.IsNative)
                {

                    // 查询准备
                    this._query<T>();
                    // 构造命令
                    this.createSql();
                }

                return DbHelperSQL.GetSingle(string.Join(" ", this.cmd.SqlString), this.cmd.Params.ToArray());
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
                return null;
            }
            finally
            {
                // 调试
                this.trace(this.GetSql());
                this._finish();
            }
        }
        /// <summary>
        /// 执行原生查询，返回受影响的行数
        /// </summary>
        /// <returns></returns>
        public int Exec()
        {
            result = new Result();
            try
            {
                if (this.cmd.IsNative)
                {
                    return DbHelperSQL.ExecuteSql(string.Join(" ", this.cmd.SqlString), this.cmd.Params.ToArray());
                }
                else
                {
                    throw new Exception("原生查询方法缺乏原生查询命令");
                }
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
                return 0;
            }
            finally
            {
                // 调试
                this.trace(this.GetSql());
                this._finish();
            }
        }


        #endregion


        #region 私有方法

        private void _finish()
        {
            this.cmd = new Sql();
        }

        private void _getTableName(Type type) {

            if (string.IsNullOrWhiteSpace(cmd.Table))
            {
                TableAttribute t_attr = type.GetCustomAttribute(typeof(TableAttribute), false) as TableAttribute;
                if (t_attr == null)
                {
                    cmd.Table = type.Name;
                }
                else
                {
                    cmd.Table = t_attr.Name;
                }
            }
        }
        private void _query<T>()
        {
            cmd.Command = Command.Select;

            // 获取对象的类型
            Type m_type = typeof(T);
            // 表名
            this._getTableName(m_type);
            
            // 如果没有指定返回列，则返回传入参数对象的全部列
            if (cmd.Fields.Count == 0)
            {
                // 获取该对象的全部属性
                PropertyInfo[] properties = m_type.GetProperties();

                for (int i = 0; i < properties.Length; i++)
                {
                    cmd.Fields.Add(properties[i].Name.ToLower());
                }
            }
        }

        /// <summary>
        /// 获取一个记录集
        /// </summary>
        /// <returns></returns>
        private DataSet _dataSet()
        {
            return DbHelperSQL.Query(string.Join(" ", this.cmd.SqlString), this.cmd.Params.ToArray());
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
            // 命令
            cmd.SqlString.Clear();
            cmd.SqlString.Add(cmd.Command);

            // Page 分页查询
            if (cmd.IsPage)
            {
                // 符合条件的记录总数
                try
                {
                    string _countSql = string.Format("{0} count(1) from {1} where {2}", cmd.Command, cmd.Table, string.Join(" ", cmd.Where.ToArray()));
                    // 调试
                    this.trace(_countSql);

                    object n = DbHelperSQL.GetSingle(_countSql, this.cmd.Params.ToArray());

                    cmd.Page.rowsCount = Convert.ToInt64(n);

                    if (cmd.Page.rowsCount > 0)
                    {
                        // 总页数
                        cmd.Page.pages = (int)Math.Ceiling(cmd.Page.rowsCount * 1.0 / cmd.Page.pageSize);
                        // 
                        cmd.Page.endIndex = cmd.Page.pageIndex * cmd.Page.pageSize;
                        cmd.Page.startIndex = cmd.Page.endIndex - cmd.Page.pageSize + 1;
                    }
                    //
                    result.data = cmd.Page;
                }
                catch (Exception e)
                {
                    result.status = -1;
                    result.data = e;

                    return;
                }

                // Fields
                if (cmd.Fields.Count > 0)
                {
                    cmd.SqlString.Add(string.Format(" {0}", string.Join(",", cmd.Fields)));
                }
                else
                {
                    cmd.SqlString.Add(" *");
                }

                //
                cmd.SqlString.Add(" from ( select row_number() over (");
                // Order
                if (cmd.OrderBy.Count > 0)
                {
                    cmd.SqlString.Add(string.Format(" order by T.{0}", string.Join(",T.", cmd.OrderBy.ToArray())));
                }
                else
                {
                    cmd.SqlString.Add(" order by T.id desc");
                }
                cmd.SqlString.Add(string.Format(")as Row,T.* from {0} T", cmd.Table));

                // Where
                if (cmd.Where.Count > 0)
                {
                    cmd.SqlString.Add(string.Format(" where {0}", string.Join(" ", cmd.Where.ToArray())));
                }

                cmd.SqlString.Add(string.Format(") TT where TT.Row between {0} and {1}", cmd.Page.startIndex, cmd.Page.endIndex));
            }
            else
            {
                // Top 
                if (cmd.Top > 0)
                {
                    cmd.SqlString.Add(" top " + cmd.Top);
                }

                // Fields
                if (cmd.Fields.Count > 0)
                {
                    cmd.SqlString.Add(string.Format(" {0}", string.Join(",", cmd.Fields)));
                }
                else
                {
                    cmd.SqlString.Add(" *");
                }
                // From
                cmd.SqlString.Add(string.Format(" from {0}", cmd.Table));

                // Join
                if (cmd.Join.Count > 0)
                {
                    cmd.SqlString.Add(string.Join(" ", cmd.Join.ToArray()));
                }

                // 如果是主键查询
                if (this.cmd.IsKey)
                {
                    cmd.SqlString.Add(" where " + cmd.Id);
                }
                else
                {
                    // Where
                    if (cmd.Where.Count > 0)
                    {
                        cmd.SqlString.Add(string.Format(" where {0}", string.Join(" ", cmd.Where.ToArray())));
                    }

                    // Order
                    if (cmd.OrderBy.Count > 0)
                    {
                        cmd.SqlString.Add(string.Format(" order by {0}", string.Join(",", cmd.OrderBy.ToArray())));
                    }

                    // GroupBy
                    if (!string.IsNullOrWhiteSpace(cmd.GroupBy))
                    {
                        cmd.SqlString.Add(string.Format(" groupby {0}", cmd.GroupBy));
                        //Having
                        if (!string.IsNullOrWhiteSpace(cmd.Having))
                        {
                            cmd.SqlString.Add(string.Format(" having {0}", cmd.Having));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 构造insert命令
        /// </summary>
        private void _insert()
        {
            // 命令
            cmd.SqlString.Clear(); 
            cmd.SqlString.Add(cmd.Command);
            cmd.SqlString.Add(cmd.Table);
            // Fields
            if (cmd.Fields.Count > 0)
            {
                cmd.SqlString.Add("(");
                cmd.SqlString.Add(string.Format(" {0}", string.Join(",", cmd.Fields)));
                cmd.SqlString.Add(")");
            }

            cmd.SqlString.Add("values (");

            //参数
            string[] _param = new string[cmd.Params.Count];
            for (int i = 0; i < cmd.Params.Count; i++)
            {
                _param[i] = cmd.Params[i].ParameterName;
            }

            cmd.SqlString.Add(string.Join(",", _param));

            cmd.SqlString.Add(")");
        }
        /// <summary>
        /// 构造update命令
        /// </summary>
        private void _update()
        {
            // 命令
            cmd.SqlString.Clear(); 
            cmd.SqlString.Add(cmd.Command);
            cmd.SqlString.Add(cmd.Table);
            cmd.SqlString.Add("set");

            // Fields
            int _count = cmd.Fields.Count;
            if (_count > 0)
            {
                string[] _set = new string[_count];

                for (int i = 0; i < _count; i++)
                {
                    _set[i] = cmd.Fields[i] + "=@" + cmd.Fields[i].ToLower();
                }
                cmd.SqlString.Add(string.Join(",", _set));
            }
            // Where
            if (this.cmd.IsKey)
            {
                cmd.SqlString.Add("where " + cmd.Id);
            }
            else
            {
                if (cmd.Where.Count > 0)
                {
                    cmd.SqlString.Add(string.Format("where {0}", string.Join(" ", cmd.Where.ToArray())));
                }
            }

            //
            //this._sqlStr = string.Join(" ", _cmd);
        }
        /// <summary>
        /// 构造delete命令
        /// </summary>
        private void _delete()
        {
            // 命令
            cmd.SqlString.Clear(); 
            cmd.SqlString.Add(cmd.Command);
            // From
            cmd.SqlString.Add(cmd.Table);
            // Where
            if (this.cmd.IsKey)
            {
                cmd.SqlString.Add("where " + cmd.Id);
            }
            else
            {
                if (cmd.Where.Count > 0)
                {
                    cmd.SqlString.Add(string.Format("where {0}", string.Join(" ", cmd.Where.ToArray())));
                }
            }
            //
            //this._sqlStr = string.Join(" ", _cmd);
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
                    if (!cmd.Fields.Contains(p.Name.ToLower()) || Convert.IsDBNull(row[p.Name]) || row[p.Name].ToString() == "")
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
