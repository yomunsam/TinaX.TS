using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Puerts;
using UnityEngine;

namespace TinaXEditor.TS
{
    [Configure]
    public class PuertsConfig
    {
        [Binding]
        static IEnumerable<Type> Binding
        {
            get
            {
                return new List<Type>
                {
                    typeof(Debug),
                };
            }
        }
    }
}
