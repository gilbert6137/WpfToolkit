using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfToolkit.Attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IdColumnAttribute : System.Attribute
    {
        // 可以添加一些屬性，用於進一步定制該類型的列
    }
}
