using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace AudioPlayer
{
    public partial class loginForm : Form
    {
        public loginForm()
        {
            InitializeComponent();
        }
        Hashtable account = new Hashtable();
        private LinkedList<string> accountList = new LinkedList<string>();
        public string CreateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
        private void buttonLogin_Click(object sender, EventArgs e)
        {
             if (account.ContainsKey(textBoxUserName.Text) && account.ContainsValue(CreateMD5Hash(textBoxPassword.Text)))
             {             
                musicPlayer form = new musicPlayer();        
                form.Show();               
            }               
            else             
            {                
                MessageBox.Show("Invalid Input");               
            }              
        }

        private void buttonRegistry_Click(object sender, EventArgs e)
        {
            String username = textBoxUserName.Text;
            String password = textBoxPassword.Text;
            using (var sw = new StreamWriter("account.csv"))
            {
                
                account.Add(username, CreateMD5Hash(password));
                 
                accountList.AddLast(username + "," + CreateMD5Hash(password));
                string security = username + "," + CreateMD5Hash(password);
                sw.WriteLine(security);         
            }
        }
    }
}
