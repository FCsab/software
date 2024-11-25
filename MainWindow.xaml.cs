using System.Windows;

namespace EscapeFromPrison
{
    public partial class MainMenu : Window
    {
        private int selectedMapSize = 7;
        private string selectedDifficulty = "Normal";

        public MainMenu()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (SevenBySevenRadioButton.IsChecked == true)
                selectedMapSize = 7;
            else if (EightByEightRadioButton.IsChecked == true)
                selectedMapSize = 8;
            else if (NineByNineRadioButton.IsChecked == true)
                selectedMapSize = 9;

            if (EasyRadioButton.IsChecked == true)
                selectedDifficulty = "Easy";
            else if (NormalRadioButton.IsChecked == true)
                selectedDifficulty = "Normal";
            else if (HardRadioButton.IsChecked == true)
                selectedDifficulty = "Hard";

            GameWindow gameWindow = new GameWindow(selectedMapSize, selectedDifficulty);
            gameWindow.Show();
            this.Close();
        }
    }
}