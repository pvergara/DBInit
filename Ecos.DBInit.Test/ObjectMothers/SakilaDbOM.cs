using System.Collections.Generic;

namespace Ecos.DBInit.Test.ObjectMothers
{
    public static class SakilaDbOM
    {
        public static IEnumerable<string> SomeTableNames
        {
            get{ return new []{ "actor", "address", "customer", "film_text", "inventory" };}
        }

        public static byte TablesCounter
        {
            get{ return 16;}
        }

        public static byte ViewsCounter
        {
            get{ return 7;}
        }

        public static byte SPsCounter
        {
            get{ return 3;}
        }

        public static  IEnumerable<string> SomeSpNames
        {
            get{ return new []{ "film_in_stock" };}
        }

        public static byte FunctionsCounter
        {
            get{ return 3;}
        }

        public static IEnumerable<string> SomeFunctionNames
        {
            get{ return new []{ "get_customer_balance" };}
        }

        public static byte TablesActorsCounter
        {
            get{ return 200;}
        }

        public static int TablesAddressCounter
        {
            get{ return 603;}
        }
    }
}

