using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cache
{
    public interface ICacheProvider<T>
    {
        Task<IEnumerable<T>> GetCachedResponse();
        Task<IEnumerable<T>> GetCachedResponse(string cacheKey, SemaphoreSlim semaphore);
    }
}
