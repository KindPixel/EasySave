using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace EasySave_Interface.Model
{
    class SocketServer
    {
        public ViewModel.ViewModel viewModel;
        public Socket server;
        public Socket client;
        private byte[] byteReceive = new byte[1024];
        private string data = null;
        private string jsondata;
        private System.Timers.Timer timer;

        public SocketServer(ViewModel.ViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.timer = new System.Timers.Timer(1000);
            timer.Elapsed += this.UpdateInfo;
            timer.AutoReset = true;
        }
        public void ToConnect()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName()); // a modifie rselon l'hote
                IPAddress ip = host.AddressList[0]; //IPAddress.Parse("");
                IPEndPoint endPoint = new IPEndPoint(ip, 9999);
                
                // Créer une socket TCP/IP 
                this.server = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                this.server.Bind(endPoint); //connexion sur adresse locale pour le moment
                this.server.Listen(10);
                this.viewModel.DisplaySocket("SocketServer ready");
                this.viewModel.DisplaySocket("Wait for client connexion...");
                WaitForClient();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void AcceptCallback(IAsyncResult ar)
        {
            byte[] byteReceive = new byte[1024];
            int length;
            try
            {
                
                this.client = this.server.EndAccept(out byteReceive,out length, ar);
                
                this.viewModel.DisplaySocket("...connection etablished");
                WaitForReceive();
                Send($@"You are connected to {Dns.GetHostName()} Easysave server");
                this.timer.Start();
                //WaitForClient();

            }
            catch (Exception e)
            {
                //ClientClose();
                Console.WriteLine(e.ToString());
            }
        }

        public void WaitForClient()
        {
            this.server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                this.client.EndReceive(ar);
                this.data = Encoding.ASCII.GetString(this.byteReceive, 0, this.byteReceive.Length);
                

                if (this.data.IndexOf("<ACTION>", 0) > -1)
                {
                    int index = this.data.IndexOf("$", 0);
                    string ids = data.Substring(index + 1);
                    index = ids.IndexOf("<END>", 0);
                    ids = ids.Substring(0, index);

                    Backup backupfind = this.viewModel.backups.Find(backup => backup.ID == ids);

                    if (this.data.IndexOf("PLAY$", 0) > -1)
                    {
                        this.viewModel.ResumeBackup(backupfind);
                    }
                    if (this.data.IndexOf("PAUSE$", 0) > -1)
                    {
                        this.viewModel.PauseBackup(backupfind);
                    }
                    if (this.data.IndexOf("STOP$", 0) > -1)
                    { 
                        this.viewModel.StopBackup(backupfind);
                    }
                }
                if (this.data.IndexOf("<Ikillyou>", 0) > -1)
                {
                    ClientClose();
                }
                else
                {
                    this.viewModel.DisplaySocket(data);

                }

                this.byteReceive = new byte[1024];
                this.data = null;
                WaitForReceive();
            }
            catch (Exception e) 
            {
                this.viewModel.DisplaySocket("Client was disconnected");
                Console.WriteLine(e.ToString());
            }
        }
        public void WaitForReceive()
        {
            this.client.BeginReceive(byteReceive, 0, byteReceive.Length, SocketFlags.None,new AsyncCallback(ReceiveCallback), null);
        }



        public void Send(string texte)
        {
            try {
            byte[] msg = Encoding.ASCII.GetBytes(texte);
            int byteSent = this.client.Send(msg); } catch{}
            
        }

        public void UpdateInfo(Object source, System.Timers.ElapsedEventArgs e)
        {
            this.jsondata = Newtonsoft.Json.JsonConvert.SerializeObject(this.viewModel.backups);
            Send($"<UPDATE>{this.jsondata}");
        }

        public void ClientClose()
        {
           try { this.client.Shutdown(SocketShutdown.Both); } catch { }
            this.client.Close();
        }

        public void Close()
        {
            if(this.client != null) Send("<Ikillyou>");
            
            this.timer.Stop();
            this.timer.Dispose();

            if(this.server != null) this.server.Close();

            this.viewModel.DisplaySocket("SocketServer is shutdown"); 
        }

    }
}
