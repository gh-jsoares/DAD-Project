using GIGAClient.domain;
using GIGAServerProto;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;


namespace GIGAClient.services
{
    class GIGAClientService
    {
        private GIGAClientObject gigaClientObject;


        public GIGAClientService(string name, string url, string file)
        {
            gigaClientObject = new GIGAClientObject(name, url, file);
        }

        public void read(string partitionId, string objecetId, string serverId)
        {
            throw new NotImplementedException();
        }

        public void write(string partitionId, string objectId, String value)
        {
            throw new NotImplementedException();
        }

        public void listServer(string serverId)
        {
            throw new NotImplementedException();
        }

        internal bool ShowStatus()
        {
            throw new NotImplementedException();
        }

        public void listGlobal()
        {
            throw new NotImplementedException();
        }

        public void wait(int time)
        {
            throw new NotImplementedException();
        }
        
    
    }
}
