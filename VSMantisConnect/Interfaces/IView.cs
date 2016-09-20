using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSMantisConnect.Interfaces
{
	interface IView
	{
		string DisplayName { get; }
		bool Initialized { get; }
		void InitializeData();
	}
}
