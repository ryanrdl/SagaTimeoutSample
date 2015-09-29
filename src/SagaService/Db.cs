﻿using System;
using System.Collections.Concurrent;

namespace SagaService
{
    public class Db
    {
        private static readonly ConcurrentBag<RequestModel> _requests = new ConcurrentBag<RequestModel>();
  
        public static void Save(Guid requestId, Guid customerId)
        {
            _requests.Add(new RequestModel{Id = requestId, CustomerId = customerId});
        }

        public static RequestModel[] GetAll()
        {
            return _requests.ToArray();
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
    }

    public class RequestModel
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        
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