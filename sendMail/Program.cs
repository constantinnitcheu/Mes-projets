using HuffmanTest;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;

namespace sendMail
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Veuillez entrer les éléments de facturation du client :");
            string input = Console.ReadLine();
            HuffmanTree huffmanTree = new HuffmanTree();

            // Construire l'arbre de huffman
            huffmanTree.Build(input);

            // Encoder
            BitArray encoded = huffmanTree.Encode(input);

            //on retourne le nombre d'octets du message lu au clavier
            Console.WriteLine();
            Console.WriteLine("Votre texte est de " + input.Length + " octets");

            //on ouvre une session de connexion sur gmail à travers smtp
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("isjtestmail@gmail.com"); //expéditeur
                mail.To.Add("constantinnitcheu6@gmail.com"); //recepteur
                mail.Subject = "Informations client";  //objet

                //ic on affiche d'abord le message compréssé chez l'expéditeur
                Console.WriteLine();

                int cpt = 0; //j'initialise un compte ici pour avoir le nombre de bit à la fin de la compression
                Console.Write("Compressé: ");
                foreach (bool bit in encoded)
                {
                    Console.Write((bit ? 1 : 0) + "");
                    cpt = cpt + 1;
                }
                Console.WriteLine();
                Console.WriteLine("le texte compréssé est de " + (cpt / 8 + 1) + " octets");

                Console.WriteLine();
                Console.WriteLine("En cours d'envoi à " + mail.To + "...");

                /*foreach (bool bit in encoded)
                {
                    mail.Body = (bit ? 1 : 0) + "";
                }
                Console.WriteLine();*/

                string chaine;
                string message = "";
                foreach (bool bit in encoded)
                {
                    chaine = (bit ? 1 : 0) + "";

                    message = $"{message}{chaine}";
                }

                mail.Body = message;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("isjtestmail@gmail.com", "testisj2019");
                SmtpServer.EnableSsl = true;

                Console.WriteLine();
                SmtpServer.Send(mail);
                Console.WriteLine("Votre mail à été envoyé avec succes!!!");//Message succes
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //cette partie du code permet de lire les mails de l'adresse passée en paramètre
            try
            {
                using (Pop3 pop3 = new Pop3())
            {
                pop3.ConnectSSL("pop.gmail.com");  // or ConnectSSL for SSL      
                pop3.Login("constantinnitcheu6@gmail.com", "mayelle2010");
                List<string> uids = pop3.GetAll();
                foreach (string uid in uids)
                {
                    IMail email = new MailBuilder()
                        .CreateFromEml(pop3.GetMessageByUID(uid));

                        Console.WriteLine("");
                        Console.WriteLine(email.Date);
                        Console.WriteLine(email.From);
                        Console.WriteLine(email.Subject);
                    }
                    pop3.Close();
            }
            }
            catch (Limilabs.Client.ServerException e)
            {
                Console.WriteLine(e.ToString());
            }



        }
    }
}
