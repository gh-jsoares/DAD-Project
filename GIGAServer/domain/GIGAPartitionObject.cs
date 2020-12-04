﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GIGAServer.domain
{
    internal class GIGAPartitionObject
    {
        private readonly List<GIGALogEntry> log;

        private readonly Dictionary<string, GIGAObject> objects;

        public GIGAPartitionObject(string name, int replicationFactor, GIGAServerObject[] servers)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ReplicationFactor = replicationFactor;
            if (servers == null) throw new ArgumentNullException(nameof(servers));
            Servers = servers.ToDictionary(server => server.Name, server => server);
            objects = new Dictionary<string, GIGAObject>();
            log = new List<GIGALogEntry>();
        }

        public string Name { get; }
        public Dictionary<string, GIGAServerObject> Servers { get; }
        public int ReplicationFactor { get; }
        public GIGAServerObject MasterServer { get; set; }
        internal GIGARaftObject RaftObject { get; set; }

        internal void ShowStatus()
        {
            if (MasterServer == null)
                Console.WriteLine($"\tPartition \"{Name}\":\n\t\tCurrent Master: NO MASTER");
            else
                Console.WriteLine($"\tPartition \"{Name}\":\n\t\tCurrent Master: {MasterServer}");
            foreach (var entry in Servers) Console.WriteLine($"\t\t{entry.Value}");
        }

        internal GIGAObject Read(string name)
        {
            return GetObject(name);
        }

        internal bool HasServer(string serverId)
        {
            return Servers.ContainsKey(serverId);
        }

        public GIGALogEntry CreateLog(string name, string value)
        {
            var entry = new GIGALogEntry(RaftObject.Term, log.Count, new GIGAObject(this, name, value, log.Count));
            log.Add(entry);

            return entry;
        }

        internal List<GIGAObject> GetObjects()
        {
            return objects.Values.ToList();
        }

        public GIGALogEntry GetEntry(string name)
        {
            return log[log.FindIndex(obj => obj.Data.Name == name)];
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
            RaftObject = new GIGARaftObject(Servers);
        }

        public void SetNewMasterServer(string serverId)
        {
            foreach (var server in Servers)
                if (server.Value.Name == serverId)
                {
                    MasterServer = server.Value;
                    break;
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
                objects[obj.Name] = obj;
            else
                objects.Add(obj.Name, obj);
        }

        public void CommitEntry(GIGALogEntry entry)
        {
            entry.Data.Timestamp = entry.Index;
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
                        return lastCommitted;

                    // I have inconsistent logs, but ive received the correct ones, so ill apply them
                    var lastIndex = log.FindIndex(logEntry
                        => logEntry.Term == lastCommitted.Term && logEntry.Index == lastCommitted.Index);
                    log.RemoveRange(lastIndex, log.Count() - lastIndex);
                }

                foreach (var lastCommittedEntry in lastCommittedEntries) CommitEntry(lastCommittedEntry);
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