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

        public static IEnumerable<string> SPsNames
        {
            get{ return new []{ "film_in_stock", "film_not_in_stock", "rewards_report" }; }
        }

        public static int SPsCounter
        {
            get{ return SPsNames.Count(); }
        }

        public static IEnumerable<string> FunctionNames
        {
            get{ return new []{ "get_customer_balance", "inventory_held_by_customer", "inventory_in_stock" }; }
        }

        public static int FunctionsCounter
        {
            get{ return FunctionNames.Count(); }
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

