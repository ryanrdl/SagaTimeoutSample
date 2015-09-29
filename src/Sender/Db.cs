using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sender
{
    public class Db
    {
        
        private static readonly ConcurrentBag<Guid> _customers = new ConcurrentBag<Guid>(); 
        private static readonly ConcurrentDictionary<Guid, List<Guid>> _requests = new ConcurrentDictionary<Guid, List<Guid>>();

        public static Guid[] AllCustomers()
        {
            return _customers.ToArray();
        }

        public static Guid NewCustomer()
        {
            Guid customerId = Guid.NewGuid();
            _customers.Add(customerId); 
            return customerId;
        }

        public static Guid[] AllRequests(Guid customerId)
        {
            return _requests.GetOrAdd(customerId, new List<Guid>()).ToArray();
        }

        public static Guid NewRequest(Guid customerId)
        {
            Guid requestId = Guid.NewGuid();
            _requests.GetOrAdd(customerId, new List<Guid>()).Add(requestId);
        
            return requestId;
       }
    }
}
