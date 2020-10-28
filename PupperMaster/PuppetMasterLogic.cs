using System;
using System.Collections.Generic;
using Grpc.Net.Client;
using ProcessCreationService;

namespace PuppetMaster
{
    class PuppetMasterLogic
    {

        private GrpcChannel channel;
        private CommandListener.CommandListenerClient pcs;

        private Dictionary<string,string> clientMap = new Dictionary<string, string>();
        private Dictionary<string, string> serverMap = new Dictionary<string, string>();


        public PuppetMasterLogic(ConfigReader cr)
        {

            cr.printPcss();

            if(cr == null)
            {
                AppContext.SetSwitch(
            "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                channel = GrpcChannel.ForAddress("http://localhost:10000");
                pcs = new CommandListener.CommandListenerClient(channel);
            }
            else                                                        //Apenas implementado para quando so ha 1 PCS
            {
                AppContext.SetSwitch(
            "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                channel = GrpcChannel.ForAddress(cr.Pcss[0]);
                pcs = new CommandListener.CommandListenerClient(channel);
            }

            

        }

        public CommandReply SendCommand(string commandText)
        {
            CommandReply reply = pcs.ReceiveCommand(new CommandRequest
            {
                Command = commandText
            });

            if (reply.Ok)
                GatherInfo(reply);

            return reply;
        }

        private void GatherInfo(CommandReply reply)
        {
            switch (reply.Type)
            {
                case "C":
                    clientMap.Add(reply.Id, reply.Url);
                    break;

                case "S":
                    serverMap.Add(reply.Id, reply.Url);
                    break;

                default:
                    break;
            }
        }
    }
}