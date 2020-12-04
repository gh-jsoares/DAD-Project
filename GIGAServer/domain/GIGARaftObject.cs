using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GIGAPartitionProto;

namespace GIGAServer.domain
{
    internal class GIGARaftObject
    {
        private const int minTimeout = 10000;
        private const int maxTimeout = 20000;

        public bool electionTimeout;

        private GIGAServerObject server;

        public string votedFor;


        public GIGARaftObject(Dictionary<string, GIGAServerObject> Servers)
        {
            State = 1;
            Term = 0;
            TokenSource = new CancellationTokenSource();

            var rnd = new Random();
            Timeout = rnd.Next(minTimeout, maxTimeout);

            Votes = new Dictionary<string, int>();
            foreach (var server in Servers) Votes.Add(server.Key, -1);

            electionTimeout = false;
        }

        public int State { get; set; } //1 - Follower; 2 - Candidate ; 3 - Leader
        public int Term { get; set; }
        public int Timeout { get; set; }

        public Dictionary<string, int>
            Votes { get; set; } // -1 -> No vote yet received ; 0 -> Received no vote ; 1 -> Received vote

        public CancellationTokenSource TokenSource { get; set; }

        //Returns -1 - Not enough responses ; 0 - No Majority ; 1 - Majority
        public int CheckMajority()
        {
            var votesFor = 0;
            var votesAgainst = 0;

            foreach (var vote in Votes)
                if (vote.Value == 1)
                    votesFor++;
                else if (vote.Value == 0)
                    votesAgainst++;

            if (votesFor > Votes.Count / 2)
            {
                Console.WriteLine("Majority achieved");
                return 1;
            }

            if (votesAgainst > Votes.Count / 2)
            {
                Console.WriteLine("Majority not achieved");
                return 0;
            }

            return -1;
        }

        public void HandleVoteReply(VoteReply reply)
        {
            if (reply.VoteForCandidate)
                Votes[reply.ServerId] = 1;
            else
                Votes[reply.ServerId] = 0;
        }

        public bool VoteReplyDecision(VoteRequest request, GIGAPartitionObject partition)
        {
            if (State == 1)
                TokenSource.Cancel();

            CheckTerm(request.Term);
            Console.WriteLine($"Voted for {votedFor}");

            return request.Term >= Term && (votedFor == null || votedFor == request.ServerId) &&
                   request.LastLogIndex >= partition.GetLastLogIndex() &&
                   request.LastLogTerm >= partition.GetLastLogTerm();
        }

        public void AcceptNewLeader(string serverId, int leaderTerm, GIGAPartitionObject partitionObject)
        {
            if (leaderTerm >= Term)
            {
                partitionObject.SetNewMasterServer(serverId);

                ReturnToFollower();
            }
        }

        public void ReturnToFollower()
        {
            State = 1;
            ResetVotes();
            ResetTimeout();
            TokenSource.Cancel();
            TokenSource = new CancellationTokenSource();

            lock (this)
            {
                Monitor.Pulse(this);
            }
        }

        public void VoteForSelf(string server_id)
        {
            Votes[server_id] = 1;
            votedFor = server_id;
        }

        public void CheckTerm(int term)
        {
            if (term > Term)
            {
                Term = term;
                votedFor = null;
                ReturnToFollower();
            }
        }

        public void NewTerm()
        {
            Term++;
            votedFor = null;
        }

        public void ResetVotes()
        {
            Votes = Votes.ToDictionary(vote => vote.Key, p => -1);
        }

        public void ResetTimeout()
        {
            var rnd = new Random();
            Timeout = rnd.Next(minTimeout, maxTimeout);
        }
    }
}