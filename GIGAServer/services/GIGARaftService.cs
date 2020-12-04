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

            gigaPartitionService.InitRaftService(this);
            this.gigaPartitionService = gigaPartitionService;
            this.gigaServerService = gigaServerService;

            Random rnd = new Random();
            foreach (KeyValuePair<string, GIGAPartition> partition in gigaPartitionService.Partitions)
            {
                if (partition.Value.Partition.HasServer(gigaServerService.Server.Name))
                {
                    partition.Value.NewRaftObject();

                    Console.WriteLine($"Thread for partition {partition.Key} about to start");

                    StartFollowerThread(partition.Value);
                }
            }
        }

        //RAFT Methods

        public void FollowerState(GIGAPartition partition)
        {
            Console.WriteLine($"Thread for partition {partition.Partition.Name} started");

            bool cancelled =
                partition.Partition.RaftObject.TokenSource.Token.WaitHandle.WaitOne(partition.Partition.RaftObject
                    .Timeout);

            CheckLeaderAlive(partition, cancelled);

            if (!cancelled && !gigaServerService.Frozen)
                StartElection(partition);
        }

        public void StartElection(GIGAPartition partition)
        {
            partition.Partition.RaftObject.NewTerm(); //Increment term

            partition.Partition.RaftObject.State = 2; //Turn into candidate

            partition.Partition.RaftObject.VoteForSelf(gigaServerService.Server.Name); //Vote for self

            Console.WriteLine($"START ELECTION ; Term: {partition.Partition.RaftObject.Term}");

            foreach (var partitionClient in partition.PartitionMap)
            {
                if (partitionClient.Key != gigaServerService.Server.Name)
                {
                    Console.WriteLine($"Sent vote for partition {partition.Partition.Name}");

                    Thread sendVoteThread = new Thread(() =>
                        partition.SendVoteRequest(gigaServerService.Server.Name, partitionClient.Key,
                            partitionClient.Value));
                    sendVoteThread.Start();
                }
            }


            //Election Timeout

            partition.Partition.RaftObject.electionTimeout = false;
            CancellationTokenSource timeoutToken = new CancellationTokenSource();
            Thread electionTimeout = new Thread(() => ElectionTimeoutThread(partition, timeoutToken));
            electionTimeout.Start();


            //Wait for answers
            int majority;

            lock (partition.Partition.RaftObject)
            {
                while ((majority = partition.Partition.RaftObject.CheckMajority()) != 1 &&
                       partition.Partition.RaftObject.State == 2 && !partition.Partition.RaftObject.electionTimeout)
                {
                    Monitor.Wait(partition.Partition.RaftObject);
                }
            }

            timeoutToken.Cancel();
            Console.WriteLine(majority);

            //--- Election Outcome ---

            //My term is worse than the other server. I must be a follower.
            if (partition.Partition.RaftObject.State == 1)
                StartFollowerThread(partition);
            //I got the majority. I'm the new leader
            else if (majority == 1)
                BecomeLeader(partition);
            else
                StartElection(partition);
        }

        public void CheckLeaderAlive(GIGAPartition partition, bool cancelled)
        {
            if (cancelled || gigaServerService.Frozen)
            {
                StartFollowerThread(partition);
            }
        }

        public void StartFollowerThread(GIGAPartition partition)
        {
            partition.Partition.RaftObject.ReturnToFollower();
            Thread initState = new Thread(() => FollowerState(partition));
            initState.Start();
        }

        public void BecomeLeader(GIGAPartition partition)
        {
            Console.WriteLine("I'm the new leader");

            partition.Partition.RaftObject.State = 3; //Become Leader
            partition.Partition.MasterServer = gigaServerService.Server;

            Console.WriteLine("Tell everyone you're the new leader");

            while (partition.Partition.RaftObject.State == 3)
            {
                BroadcastAppendEntries(partition, null);
                Thread.Sleep(5000);
            }

            StartFollowerThread(partition);
        }

        public void BroadcastAppendEntries(GIGAPartition partition, GIGALogEntry entry)
        {
            int successful = 0;
            Console.WriteLine(partition.PartitionMap.Count);

            while (successful < Math.Ceiling(partition.PartitionMap.Count / 2.0) && partition.Partition.RaftObject.State == 3)
            {
                foreach (var partitionClient in partition.PartitionMap)
                {
                    if (partitionClient.Key != gigaServerService.Server.Name && !gigaServerService.Frozen)
                    {
                        Console.WriteLine($"Sent heartbeat for partition {partition.Partition.Name}");

                        new Thread(() =>
                        {
                            partition.SendAppendNewEntries(gigaServerService.Server.Name, partitionClient.Key,
                                partitionClient.Value, entry);

                            // accepted
                            successful++;
                        }).Start();
                    }
                }

                Thread.Sleep(100);
            }
        }

        public void ElectionTimeoutThread(GIGAPartition partition, CancellationTokenSource timeoutToken)
        {
            partition.Partition.RaftObject.ResetTimeout(); //Set new timeout

            bool cancelled = timeoutToken.Token.WaitHandle.WaitOne(partition.Partition.RaftObject.Timeout);

            if (!cancelled)
            {
                partition.Partition.RaftObject.electionTimeout = true;

                lock (partition.Partition.RaftObject)
                {
                    Monitor.Pulse(partition.Partition.RaftObject);
                }
            }
        }
    }
}