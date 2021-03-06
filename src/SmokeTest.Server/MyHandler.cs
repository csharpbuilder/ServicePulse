﻿using System;
using NServiceBus;

namespace SmokeTest.Server.Particular.Core.Deliberately.Insanely.Long.NamespaceToEmulateTheCrazyNamespaceLengthsPeopleGiveNamespacesInTheirSystems
{
    public class MyHandler : IHandleMessages<MyMessage>
    {
        public void Handle(MyMessage message)
        {
            Console.WriteLine(@"Message received. Id: {0}", message.Id);

            if (Program.goodretries || !message.KillMe)
            {
                return;
            }

            if (!Program.emulateFailures)
            {
                RandomException(message.SomeText);
            }
            else
            {
                throw new InvalidOperationException(message + "Uh oh...Nulls are bad MK");
            }


        }

        static void RandomException(string message)
        {
            var rand = new Random();
            var wheelOfFortune = rand.Next(3) + 1;

            switch (wheelOfFortune)
            {
                case 1:
                    throw new OutOfMemoryException(message + "Uh oh...I forget Why this happened");
                case 2:
                    throw new NullReferenceException(message + "Uh oh...Nulls are bad MK");
                case 3:
                    throw new DivideByZeroException(message + "Uh oh...Zero and Divisions just don't get along");
                default:
                    throw new Exception(message + "Uh oh...Because, Reasons");
            }

        }
    }

}