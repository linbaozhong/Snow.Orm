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
        public void Entity(BaseEntity model)
        {
            dataRow2Model(Row(model), model);
        }


        private void dataRow2Model(DataRow row, BaseEntity model)
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
