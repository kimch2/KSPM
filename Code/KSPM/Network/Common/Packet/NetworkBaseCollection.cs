﻿using System.Net.Sockets;

namespace KSPM.Network.Common.Packet
{
    /// <summary>
    /// Holds the basic properties to create a network connection.
    /// </summary>
    class NetworkBaseCollection : System.IDisposable
    {
        public Socket socketReference;
        public byte[] rawBuffer;
        public byte[] secondarayRawBuffer;

        /// <summary>
        /// Initializes the buffers, but the Socket is set to null.
        /// <see cref="Socket"/>
        /// </summary>
        /// <param name="buffersSize">Size used to alloc the memory buffers.</param>
        public NetworkBaseCollection(int buffersSize)
        {
            this.rawBuffer = new byte[buffersSize];
            this.secondarayRawBuffer = new byte[buffersSize];
            this.socketReference = null;
        }

        /// <summary>
        /// Releases the buffers and set the Socket to null, so It has to be shut down by you.
        /// <see cref="Socket"/>
        /// </summary>
        public void Dispose()
        {
            this.rawBuffer = null;
            this.secondarayRawBuffer = null;
            this.socketReference = null;
        }
    }
}
