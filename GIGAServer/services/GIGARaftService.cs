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

                StartFollowerThread(partition.Value);
            }
            
        }

        //RAFT Methods

        public void FollowerState(GIGAPartition partition)
        {
            Console.WriteLine($"Thread for partition {partition.Partition.Name} started");

            //Reset everything
            partition.Partition.RaftObject.ResetVotes();

            var cancelled = partition.Partition.RaftObject.TokenSource.Token.WaitHandle.WaitOne(partition.Partition.RaftObject.Timeout);

            if (cancelled)
            {
                partition.Partition.RaftObject.ReturnToFollower();
                StartFollowerThread(partition);
            }
            else
                StartElection(partition);
        }

        public void StartElection(GIGAPartition partition)
        {
            partition.Partition.RaftObject.State = 2; //Turn into candidate

            partition.Partition.RaftObject.Votes[gigaServerService.Server.Name] = 1;  //Vote for self

            partition.Partition.RaftObject.Term++; //Increment term

            Console.WriteLine("START ELECTION");

            foreach (var partitionClient in partition.PartitionMap)
            {

                if(partitionClient.Key != gigaServerService.Server.Name)
                {
                    Console.WriteLine($"Sent vote for partition {partition.Partition.Name}");

                    Thread sendVoteThread = new Thread(() => partition.SendVoteRequest(gigaServerService.Server.Name, partitionClient.Value));
                    sendVoteThread.Start();
                }

            }


            //Wait for answers
            int majority;

            lock (partition.Partition.RaftObject)
            {
                while((majority = partition.Partition.RaftObject.CheckMajority()) == -1 && partition.Partition.RaftObject.State == 2)
                {
                    Console.WriteLine(majority);
                    Monitor.Wait(partition.Partition.RaftObject);
                }
            }

            Console.WriteLine(majority);

            //--- Election Outcome ---

            //My term is worse than the other server. I must be a follower.
            if(partition.Partition.RaftObject.State == 1)
                StartFollowerThread(partition);
            //I got the majority. I'm the new leader
            else if (majority == 1)
                BecomeLeader(partition);



                

        }

        public void StartFollowerThread(GIGAPartition partition)
        {
            Thread initState = new Thread(() => FollowerState(partition));
            initState.Start();
        }

        public void BecomeLeader(GIGAPartition partition)
        {
            Console.WriteLine("I'm the new leader");

            partition.Partition.RaftObject.State = 3; //Become Leader

            Console.WriteLine("Tell everyone you're the new leader");

            foreach (var partitionClient in partition.PartitionMap)
            {

                if (partitionClient.Key != gigaServerService.Server.Name)
                {
                    Console.WriteLine($"Sent info for partition {partition.Partition.Name}");

                    Thread sendLeaderThread = new Thread(() => partition.SendNewLeader(gigaServerService.Server.Name, partitionClient.Value));
                    sendLeaderThread.Start();
                }

            }
        }

    }
}
