using Snow.DBUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        public Result Get<T>(T model, [CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0)
        {

            this._prepare(filePath, methodName, lineNumber);
            
            try
            {
                if (!this.cmd.IsNative)
                {
                    // 读取一条记录
                    cmd.Top = 1;
                    // 查询准备
                    this._query<T>(model);
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
        public Result Find<T>(List<T> model, [CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0) where T : class,new()
        {
            this._prepare(filePath, methodName, lineNumber);
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
        public Result Insert<T>(T model, [CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0)
        {
            this._prepare(filePath, methodName, lineNumber);
            try
            {
                // 获取对象的类型
                Type m_type = typeof(T);
                // 获取该对象的全部属性
                PropertyInfo[] properties = m_type.GetProperties();

                if (!this.cmd.IsNative)
                {
                    // 插入
                    cmd.Command = Command.Insert;
                    // 表名
                    this._getTableName(m_type);

                    // 如果没有指定返回列，则返回传入参数对象的全部列
                    if (cmd.Fields.Count == 0)
                    {

                        for (int i = 0; i < properties.Length; i++)
                        {
                            // 忽略排除字段
                            if (cmd.ExcludeFields.Count > 0 && cmd.ExcludeFields.Contains(properties[i].Name.ToLower()))
                            {
                                continue;
                            }
                            // 数据库自增字段
                            if (properties[i].IsDefined(typeof(DatabaseGeneratedAttribute), false))
                            {
                                SqlParameter idParameter = new SqlParameter("@id", 0);
                                idParameter.Direction = ParameterDirection.Output;

                                cmd.Params.Add(idParameter);
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

                parameters = this.cmd.Params.ToArray();

                // 返回数据库自增关键字段值
                DbHelperSQL.ExecuteSql(string.Join(" ", this.cmd.SqlString), this.parameters);
                // 将自增字段值写入model
                for (int i = 0; i < properties.Length; i++)
                {
                    // 数据库自增字段
                    if (properties[i].IsDefined(typeof(DatabaseGeneratedAttribute), false))
                    {
                        switch (properties[i].PropertyType.Name)
                        {
                            case "Int64":
                                properties[i].SetValue(model, Convert.ToInt64(cmd.Params[i].Value));
                                break;
                        }
                    }
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

        public Result Insert<T>(List<T> model, [CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0)
        {
            this._prepare(filePath, methodName, lineNumber);

            try
            {
                if (this.cmd.IsNative)
                {
                    this.createSql();

                    parameters = this.cmd.Params.ToArray();

                    DbHelperSQL.ExecuteSql(string.Join(" ", this.cmd.SqlString), this.parameters);
                }
                else
                {
                    // 命令列表
                    List<CommandInfo> l_command = new List<CommandInfo>();
                    // 插入
                    cmd.Command = Command.Insert;
                    // 获取对象的类型
                    Type m_type = typeof(T);
                    // 表名
                    this._getTableName(m_type);

                    // 获取该对象的全部属性
                    PropertyInfo[] properties = m_type.GetProperties();

                    // 如果没有指定返回列，则返回传入参数对象的全部列
                    if (cmd.Fields.Count == 0)
                    {
                        foreach (T item in model)
                        {
                            for (int i = 0; i < properties.Length; i++)
                            {
                                // 忽略排除字段
                                if (cmd.ExcludeFields.Count > 0 && cmd.ExcludeFields.Contains(properties[i].Name.ToLower()))
                                {
                                    continue;
                                }

                                // 忽略数据库自增字段
                                if (properties[i].IsDefined(typeof(DatabaseGeneratedAttribute), false))
                                {
                                    SqlParameter idParameter = new SqlParameter("@id", 0);
                                    idParameter.Direction = ParameterDirection.Output;

                                    cmd.Params.Add(idParameter);
                                }
                                else
                                {
                                    cmd.Fields.Add(properties[i].Name.ToLower());

                                    cmd.Params.Add(new SqlParameter("@" + properties[i].Name.ToLower(), properties[i].GetValue(item)));
                                }
                            }

                            this.createSql();

                            parameters = this.cmd.Params.ToArray();

                            l_command.Add(new CommandInfo(string.Join(" ", this.cmd.SqlString), this.parameters));

                            // 调试
                            this.trace(this.GetSql());

                            cmd.Fields.Clear();
                            cmd.Params.Clear();
                        }
                    }
                    DbHelperSQL.ExecuteSqlTran(l_command);
                    // 将自增字段值写入model
                    for (int t = 0; t < model.Count; t++)
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            // 数据库自增字段
                            if (properties[i].IsDefined(typeof(DatabaseGeneratedAttribute), false))
                            {
                                switch (properties[i].PropertyType.Name)
                                {
                                    case "Int64":
                                        properties[i].SetValue(model[t], Convert.ToInt64(l_command[t].Parameters[i].Value));
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.status = -1;
                result.data = e;
            }
            finally
            {
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
        public Result Update<T>(T model, [CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0)
        {
            this._prepare(filePath, methodName, lineNumber);
            try
            {
                if (!this.cmd.IsNative)
                {

                    // 更新
                    cmd.Command = Command.Update;
                    // 获取对象的类型
                    Type m_type = typeof(T);
                    // 表名
                    this._getTableName(m_type);

                    // 获取该对象的全部属性
                    PropertyInfo[] properties = m_type.GetProperties();

                    // 如果没有指定更新列，则更新传入参数对象的全部列
                    if (cmd.Fields.Count == 0)
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            // 忽略排除字段
                            if (cmd.ExcludeFields.Count > 0 && cmd.ExcludeFields.Contains(properties[i].Name.ToLower()))
                            {
                                continue;
                            }

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

                parameters = this.cmd.Params.ToArray();

                DbHelperSQL.ExecuteSql(string.Join(" ", this.cmd.SqlString), this.parameters);
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
        public Result Delete<T>([CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0)
        {
            this._prepare(filePath, methodName, lineNumber);
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

                parameters = this.cmd.Params.ToArray();

                DbHelperSQL.ExecuteSql(string.Join(" ", this.cmd.SqlString), this.parameters);
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
        public bool Exists<T>([CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0)
        {
            this._prepare(filePath, methodName, lineNumber);
            try
            {
                if (!this.cmd.IsNative)
                {
                    // 查询准备
                    this._query<T>();
                    // 构造命令
                    this.createSql();
                }

                parameters = this.cmd.Params.ToArray();

                return DbHelperSQL.Exists(string.Join(" ", this.cmd.SqlString), this.parameters);
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
        public Int64 Count<T>([CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0)
        {
            this._prepare(filePath, methodName, lineNumber);
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

                parameters = this.cmd.Params.ToArray();

                object n = DbHelperSQL.GetSingle(string.Join(" ", this.cmd.SqlString), this.parameters);

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
        public object Single<T>([CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0)
        {
            this._prepare(filePath, methodName, lineNumber);
            try
            {
                if (!this.cmd.IsNative)
                {
                    // 查询准备
                    this._query<T>();
                    // 构造命令
                    this.createSql();
                }

                parameters = this.cmd.Params.ToArray();

                return DbHelperSQL.GetSingle(string.Join(" ", this.cmd.SqlString), this.parameters);
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
        public int Exec([CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0)
        {
            this._prepare(filePath, methodName, lineNumber);
            try
            {
                if (this.cmd.IsNative)
                {
                    parameters = this.cmd.Params.ToArray();

                    return DbHelperSQL.ExecuteSql(string.Join(" ", this.cmd.SqlString), this.parameters);
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

        /// <summary>
        /// 执行指定的存储过程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procedureName">存储过程名</param>
        /// <param name="model"></param>
        /// <param name="outputs">参数</param>
        /// <returns></returns>
        public Result Procedure<T>(string procedureName, T model, [CallerFilePath]string filePath = "", [CallerMemberName]string methodName = "", [CallerLineNumber]int lineNumber = 0, params Direction[] direction)
        {
            this._prepare(filePath, methodName, lineNumber);
            // 
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                result.status = -1;
                result.data = new Exception("存储过程名缺失");
            }
            else
            {
                try
                {
                    if (!this.cmd.IsNative)
                    {
                        // 将输入输出参数数组转为字典
                        Dictionary<string, ParameterDirection> d_param = new Dictionary<string, ParameterDirection>();

                        Array.ForEach(direction, d => d_param.Add(d.field.ToLower(), d.direction));

                        // 获取对象的类型
                        Type m_type = typeof(T);

                        // 获取该对象的全部属性
                        PropertyInfo[] properties = m_type.GetProperties();

                        // 如果没有指定更新列，则更新传入参数对象的全部列
                        if (cmd.Fields.Count == 0)
                        {
                            for (int i = 0; i < properties.Length; i++)
                            {
                                // 忽略排除字段
                                if (cmd.ExcludeFields.Count > 0 && cmd.ExcludeFields.Contains(properties[i].Name.ToLower()))
                                {
                                    continue;
                                }

                                cmd.Fields.Add(properties[i].Name.ToLower());

                                SqlParameter param = new SqlParameter("@" + properties[i].Name.ToLower(), properties[i].GetValue(model));

                                // 参数方向
                                ParameterDirection dir;
                                if (d_param.TryGetValue(properties[i].Name.ToLower(), out dir))
                                {
                                    param.Direction = dir;
                                }

                                cmd.Params.Add(param);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < properties.Length; i++)
                            {
                                if (cmd.Fields.Contains(properties[i].Name.ToLower()))
                                {
                                    SqlParameter param = new SqlParameter("@" + properties[i].Name.ToLower(), properties[i].GetValue(model));

                                    // 参数方向
                                    ParameterDirection dir;
                                    if (d_param.TryGetValue(properties[i].Name.ToLower(), out dir))
                                    {
                                        param.Direction = dir;
                                    }

                                    cmd.Params.Add(param);
                                }
                            }
                        }
                    }
                    int rowsAffected = 0;

                    parameters = this.cmd.Params.ToArray();

                    int n = DbHelperSQL.RunProcedure(procedureName, parameters, out rowsAffected);
                    if (n != 0)
                    {
                        result.status = -1;
                        result.data = n;
                    }
                }
                catch (Exception e)
                {
                    result.status = -1;
                    result.data = e;
                }
                finally
                {
                    this._finish();
                }
            }
            return result;
        }

        #endregion


        #region 私有方法
        /// <summary>
        /// 预处理
        /// </summary>
        private void _prepare(string filePath, string methodName, int lineNumber)
        {
            result = new Result();

            cacheKay = MD5Encrypt.Get32(filePath + "-" + methodName + "-" + lineNumber);

            result.data = filePath + "-" + methodName + "-" + lineNumber;
        }
        /// <summary>
        /// 结束处理
        /// </summary>
        private void _finish()
        {
            this.cmd = new Sql();
        }

        private void _getTableName(Type type)
        {

            if (string.IsNullOrWhiteSpace(cmd.Table))
            {
                TableAttribute t_attr = type.GetCustomAttribute(typeof(TableAttribute), false) as TableAttribute;
                if (t_attr == null)
                {
                    cmd.Table = _getName(type.Name);
                }
                else
                {
                    cmd.Table = _getName(t_attr.Name);
                }
            }
        }
        private string _getName(string str) {
            return "[" + str + "]";
        }
        private void _query<T>(T model = default(T))
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
                    // 忽略排除字段
                    if (cmd.ExcludeFields.Count > 0 && cmd.ExcludeFields.Contains(properties[i].Name.ToLower()))
                    {
                        continue;
                    }
                    cmd.Fields.Add(_getName(properties[i].Name.ToLower()));
                    //
                    //properties[i].GetValue(model);
                }
            }
        }

        /// <summary>
        /// 获取一个记录集
        /// </summary>
        /// <returns></returns>
        private DataSet _dataSet()
        {
            parameters = this.cmd.Params.ToArray();

            return DbHelperSQL.Query(string.Join(" ", this.cmd.SqlString), parameters);
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
            else
            {
                model = default(T);
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

                    parameters = this.cmd.Params.ToArray();

                    object n = DbHelperSQL.GetSingle(_countSql, this.parameters);

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
                if (cmd.Fields.Count == 0)
                {
                    cmd.SqlString.Add(" *");
                }
                else
                {
                    cmd.SqlString.Add(string.Format(" {0}", string.Join(",", cmd.Fields)));
                }

                //
                cmd.SqlString.Add(" from ( select row_number() over (");
                // Order
                if (cmd.OrderBy.Count == 0)
                {
                    cmd.SqlString.Add(" order by T.id desc");
                }
                else
                {
                    cmd.SqlString.Add(string.Format(" order by T.{0}", string.Join(",T.", cmd.OrderBy.ToArray())));
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
                if (cmd.Fields.Count == 0)
                {
                    cmd.SqlString.Add(" *");
                }
                else
                {
                    cmd.SqlString.Add(string.Format(" {0}", string.Join(",", cmd.Fields)));
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
                    // 构造查询条件 Where子句
                    if (cmd.Where.Count == 0)
                    {
                        // 如果不存在where条件,已当前model的非初始字段为条件
                    }
                    else
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
            List<string> _param = new List<string>();
            for (int i = 0; i < cmd.Params.Count; i++)
            {
                // 忽略output参数
                if (cmd.Params[i].Direction == ParameterDirection.Output)
                {
                    continue;
                }
                _param.Add(cmd.Params[i].ParameterName);
            }

            cmd.SqlString.Add(string.Join(",", _param));

            cmd.SqlString.Add(")");
            // 返回自增id
            cmd.SqlString.Add(";select @id = SCOPE_IDENTITY()");
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
