using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcReportViewer
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class UriParameterAttribute : Attribute
    {
        public UriParameterAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
