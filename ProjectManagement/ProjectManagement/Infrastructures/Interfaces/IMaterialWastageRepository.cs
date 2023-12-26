using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.ViewModels;

namespace ProjectManagement.Infrastructures.Interfaces
{
    public interface IMaterialWastageRepository
    {
        bool GetMaterialWastageReportByMonthAndYear(int monthNumber, int yearNumber);
        MaterialWastageReportTopSheetViewModel GetMaterailWastageTopSheet(long id);
    }
}
