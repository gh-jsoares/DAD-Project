using GIGAServer.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GIGAServer.domain
{
    class GIGARaftObject
    {
        public int State { get; set; }   //1 - Follower; 2 - Candidate ; 3 - Leader
        public int Term { get; set; }
        public int Timeout { get; set; }
        public Dictionary<string, int> Votes { get; set; }  // -1 -> No vote yet received ; 0 -> Received no vote ; 1 -> Received vote

        public GIGARaftObject(Dictionary<string, GIGAServerObject> Servers)
        {
            State = 1;
            Term = 0;

            Random rnd = new Random();
            Timeout = rnd.Next(1000, 2000);


            Votes = new Dictionary<string, int>();
            foreach (var server in Servers)
            {
                Votes.Add(server.Key, -1);           
            }
        }

        public void CheckMajority()
        {
            int numVotes = 0;

            foreach (var vote in Votes)
            {
                if (vote.Value == 1)
                    numVotes++;
            }

            if (numVotes > Votes.Count / 2)
                Console.WriteLine("Majority achieved");

        }

        public void HandleVoteReply(GIGAPartitionProto.VoteReply reply)
        {
            if (reply.VoteForCandidate)
                Votes[reply.ServerId] = 1;
            else
                Votes[reply.ServerId] = 0;

            CheckMajority();
        }

        public bool VoteReplyDecision()
        {

            if (State == 2)
                return false;
            else
                return true;
        }

        public void ResetVotes()
        {
            Votes = Votes.ToDictionary(vote => vote.Key, p => -1);
        }
    }
}
