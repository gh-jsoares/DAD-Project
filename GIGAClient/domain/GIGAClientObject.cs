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
        public Dictionary<string, string> ServerMap { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string[]> PartitionMap { get; set; } = new Dictionary<string, string[]>();
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





    }
}
