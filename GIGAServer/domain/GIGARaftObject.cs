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
        public CancellationTokenSource TokenSource { get; set; }

        public bool votedFor;

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

                if(reply.Term > Term)
                {
                    Term = reply.Term;      //Update term

                    ReturnToFollower();
                }
                
            }
                

            
        }

        public bool VoteReplyDecision(int term)
        {
            TokenSource.Cancel();

            if (State == 2 || Term > term)
                return false;
            else
                return true;
        }

        public void AcceptNewLeader(string serverId, GIGAPartitionObject partitionObject)
        {
            State = 1;

            partitionObject.SetNewMasterServer(serverId);
        }

        public void ReturnToFollower()
        {
            State = 1;
            ResetVotes();
            ResetTimeout();
            TokenSource = new CancellationTokenSource();

            lock (this)
            {
                Monitor.Pulse(this);
            }
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
