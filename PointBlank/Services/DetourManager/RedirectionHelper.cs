﻿/*
The MIT License (MIT)
Copyright (c) 2015 Sebastian Schöner
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using PointBlank.API;

namespace PointBlank.Services.DetourManager
{
    internal class RedirectionHelper
    {
        #region Public Functions
        public static bool DetourFunction(IntPtr ptrOriginal, IntPtr ptrModified)
        {
            try
            {
                if(IntPtr.Size == sizeof(Int32))
                {
                    unsafe
                    {
                        byte* ptrFrom = (byte*)ptrOriginal.ToPointer();

                        *(ptrFrom + 1) = 0xBB;
                        *((uint*)(ptrFrom + 2)) = (uint)ptrModified.ToInt32();
                        *(ptrFrom + 11) = 0xFF;
                        *(ptrFrom + 12) = 0xE3;
                    }
                }
                else if(IntPtr.Size == sizeof(Int64))
                {
                    unsafe
                    {
                        byte* ptrFrom = (byte*)ptrOriginal.ToPointer();

                        *ptrFrom = 0x49;
                        *(ptrFrom + 1) = 0xBB;
                        *((ulong*)(ptrFrom + 2)) = (ulong)ptrModified.ToInt64();
                        *(ptrFrom + 10) = 0x41;
                        *(ptrFrom + 11) = 0xFF;
                        *(ptrFrom + 12) = 0xE3;
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logging.LogError("Error adding detour!", ex);
                return false;
            }
        }

        public static bool RevertDetour(OffsetBackup backup)
        {
            try
            {
                unsafe
                {
                    byte* ptrOriginal = (byte*)backup.Method.ToPointer();

                    *ptrOriginal = backup.A;
                    *(ptrOriginal + 1) = backup.B;
                    *(ptrOriginal + 10) = backup.C;
                    *(ptrOriginal + 11) = backup.D;
                    *(ptrOriginal + 12) = backup.E;
                    if (IntPtr.Size == sizeof(Int32))
                        *((uint*)(ptrOriginal + 2)) = backup.F32;
                    else
                        *((ulong*)(ptrOriginal + 2)) = backup.F64;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logging.LogError("Error reverting detour!", ex);
                return false;
            }
        }
        #endregion

        #region SubClasses
        internal class OffsetBackup
        {
            #region Variables
            public IntPtr Method;

            public byte A, B, C, D, E;
            public ulong F64;
            public uint F32;
            #endregion

            public OffsetBackup(IntPtr method)
            {
                Method = method;

                unsafe
                {
                    byte* ptrMethod = (byte*)method.ToPointer();

                    A = *ptrMethod;
                    B = *(ptrMethod + 1);
                    C = *(ptrMethod + 10);
                    D = *(ptrMethod + 11);
                    E = *(ptrMethod + 12);
                    if (IntPtr.Size == sizeof(Int32))
                        F32 = *((uint*)(ptrMethod + 2));
                    else
                        F64 = *((ulong*)(ptrMethod + 2));
                }
            }
        }
        #endregion
    }
}
