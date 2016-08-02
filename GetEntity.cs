using System;
using System.Collections.Generic;
using System.Data;

namespace Snow
{
    public partial class Orm : NativeDb
    {
        public void GetModel(BaseEntity model)
        {
            prepare(model);
            _Model(_Row(model), model);
            finish();
        }

        public  List<T> GetList<T>(T model) where T : class, new()
        {
            prepare(model as BaseEntity);
            List<T> _list = _List<T>(_Rows(model as BaseEntity));
            finish();

            return _list;
        }

        /// <summary>
        /// table转list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dt"></param>
        private  List<T> _List<T>(DataRow[] rows) where T : class, new()
        {
            List<T> _list = new List<T>();

            foreach (DataRow row in rows)
            {
                var model = new T();

                _Model(row, model as BaseEntity);

                _list.Add(model);
            }
            return _list;
        }
        private  void _Model(DataRow row, BaseEntity model)
        {
            model.Clear();

            try
            {
                string _key;
                foreach (DataColumn col in row.Table.Columns)
                {
                    _key = col.ColumnName;
                    model[_key] = row[_key];
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }

}
