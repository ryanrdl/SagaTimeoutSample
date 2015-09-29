using System;
using Messages;
using NServiceBus;

namespace Sender
{
    public class Handler : 
        IHandleMessages<RmaRequestApproved>,
        IHandleMessages<RmaRequestRejected>
    {
        public void Handle(RmaRequestApproved message)
        {
            using (Colr.Green())
                Console.WriteLine(Environment.NewLine + "Removing request {0} because it was approved for customer {1}", 
                    message.RequestId, 
                    message.CustomerId);

            Db.RemoveRequest(message.RequestId);
        }

        public void Handle(RmaRequestRejected message)
        {
            using (Colr.Red())
                Console.WriteLine(Environment.NewLine + "Removing request {0} because it was rejected for customer {1}",
                    message.RequestId,
                    message.CustomerId);

            Db.RemoveRequest(message.RequestId);
        }
    }
}
