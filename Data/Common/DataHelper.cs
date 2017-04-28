using Data.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Common
{
    public static class DataHepler
    {
  

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Encoding(object obj)
        {
            return JsonHelper.SerializeObject(obj);
        }

        //解码

        public static Object Decoding(string json)
        {
            return JsonHelper.DeserializeJsonToObject<Pf_Message_Obj<object>>(json);
        }




    }
}
