using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.BD;


namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddAppointmentPage.xaml
    /// </summary>
    public partial class AddAppointmentPage : Page
    {
        public AddAppointmentPage()
        {
            InitializeComponent();
            LoadPets();
        }
        private void LoadPets()
        {
            try
            {
                var pets = App.bd.Animal.ToList();
                PetComboBox.ItemsSource = pets;
                PetComboBox.DisplayMemberPath = "name"; 
                PetComboBox.SelectedValuePath = "animal_id"; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки питомцев: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SaveVisitButton_Click(object sender, RoutedEventArgs e)
        {
            if (PetComboBox.SelectedItem is Animal selectedPet)
            {

                var medicalRecord = new MedicalRecord
                {
                    animal_id = selectedPet.animal_id,

                    diagnosis = DiagnosisTextBox.Text, 
                    treatment = PrescriptionsTextBox.Text 
                };
                
                App.bd.MedicalRecord.Add(medicalRecord);
               

                if (!string.IsNullOrWhiteSpace(AnalysisTextBox.Text))
                {
                    var testType = new TestTypes
                    {
                        test_type_name = AnalysisTextBox.Text,
                    };

                    App.bd.TestTypes.Add(testType);
                }

                App.bd.SaveChanges();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите питомца.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearInputs()
        {
            DiagnosisTextBox.Clear();
            PrescriptionsTextBox.Clear();
            AnalysisTextBox.Clear();
            PetComboBox.SelectedIndex = -1; 
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
           
            ClearInputs();
         
        }


        private void ClearDiagnosisButton_Click(object sender, RoutedEventArgs e)
        {
            DiagnosisTextBox.Clear();
        }

        private void ClearPrescriptionsButton_Click(object sender, RoutedEventArgs e)
        {
            PrescriptionsTextBox.Clear();
        }

        private void ClearAnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            AnalysisTextBox.Clear();
        }

        private void ClearPetComboBoxButton_Click(object sender, RoutedEventArgs e)
        {
            PetComboBox.SelectedItem = null; // Очистка выбранного элемента
        }


    }
}