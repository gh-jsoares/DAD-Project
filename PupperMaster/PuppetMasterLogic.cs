using System;
using System.Collections.Generic;
using Grpc.Net.Client;
using PuppetMaster.Commands;

namespace PuppetMaster
{
    class PuppetMasterLogic
    {

        private Dictionary<string,string> clientMap = new Dictionary<string, string>();
        private Dictionary<string, string> serverMap = new Dictionary<string, string>();
        private Dictionary<string, string[]> partitionMap = new Dictionary<string, string[]>();
        private int replicationFactor;

        CommandExecutor commands;

        public PuppetMasterLogic()
        {

            commands = new CommandExecutor();

        }

        public String SendCommand(string commandText)
        {
            return commands.Run(commandText, this);
        
        }


        public Dictionary<string, string> ClientMap { get => clientMap; set => clientMap = value; }
        public Dictionary<string, string> ServerMap { get => serverMap; set => serverMap = value; }
        public int ReplicationFactor { get => replicationFactor; set => replicationFactor = value; }
        public Dictionary<string, string[]> PartitionMap { get => partitionMap; set => partitionMap = value; }
    }
}