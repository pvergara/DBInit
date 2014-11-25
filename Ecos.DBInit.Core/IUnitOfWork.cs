using Ecos.DBInit.Core.Model;
using System.Collections.Generic;

namespace Ecos.DBInit.Core
{
	public interface IUnitOfWork
	{
        void Add(IEnumerable<Script> scripts);
        void Flush();
	}

}