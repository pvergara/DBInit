using System.Collections.Generic;
using System.Linq;

namespace Ecos.DBInit.Test.ObjectMothers
{
    public static class SakilaDbOM
    {
        public static IEnumerable<string> SomeTableNames
        {
            get{ return new []{ "actor", "address", "customer", "film_text", "inventory" }; }
        }

        public static byte TablesCounter
        {
            get{ return 16; }
        }

        public static IEnumerable<string> ViewNames
        {
            get{ return new []{ "actor_info", "customer_list", "film_list", "nicer_but_slower_film_list", "sales_by_film_category", "sales_by_store", "staff_list"  }; }
        }

        public static int ViewsCounter
        {
            get{ return ViewNames.Count(); }
        }

        public static byte SPsCounter
        {
            get{ return 3; }
        }

        public static  IEnumerable<string> SomeSpNames
        {
            get{ return new []{ "film_in_stock" }; }
        }

        public static byte FunctionsCounter
        {
            get{ return 3; }
        }

        public static IEnumerable<string> SomeFunctionNames
        {
            get{ return new []{ "get_customer_balance" }; }
        }

        public static byte TablesActorsCounter
        {
            get{ return 200; }
        }

        public static int TablesAddressCounter
        {
            get{ return 603; }
        }
    }
}

