using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Snow.Orm
{
    public class Db
    {
        Sql cmd;
        /// <summary>
        /// 返回的结果
        /// </summary>
        Result result = new Result();

        /// <summary>
        /// 以id为条件，忽略其他条件和排序
        /// </summary>
        bool _idCondition = false;
        /// <summary>
        ///  sql命令
        /// </summary>
        string _sql = string.Empty;

        public Db()
        {
            this.cmd = new Sql();
        }

        public Db Id(string id, params object[] args)
        {
            this._idCondition = true;

            cmd.Id = string.Format("{0} = @{0}", id);
            // 
            cmd.Params.Add(new SqlParameter("@" + id, args[0]));

            return this;
        }

        public Db Where(string condition, params object[] args)
        {
            parameter(condition, args);
            return this;
        }
        public Db And(string condition, params object[] args)
        {
            cmd.Where.Add(" and ");
            parameter(condition, args);
            return this;
        }
        public Db Or(string condition, params object[] args)
        {
            cmd.Where.Add(" or ");
            parameter(condition, args);
            return this;
        }
        public Db OrderBy(string asc)
        {
            cmd.OrderBy.Add(asc);
            return this;
        }
        public Db Desc(string desc)
        {
            cmd.OrderBy.Add(desc + " desc");
            return this;
        }
        public Db Asc(string asc)
        {
            cmd.OrderBy.Add(asc + " asc");
            return this;
        }       /// <summary>
        /// 返回或者更新的列
        /// </summary>
        /// <param name="cols">以逗号分隔的列名字符串</param>
        /// <returns></returns>
        public Db Cols(string cols)
        {
            cmd.Fields.Add(cols);
            return this;
        }

        public Result Get<T>(T model)
        {
            try
            {
                // 读取一条记录
                cmd.Command = Command.Select;
                cmd.Top = 1;

                // 获取对象的类型
                Type m_type = model.GetType();
                // 表名
                cmd.Table = m_type.Name;

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
                this.Sql();
            }
            catch (Exception e)
            {
                result.Ok = false;
                result.Data = e;
            }
            return result;
        }

        public Result Find<T>(T model)
        {
            try
            {

            }
            catch (Exception e)
            {
                result.Ok = false;
                result.Data = e;
            }
            return result;
        }

        public Result Insert<T>(T model)
        {
            try
            {
                // 插入
                cmd.Command = Command.Insert;
                // 获取对象的类型
                Type m_type = model.GetType();
                // 表名
                cmd.Table = m_type.Name;

                // 如果没有指定返回列，则返回传入参数对象的全部列
                if (cmd.Fields.Count == 0)
                {
                    // 获取该对象的全部属性
                    PropertyInfo[] properties = m_type.GetProperties();

                    for (int i = 0; i < properties.Length; i++)
                    {
                        cmd.Fields.Add(properties[i].Name);

                        cmd.Params.Add(new SqlParameter("@" + properties[i].Name, properties[i].GetValue(model)));
                    }
                }
                this.Sql();
            }
            catch (Exception e)
            {
                result.Ok = false;
                result.Data = e;
            }
            return result;
        }

        public Result Update<T>(T model)
        {
            try
            {

                // 更新
                cmd.Command = Command.Update;
                // 获取对象的类型
                Type m_type = model.GetType();
                // 表名
                cmd.Table = m_type.Name;

                // 如果没有指定返回列，则返回传入参数对象的全部列
                if (cmd.Fields.Count == 0)
                {
                    // 获取该对象的全部属性
                    PropertyInfo[] properties = m_type.GetProperties();

                    for (int i = 0; i < properties.Length; i++)
                    {
                        cmd.Fields.Add(properties[i].Name);

                        cmd.Params.Add(new SqlParameter("@" + properties[i].Name, properties[i].GetValue(model)));
                    }
                }
                this.Sql();
            }
            catch (Exception e)
            {
                result.Ok = false;
                result.Data = e;
            }
            return result;

        }

        public Result Delete<T>(T model)
        {
            try
            {
                // 删除
                cmd.Command = Command.Delete;
                // 获取对象的类型
                Type m_type = model.GetType();
                // 表名
                cmd.Table = m_type.Name;

                this.Sql();
            }
            catch (Exception e)
            {
                result.Ok = false;
                result.Data = e;
            }
            return result;
        }
        /// <summary>
        /// 获取最终sql命令
        /// </summary>
        /// <returns></returns>
        public string GetSql()
        {
            string[] _param = new string[cmd.Params.Count];
            for (int i = 0; i < cmd.Params.Count; i++)
            {
                _param[i] = cmd.Params[i].ParameterName + "=" + cmd.Params[i].Value;
            }
            return this._sql + ";" + string.Join(",", _param);
        }

        /// <summary>
        /// 构造sql命令
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public Db Sql(string str = "")
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                switch (cmd.Command.ToLower())
                {
                    case "insert":
                        this._insert();
                        break;
                    case "update":
                        this._update();
                        break;
                    case "delete":
                        this._delete();
                        break;
                    case "select":
                        this._select();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                this._sql = str;
            }
            return this;
        }

        #region 私有方法

        /// <summary>
        /// 构造查询参数
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="args"></param>
        private void parameter(string condition, params object[] args)
        {
            if (args.Length > 0)
            {
                int _start = 0, _next = 0;
                string _paramName = "@param_", _count = "";
                for (int i = 0; i < args.Length; i++)
                {
                    _count = cmd.Params.Count.ToString();
                    _next = condition.IndexOf("?", _start);
                    cmd.Where.Add(condition.Substring(_start, _next - _start) + _paramName + _count);
                    cmd.Params.Add(new SqlParameter(_paramName + _count, args[i]));
                    _start = _next + 1;
                }
            }
            else
            {
                cmd.Where.Add(condition);
            }
        }
        /// <summary>
        /// 构造Select命令
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
            // Where
            if (_idCondition)
            {
                _cmd.Add(" where " + cmd.Id);
            }
            else
            {
                if (cmd.Where.Count > 0)
                {
                    _cmd.Add(string.Format(" where {0}", string.Join(" ", cmd.Where.ToArray())));
                }
            }
            // Order
            if (!_idCondition && cmd.OrderBy.Count > 0)
            {
                _cmd.Add(string.Format(" order by {0}", string.Join(",", cmd.OrderBy.ToArray())));
            }

            //
            this._sql = string.Join("", _cmd);
        }

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
            this._sql = string.Join(" ", _cmd);
        }

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
            this._sql = string.Join(" ", _cmd);
        }

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
            this._sql = string.Join(" ", _cmd);
        }

        #endregion
    }
}
