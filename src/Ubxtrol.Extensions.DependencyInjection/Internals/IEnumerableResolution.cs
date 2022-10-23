using System.Collections.Generic;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal interface IEnumerableResolution
    {
        IEnumerator<IDependencyResolution> GetResolutionEnumerator();
    }
}
