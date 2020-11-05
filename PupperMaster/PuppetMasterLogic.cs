using System;
using System.Collections.Generic;
using GIGAPuppetMaster.domain;
using Grpc.Net.Client;
using PuppetMaster.Commands;

namespace PuppetMaster
{
    class PuppetMasterLogic
    {
        private CommandExecutor commands;
        public Dictionary<string, GIGAServerObject> ServerMap { get; } = new Dictionary<string, GIGAServerObject>();
        public Dictionary<string, GIGAPartitionObject> PartitionMap { get; } = new Dictionary<string, GIGAPartitionObject>();
        public Dictionary<string, string> ClientMap { get; set; } = new Dictionary<string, string>();
        public int ReplicationFactor { get; set; }

        public PuppetMasterLogic()
        {

            commands = new CommandExecutor();

        }

        public string SendCommand(string commandText)
        {
            return commands.Run(commandText, this);
        
        }

    }
}