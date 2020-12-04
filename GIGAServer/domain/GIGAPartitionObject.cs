using GIGAServer.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GIGAPartitionProto;

namespace GIGAServer.domain
{
    class GIGAPartitionObject
    {
        public string Name { get; }
        public Dictionary<string, GIGAServerObject> Servers { get; }
        public int ReplicationFactor { get; }
        public GIGAServerObject MasterServer { get; set; }
        internal GIGARaftObject RaftObject { get; set; }

        private Dictionary<string, GIGAObject> objects;
        private List<GIGALogEntry> log;

        public GIGAPartitionObject(string name, int replicationFactor, GIGAServerObject[] servers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ReplicationFactor = replicationFactor;
            if (servers == null) throw new ArgumentNullException(nameof(servers));
            Servers = servers.ToDictionary(server => server.Name, server => server);
            objects = new Dictionary<string, GIGAObject>();
            log = new List<GIGALogEntry>();
        }

        internal void ShowStatus()
        {
            Console.WriteLine("\tPartition \"{0}\":\n\t\tCurrent Master: {1}", Name, MasterServer.ToString());
            foreach (KeyValuePair<string, GIGAServerObject> entry in Servers)
            {
                Console.WriteLine("\t\t{0}", entry.Value.ToString());
            }
        }

        internal GIGAObject Read(string name)
        {
            // TODO LOCKED
            return GetObject(name);
        }

        internal bool HasServer(string serverId)
        {
            return Servers.ContainsKey(serverId);
        }

        public GIGALogEntry CreateLog(string name, string value)
        {
            var entry = new GIGALogEntry(RaftObject.Term, log.Count, new GIGAObject(this, name, value));
            log.Add(entry);

            return entry;
        }

        internal List<GIGAPartitionObjectID> GetPartitionObjectIDList()
        {
            return objects.Values.Select(obj => obj.ToPartitionObjectID()).ToList();
        }

        public GIGAObject GetObject(string name)
        {
            return objects.GetValueOrDefault(name, null);
        }

        public bool HasObject(string name)
        {
            return objects.ContainsKey(name);
        }

        //Raft
        public void CreateRaftObject()
        {
            this.RaftObject = new GIGARaftObject(Servers);
        }

        public void SetNewMasterServer(string serverId)
        {
            foreach (var server in Servers)
            {
                if (server.Value.Name == serverId)
                {
                    MasterServer = server.Value;
                    break;
                }
            }

            Console.WriteLine($"New master server is {serverId}");
        }

        public void RemoveServer(string server_id)
        {
            Servers.Remove(server_id);
            RaftObject.Votes.Remove(server_id);
        }

        public int GetLastLogIndex()
        {
            return GetLastCommittedLog()?.Index ?? 0;
        }

        public int GetLastLogTerm()
        {
            return GetLastCommittedLog()?.Term ?? 0;
        }

        public GIGALogEntry GetLastCommittedLog()
        {
            try
            {
                return log.FindAll(entry => entry.Committed).Last();
            }
            catch (Exception)
            {
                // not found
                return null;
            }
        }

        private void simpleWrite(GIGAObject obj)
        {
            if (objects.ContainsKey(obj.Name))
            {
                objects[obj.Name] = obj;
            }
            else
            {
                objects.Add(obj.Name, obj);
            }
        }

        public void CommitEntry(GIGALogEntry entry)
        {
            simpleWrite(entry.Data);
            entry.Committed = true;
        }

        public GIGALogEntry AppendEntries(GIGALogEntry entry, List<GIGALogEntry> lastCommittedEntries)
        {
            var lastCommitted = GetLastCommittedLog();

            if (lastCommittedEntries.Count() != 0)
            {
                if (lastCommitted != null)
                {
                    var receivedLastCommitted = lastCommittedEntries.First();
                    if (lastCommitted.Term != receivedLastCommitted.Term ||
                        lastCommitted.Index != receivedLastCommitted.Index)
                    {
                        return lastCommitted;
                    }

                    // I have inconsistent logs, but ive received the correct ones, so ill apply them
                    var lastIndex = log.FindIndex(logEntry
                        => logEntry.Term == lastCommitted.Term && logEntry.Index == lastCommitted.Index);
                    log.RemoveRange(lastIndex, log.Count() - lastIndex);
                }

                foreach (var lastCommittedEntry in lastCommittedEntries)
                {
                    CommitEntry(lastCommittedEntry);
                }
            }

            // Finally, add the current entry
            log.Add(entry);

            return null;
        }

        public GIGALogEntry GetLastCommittedLogBefore(int index, int term)
        {
            return log.FindLast(entry => entry.Index == index && entry.Term == term && entry.Committed);
        }
    }
}