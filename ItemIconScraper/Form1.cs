using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Http;

namespace ItemIconScraper
{
    public partial class Form1 : Form
    {
        public string source;
        static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void Run_Button_Click(object sender, EventArgs e)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            string iconString;
            int itemId;
            int itemIteration = 1;
            int itemMaxIteration = 5855;

            while (itemIteration <= itemMaxIteration)
            {
                counter_Label.Text = itemIteration.ToString();
                HttpResponseMessage response = await client.GetAsync("https://wotlk.evowow.com/?icon=" + itemIteration.ToString());
                string source = await response.Content.ReadAsStringAsync();

                Match iconMatch = Regex.Match(source, "^.+(g_icons).+$", RegexOptions.Multiline);
                Match itemMatch = Regex.Match(source, "^.+(g_items).+$", RegexOptions.Multiline);
                if (iconMatch.Success && itemMatch.Success)
                {
                    List<string> iconIdBuilder = new List<string>();
                    int i = iconMatch.Value.IndexOf(':');
                    while (iconMatch.Value[i + 2] != '"')
                    {
                        iconIdBuilder.Add(iconMatch.Value[i + 2].ToString());
                        i++;
                    }
                    iconString = String.Join("", iconIdBuilder);

                    string original = itemMatch.Value;
                    while (original.Contains('['))
                    {
                        StringBuilder sb = new StringBuilder(original);
                        List<string> itemIdBuilder = new List<string>();
                        int index = original.IndexOf('[');
                        while (char.IsDigit(original[index + 1]))
                        {
                            itemIdBuilder.Add(original[index + 1].ToString());
                            index++;
                        }
                        try
                        {
                            itemId = int.Parse(String.Join("", itemIdBuilder));
                            dict.Add(itemId, iconString);
                        }
                        catch { }
                        sb.Remove(original.IndexOf('['), 1);
                        original = sb.ToString();
                    }
                }
                foreach (var item in dict)
                {
                    File.AppendAllText("C:\\Users\\Magmonix\\Documents\\Test.txt", "{" + item.Key + " , \"" + item.Value + "\"},\n");
                }
                dict.Clear();
                itemIteration++;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
