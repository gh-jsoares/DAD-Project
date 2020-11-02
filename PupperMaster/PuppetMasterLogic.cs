using System;
using System.Collections.Generic;
using Grpc.Net.Client;
using PuppetMaster.Commands;

namespace PuppetMaster
{
    class PuppetMasterLogic
    {
        CommandExecutor commands;

        public PuppetMasterLogic()
        {

            commands = new CommandExecutor();

        }

        public string SendCommand(string commandText)
        {
            return commands.Run(commandText, this);
        
        }

        public Dictionary<string, string> ClientMap { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> ServerMap { get; set; } = new Dictionary<string, string>();
        public int ReplicationFactor { get; set; }
        public Dictionary<string, string[]> PartitionMap { get; set; } = new Dictionary<string, string[]>();
    }
}