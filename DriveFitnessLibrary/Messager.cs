using System.Windows;

namespace DriveFitnessLibrary
{
    public class Messager
    {
        public void SuccessMessage(string message)
        {
            MessageBox.Show(message, "Операция успешна!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void QuestionMessage(string message)
        {
            MessageBox.Show(message, "Операция успешна!", MessageBoxButton.OK, MessageBoxImage.Question);
        }

        public void ErrorMessage(string message)
        {
            MessageBox.Show(message, "Операция успешна!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
