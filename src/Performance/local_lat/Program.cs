﻿using System;
using NetMQ;

namespace local_lat
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("usage: local_lat <bind-to> <message-size> <roundtrip-count>");
                return 1;
            }

            string bindTo = args[0];
            int messageSize = int.Parse(args[1]);
            int roundtripCount = int.Parse(args[2]);

            using (var context = NetMQContext.Create())
            using (var rep = context.CreateResponseSocket())
            {
                rep.Bind(bindTo);

                var msg = new Msg();
                msg.InitEmpty();

                for (int i = 0; i != roundtripCount; i++)
                {
                    rep.Receive(ref msg);
                    if (msg.Count != messageSize)
                    {
                        Console.WriteLine("message of incorrect size received. Received: " + msg.Count + " Expected: " + messageSize);
                        return -1;
                    }

                    rep.Send(ref msg, SendReceiveOptions.None);
                }

                msg.Close();
            }

            return 0;
        }
    }
}