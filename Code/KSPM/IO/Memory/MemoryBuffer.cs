﻿namespace KSPM.IO.Memory
{
    public class MemoryBuffer
    {
        protected byte[] ioBuffer;
        protected uint bufferSize;
        protected uint readingPosition;
        protected uint writingPosition;

        public MemoryBuffer(uint bufferLength)
        {
            this.bufferSize = bufferLength;
            this.ioBuffer = new byte[this.bufferSize];
            this.readingPosition = this.writingPosition = 0;
        }

        /// <summary>
        /// Writes a byte array into the buffer.
        /// </summary>
        /// <param name="source">Byte array from where the bytes are going to copied.</param>
        /// <param name="srcOffset">Reference to the offset where the bytes are being copied from, and is set to the starting position inside the underlying buffer</param>
        /// <param name="size">How many buffers should be written.</param>
        /// <returns>Amount of written bytes.</returns>
        public uint Write(byte[] source, ref uint srcOffset,  uint size)
        {
            uint availableBytes;
            if (size <= 0)
                return 0;
            ///If the size is greater than the buffer size exits the method, because the buffer is not capable to hold that amount of bytes.
            if (size > this.bufferSize)
                return 0;
            lock (this.ioBuffer)
            {
                ///Checking if the amount of bytes can be stored in the remaining space or it has to reset the writing position and overwrite 
                ///the previous content.
                availableBytes = bufferSize - this.writingPosition;
                if (availableBytes < size)
                {
                    this.writingPosition = 0;
                }
                System.Buffer.BlockCopy(source, (int)srcOffset, this.ioBuffer, (int)this.writingPosition, (int)size);
                srcOffset = this.writingPosition;
                this.writingPosition += size;
            }
            return size;
        }

        /// <summary>
        /// Releases the underlying buffer and resets the writing and reading positions also resets the bufferSize.
        /// </summary>
        public void Release()
        {
            this.ioBuffer = null;
            this.bufferSize = this.readingPosition = this.writingPosition = 0;
        }

        /// <summary>
        /// This is not thread safe so be carefull.
        /// </summary>
        public byte[] IOBuffer
        {
            get
            {
                return this.ioBuffer;
            }
        }
    }
}
