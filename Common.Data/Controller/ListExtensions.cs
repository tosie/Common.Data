using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common.Data {
    public static class ListExtensions {
        /// <summary>  
        /// This method adds the items in the first list to the second list.  
        /// This method is  helpful because even when X inherits from Y, List&lt;X&gt; does not inherit from List&lt;Y&gt;, and so you cannot cast between them, only copy.
        /// </summary>  
        /// <typeparam name="FROM_TYPE"></typeparam>  
        /// <typeparam name="TO_TYPE"></typeparam>  
        /// <param name="listToCopyFrom"></param>  
        /// <returns></returns>  
        public static List<TO_TYPE> ConvertTo<FROM_TYPE, TO_TYPE>(List<FROM_TYPE> listToCopyFrom) where FROM_TYPE : TO_TYPE {
            if (listToCopyFrom == null)
                throw new ArgumentNullException("listToCopyFrom");

            List<TO_TYPE> listToCopyTo = new List<TO_TYPE>(listToCopyFrom.Count);

            foreach (FROM_TYPE item in listToCopyFrom) {
                listToCopyTo.Add(item);
            }

            return listToCopyTo;
        }
    }
}
