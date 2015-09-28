using System;
using Messages;
using NServiceBus;
using NServiceBus.Saga;

namespace SagaService
{
    public class RmaSaga : Saga<RmaSagaData>,
        IAmStartedByMessages<RmaRequestCreated>,
        IHandleMessages<ExtendTimeout1>,
        IHandleMessages<ReduceTimeout2>,
        IHandleMessages<RmaRequestApproved>,

        IHandleTimeouts<Timeout1>,
        IHandleTimeouts<Timeout2>,

        IHandleSagaNotFound
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<RmaSagaData> mapper)
        {
            mapper.ConfigureMapping<RmaRequestCreated>(o => o.RequestId).ToSaga(o => o.RequestId);
            mapper.ConfigureMapping<ExtendTimeout1>(o => o.RequestId).ToSaga(o => o.RequestId);
            mapper.ConfigureMapping<ReduceTimeout2>(o => o.RequestId).ToSaga(o => o.RequestId);
            mapper.ConfigureMapping<RmaRequestApproved>(o => o.RequestId).ToSaga(o => o.RequestId);
        }

        public void Handle(RmaRequestCreated message)
        {
            Data.RequestId = message.RequestId;

            Data.Timeout1Expires = DateTime.Now.AddSeconds(message.Timeout1Seconds)
                //add extra time to avoid boundary condition
                .AddSeconds(1);

            RequestTimeout<Timeout1>(Data.Timeout1Expires);

            Data.Timeout2Expires = DateTime.Now.AddSeconds(message.Timeout2Seconds)
                //add extra time to avoid boundary condition
                .AddSeconds(1);

            RequestTimeout<Timeout2>(Data.Timeout2Expires);



            using (Colr.Green())
            {
                Console.WriteLine("Current Time = {0}", DateTime.Now.ToLongTimeString());

                Console.WriteLine("Saga timeout 1 set to " + Data.Timeout1Expires.ToLongTimeString());

                Console.WriteLine("Saga timeout 2 set to " + Data.Timeout2Expires.ToLongTimeString());
            }
        }


        public void Timeout(Timeout1 state)
        {
            if (Data.Timeout1Expires < DateTime.Now)
            {
                using (Colr.Red())
                    Console.WriteLine("Terminating saga because timeout 1 timed out at {0}", DateTime.Now.ToLongTimeString());
                MarkAsComplete();
            }
            else
            {
                using (Colr.Blue())
                    Console.WriteLine("Saga not terminated because it has not exceeded timeout 1, it will expire at {0} (CurrentTime = {1})",
                                      Data.Timeout1Expires.ToLongTimeString(),
                                      DateTime.Now.ToLongTimeString());
            }
        }

        public void Handle(ExtendTimeout1 message)
        {
            Data.Timeout1Expires = Data.Timeout1Expires.AddSeconds(message.ExtendBySeconds);
            RequestTimeout<Timeout1>(Data.Timeout1Expires);

            using (Colr.Green())
                Console.WriteLine("Saga timeout 1 extended to {0} at {1}",
                    Data.Timeout1Expires.ToLongTimeString(),
                    DateTime.Now.ToLongTimeString());
        }

        public void Timeout(Timeout2 state)
        {
            if (Data.Timeout2Expires < DateTime.Now)
            {
                using (Colr.Red())
                    Console.WriteLine("Terminating saga because timeout 2 timed out at {0}", DateTime.Now.ToLongTimeString());
                MarkAsComplete();
            }
            else
            {
                using (Colr.Blue())
                    Console.WriteLine("Saga not terminated because it has not exceeded timeout 2, it will expire at {0} (CurrentTime = {1})", 
                        Data.Timeout2Expires.ToLongTimeString(),
                        DateTime.Now.ToLongTimeString());
            }
        }

        public void Handle(ReduceTimeout2 message)
        {
            Data.Timeout2Expires = Data.Timeout2Expires.AddSeconds(message.ReduceBySeconds*-1);
            RequestTimeout<Timeout2>(Data.Timeout2Expires);

            using (Colr.Green())
                Console.WriteLine("Saga timeout 2 reduced to {0} at {1}", Data.Timeout2Expires.ToLongTimeString(), DateTime.Now.ToLongTimeString());
        }

        public void Handle(object message)
        {
            using (Colr.Red())
            {
                Console.WriteLine("Saga not found so it must have been expired or approved ({0}).", DateTime.Now.ToLongTimeString());
                Console.WriteLine(message);
            }
        }

        public void Handle(RmaRequestApproved message)
        {
            using (Colr.Green())
                Console.WriteLine("Completing saga because RMA request {0} was approved at {1}", message.RequestId, DateTime.Now.ToLongTimeString());
            MarkAsComplete();
        } 
    }

    public class Timeout1
    {

    }

    public class Timeout2
    {

    }
}