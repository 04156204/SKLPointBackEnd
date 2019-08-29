using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceWeb.Model
{
    [Serializable]
    public class RtnResult
    {/// <summary>
     /// 是否成功
     /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 回傳成功或失敗代碼
        /// </summary>
		public string Code { get; set; }
        /// <summary>
        /// 回傳訊息
        /// </summary>
		public string Message { get; set; }
        /// <summary>
        /// 邏輯錯誤
        /// </summary>
        public System.Collections.Generic.List<System.Exception> Exceptions { get; set; }
    }

    [Serializable]
    public class RtnResult<T> : RtnResult
    {
        //public Action ResultAction { get; set; }
        public T Result { get; set; }
        //public int PointSum { get; set; }
    }

}
