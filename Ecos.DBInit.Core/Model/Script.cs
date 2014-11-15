namespace Ecos.DBInit.Core.Model
{
	public class Script
	{
        private string _script;

        public string Query
        {
            get{ return _script; }
        }

        private Script(){
        }

        public static Script From(string script){
            return new Script{ _script = script };
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Script);
        }

        public bool Equals(Script script){
            if (script == null)
            {
                return false;
            }
            return _script == script._script; 
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[Script]: {0}",_script);
        }
	}
 

}