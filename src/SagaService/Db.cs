using System;
using System.Collections.Concurrent;
using System.Linq;

namespace SagaService
{
    public class Db
    {
        private static readonly ConcurrentBag<RequestModel> _requests = new ConcurrentBag<RequestModel>();
  
        public static void Save(Guid requestId, int customerId)
        {
            _requests.Add(new RequestModel{Id = requestId, CustomerId = customerId});
        }

        public static RequestModel[] GetAll(int customerId)
        {
            return _requests.Where(o => o.CustomerId == customerId).ToArray();
        }

        public static void Approve(Guid requestId)
        {
            foreach (var model in _requests)
            {
                if (model.Id != requestId) continue;

                model.Approve();
                break;
            }
        }

        public static void Reject(Guid requestId)
        {
            foreach (var model in _requests)
            {
                if (model.Id != requestId) continue;

                model.Reject();
                break;
            }
        }

        public static RequestModel Get(Guid requestId)
        {
            return _requests.FirstOrDefault(model => model.Id == requestId);
        }
    }

    public class RequestModel
    {
        public Guid Id { get; set; }
        public int CustomerId { get; set; }
        
        public RequestState State { get; private set; }

        public enum RequestState
        {
            Pending,
            Accepted,
            Rejected
        }

        public void Approve()
        {
            State = RequestState.Accepted;
        }

        public void Reject()
        {
            State = RequestState.Rejected;
        }

        public RequestModel()
        {
            State = RequestState.Pending;
        }
    }
}
