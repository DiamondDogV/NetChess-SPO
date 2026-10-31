namespace Chess.Models
{
    public class ChessBoardModel
    {
        public string RoomName { get; set; }
        public bool IsFriendlyRoom { get; set; }
        public bool IsWhite { get; set; }
        public bool IsBlack { get; set; }

        public string SelectedColor { get; set; } = "White";

        public string[,] Board { get; set; }

        public ChessBoardModel()
        {
            Board = new string[8, 8];

            string[] blackBackRow = { "r", "n", "b", "q", "k", "b", "n", "r" };
            for (int i = 0; i < 8; i++)
            {
                Board[0, i] = "b_" + blackBackRow[i];
                Board[1, i] = "b_p";                  

                Board[6, i] = "w_p";                 
                Board[7, i] = "w_" + blackBackRow[i]; 
            }

        }
    }
}
