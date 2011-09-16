using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpUPnP
{
    public class UPnPException
        : Exception
    {
        public UPnPException(String message)
            : base(message)
        {

        }
    }
}