﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace cod4_compileToolsXaml
{
    public static class BinaryUtility
    {
        public static IEnumerable<byte> GetByteStream(BinaryReader reader)
        {
            const int bufferSize = 1024;
            byte[] buffer;
            
            do
            {
                buffer = reader.ReadBytes(bufferSize);
                foreach (var d in buffer) 
                { 
                    yield return d; 
                }
            } 
            while (bufferSize == buffer.Length);
        }

        public static void Replace(BinaryReader reader, BinaryWriter writer, IEnumerable<Tuple<byte[], byte[]>> searchAndReplace)
        {
            foreach (byte d in Replace(GetByteStream(reader), searchAndReplace)) { writer.Write(d); }
        }

        public static IEnumerable<byte> Replace(IEnumerable<byte> source, IEnumerable<Tuple<byte[], byte[]>> searchAndReplace)
        {
            foreach (var s in searchAndReplace)
            {
                source = Replace(source, s.Item1, s.Item2);
            }

            return source;
        }

        public static IEnumerable<byte> Replace(IEnumerable<byte> input, IEnumerable<byte> from, IEnumerable<byte> to)
        {
            var fromEnumerator = from.GetEnumerator();
            fromEnumerator.MoveNext();

            int match = 0;

            foreach (var data in input)
            {
                if (data == fromEnumerator.Current)
                {
                    match++;

                    if (fromEnumerator.MoveNext()) 
                    { 
                        continue; 
                    }

                    foreach (byte d in to) 
                    { 
                        yield return d; 
                    }

                    match = 0;
                    fromEnumerator.Reset();
                    fromEnumerator.MoveNext();

                    continue;
                }

                if (0 != match)
                {
                    foreach (byte d in from.Take(match)) 
                    { 
                        yield return d; 
                    }

                    match = 0;
                    fromEnumerator.Reset();
                    fromEnumerator.MoveNext();
                }

                yield return data;
            }

            if (0 != match)
            {
                foreach (byte d in from.Take(match)) 
                { 
                    yield return d; 
                }
            }
        }
    }
}