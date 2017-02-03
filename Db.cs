using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Snow
{
    public partial class Orm : NativeDb
    {
        #region 属性

        Sql cmd;
        /// <summary>
        /// 查询参数
        /// </summary>
        SqlParameter[] parameters;

        #endregion

        #region 公共方法

        internal Orm()
        {
            init();
        }
        private void init()
        {
            cmd = new Snow.Sql();
            parameters = null;
        }

        /// <summary>
        /// 主键查询(不支持复合主键)
        /// </summary>
        /// <param name="key">主键字段名</param>
        /// <param name="args">参数值</param>
        /// <returns></returns>
        public Orm Id(string key, object arg)
        {
            //缓存键
            cmd.CacheKey.Add(arg.ToString());

            cmd.IsKey = true;

            if (string.IsNullOrWhiteSpace(cmd.Id))
            {
                cmd.Id = string.Format("[{0}] = @{0}", key);
            }
            else
            {
                cmd.Id += string.Format(" and [{0}] = @{0}", key);
            }
            // 
            cmd.Params.Add(new SqlParameter("@" + key, arg));

            return this;
        }
        /// <summary>
        /// 适用于联合主键
        /// </summary>
        /// <param name="key">逗号分隔的关键字段字符串（"key1,key2"）</param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Orm Id(string key, params object[] args)
        {
            string[] keys = key.Split(',');

            int len = System.Math.Min(keys.Length, args.Length);

            for (int i = 0; i < len; i++)
            {
                Id(keys[i], args[i]);
            }
            return this;
        }
        /// <summary>
        /// 返回前n个记录
        /// </summary>
        /// <param name="n">前n个记录</param>
        /// <returns></returns>
        public Orm Top(int n)
        {
            if (n <= 1)
            {
                n = 1;
            }
            cmd.Top = n;
            return this;
        }
        /// <summary>
        /// 内联结
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="on">联结条件</param>
        /// <returns></returns>
        public Orm Inner(string table, string on)
        {
            cmd.Join.Add(string.Format(" inner join [{0}] on {1}", table, on));
            return this;
        }
        /// <summary>
        /// 左联结
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="on">联结条件</param>
        /// <returns></returns>
        public Orm Left(string table, string on)
        {
            cmd.Join.Add(string.Format(" left outer join [{0}] on {1}", table, on));
            return this;
        }
        /// <summary>
        /// 右联结
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="on">联结条件</param>
        /// <returns></returns>
        public Orm Right(string table, string on)
        {
            cmd.Join.Add(string.Format(" right outer join [{0}] on {1}", table, on));
            return this;
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="condition">查询条件,例如: name = ? </param>
        /// <param name="args">参数值</param>
        /// <returns></returns>
        public Orm Where(string condition, params object[] args)
        {
            if (cmd.Where.Count > 0)
            {
                cmd.Where.Add(" and ");
            }
            parameter(condition, args);
            return this;
        }
        /// <summary>
        /// And 条件
        /// </summary>
        /// <param name="condition">查询条件,例如: name = ? </param>
        /// <param name="args">参数值</param>
        /// <returns></returns>
        public Orm And(string condition, params object[] args)
        {
            if (cmd.Where.Count > 0)
            {
                cmd.Where.Add(" and ");
            }
            parameter(condition, args);
            return this;
        }
        /// <summary>
        /// Or 条件
        /// </summary>
        /// <param name="condition">查询条件,例如: name = ? </param>
        /// <param name="args">参数值</param>
        /// <returns></returns>
        public Orm Or(string condition, params object[] args)
        {
            if (cmd.Where.Count > 0)
            {
                cmd.Where.Add(" or ");
            }
            parameter(condition, args);
            return this;
        }
        /// <summary>
        /// 模糊查询 like
        /// </summary>
        /// <param name="field"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Orm Like(string field, string arg)
        {
            if (cmd.Where.Count > 0)
            {
                cmd.Where.Add(" and ");
            }
            cmd.Where.Add(string.Format(" [{0}] like '%{1}%'", field, arg));
            return this;
        }
        /// <summary>
        /// 前缀查询(索引优化)
        /// </summary>
        /// <param name="field">字段名</param>
        /// <param name="arg">参数</param>
        /// <returns></returns>
        public Orm StartsWith(string field, string arg)
        {
            if (cmd.Where.Count > 0)
            {
                cmd.Where.Add(" and ");
            }
            cmd.Where.Add(string.Format(" [{0}] like '{1}%'", field, arg));
            return this;
        }
        /// <summary>
        /// in 查询
        /// </summary>
        /// <param name="field">字段名</param>
        /// <param name="args">参数值</param>
        /// <returns></returns>
        public Orm In(string field, params object[] args)
        {
            if (args.Length > 0)
            {
                if (cmd.Where.Count > 0)
                {
                    cmd.Where.Add(" and ");
                }
                cmd.Where.Add(string.Format("[{0}] in (", field));
                cmd.Where.Add(string.Join(",", args));
                cmd.Where.Add(")");
            }
            return this;
        }
        /// <summary>
        /// not in 查询
        /// </summary>
        /// <param name="field">字段名</param>
        /// <param name="args">参数值</param>
        /// <returns></returns>
        public Orm NotIn(string field, params object[] args)
        {
            if (args.Length > 0)
            {
                if (cmd.Where.Count > 0)
                {
                    cmd.Where.Add(" and ");
                }
                cmd.Where.Add(string.Format("[{0}] not in (", field));
                cmd.Where.Add(string.Join(",", args));
                cmd.Where.Add(")");
            }
            return this;
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public Orm OrderBy(string asc)
        {
            cmd.OrderBy.Add(asc);
            return this;
        }
        /// <summary>
        /// 倒序排序
        /// </summary>
        /// <param name="desc"></param>
        /// <returns></returns>
        public Orm Desc(string desc)
        {
            cmd.OrderBy.Add(desc + " desc");
            return this;
        }
        /// <summary>
        /// 正向排序
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public Orm Asc(string asc)
        {
            cmd.OrderBy.Add(asc + " asc");
            return this;
        }
        /// <summary>
        /// 返回或者更新的字段
        /// </summary>
        /// <param name="cols">列名,多个字段名以逗号分隔</param>
        /// <returns></returns>
        public Orm Cols(params string[] cols)
        {
            cmd.Fields.Clear();
            cmd.Fields.AddRange(cols);
            // 全部元素转为小写
            for (int i = 0; i < cmd.Fields.Count; i++)
            {
                cmd.Fields[i] = cmd.Fields[i].ToLower();
            }
            return this;
        }
        /// <summary>
        /// 排除的字段
        /// </summary>
        /// <param name="cols">字段名参数</param>
        /// <returns></returns>
        public Orm Exclude(params string[] cols)
        {
            cmd.ExcludeFields.Clear();
            cmd.ExcludeFields.AddRange(cols);

            for (int i = 0; i < cmd.ExcludeFields.Count; i++)
            {
                cmd.ExcludeFields[i] = cmd.ExcludeFields[i].ToLower();
            }
            return this;
        }
        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="groupby"></param>
        /// <returns></returns>
        public Orm GroupBy(string groupby)
        {
            cmd.GroupBy = groupby;
            return this;
        }
        /// <summary>
        /// 分组条件
        /// </summary>
        /// <param name="having"></param>
        /// <returns></returns>
        public Orm Having(string having)
        {
            cmd.Having = having;
            return this;
        }
        /// <summary>
        /// 强调要查询的表名
        /// </summary>
        /// <param name="name">表名</param>
        /// <returns></returns>
        public Orm Table(string name)
        {
            cmd.TableName = name;
            return this;
        }


        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页的记录数</param>
        /// <returns></returns>
        public Orm Page(int pageIndex, int pageSize)
        {
            cmd.Page.pageIndex = pageIndex;
            cmd.Page.pageSize = pageSize;

            cmd.IsPage = true;
            return this;
        }

        /// <summary>
        /// 获取最终sql命令
        /// </summary>
        /// <returns></returns>
        private string getSql()
        {
            parameters = cmd.Params.ToArray();

            string[] _param = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                _param[i] = parameters[i].ParameterName + "=" + parameters[i].Value;
            }
            return string.Join(" ", cmd.SqlString) + ";" + string.Join(",", _param);
        }
        /// <summary>
        /// 原生数据库操作命令
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Orm Sql(string sql, params object[] args)
        {
            cmd.Where.Clear();
            cmd.Params.Clear();

            // 构造查询命令
            this.parameter(sql, args);

            cmd.SqlString.Clear();
            cmd.SqlString.AddRange(cmd.Where);

            cmd.IsNative = true;

            return this;
        }
        /// <summary>
        /// 获取查询参数集
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> Output()
        {
            Dictionary<string, object> _param = new Dictionary<string, object>();
            for (int i = 0; i < parameters.Length; i++)
            {
                _param.Add(parameters[i].ParameterName, parameters[i].Value);
            }
            return _param;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取cache键值
        /// </summary>
        /// <returns></returns>
        private string getCacheKey()
        {
            return string.Concat(cmd.TableName, "-Row-", string.Join(" ", cmd.CacheKey));
        }

        /// <summary>
        /// 构造sql命令
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private void createSql()
        {
            if (cmd.SqlString.Count == 0)
            {
                switch (cmd.Command.ToLower())
                {
                    case "insert":
                        insert();
                        break;
                    case "update":
                        update();
                        break;
                    case "delete":
                        delete();
                        break;
                    case "select":
                        select();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 构造查询参数
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="args"></param>
        private void parameter(string condition, params object[] args)
        {
            if (condition.Trim() == "")
                return;

            if (args.Length > 0)
            {
                int _start = 0, _next = 0;
                string _param = "@param_", _paramName = "", _cond = "";
                // 参数集合元素个数
                int _count = cmd.Params.Count;

                for (int i = 0; i < args.Length; i++)
                {
                    //参数名
                    _paramName = _param + _count.ToString();
                    _next = condition.IndexOf("?", _start);
                    _cond = condition.Substring(_start, _next - _start).Trim();

                    cmd.Where.Add(_cond + _paramName);
                    cmd.Params.Add(new SqlParameter(_paramName, args[i]));

                    // 如果不是主键查询，构造缓存键
                    if (!cmd.IsKey)
                    {
                        cmd.CacheKey.Add(_cond + args[i]);
                    }
                    _start = _next + 1;
                    _count++;
                }
                // 之后的其他字符
                _cond = condition.Substring(_start).Trim();
                cmd.Where.Add(_cond);
                // 如果不是主键查询，构造缓存键
                if (!cmd.IsKey && _cond != "")
                {
                    cmd.CacheKey.Add(_cond);
                }
            }
            else
            {
                cmd.Where.Add(condition);
                // 如果不是主键查询，构造缓存键
                if (!cmd.IsKey)
                {
                    cmd.CacheKey.Add(condition);
                }
            }
        }

        /// <summary>
        /// 构造Select命令
        /// </summary>
        private void trace(string msg)
        {
            Log.Debug("Snow.Orm", msg);
        }

        #endregion
    }
}
