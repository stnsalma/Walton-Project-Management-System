using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Infrastructures.Interfaces
{
    interface IInventoryRepository
    {
        string SaveFocClaimBomDetailModel(string receiveQuantity, string receiveRemarks, long id);
    }
}
