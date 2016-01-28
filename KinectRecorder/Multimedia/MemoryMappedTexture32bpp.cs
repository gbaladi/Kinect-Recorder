﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KinectRecorder.Multimedia
{
    public unsafe class MemoryMappedTexture32bpp : IDisposable
    {
        #region The native structure, where we store all ObjectIDs uploaded from graphics hardware
        private IntPtr m_pointer;
        private int* m_pointerNative;
        private Size2 m_size;
        private int m_countInts;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryMappedTexture32bpp"/> class.
        /// </summary>
        /// <param name="size">The total size of the texture.</param>
        public MemoryMappedTexture32bpp(Size2 size)
        {
            m_pointer = Marshal.AllocHGlobal(size.Width * size.Height * 4);
            m_pointerNative = (int*)m_pointer.ToPointer();
            m_size = size;
            m_countInts = m_size.Width * m_size.Height;
        }

        /// <summary>
        /// Converts the underlying buffer to a managed byte array.
        /// </summary>
        public byte[] ToArray()
        {
            byte[] result = new byte[this.SizeInBytes];
            Marshal.Copy(m_pointer, result, 0, (int)this.SizeInBytes);
            return result;
        }

        /// <summary>
        /// Führt anwendungsspezifische Aufgaben aus, die mit dem Freigeben, Zurückgeben oder Zurücksetzen von nicht verwalteten Ressourcen zusammenhängen.
        /// </summary>
        public void Dispose()
        {
            Marshal.FreeHGlobal(m_pointer);
            m_pointer = IntPtr.Zero;
            m_pointerNative = (int*)0;
            m_size = new Size2(0, 0);
        }

        /// <summary>
        /// Gets the value at the given (pixel) location.
        /// </summary>
        /// <param name="xPos">The x position.</param>
        /// <param name="yPos">The y position.</param>
        public int GetValue(int xPos, int yPos)
        {
            return m_pointerNative[xPos + (yPos * m_size.Width)];
        }

        /// <summary>
        /// Sets all alpha values to one.
        /// </summary>
        public void SetAllAlphaValuesToOne_ARGB()
        {
            uint alphaByteValue = 0xFF000000;
            uint* pointerUInt = (uint*)m_pointerNative;
            for (int loopIndex = 0; loopIndex < m_countInts; loopIndex++)
            {
                pointerUInt[loopIndex] |= alphaByteValue;
            }
        }

        public void FillWith(IEnumerable<byte> data)
        {
            FillWith(data.ToArray());
        }

        public void FillWith(byte[] data)
        {
            Marshal.Copy(data, 0, m_pointer, data.Length);
        }

        /// <summary>
        /// Gets the total size of the buffer in bytes.
        /// </summary>
        public uint SizeInBytes
        {
            get
            {
                return (uint)(m_size.Width * m_size.Height * 4);
            }
        }

        public int CountInts
        {
            get { return m_countInts; }
        }

        /// <summary>
        /// Gets the width of the buffer.
        /// </summary>
        public int Width
        {
            get { return m_size.Width; }
        }

        /// <summary>
        /// Gets the pitch of the underlying texture data.
        /// (pitch = stride, see https://msdn.microsoft.com/en-us/library/windows/desktop/aa473780(v=vs.85).aspx )
        /// </summary>
        public int Pitch
        {
            get { return m_size.Width * 4; }
        }

        /// <summary>
        /// Gets the pitch of the underlying texture data.
        /// (pitch = stride, see https://msdn.microsoft.com/en-us/library/windows/desktop/aa473780(v=vs.85).aspx )
        /// </summary>
        public int Stride
        {
            get { return m_size.Width * 4; }
        }

        /// <summary>
        /// Gets the height of the buffer.
        /// </summary>
        public int Height
        {
            get { return m_size.Height; }
        }

        /// <summary>
        /// Gets the pixel size of this texture.
        /// </summary>
        public Size2 PixelSize
        {
            get
            {
                return m_size;
            }
        }

        /// <summary>
        /// Gets the pointer of the buffer.
        /// </summary>
        public IntPtr Pointer
        {
            get
            {
                if (m_pointer == IntPtr.Zero) { throw new ObjectDisposedException("MemoryMappedTextureFloat"); }
                return m_pointer;
            }
        }
    }
}
