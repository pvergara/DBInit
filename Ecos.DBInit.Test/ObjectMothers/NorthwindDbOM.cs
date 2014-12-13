using System.Collections.Generic;

namespace Ecos.DBInit.Test.ObjectMothers
{
    public static class NorthwindDbOM
    {
        public static string ConnectionStringName { get { return "NorthwindCS"; } }

        public static IEnumerable<string> SomeTableNames
        {
            get { return new[] { "Categories", "CustomerCustomerDemo", "Order Details", "Region", "Territories" }; }
        }

        public static byte TablesCounter { get { return 13; } }

        public static byte ViewsCounter { get { return 16; } }

        public static byte SPsCounter { get { return 7; } }

        public static byte FunctionsCounter { get { return 0; } }

        public static IEnumerable<string> FunctionNames
        {
            get
            {
                return new string[0];
            }
        }
        public static IEnumerable<string> SPsNames
        {
            get
            {
                return new string[] { "CustOrdersDetail" };
            }
        }

        public static int OrderDetailsCounter { get { return 2155; } }
        
        public static int ProductsCounter { get { return 77; } }
    }
}