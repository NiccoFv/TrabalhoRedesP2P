namespace TrabalhoP2P;
using System.Collections.Generic;

public class Peer
{
    public string Address { get; set; }
    public HashSet<string> KnownPeers { get; set; }
    public string Directory { get; set; }

    public Peer(string address, string directory)
    {
        Address = address;
        Directory = directory;
        KnownPeers = new HashSet<string>();
    }
}