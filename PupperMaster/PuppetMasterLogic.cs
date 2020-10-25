using System;
using Grpc.Net.Client;
using ProcessCreationService;

namespace PuppetMaster
{
    class PuppetMasterLogic
    {

        private GrpcChannel channel;
        private CommandListener.CommandListenerClient pcs;

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

        public bool SendCommand(string commandText)
        {
            CommandReply reply = pcs.ReceiveCommand(new CommandRequest
            {
                Command = commandText
            });

            return reply.Ok;
        }
    }
}