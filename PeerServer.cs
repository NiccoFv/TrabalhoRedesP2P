using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TrabalhoP2P
{
    public class PeerServer
    {
        private readonly Peer _peer;
        private readonly FileManager _fileManager;
        private readonly int _port;

        public PeerServer(Peer peer, int port, FileManager fileManager)
        {
            _peer = peer;
            _port = port;
            _fileManager = fileManager;
        }

        public async Task Start()
        {
            using (UdpClient listener = new UdpClient(_port))
            {
                Console.WriteLine($"Peer escutando na porta {_port}");

                while (true)
                {
                    try
                    {
                        UdpReceiveResult result = await listener.ReceiveAsync();
                        byte[] receivedBytes = result.Buffer;
                        await ProcessMessage(receivedBytes);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Erro ao receber mensagem: {e.Message}");
                    }
                }
            }
        }

        private async Task ProcessMessage(byte[] data)
        {
            string command = Encoding.UTF8.GetString(data);

            try
            {
                if (command.StartsWith("FILE:"))
                {
                    await ProcessFileMessage(data);
                }
                else if (command.StartsWith("DELETE:"))
                {
                    await ProcessDeleteMessage(command);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao processar comando: {e.Message}");
            }
        }

        private async Task ProcessFileMessage(byte[] data)
        {
            int firstColon = Array.IndexOf(data, (byte)':');
            int secondColon = -1;
            for (int i = firstColon + 1; i < data.Length; i++)
            {
                if (data[i] == (byte)':')
                {
                    secondColon = i;
                    break;
                }
            }

            if (secondColon > -1)
            {
                string header = Encoding.UTF8.GetString(data, 0, secondColon);
                string[] parts = header.Split(':');
                string fileName = parts[1];

                int contentStartIndex = secondColon + 1;
                int contentLength = data.Length - contentStartIndex;
                byte[] fileContent = new byte[contentLength];
                Array.Copy(data, contentStartIndex, fileContent, 0, contentLength);

                Console.WriteLine($"Recebendo arquivo: {fileName}");
                await _fileManager.SaveFile(fileName, fileContent);
            }
        }

        private async Task ProcessDeleteMessage(string message)
        {
            string[] parts = message.Split(':');
            if (parts.Length >= 2)
            {
                string fileName = parts[1];
                Console.WriteLine($"Recebendo comando DELETE: {fileName}");
                await _fileManager.DeleteReceivedFile(fileName);
            }
        }
    }
}