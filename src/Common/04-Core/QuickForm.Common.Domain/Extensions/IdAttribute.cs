using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickForm.Common.Domain;
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class IdAttribute : Attribute
{
    public Guid Value { get; }

    // Permite pasar el GUID como string en el atributo
    public IdAttribute(string value)
    {
        Value = Guid.Parse(value);
    }

    public IdAttribute(Guid value)
    {
        Value = value;
    }
}
