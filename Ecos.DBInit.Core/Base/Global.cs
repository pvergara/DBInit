namespace Ecos.DBInit.Core.Base
{
    public static class Global
    {
        private static bool _isFirstTime = true;
        public static bool IsFirstTime
        {
            get
            {
                var aux = _isFirstTime;
                _isFirstTime = false;
                return aux;
            }
        }
    }
}
