﻿using System;
using System.Linq;
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
            while (Menu(_bus))
            {
            }
        }

        public static bool Menu(ISendOnlyBus bus)
        {
            int[] customers = Db.AllCustomers();

            using (Colr.Green())
            {
                Console.WriteLine(Environment.NewLine + "Menu:");
                Console.WriteLine("(n)ew customer");
                foreach (var customer in customers)
                {
                    Console.WriteLine("({0}) Customer {1}", customer, customer);
                }
                Console.WriteLine("(q) to quit");
                Console.Write(" > ");
            }

            char input = Console.ReadKey().KeyChar;
            switch (input)
            {
                case 'n':
                    int customerId = Db.NewCustomer();
                    while (CustomerMenu(bus, customerId))
                    {
                    }
                    return true;
                case 'q':
                    return false;
                default:
                    int i;
                    if (int.TryParse(input.ToString(), out i))
                        if (customers.Contains(i))
                        {
                            while (CustomerMenu(bus, i))
                            {
                            }
                            return true;
                        }

                    using (Colr.Red())
                        Console.WriteLine("Unknown input '{0}', please try again" + Environment.NewLine, input);

                    return true;
            }
        }

        public static bool CustomerMenu(ISendOnlyBus bus, int customerId)
        {
            Guid[] requests = Db.AllRequests(customerId);

            Console.WriteLine(Environment.NewLine);
            using (Colr.Yellow())
            {
                Console.WriteLine(Environment.NewLine + "Customer {0} Menu:", customerId);
                Console.WriteLine("(n)ew request");
                Console.WriteLine("(e)xtend all acceptance timeouts");
                Console.WriteLine("(r)educe all reject timeouts");
                for (var i = 0; i < requests.Length; i++)
                {
                    Console.WriteLine("({0}) Request {1}", i, requests[i]);
                }
                Console.WriteLine("(b)ack to previous menu");
                Console.Write(" > ");
            }

            char input = Console.ReadKey().KeyChar;
            switch (input)
            {
                case 'n':
                    while (RequestMenu(bus, customerId, CreateNewRequest(bus, customerId)))
                    {
                    }
                    return true;
                case 'e':
                    Console.WriteLine(Environment.NewLine);
                    int extendBy = GetNumericValue("Number of seconds to extend by or (0) to cancel: ");

                    if (extendBy > 0)
                    {
                        bus.Send(new ExtendAllAcceptanceTimeouts {CustomerId = customerId, ExtendBySeconds = extendBy});
                    }
                    else
                    {
                        using (Colr.Red())
                            Console.WriteLine("Canceled");
                    }
                    return true;
                case 'r':
                    Console.WriteLine(Environment.NewLine);
                    int reduceBy = GetNumericValue("Number of seconds to reduce by or (0) to cancel: ");

                    if (reduceBy > 0)
                    {
                        bus.Send(new ReduceAllRejectionTimeouts {CustomerId = customerId, ReduceBySeconds = reduceBy});
                    }
                    else
                    {
                        using (Colr.Red())
                            Console.WriteLine("Canceled");
                    }
                    return true;
                case 'b':
                    return false;
                default:
                    int i;
                    if (int.TryParse(input.ToString(), out i))
                        if (i < requests.Length)
                        {
                            while (RequestMenu(bus, customerId, requests[i]))
                            {
                            }
                            return true;
                        }

                    using (Colr.Red())
                        Console.WriteLine("Unknown input '{0}', please try again" + Environment.NewLine, input);
                    return true;
            }
        }

        public static int GetNumericValue(string prompt)
        { 
            int value = 0;
            bool prompted = false;

            do
            {
                Console.Write(prompt);

                if (prompted && value <= 0)
                {
                    using (Colr.Red())
                        Console.WriteLine("'{0}' is not a numeric value or less than 0, please try again");
                }

                prompted = true;

            } while (!int.TryParse(Console.ReadLine(), out value));

            return value;
        }

        public static bool RequestMenu(ISendOnlyBus bus, int customerId, Guid requestId)
        {
            Console.WriteLine(Environment.NewLine);
            using (Colr.Magenta())
            {
                Console.WriteLine(Environment.NewLine + "Request {0} Menu:", requestId);
                Console.WriteLine("(a)pprove");
                Console.WriteLine("(r)eject");
                Console.WriteLine("(e)xtend acceptance timeout");
                Console.WriteLine("re(d)uce rejection timeout");
                Console.WriteLine("(b)ack to previous menu");
                Console.Write(" > ");
            }
            
            char input = Console.ReadKey().KeyChar;
            switch (input)
            { 
                case 'a':
                    bus.Send(new ApproveRmaRequest{CustomerId = customerId, RequestId = requestId});
                    return true;
                case 'r':
                    bus.Send(new RejectRmaRequest{CustomerId = customerId, RequestId = requestId});
                    return true;
                case 'e':
                    bus.Send(new ExtendAcceptanceTimeout {CustomerId = customerId, RequestId = requestId, ExtendBySeconds = 15});
                    return true;
                case 'd':
                    bus.Send(new ReduceRejectionTimeout {CustomerId = customerId, RequestId = requestId, ReduceBySeconds = 15});
                    return true;
                case 'b':
                    return false; 
                default:
                    using (Colr.Red())
                        Console.WriteLine("I don't understand, try again...");
                    return true;
            }

        }

        private static void Subscribe(IStartableBus bus, WaitHandle waitHandle)
        {
            try
            {
                bus.Start();

                //Console.WriteLine("Waiting for messages.");
                waitHandle.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled ex: {0}", ex);
            }
        }

        private static Guid CreateNewRequest(ISendOnlyBus bus, int customerId)
        { 
            Guid requestId = Db.NewRequest(customerId);
            bus.Send(new CreateRmaRequest
            {
                CustomerId = customerId,
                RequestId = requestId,
                Timeout1Seconds = 30,
                Timeout2Seconds = 30
            });  
            return requestId;
        } 
    }
}