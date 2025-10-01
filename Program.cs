using TrabalhoP2P;

using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Uso: dotnet run <porta> <peers.txt> <diretorio>");
            return;
        }

        int port = int.Parse(args[0]);
        string peersFile = args[1];
        string dir = args[2];

        // Cria o diretório se não existir
        Directory.CreateDirectory(dir);

        Peer peer = new Peer($"127.0.0.1:{port}", dir);

        // Carrega os peers conhecidos do arquivo
        string[] lines = await File.ReadAllLinesAsync(peersFile);
        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                peer.KnownPeers.Add(line.Trim());
            }
        }

        FileManager fileManager = new FileManager(dir, peer);
        PeerServer server = new PeerServer(peer, port, fileManager);
        DirectoryWatcher watcher = new DirectoryWatcher(dir, fileManager);

        // Inicia o servidor e o watcher em tarefas separadas
        Task serverTask = server.Start();
        watcher.Start();

        Console.WriteLine($"Peer rodando na porta {port} e diretório {dir}");
        Console.WriteLine($"Peers conhecidos: {string.Join(", ", peer.KnownPeers)}");

        await serverTask;
    }
}