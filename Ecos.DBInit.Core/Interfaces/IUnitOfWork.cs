using Ecos.DBInit.Core.Model;
using System.Collections.Generic;
using System;

namespace Ecos.DBInit.Core.Interfaces
{
	public interface IUnitOfWork:IDisposable
	{
        void Add(IEnumerable<Script> scripts);
        void Flush();
	}

}