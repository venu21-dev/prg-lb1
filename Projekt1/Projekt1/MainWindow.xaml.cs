#nullable disable
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Runtime.CompilerServices;

namespace Projekt1
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // ObservableCollections für Datenbindung (erfüllt A8)
        public ObservableCollection<Kontakt> Kontakte { get; set; }
        public ObservableCollection<Kontakt> GefilterteKontakte { get; set; }

        private string _suchText = "";
        public string SuchText
        {
            get => _suchText;
            set
            {
                _suchText = value;
                OnPropertyChanged();
                FilterKontakte();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeData();
        }

        private void InitializeData()
        {
            Kontakte = new ObservableCollection<Kontakt>();
            GefilterteKontakte = new ObservableCollection<Kontakt>();

            // Beispieldaten hinzufügen (nur im RAM, gehen beim Schliessen verloren)
            Kontakte.Add(new Kontakt
            {
                Anrede = "Herr",
                Vorname = "Max",
                Nachname = "Mustermann",
                Email = "max@example.com",
                Strasse = "Musterstrasse 123",
                PLZ = "8000",
                Ort = "Zürich",
                Land = "Schweiz",
                Geburtsdatum = new DateTime(1985, 5, 15)
            });

            Kontakte.Add(new Kontakt
            {
                Anrede = "Frau",
                Vorname = "Anna",
                Nachname = "Muster",
                Email = "anna@example.com",
                Strasse = "Beispielweg 456",
                PLZ = "3000",
                Ort = "Bern",
                Land = "Schweiz",
                Geburtsdatum = new DateTime(1990, 8, 22)
            });

            Kontakte.Add(new Kontakt
            {
                Anrede = "Herr",
                Vorname = "Peter",
                Nachname = "Schmidt",
                Email = "peter@example.com",
                PLZ = "4000",
                Ort = "Basel",
                Land = "Schweiz"
            });

            // DataGrid binden
            dgKontakte.ItemsSource = GefilterteKontakte;
            FilterKontakte();
            UpdateStatistiken();
        }

        private void FilterKontakte()
        {
            GefilterteKontakte.Clear();

            var gefiltert = string.IsNullOrWhiteSpace(SuchText)
                ? Kontakte
                : Kontakte.Where(k =>
                    k.Vorname.ToLower().Contains(SuchText.ToLower()) ||
                    k.Nachname.ToLower().Contains(SuchText.ToLower()) ||
                    k.Email.ToLower().Contains(SuchText.ToLower()) ||
                    k.Ort.ToLower().Contains(SuchText.ToLower()));

            foreach (var kontakt in gefiltert)
            {
                GefilterteKontakte.Add(kontakt);
            }

            UpdateStatistiken();
        }

        private void UpdateStatistiken()
        {
            var gesamt = Kontakte.Count;
            var mitEmail = Kontakte.Count(k => !string.IsNullOrWhiteSpace(k.Email));
            var mitAdresse = Kontakte.Count(k => !string.IsNullOrWhiteSpace(k.Strasse));
            var gefiltert = GefilterteKontakte.Count;

            lblStatistiken.Text = $"Gesamt: {gesamt} Kontakte\n" +
                                 $"Angezeigt: {gefiltert} Kontakte\n" +
                                 $"Mit E-Mail: {mitEmail} ({(gesamt > 0 ? mitEmail * 100 / gesamt : 0)}%)\n" +
                                 $"Mit Adresse: {mitAdresse} ({(gesamt > 0 ? mitAdresse * 100 / gesamt : 0)}%)";
        }

        private void BtnNeu_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            HideFeedback();
        }

        private void ClearForm()
        {
            txtVorname.Clear();
            txtNachname.Clear();
            txtMail.Clear();
            txtStrasse.Clear();
            txtPLZ.Clear();
            txtOrt.Clear();
            cmbAnrede.SelectedIndex = 0; // "Bitte wählen..."
            cmbLand.SelectedIndex = 0;   // "Schweiz"
            dpGeburtsdatum.SelectedDate = null;

            ClearValidationErrors();
        }

        private void ClearValidationErrors()
        {
            lblErrorAnrede.Text = "";
            lblErrorVorname.Text = "";
            lblErrorNachname.Text = "";
            lblErrorMail.Text = "";
            lblErrorPLZ.Text = "";
        }

        private void BtnLoeschen_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Möchten Sie wirklich alle Eingaben löschen?",
                "Bestätigung",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ClearForm();
                ShowFeedback("🗑️ Alle Eingaben wurden gelöscht.", true);
            }
        }

        private void BtnSuchen_Click(object sender, RoutedEventArgs e)
        {
            SuchText = txtSuche.Text;
            var anzahl = GefilterteKontakte.Count;
            ShowFeedback($"🔍 {anzahl} Kontakte gefunden für '{SuchText}'", true);
        }

        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                SaveKontakt();
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;
            ClearValidationErrors();

            // Anrede prüfen
            if (cmbAnrede.SelectedIndex <= 0)
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
            else if (txtVorname.Text.Length < 2)
            {
                lblErrorVorname.Text = "Vorname muss mindestens 2 Zeichen haben.";
                isValid = false;
            }

            // Nachname prüfen
            if (string.IsNullOrWhiteSpace(txtNachname.Text))
            {
                lblErrorNachname.Text = "Nachname darf nicht leer sein.";
                isValid = false;
            }
            else if (txtNachname.Text.Length < 2)
            {
                lblErrorNachname.Text = "Nachname muss mindestens 2 Zeichen haben.";
                isValid = false;
            }

            // E-Mail prüfen (robustere Validierung)
            if (string.IsNullOrWhiteSpace(txtMail.Text))
            {
                lblErrorMail.Text = "E-Mail darf nicht leer sein.";
                isValid = false;
            }
            else if (!IsValidEmail(txtMail.Text))
            {
                lblErrorMail.Text = "Bitte gültige E-Mail eingeben.";
                isValid = false;
            }

            // PLZ prüfen (nur wenn ausgefüllt)
            if (!string.IsNullOrWhiteSpace(txtPLZ.Text) && !IsValidPLZ(txtPLZ.Text))
            {
                lblErrorPLZ.Text = "PLZ muss aus 4–5 Ziffern bestehen.";
                isValid = false;
            }

            return isValid;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPLZ(string plz)
        {
            return Regex.IsMatch(plz, @"^\d{4,5}$");
        }

        private void SaveKontakt()
        {
            try
            {
                var neuerKontakt = new Kontakt
                {
                    Anrede = ((ComboBoxItem)cmbAnrede.SelectedItem).Content.ToString(),
                    Vorname = txtVorname.Text.Trim(),
                    Nachname = txtNachname.Text.Trim(),
                    Email = txtMail.Text.Trim(),
                    Strasse = txtStrasse.Text.Trim(),
                    PLZ = txtPLZ.Text.Trim(),
                    Ort = txtOrt.Text.Trim(),
                    Land = ((ComboBoxItem)cmbLand.SelectedItem).Content.ToString(),
                    Geburtsdatum = dpGeburtsdatum.SelectedDate
                };

                // Prüfen ob Kontakt bereits existiert (gleiche E-Mail)
                var existing = Kontakte.FirstOrDefault(k =>
                    k.Email.Equals(neuerKontakt.Email, StringComparison.OrdinalIgnoreCase));

                if (existing != null)
                {
                    var result = MessageBox.Show(
                        $"Ein Kontakt mit der E-Mail '{neuerKontakt.Email}' existiert bereits.\nMöchten Sie ihn überschreiben?",
                        "Kontakt bereits vorhanden",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Kontakt überschreiben
                        var index = Kontakte.IndexOf(existing);
                        Kontakte[index] = neuerKontakt;
                        ShowFeedback($"✅ Kontakt '{neuerKontakt.Vorname} {neuerKontakt.Nachname}' wurde aktualisiert!", true);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    // Neuen Kontakt hinzufügen
                    Kontakte.Add(neuerKontakt);
                    ShowFeedback($"✅ Vielen Dank '{neuerKontakt.Anrede} {neuerKontakt.Nachname}'Ihre Kontaktdaten wurden gespeichert!", true);
                }

                FilterKontakte();
                ClearForm();
            }
            catch (Exception ex)
            {
                ShowFeedback($"❌ Fehler beim Speichern: {ex.Message}", false);
            }
        }

        private void ShowFeedback(string message, bool isSuccess)
        {
            lblFeedback.Text = message;

            if (isSuccess)
            {
                feedbackBorder.Background = new SolidColorBrush(Color.FromRgb(40, 167, 69)) { Opacity = 0.2 };
                feedbackBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(40, 167, 69));
                lblFeedback.Foreground = new SolidColorBrush(Color.FromRgb(40, 167, 69));
            }
            else
            {
                feedbackBorder.Background = new SolidColorBrush(Color.FromRgb(220, 53, 69)) { Opacity = 0.2 };
                feedbackBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 53, 69));
                lblFeedback.Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 107));
            }

            feedbackBorder.Visibility = Visibility.Visible;
        }

        private void HideFeedback()
        {
            feedbackBorder.Visibility = Visibility.Collapsed;
        }

        private void DgKontakte_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgKontakte.SelectedItem is Kontakt kontakt)
            {
                LoadKontaktToForm(kontakt);
            }
        }

        private void LoadKontaktToForm(Kontakt kontakt)
        {
            try
            {
                // Anrede setzen
                for (int i = 0; i < cmbAnrede.Items.Count; i++)
                {
                    if (((ComboBoxItem)cmbAnrede.Items[i]).Content.ToString() == kontakt.Anrede)
                    {
                        cmbAnrede.SelectedIndex = i;
                        break;
                    }
                }

                txtVorname.Text = kontakt.Vorname;
                txtNachname.Text = kontakt.Nachname;
                txtMail.Text = kontakt.Email;
                txtStrasse.Text = kontakt.Strasse;
                txtPLZ.Text = kontakt.PLZ;
                txtOrt.Text = kontakt.Ort;

                // Land setzen
                for (int i = 0; i < cmbLand.Items.Count; i++)
                {
                    if (((ComboBoxItem)cmbLand.Items[i]).Content.ToString() == kontakt.Land)
                    {
                        cmbLand.SelectedIndex = i;
                        break;
                    }
                }

                dpGeburtsdatum.SelectedDate = kontakt.Geburtsdatum;

                ClearValidationErrors();
                ShowFeedback($"📝 Kontakt '{kontakt.Vorname} {kontakt.Nachname}' in Formular geladen.", true);
            }
            catch (Exception ex)
            {
                ShowFeedback($"❌ Fehler beim Laden: {ex.Message}", false);
            }
        }

        // Test-Methoden für A4 (Validierung testen)
        public bool TestValidation()
        {
            bool allTestsPassed = true;

            try
            {
                // Test 1: Leere Pflichtfelder sollten Validierung fehlschlagen lassen
                ClearForm();
                if (ValidateForm())
                {
                    Console.WriteLine("❌ Test 1 fehlgeschlagen: Leere Felder sollten Validierung fehlschlagen lassen");
                    allTestsPassed = false;
                }
                else
                {
                    Console.WriteLine("✅ Test 1 bestanden: Leere Pflichtfelder korrekt erkannt");
                }

                // Test 2: Gültige Daten sollten Validierung bestehen
                ClearForm();
                cmbAnrede.SelectedIndex = 1; // "Herr"
                txtVorname.Text = "Max";
                txtNachname.Text = "Mustermann";
                txtMail.Text = "max@example.com";
                txtPLZ.Text = "8000";

                if (!ValidateForm())
                {
                    Console.WriteLine("❌ Test 2 fehlgeschlagen: Gültige Daten sollten Validierung bestehen");
                    allTestsPassed = false;
                }
                else
                {
                    Console.WriteLine("✅ Test 2 bestanden: Gültige Daten korrekt akzeptiert");
                }

                // Test 3: Ungültige E-Mail sollte erkannt werden
                txtMail.Text = "ungueltige-email-ohne-at";
                if (ValidateForm())
                {
                    Console.WriteLine("❌ Test 3 fehlgeschlagen: Ungültige E-Mail sollte erkannt werden");
                    allTestsPassed = false;
                }
                else
                {
                    Console.WriteLine("✅ Test 3 bestanden: Ungültige E-Mail korrekt erkannt");
                }

                // Test 4: Ungültige PLZ sollte erkannt werden
                txtMail.Text = "max@example.com"; // E-Mail wieder korrekt
                txtPLZ.Text = "abc123"; // Ungültige PLZ
                if (ValidateForm())
                {
                    Console.WriteLine("❌ Test 4 fehlgeschlagen: Ungültige PLZ sollte erkannt werden");
                    allTestsPassed = false;
                }
                else
                {
                    Console.WriteLine("✅ Test 4 bestanden: Ungültige PLZ korrekt erkannt");
                }

                ClearForm();
                return allTestsPassed;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Fehler beim Testen: {ex.Message}");
                return false;
            }
        }

        // Öffentliche Methode zum Ausführen der Tests (für A4)
        public void RunValidationTests()
        {
            var result = TestValidation();
            ShowFeedback(result ?
                "🧪 Alle Validierungstests erfolgreich bestanden!" :
                "⚠️ Einige Validierungstests fehlgeschlagen - Details in der Konsole",
                result);
        }

        private void cmbLand_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbAnrede_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    // Datenmodell für Kontakte (erfüllt A4 + A8)
    public class Kontakt : INotifyPropertyChanged
    {
        private string _anrede = "";
        private string _vorname = "";
        private string _nachname = "";
        private string _email = "";
        private string _strasse = "";
        private string _plz = "";
        private string _ort = "";
        private string _land = "";
        private DateTime? _geburtsdatum;

        public string Anrede
        {
            get => _anrede;
            set { _anrede = value ?? ""; OnPropertyChanged(); }
        }

        public string Vorname
        {
            get => _vorname;
            set { _vorname = value ?? ""; OnPropertyChanged(); }
        }

        public string Nachname
        {
            get => _nachname;
            set { _nachname = value ?? ""; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value ?? ""; OnPropertyChanged(); }
        }

        public string Strasse
        {
            get => _strasse;
            set { _strasse = value ?? ""; OnPropertyChanged(); }
        }

        public string PLZ
        {
            get => _plz;
            set { _plz = value ?? ""; OnPropertyChanged(); }
        }

        public string Ort
        {
            get => _ort;
            set { _ort = value ?? ""; OnPropertyChanged(); }
        }

        public string Land
        {
            get => _land;
            set { _land = value ?? ""; OnPropertyChanged(); }
        }

        public DateTime? Geburtsdatum
        {
            get => _geburtsdatum;
            set { _geburtsdatum = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Anrede} {Vorname} {Nachname}";
        }
    }

}
