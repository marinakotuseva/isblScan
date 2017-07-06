using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISBLScan.ViewCode
{
    public static class CodeToNameConverter
    {
        public static string FunctionParamTypeIDToName(string id)
        {
            string typeName = "";
            switch (id.ToUpper())
            {
                case "V":
                case "SYSRES_SYSCOMP.DATA_TYPE_VARIANT":
                    typeName = "Вариантный";
                    break;
                case "Д":
                case "D":
                case "SYSRES_SYSCOMP.DATA_TYPE_DATE":
                    typeName = "Дата";
                    break;
                case "Ч":
                case "F":
                case "SYSRES_SYSCOMP.DATA_TYPE_FLOAT":
                    typeName = "Дробное число";
                    break;
                case "L":
                case "SYSRES_SYSCOMP.DATA_TYPE_BOOLEAN":
                    typeName = "Логический";
                    break;
                case "С":
                case "S":
                case "SYSRES_SYSCOMP.DATA_TYPE_STRING":
                    typeName = "Строка";
                    break;
                case "Ц":
                case "I":
                case "SYSRES_SYSCOMP.DATA_TYPE_INTEGER":
                    typeName = "Целое число";
                    break;
                default:
                    typeName = "Неизвестный тип (" + id + ")";
                    break;
            }
            return typeName;
        }
    }
}
