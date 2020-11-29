using GIGAServer.domain;
using GIGAServer.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GIGAServer.services
{
    class GIGARaftService
    {

        private GIGAPartitionService gigaPartitionService;

        private GIGAServerService gigaServerService;

        public GIGARaftService(GIGAPartitionService gigaPartitionService, GIGAServerService gigaServerService)
        {
            Thread.Sleep(2000);

            this.gigaPartitionService = gigaPartitionService;
            this.gigaServerService = gigaServerService;

            Random rnd = new Random();
            foreach (KeyValuePair<string, GIGAPartition> partition in gigaPartitionService.Partitions)
            {
                partition.Value.NewRaftObject();

                Console.WriteLine($"Thread for partition {partition.Key} about to start");

                Thread initState = new Thread(() => FollowerState(partition.Value));
                initState.Start();
            }
            
        }

        //RAFT Methods

        public void FollowerState(GIGAPartition partition)
        {
            Console.WriteLine($"Thread for partition {partition.Partition.Name} started");


            Thread.Sleep(partition.Partition.RaftObject.Timeout);

            StartElection(partition);
        }

        public void StartElection(GIGAPartition partition)
        {
            partition.Partition.RaftObject.State = 2; //Turn into candidate

            partition.Partition.RaftObject.Votes++; //Vote for self

            Console.WriteLine("START ELECTION");

            foreach (var partitionClient in partition.PartitionMap.Values)
            {

                Console.WriteLine($"Sent vote for partition {partition.Partition.Name}");

                Thread initState = new Thread(() => partition.SendVoteRequest(gigaServerService.Server.Name, partitionClient));
                initState.Start();

            }

        }



    }
}
