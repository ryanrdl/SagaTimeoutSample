using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Messages;

namespace Sender
{
    public class Db
    {
        
        private static readonly ConcurrentBag<int> _customers = new ConcurrentBag<int>(); 
        private static readonly ConcurrentDictionary<int, List<Guid>> _requests = new ConcurrentDictionary<int, List<Guid>>();

        public static int[] AllCustomers()
        {
            return _customers.OrderBy(o => o).ToArray();
        }

        public static int NewCustomer()
        {
            int customerId =
                AllCustomers().Any()
                    ? AllCustomers().Max() + 1
                    : 1;

            using (Colr.White())
                Console.WriteLine(Environment.NewLine + "Created customer {0}", customerId);
            _customers.Add(customerId); 
            return customerId;
        }

        public static Guid[] AllRequests(int customerId)
        {
            return _requests.GetOrAdd(customerId, new List<Guid>()).ToArray();
        } 

        public static Guid NewRequest(int customerId)
        {
            Guid requestId = Guid.NewGuid();
            using (Colr.White())
                Console.WriteLine(Environment.NewLine + "Created request {0} for customer {1}", requestId, customerId);

            _requests.GetOrAdd(customerId, new List<Guid>()).Add(requestId);
        
            return requestId;
       }

        public static void RemoveRequest(Guid requestId)
        {
            foreach (var item in _requests)
            {
                if (item.Value.Any(request => request == requestId))
                {
                    item.Value.Remove(requestId);
                    return;
                }
            }
        }
    }
}
