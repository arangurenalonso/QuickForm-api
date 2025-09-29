using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickForm.Common.Domain.Base.Dto;
public class ExampleDto : BaseMasterEntity
{
    protected override Result SetBaseProperties(MasterUpdateBase dto)
    {
        var result= base.SetBaseProperties(dto);
        return result;
    }
}
