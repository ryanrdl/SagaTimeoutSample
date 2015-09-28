using System;
using System.Threading;
using Messages;
using NServiceBus;

namespace Sender
{
    internal class Program
    {
        private static IStartableBus _bus;
        private static AutoResetEvent _waitHandle;

        private static void Main(string[] args)
        {
            using (_bus = BusFactory.GetBus())
            using (_waitHandle = new AutoResetEvent(false))
            {
                var subscriberThread = new Thread(_ => Subscribe(_bus, _waitHandle))
                {
                    IsBackground = false,
                    Name = "Subscriber",
                };
                subscriberThread.Start(_waitHandle);

                var publisherThread = new Thread(_ => Publish(_bus))
                {
                    IsBackground = false,
                    Name = "Publisher"
                };
                publisherThread.Start();
                publisherThread.Join();

                _waitHandle.Set();
                subscriberThread.Join();
            }
        }

        private static void Publish(ISendOnlyBus bus)
        {
            Guid requestId = CreateNewRequest(bus);
            while (PromptForAction(bus, requestId))
            {
            }
        }

        private static void Subscribe(IStartableBus bus, WaitHandle waitHandle)
        {
            try
            {
                bus.Start();

                Console.WriteLine("Waiting for messages.");
                waitHandle.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled ex: {0}", ex);
            }
        }

        private static Guid CreateNewRequest(ISendOnlyBus bus)
        {
            Guid requestId = Guid.NewGuid();
            bus.Send(new CreateRmaRequest
            {
                RequestId = requestId,
                Timeout1Seconds = 40,
                Timeout2Seconds = 60
            });


            using (Colr.Blue())
                Console.WriteLine("Send CreateRmaRequest with requestId {0}", requestId);

            return requestId;
        }

        private static bool PromptForAction(ISendOnlyBus bus, Guid requestId)
        { 
            bool result;

            using (Colr.Green())
            {
                Console.WriteLine(Environment.NewLine + "Options:");
                Console.WriteLine("(a) to approve the RMA request");
                Console.WriteLine("(r) to reject the RMA request");
                Console.WriteLine("(1) to extend acceptance timeout by 15 seconds");
                Console.WriteLine("(2) to reduce rejection timeout by 15 seconds"); 
                Console.WriteLine("(q) to quit");
                Console.Write(" > ");
            }
            char input = Console.ReadKey().KeyChar;

            switch (input)
            {
                case 'a':
                    bus.Send(new ApproveRmaRequest{RequestId = requestId});
                    result = true;
                    break;
                case 'r':
                    bus.Send(new RejectRmaRequest{RequestId = requestId});
                    result = true;
                    break;
                case '1':
                    bus.Send(new ExtendTimeout1 {RequestId = requestId, ExtendBySeconds = 15});
                    result = true;
                    break;
                case '2':
                    bus.Send(new ReduceTimeout2 {RequestId = requestId, ReduceBySeconds = 15});
                    result = true;
                    break; 
                case 'q':
                    result = false;
                    break;
                default:
                    using (Colr.Red())
                        Console.WriteLine("I don't understand, try again...");
                    result = true;
                    break;
            }

            return result;
        }
    }
}