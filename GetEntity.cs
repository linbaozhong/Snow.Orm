using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snow
{
    public partial class Orm : NativeDb
    {
        public void Model(BaseEntity model)
        {
            prepare(model);
            _Model(_Row(model), model);
            finish();
        }

        public List<BaseEntity> List(BaseEntity model)
        {
            prepare(model);
            List<BaseEntity> _list = _List(_Rows(model), model);
            finish();

            return _list;
        }

        /// <summary>
        /// table转list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="dt"></param>
        private List<BaseEntity> _List(DataRow[] rows, BaseEntity model)
        {
            List<BaseEntity> _list = new List<BaseEntity>();

            

            foreach (DataRow row in rows)
            {
                _Model(row, model);

                _list.Add(model);
            }
            return _list;
        }
        private void _Model(DataRow row, BaseEntity model)
        {
            model.Clear();

            try
            {
                string _key;
                foreach (DictionaryEntry field in table)
                {
                    _key = field.Key.ToString().ToLower();

                    if (!cmd.Fields.Contains(getName(_key)) || Convert.IsDBNull(row[_key]) || row[_key].ToString() == "")
                    {
                        continue;
                    }
                    else
                    {
                        model[_key] = row[_key];
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }

}
