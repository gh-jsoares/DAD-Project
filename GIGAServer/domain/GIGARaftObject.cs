using GIGAPartitionProto;
using GIGAServer.domain;
using GIGAServer.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GIGAServer.domain
{
    class GIGARaftObject
    {

        const int minTimeout = 10000;
        const int maxTimeout = 20000;

        public int State { get; set; }   //1 - Follower; 2 - Candidate ; 3 - Leader
        public int Term { get; set; }
        public int Timeout { get; set; }
        public Dictionary<string, int> Votes { get; set; }  // -1 -> No vote yet received ; 0 -> Received no vote ; 1 -> Received vote

        public string votedFor;

        public bool electionTimeout;

        GIGAServerObject server;

        public CancellationTokenSource TokenSource { get; set; }

        

        public GIGARaftObject(Dictionary<string, GIGAServerObject> Servers)
        {
            State = 1;
            Term = 0;
            TokenSource = new CancellationTokenSource();

            Random rnd = new Random();
            Timeout = rnd.Next(minTimeout, maxTimeout);

            Votes = new Dictionary<string, int>();
            foreach (var server in Servers)
            {
                Votes.Add(server.Key, -1);           
            }

            electionTimeout = false;
        }

        //Returns -1 - Not enough responses ; 0 - No Majority ; 1 - Majority
        public int CheckMajority()
        {
            int votesFor = 0;
            int votesAgainst = 0;

            foreach (var vote in Votes)
            {
                if (vote.Value == 1)
                    votesFor++;
                else if (vote.Value == 0)
                    votesAgainst++;
            }

            if (votesFor > Votes.Count / 2)
            {
                Console.WriteLine("Majority achieved");
                return 1;
            }else if (votesAgainst > Votes.Count / 2)
            {
                Console.WriteLine("Majority not achieved");
                return 0;
            }

            return -1;

        }

        public void HandleVoteReply(GIGAPartitionProto.VoteReply reply)
        {
            if (reply.VoteForCandidate)
                Votes[reply.ServerId] = 1;
            else
            {
                Votes[reply.ServerId] = 0;               
            }
                 
        }

        public bool VoteReplyDecision(VoteRequest request)
        {

            if(State == 1)
                TokenSource.Cancel();

            CheckTerm(request.Term);
            Console.WriteLine($"Voted for {votedFor}");

            if (request.Term < Term)
                return false;
            else if ((votedFor == null || votedFor == request.ServerId) /*&& request.LastLogIndex >= 1 && request.LastLogTerm >= 1*/)
                return true;
            
                

            return false;
        }

        public void AcceptNewLeader(string serverId, GIGAPartitionObject partitionObject)
        {
            partitionObject.SetNewMasterServer(serverId);

            ReturnToFollower();
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
            if(term > Term)
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
            Random rnd = new Random();
            Timeout = rnd.Next(minTimeout, maxTimeout);
        }
    }
}
