﻿using System.Net.Mail;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiegoMoyanoProject.Models
{
    public class Email
    {
        private string mail;
        private string name;
        private float money;
        private string subject;
        private string body;

        public Email()
        {
        }

        public Email(string mail, string name, float money)
        {
            this.mail = mail;
            this.name = name;
            this.money = money;
        }
        public void Invest()
        {
            this.subject = "Mail Automatico invertir dinero";
            this.body = $"NOMBRE: {this.name}\n  CANTIDAD A INVERTIR: {this.money} \n";
        }
        public void Retire()
        {
            this.subject = "Mail Automatico retirar dinero";
            this.body = $"NOMBRE: {this.name}\n  CANTIDAD A Retirar: {this.money} \n";
        }
        public string Mail { get => mail; set => mail = value; }
        public string Name { get => name; set => name = value; }
        public float Money { get => money; set => money = value; }
        public string Subject { get => subject; set => subject = value; }
        public string Body { get => body; set => body = value; }
    }
}