using System;
using Messages;
using NServiceBus;

namespace Sender
{
    public class Handler : 
        IHandleMessages<RmaRequestApproved> ,
        IHandleMessages<RmaRequestAboutToAutoAccept>
    {
        public void Handle(RmaRequestApproved message)
        {
            using (Colr.Green())
                Console.WriteLine(Environment.NewLine + "Removing request {0} because it was approved for customer {1}", 
                    message.RequestId, 
                    message.CustomerId);

            Db.RemoveRequest(message.RequestId);
        }

        public void Handle(RmaRequestAboutToAutoAccept message)
        {
            Console.WriteLine(Environment.NewLine);

            using (Colr.Yellow())
                Console.WriteLine(
                    "!!!!!!!!!!! Rma request {0} for customer {1} will auto accept in {2} seconds if you don't extend it!",
                    message.RequestId,
                    message.CustomerId, 
                    message.AutoAcceptAt.Subtract(DateTime.Now).TotalSeconds
                    );

            Console.WriteLine(Environment.NewLine);
        }
    }
}
