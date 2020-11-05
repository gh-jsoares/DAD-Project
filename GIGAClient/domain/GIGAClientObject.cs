using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;
using GIGAClient.Commands;
using GIGAClient.services;
using Grpc.Core;

namespace GIGAClient.domain
{
    class GIGAClientObject
    {
        public Dictionary<string, string> ServerMap { get; } = new Dictionary<string, string>();
        public Dictionary<string, GIGAPartitionObject> PartitionMap { get; } = new Dictionary<string, GIGAPartitionObject>();
        public string Username { get; }
        public string Url { get; }
        public string File { get; }
        public string AttachedServer { get; }

        public GIGAClientObject(string username, string url, string file)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            File = file ?? throw new ArgumentNullException(nameof(file));
            AttachedServer = null;
        }

        internal void ShowStatus()
        {
            Console.WriteLine("Client: \"{0}\" @ {1}", Username, Url);
            Console.WriteLine("Current Partitions:");
            foreach (GIGAPartitionObject partition in PartitionMap.Values)
            {
                partition.ShowStatus();
            }
        }
    }
}
