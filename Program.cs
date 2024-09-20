using System;
using System.Windows.Forms;

namespace Letra_T
{
    public class Program 
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            //using (Game game = new Game(800, 600, "Letra T"))
            //{
            //    game.Run(60.0);
            //}
        }
    }
}
