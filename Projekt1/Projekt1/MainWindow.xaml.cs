using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Projekt1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnNeu_Click(object sender, RoutedEventArgs e)
        {
            // Alle Felder leeren
            txtVorname.Clear();
            txtNachname.Clear();
            txtMail.Clear();
            txtStrasse.Clear();
            txtPLZ.Clear();
            txtOrt.Clear();
            cmbAnrede.SelectedIndex = -1;
            cmbLand.SelectedIndex = -1;
            dpGeburtsdatum.SelectedDate = null;

            // Fehlermeldungen löschen
            lblErrorAnrede.Text = "";
            lblErrorVorname.Text = "";
            lblErrorNachname.Text = "";
            lblErrorMail.Text = "";
            lblErrorPLZ.Text = "";
            lblFeedback.Text = "";
        }

        private void BtnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            BtnNeu_Click(sender, e); // Wiederverwenden
            MessageBox.Show("Alle Eingaben wurden gelöscht.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnSuchen_Click(object sender, RoutedEventArgs e)
        {
            if (txtNachname.Text == "Muster")
            {
                MessageBox.Show("Eintrag gefunden: Max Muster, Zürich", "Suche", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Kein Eintrag gefunden!", "Suche", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnAbsenden_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            lblErrorAnrede.Text = "";
            lblErrorVorname.Text = "";
            lblErrorNachname.Text = "";
            lblErrorMail.Text = "";
            lblErrorPLZ.Text = "";
            lblFeedback.Text = "";

            // Anrede prüfen
            if (cmbAnrede.SelectedItem == null)
            {
                lblErrorAnrede.Text = "Bitte Anrede auswählen.";
                isValid = false;
            }

            // Vorname prüfen
            if (string.IsNullOrWhiteSpace(txtVorname.Text))
            {
                lblErrorVorname.Text = "Vorname darf nicht leer sein.";
                isValid = false;
            }

            // Nachname prüfen
            if (string.IsNullOrWhiteSpace(txtNachname.Text))
            {
                lblErrorNachname.Text = "Nachname darf nicht leer sein.";
                isValid = false;
            }

            // E-Mail prüfen (einfache Regex)
            if (!Regex.IsMatch(txtMail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblErrorMail.Text = "Bitte gültige E-Mail eingeben.";
                isValid = false;
            }

            // PLZ prüfen
            if (!Regex.IsMatch(txtPLZ.Text, @"^\d{4,5}$"))
            {
                lblErrorPLZ.Text = "PLZ muss aus 4–5 Ziffern bestehen.";
                isValid = false;
            }

            // Feedback
            if (isValid)
            {
                string anrede = ((ComboBoxItem)cmbAnrede.SelectedItem).Content.ToString();
                lblFeedback.Text = $"✅ Vielen Dank {anrede} {txtNachname.Text} für Ihre Eingabe!";
                lblFeedback.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                lblFeedback.Text = "❌ Bitte Eingaben korrigieren.";
                lblFeedback.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}
