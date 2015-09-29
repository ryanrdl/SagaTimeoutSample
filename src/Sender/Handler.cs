using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            throw new NotImplementedException();
        }

        public void Handle(RmaRequestRejected message)
        {
            throw new NotImplementedException();
        }
    }
}
