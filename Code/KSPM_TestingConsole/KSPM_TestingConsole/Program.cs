﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSPM.Network.Server;
using KSPM.IO.Logging;
using KSPM.Diagnostics;
using KSPM.Globals;
using KSPM.Network.Client.RemoteServer;
using KSPM.Network.Client;
using KSPM.Game;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

using System.Threading;

namespace KSPM_TestingConsole
{
    class Program
    {
        static long totalMessages = 0;
        static long callCounter = 0;
        static void Main(string[] args)
        {
            KSPMGlobals.Globals.InitiLogging(Log.LogginMode.Console, false);
            System.Timers.Timer eventRiser = new System.Timers.Timer(10000);
            eventRiser.Elapsed += new System.Timers.ElapsedEventHandler(eventRiser_Elapsed);

			////Server test
			
            ServerSettings gameSettings = null;
            if (ServerSettings.ReadSettings(out gameSettings) == KSPM.Network.Common.Error.ErrorType.Ok)
            {
                GameServer server = new GameServer(ref gameSettings);
                KSPMGlobals.Globals.SetServerReference(ref server);
                server.UDPMessageArrived += new KSPM.Network.Common.Events.UDPMessageArrived(server_UDPMessageArrived);
                server.TCPMessageArrived += server_TCPMessageArrived;
                server.StartServer();
                eventRiser.Enabled = true;
                Console.ReadLine();
                eventRiser.Enabled = false;
                server.DisconnectAll();
                Console.WriteLine(string.Format("Event raised:{0} times, total of the messages: {1}", Program.callCounter, Program.totalMessages));
                Console.ReadLine();
                server.ShutdownServer();
            }
            Console.ReadLine();

        }

        static void server_TCPMessageArrived(object sender, KSPM.Network.Common.Messages.Message message)
        {
            if( message.Command == KSPM.Network.Common.Messages.Message.CommandType.User)
            {
                KSPMGlobals.Globals.KSPMServer.ClientsManager.TCPSelectiveBroadcast(message);
            }
        }

        static void server_UDPMessageArrived(object sender, KSPM.Network.Common.Messages.Message message)
        {
            //Console.WriteLine( string.Format("{0}-{1}", ((ServerSideClient)sender).Id, message.MessageBytesSize.ToString()));
            if( message.Command == KSPM.Network.Common.Messages.Message.CommandType.User)
            {
                KSPMGlobals.Globals.Log.WriteTo("USER");
                KSPMGlobals.Globals.KSPMServer.ClientsManager.UDPSelectiveBroacast(message);
            }/*
            else
            {
                KSPMGlobals.Globals.KSPMServer.ClientsManager.UDPBroadcastClients(message);
            }
            */
            //((ServerSideClient)sender).IOUDPMessagesPool.Recycle(message);
        }

        static void eventRiser_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Program.callCounter++;
            Program.totalMessages += KSPMGlobals.Globals.KSPMServer.outgoingMessagesQueue.DirtyCount;
            //KSPMGlobals.Globals.Log.WriteTo(KSPMGlobals.Globals.KSPMServer.outgoingMessagesQueue.DirtyCount.ToString());
        }
    }
}
