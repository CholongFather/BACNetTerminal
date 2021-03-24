using System;

namespace BACnet
{
    [Serializable]
    internal class BACnetPacketException : Exception
    {
        public BACnetPacketException(string message)
            : base(message)
        {
        }
    }
}
