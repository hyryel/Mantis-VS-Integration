using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSMantisConnect.Interfaces
{
    public interface IStatusUpdater
    {
        event EventHandler<StatusUpdatedEventArgs> UpdateStatus;
    }
}
