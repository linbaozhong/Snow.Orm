﻿using Snow.DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Snow
{
    public partial class Orm : NativeDb
    {
        #region 公共方法
        /// <summary>
        /// 获取单个数据对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private DataSet _Get(BaseEntity entity)
        {
            return _Find(entity,1);
        }

        /// <summary>
        /// 获取数据对象列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private DataSet _Find(BaseEntity entity,int top = 0)
        {
            DataSet ds = null;
            try
            {
                if (!cmd.IsNative)
                {
                    if (!cmd.IsKey && top > 0)
                    {
                        // 读取一条记录
                        cmd.Top = top;
                    }
                    // 查询准备
                    query(entity);
                    // 构造命令
                    createSql();
                }
                // 调试
                trace(getSql());
                // 读取记录集
                ds = getDataSet();
            }
            catch (Exception e)
            {
                trace(e.Message);
                throw e;
            }
            finally
            {
            }
            return ds;
        }

        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <returns>返回自增键值</returns>
        public object Insert(BaseEntity entity)
        {
            prepare(entity);
            try
            {
                if (!cmd.IsNative)
                {
                    // 插入
                    cmd.Command = Command.Insert;

                    // 如果没有指定更新列，则更新传入参数对象的全部列
                    if (cmd.Fields.Count == 0)
                    {
                        // 主键
                        HashSet<string> primary = new HashSet<string>(entity.Table.PrimaryKey.Key.Split(','));
                        var isGenerate = entity.Table.PrimaryKey.Type;

                        // 字段名
                        string _fieldname = "";

                        foreach (DictionaryEntry field in entity)
                        {
                            _fieldname = field.Key.ToString().ToLower();
                            // 忽略排除字段和数据库计算字段
                            if ((cmd.ExcludeFields.Count > 0 && cmd.ExcludeFields.Contains(_fieldname))
                                || (isGenerate && primary.Contains(_fieldname)))
                            {
                                continue;
                            }

                            cmd.Fields.Add(_fieldname);
                            cmd.Params.Add(new SqlParameter("@" + _fieldname, field.Value));
                        }

                        // 设置返回参数
                        SqlParameter idParameter = new SqlParameter("@id", "0");
                        idParameter.Direction = ParameterDirection.Output;

                        cmd.Params.Add(idParameter);

                    }
                    createSql();
                }

                parameters = cmd.Params.ToArray();

                // 返回数据库自增关键字段值
                DbHelperSQL.ExecuteSql(string.Join(" ", cmd.SqlString), parameters);

                object returnVal = null;
                foreach (SqlParameter q in parameters)
                {
                    if (q.Direction == ParameterDirection.Output)
                    {
                        returnVal = q.Value;
                        break;
                    }
                }
                return returnVal;
            }
            catch (Exception e)
            {
                trace(e.Message);
                throw e;
            }
            finally
            {
                // 调试
                trace(getSql());
                finish();
            }
        }

        public bool Insert<T>(List<T> list) where T : class, new()
        {
            try
            {
                if (cmd.IsNative)
                {
                    createSql();

                    parameters = cmd.Params.ToArray();

                    return DbHelperSQL.ExecuteSql(string.Join(" ", cmd.SqlString), parameters) > 0;
                }
                else
                {
                    // 命令列表
                    List<CommandInfo> l_command = new List<CommandInfo>();
                    // 插入
                    cmd.Command = Command.Insert;

                    // 如果没有指定返回列，则返回传入参数对象的全部列
                    if (cmd.Fields.Count == 0)
                    {
                        BaseEntity _model;
                        // 主键
                        var primary = new HashSet<string>();
                        // 是否由数据库生成
                        var isGenerate = false;

                        foreach (var model in list)
                        {
                            _model = model as BaseEntity;
                            // 表名
                            cmd.TableName = _model.Table.Name;

                            primary = new HashSet<string>(_model.Table.PrimaryKey.Key.Split(','));
                            isGenerate = _model.Table.PrimaryKey.Type;

                            string _fieldname;
                            foreach (DictionaryEntry field in _model)
                            {
                                _fieldname = field.Key.ToString().ToLower();
                                // 忽略排除字段
                                if ((cmd.ExcludeFields.Count > 0 && cmd.ExcludeFields.Contains(_fieldname))
                                || (isGenerate && primary.Contains(_fieldname)))
                                {
                                    continue;
                                }
                                cmd.Fields.Add(_fieldname);

                                cmd.Params.Add(new SqlParameter("@" + _fieldname, field.Value));

                            }

                            //SqlParameter idParameter = new SqlParameter("@id", 0);
                            //idParameter.Direction = ParameterDirection.Output;

                            //cmd.Params.Add(idParameter);

                            createSql();

                            parameters = cmd.Params.ToArray();

                            l_command.Add(new CommandInfo(string.Join(" ", cmd.SqlString), parameters));

                            // 调试
                            trace(getSql());

                            cmd.Fields.Clear();
                            cmd.Params.Clear();
                        }
                    }

                    return DbHelperSQL.ExecuteSqlTran(l_command) > 0;

                }
            }
            catch (Exception e)
            {
                trace(e.Message);
                throw e;
            }
            finally
            {
                finish();
            }
        }
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(BaseEntity entity)
        {
            prepare(entity);
            try
            {
                if (!cmd.IsNative)
                {
                    // 更新
                    cmd.Command = Command.Update;
                    // 主键
                    HashSet<string> primary = new HashSet<string>(entity.Table.PrimaryKey.Key.Split(','));
                    var isGenerate = entity.Table.PrimaryKey.Type;

                    // 字段名
                    string _fieldname = "";

                    // 如果没有指定更新列，则更新传入参数对象的全部列
                    if (cmd.Fields.Count == 0)
                    {
                        foreach (DictionaryEntry field in entity)
                        {
                            _fieldname = field.Key.ToString().ToLower();
                            // 忽略排除字段和数据库计算字段
                            if ((cmd.ExcludeFields.Count > 0 && cmd.ExcludeFields.Contains(_fieldname))
                                || (isGenerate && primary.Contains(_fieldname)))
                            {
                                continue;
                            }

                            cmd.Fields.Add(_fieldname);
                            cmd.Params.Add(new SqlParameter("@" + _fieldname, field.Value));
                        }
                    }
                    else
                    {
                        foreach (DictionaryEntry field in entity)
                        {
                            _fieldname = field.Key.ToString().ToLower();
                            if (cmd.Fields.Contains(_fieldname))
                            {
                                cmd.Params.Add(new SqlParameter("@" + _fieldname, field.Value));
                            }
                        }
                    }
                    createSql();
                }

                parameters = cmd.Params.ToArray();

                int n = DbHelperSQL.ExecuteSql(string.Join(" ", cmd.SqlString), parameters);

                if (n > 0)
                {
                    DataCache.Remove(getCacheKey());
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error(this.GetType().Name, e.Message);
                throw e;
            }
            finally
            {
                // 调试
                trace(getSql());
                finish();
            }
        }
        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Delete(BaseEntity entity)
        {
            prepare(entity);
            try
            {
                if (!cmd.IsNative)
                {
                    // 删除
                    cmd.Command = Command.Delete;
                    createSql();
                }

                parameters = cmd.Params.ToArray();

                int n = DbHelperSQL.ExecuteSql(string.Join(" ", cmd.SqlString), parameters);

                if (n > 0)
                {
                    DataCache.Remove(getCacheKey());
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                trace(e.Message);
                throw e;
            }
            finally
            {
                // 调试
                trace(getSql());
                finish();
            }
        }
        /// <summary>
        /// 是否存在符合条件的记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Exists(BaseEntity entity)
        {
            prepare(entity);
            try
            {
                if (!cmd.IsNative)
                {
                    // 查询准备
                    query(entity);
                    // 构造命令
                    createSql();
                }

                parameters = cmd.Params.ToArray();

                return DbHelperSQL.Exists(string.Join(" ", cmd.SqlString), parameters);
            }
            catch (Exception e)
            {
                trace(e.Message);
                throw e;
            }
            finally
            {
                // 调试
                trace(getSql());

                finish();
            }
        }
        /// <summary>
        /// 统计符合条件的记录数
        /// </summary>
        /// <returns></returns>
        public Int64 Count(BaseEntity entity)
        {
            prepare(entity);

            try
            {
                if (!cmd.IsNative)
                {
                    cmd.Fields.Add("@count(1)");
                    // 查询准备
                    query(entity);
                    // 构造命令
                    createSql();
                }

                parameters = cmd.Params.ToArray();

                object n = DbHelperSQL.GetSingle(string.Join(" ", cmd.SqlString), parameters);

                return Convert.ToInt64(n);
            }
            catch (Exception e)
            {
                trace(e.Message);
                throw e;
            }
            finally
            {
                // 调试
                trace(getSql());
                finish();
            }
        }
        //public object Single<T>(BaseEntity model = null) where T : class, new()
        /// <summary>
        /// 执行查询，返回首行首列结果
        /// </summary>
        /// <returns></returns>
        public object Single(BaseEntity entity)
        {
            //if (model == null)
            //    model = new T() as BaseEntity;

            prepare(entity);

            try
            {
                if (!cmd.IsNative)
                {
                    // 查询准备
                    query(entity);
                    // 构造命令
                    createSql();
                }

                parameters = cmd.Params.ToArray();

                return DbHelperSQL.GetSingle(string.Join(" ", cmd.SqlString), parameters);
            }
            catch (Exception e)
            {
                trace(e.Message);
                throw e;
            }
            finally
            {
                // 调试
                trace(getSql());
                finish();
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
                if (cmd.IsNative)
                {
                    parameters = cmd.Params.ToArray();

                    return DbHelperSQL.ExecuteSql(string.Join(" ", cmd.SqlString), parameters);
                }
                else
                {
                    throw new Exception("原生查询方法缺乏原生查询命令");
                }
            }
            catch (Exception e)
            {
                trace(e.Message);
                throw e;
            }
            finally
            {
                // 调试
                trace(getSql());
                finish();
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
        public int Procedure(string procedureName,BaseEntity entity, params Direction[] direction)
        {
            prepare(entity);
            // 
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                trace("存储过程名缺失");
                throw new Exception("存储过程名缺失");
            }
            else
            {
                try
                {
                    if (!cmd.IsNative)
                    {
                        // 将输入输出参数数组转为字典
                        Dictionary<string, ParameterDirection> d_param = new Dictionary<string, ParameterDirection>();

                        Array.ForEach(direction, d => d_param.Add(d.field.ToLower(), d.direction));

                        // 主键
                        HashSet<string> primary = new HashSet<string>(entity.Table.PrimaryKey.Key.Split(','));
                        var isGenerate = entity.Table.PrimaryKey.Type;

                        // 字段名
                        string _fieldname = "";

                        // 如果没有指定更新列，则更新传入参数对象的全部列
                        if (cmd.Fields.Count == 0)
                        {
                            foreach (DictionaryEntry field in entity)
                            {
                                _fieldname = field.Key.ToString().ToLower();
                                // 忽略排除字段和数据库计算字段
                                if ((cmd.ExcludeFields.Count > 0 && cmd.ExcludeFields.Contains(_fieldname))
                                    || (isGenerate && primary.Contains(_fieldname)))
                                {
                                    continue;
                                }

                                cmd.Fields.Add(_fieldname);

                                SqlParameter param = new SqlParameter("@" + _fieldname, field.Value);
                                // 参数方向
                                ParameterDirection dir;
                                if (d_param.TryGetValue(_fieldname, out dir))
                                {
                                    param.Direction = dir;
                                }

                                cmd.Params.Add(param);

                            }
                        }
                        else
                        {
                            foreach (DictionaryEntry field in entity)
                            {
                                _fieldname = field.Key.ToString().ToLower();
                                // 
                                if (cmd.Fields.Contains(_fieldname))
                                {
                                    SqlParameter param = new SqlParameter("@" + _fieldname, field.Value);
                                    // 参数方向
                                    ParameterDirection dir;
                                    if (d_param.TryGetValue(_fieldname, out dir))
                                    {
                                        param.Direction = dir;
                                    }

                                    cmd.Params.Add(param);
                                }
                            }
                        }
                    }
                    int rowsAffected = 0;

                    parameters = cmd.Params.ToArray();

                    return DbHelperSQL.RunProcedure(procedureName, parameters, out rowsAffected);
                }
                catch (Exception e)
                {
                    trace(e.Message);
                    throw e;
                }
                finally
                {
                    finish();
                }
            }
        }

        #endregion


        #region 私有方法

        /// <summary>
        /// 结束处理
        /// </summary>
        private void finish()
        {
            cmd = new Snow.Sql();
            parameters = null;
        }

        /// <summary>
        /// 预处理
        /// </summary>
        private void prepare(BaseEntity entity)
        {
            // 读取表名
            setTableName(entity);
            // 主键查询
            if (cmd.IsKey
                || entity.Table.PrimaryKey.Key == null
                || entity[entity.Table.PrimaryKey.Key] == null
                || entity[entity.Table.PrimaryKey.Key].ToString() == ""
                || entity[entity.Table.PrimaryKey.Key].ToString() == "0")
            {
                return;
            }

            Id(entity.Table.PrimaryKey.Key, entity[entity.Table.PrimaryKey.Key]);
        }

        private void setTableName(BaseEntity entity)
        {
            if (string.IsNullOrWhiteSpace(cmd.TableName))
            {
                cmd.TableName = entity.Table.Name;
            }
        }

        //private Table[] prepare(List<BaseEntity> model)
        //{
        //    prepare();

        //    string _guid = model.GetType().GUID.ToString();

        //    // 读取cache
        //    object _obj = DataCache.Get(_guid);

        //    if (_obj == null)
        //    {
        //        int len = model.Count;
        //        Table[] _tables = new Table[len];

        //        for (int i = 0; i < len; i++)
        //        {
        //            _tables[i] = getTable(model[i]);
        //        }

        //        _obj = _tables;

        //        DataCache.Set(_guid, _obj);
        //    }

        //    return _obj as Table[];
        //}
        /// <summary>
        /// 获取实体映射表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //private Table getTable(BaseEntity model)
        //{
        //    string _guid = model.GetType().GUID.ToString();

        //    // 读取cache
        //    object _obj = DataCache.Get(_guid);

        //    if (_obj == null)
        //    {
        //        _obj = getRuntimeEntity(model);

        //        DataCache.Set(_guid, _obj);
        //    }
        //    return _obj as Table;
        //}
        /// <summary>
        /// 获取运行时实体
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //Table getRuntimeEntity(BaseEntity model)
        //{
        //    Table _table = new Snow.Table();

        //    _table.Name = model.TableName;

        //    string _key = "";

        //    foreach (DictionaryEntry field in model)
        //    {
        //        _key = field.Key.ToString().ToLower();

        //        _table[_key] = new Column(
        //            _key,
        //            field.GetType()
        //            );
        //    }
        //    return _table;
        //}
        ///// <summary>
        ///// 读取映射的表名
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //string getTableName(Type type)
        //{
        //    if (string.IsNullOrWhiteSpace(cmd.TableName))
        //    {
        //        TableAttribute t_attr = type.GetCustomAttribute(typeof(TableAttribute), false) as TableAttribute;
        //        if (t_attr == null)
        //        {
        //            cmd.TableName = getName(type.Name);
        //        }
        //        else
        //        {
        //            cmd.TableName = getName(t_attr.Name);
        //        }
        //    }

        //    return cmd.TableName;
        //}
        /// <summary>
        /// 读取全部映射字段
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string getName(string str)
        {
            if (str.StartsWith("@"))
            {
                return str.Substring(1);
            }
            return "[" + str + "]";
        }

        private void query(BaseEntity entity)
        {
            cmd.Command = Command.Select;

            // 如果没有指定返回列，则返回传入参数对象的全部列
            if (cmd.Fields.Count == 0)
            {
                // 主键
                HashSet<string> primary = new HashSet<string>(entity.Table.PrimaryKey.Key.Split(','));

                string _fieldname = "";

                foreach (DictionaryEntry field in entity)
                {
                    _fieldname = field.Key.ToString();
                    // 忽略排除字段
                    if (cmd.ExcludeFields.Count > 0 && cmd.ExcludeFields.Contains(_fieldname)
                         || primary.Contains(_fieldname))
                    {
                        continue;
                    }
                    cmd.Fields.Add(field.Key.ToString());

                }
            }
        }

        /// <summary>
        /// 获取一个记录集
        /// </summary>
        /// <returns></returns>
        private DataSet getDataSet()
        {
            parameters = cmd.Params.ToArray();

            return DbHelperSQL.Query(string.Join(" ", cmd.SqlString), parameters);
        }

        /// <summary>
        /// 构造select命令
        /// </summary>
        private void select()
        {
            // 命令
            cmd.SqlString.Clear();
            cmd.SqlString.Add(cmd.Command);

            // Page 分页查询
            if (cmd.IsPage)
            {
                //// 符合条件的记录总数
                //try
                //{
                //    string countSql = string.Format("{0} count(1) from {1}", cmd.Command, getName(cmd.TableName));

                //    if (cmd.Where.Count > 0)
                //    {
                //        countSql += string.Format(" where {0}",string.Join(" ",cmd.Where));
                //    }

                //    // 调试
                //    //trace(countSql);

                //    parameters = cmd.Params.ToArray();

                //    object n = DbHelperSQL.GetSingle(countSql, parameters);

                //    cmd.Page.rowsCount = Convert.ToInt64(n);

                //    if (cmd.Page.rowsCount > 0)
                //    {
                //        // 总页数
                //        cmd.Page.pages = (int)Math.Ceiling(cmd.Page.rowsCount * 1.0 / cmd.Page.pageSize);
                //    }
                //}
                //catch (Exception e)
                //{
                //    trace(e.Message);
                //    throw e;
                //}

                // 
                cmd.Page.endIndex = cmd.Page.pageIndex * cmd.Page.pageSize;
                cmd.Page.startIndex = cmd.Page.endIndex - cmd.Page.pageSize + 1;

                // Fields
                if (cmd.Fields.Count == 0)
                {
                    cmd.SqlString.Add(" *");
                }
                else
                {
                    cmd.SqlString.Add(string.Format(" {0}", string.Join(",", cmd.Fields.Select(x => getName(x)))));
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
                cmd.SqlString.Add(string.Format(")as Row,T.* from {0} T", getName(cmd.TableName)));

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
                    cmd.SqlString.Add(string.Format(" {0}", string.Join(",", cmd.Fields.Select(x => getName(x)))));
                }
                // From
                cmd.SqlString.Add(string.Format(" from {0}", getName(cmd.TableName)));

                // Join
                if (cmd.Join.Count > 0)
                {
                    cmd.SqlString.Add(string.Join(" ", cmd.Join.ToArray()));
                }

                // 如果是主键查询
                if (cmd.IsKey)
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
        private void insert()
        {
            // 命令
            cmd.SqlString.Clear();
            cmd.SqlString.Add(cmd.Command);
            cmd.SqlString.Add(getName(cmd.TableName));
            // Fields
            if (cmd.Fields.Count > 0)
            {
                cmd.SqlString.Add("(");
                cmd.SqlString.Add(string.Format(" {0}", string.Join(",", cmd.Fields.Select(x => getName(x)))));
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
        private void update()
        {
            // 命令
            cmd.SqlString.Clear();
            cmd.SqlString.Add(cmd.Command);
            cmd.SqlString.Add(getName(cmd.TableName));
            cmd.SqlString.Add("set");

            // Fields
            int _count = cmd.Fields.Count;
            if (_count > 0)
            {
                string[] _set = new string[_count];

                for (int i = 0; i < _count; i++)
                {
                    _set[i] = getName(cmd.Fields[i]) + "=@" + cmd.Fields[i].ToLower();
                }
                cmd.SqlString.Add(string.Join(",", _set));
            }
            // Where
            if (cmd.IsKey)
            {
                cmd.SqlString.Add("where " + cmd.Id);
            }
            else
            {
                if (cmd.Where.Count > 0)
                {
                    cmd.SqlString.Add(string.Format("where {0}", string.Join(" ", cmd.Where)));
                }
            }

            //
            //_sqlStr = string.Join(" ", _cmd);
        }
        /// <summary>
        /// 构造delete命令
        /// </summary>
        private void delete()
        {
            // 命令
            cmd.SqlString.Clear();
            cmd.SqlString.Add(cmd.Command);
            // From
            cmd.SqlString.Add(cmd.TableName);
            // Where
            if (cmd.IsKey)
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
        }

        #endregion
    }

}
