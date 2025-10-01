namespace TrabalhoP2P;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

public class FileManager
{
    private readonly string _baseDir;
    private readonly Peer _peer;

    public bool IsReceivingOperation { get; private set; } = false;

    public FileManager(string baseDir, Peer peer)
    {
        _baseDir = baseDir;
        _peer = peer;
    }

    // Método para enviar arquivo (CREATE/MODIFY)
    public async Task SendFile(string filePath)
    {
        if (IsReceivingOperation) return;

        Console.WriteLine($"Preparando envio do arquivo: {filePath}");
        try
        {
            byte[] content = await File.ReadAllBytesAsync(filePath);
            string fileName = Path.GetFileName(filePath);

            string header = $"FILE:{fileName}:";
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);
            
            byte[] message = new byte[headerBytes.Length + content.Length];
            Buffer.BlockCopy(headerBytes, 0, message, 0, headerBytes.Length);
            Buffer.BlockCopy(content, 0, message, headerBytes.Length, content.Length);

            await SendToAllPeers(message);
        }
        catch (IOException e)
        {
            Console.WriteLine($"Erro ao ler o arquivo: {e.Message}");
        }
    }

    // Método para deletar arquivo
    public async Task DeleteFile(string fileName)
    {
        if (IsReceivingOperation) return;
        
        Console.WriteLine($"Solicitando deleção do arquivo: {fileName}");
        string message = $"DELETE:{fileName}";
        await SendToAllPeers(Encoding.UTF8.GetBytes(message));
    }

    private async Task SendToAllPeers(byte[] message)
    {
        Console.WriteLine($"Tentando enviar para peers: {string.Join(", ", _peer.KnownPeers)}");
        
        using (var client = new UdpClient())
        {
            foreach (var peerAddress in _peer.KnownPeers)
            {
                try
                {
                    string[] parts = peerAddress.Split(':');
                    string host = parts[0];
                    int port = int.Parse(parts[1]);
                    
                    await client.SendAsync(message, message.Length, host, port);
                    
                    Console.WriteLine($"Operação enviada com SUCESSO para: {peerAddress}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERRO ao enviar para {peerAddress}: {e.Message}");
                }
            }
        }
    }

    // Salva um arquivo recebido de outro peer
    public async Task SaveFile(string fileName, byte[] content)
    {
        IsReceivingOperation = true;
        try
        {
            string dest = Path.Combine(_baseDir, fileName);
            await File.WriteAllBytesAsync(dest, content);
            Console.WriteLine($"Arquivo salvo em: {dest}");
            await Task.Delay(100); 
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            IsReceivingOperation = false;
        }
    }

    // Deleta um arquivo com base em uma mensagem recebida
    public async Task DeleteReceivedFile(string fileName)
    {
        IsReceivingOperation = true;
        try
        {
            string filePath = Path.Combine(_baseDir, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine($"Arquivo deletado: {fileName}");
            }
            await Task.Delay(100);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            IsReceivingOperation = false;
        }
    }
}