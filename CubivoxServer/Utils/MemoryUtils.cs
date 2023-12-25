﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubivoxServer.Utils
{
    public class MemoryUtils
    {
        public static void Fill3DArray<T>(ref T[,,] array, T value, int totalLength) where T : unmanaged
        {
            unsafe
            {
                fixed (T* start = &array[0, 0, 0])
                {
                    T* current = start;
                    var span = new Span<T>(current, totalLength);
                    span.Fill(value);
                }
            }
        }

        public static T[] ThreeDArrayTo1DArray<T>(ref T[,,] array, int totalLength) where T : unmanaged
        {
            T[] result = new T[totalLength];
            unsafe
            {
                fixed (T* start = &array[0, 0, 0])
                {
                    T* current = start;
                    var span = new Span<T>(current, totalLength);
                    fixed(T* singleStart = &result[0])
                    {
                        T* singleCurrent = singleStart;
                        var resultSpan = new Span<T>(singleCurrent, totalLength);
                        span.CopyTo(resultSpan);
                    }
                }
            }
            return result;
        }
    }
}
