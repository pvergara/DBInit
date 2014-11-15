namespace Ecos.DBInit.Core.Model
{
    public class Container
    {
        private string _path;

        private Container(){}

        public static Container From(string path){
            return new Container{ _path = path };
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }
            
        public override bool Equals(object obj)
        {
            return Equals(obj as Container);
        }

        public bool Equals(Container container){
            if (container == null)
                return false;
            return _path == container._path;
        }

        public override int GetHashCode()
        {
            return _path.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[Container]: {0}",_path);
        }
    }
}