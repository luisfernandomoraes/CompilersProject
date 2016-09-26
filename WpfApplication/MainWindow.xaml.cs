using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Petzold.Media2D;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private string[] _tok;
        private bool _variavel1;
        private bool _variavel2;
        public MainWindow()
        {
            InitializeComponent();
            Height -= 30;
            Width -= 100;
            TxtInput.Focus();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (TxtInput.Text == string.Empty)
            {
                ShowMessage("Please, inpute some text.");
                return;
            }

            listView1.Items.Clear();
            textAnalise.Clear();
            Panel.Children.Clear();

            if (rbVowels.IsChecked != null && (bool)rbVowels.IsChecked)
            {
                string texto;
                GetValue(VogalScanner(out texto), texto);
            }

            if (rbMath.IsChecked != null && (bool)rbMath.IsChecked)
            {
                //ShowMessage("Ainda não está implementado! \n\nComing soon!!");
                MathScanner();
                DrawAutomat();
            }
        }

        private void GetValue(bool incorreto, string texto)
        {
            //Se não contem apenas letras
            if (!VerificaLetras(texto))
            {
                textAnalise.Text = "Sintaticamente incorreto & Lexicamente incorreto";
            }

            //Se contem apenas letras e essas letras são todas vogais. 
            else if (VerificaLetras(texto) && VerificaVogais(texto))
            {
                textAnalise.Text = "Sintaticamente correto & Lexicamente correto";
            }

            else
                textAnalise.Text = "Sintaticamente correto & Lexicamente incorreto";
        }

        private static bool VerificaVogais(string texto)
        {
            const string vowesl = "aeiouAEIOU";
            return texto.All(c => vowesl.Contains(c));
        }

        private static bool VerificaLetras(string input)
        {
            return Regex.IsMatch(input, @"^[a-zA-Z]+$");
        }

        private bool VogalScanner(out string texto)
        {
            var incorreto = false;
            const string vowesl = "aeiouAEIOU";
            var tokens = TxtInput.Text.ToCharArray();
            texto = null;
            int flag = 50;
            foreach (var token in tokens)
            {

                texto = texto + token;
                if (vowesl.Contains(token))
                {
                    ListViewItem lvitem = new ListViewItem
                    {
                        Content = String.Format(token + " :Lexicamente correto"),
                        Foreground = Brushes.Green
                    };

                    listView1.Items.Add(lvitem);
                }
                else
                {
                    var lvitem = new ListViewItem
                    {
                        Content = String.Format(token + " :Lexicamente incorreto"),
                        Foreground = Brushes.Red
                    };
                    listView1.Items.Add(lvitem);
                    incorreto = true;
                }
            }
            if (TxtInput.Text.Contains(','))
            {
                var virgula = new ListViewItem
                {
                    Content = String.Format(" , :Lexicamente incorreto"),
                    Foreground = Brushes.Red
                };
                listView1.Items.Add(virgula);
            }
            return incorreto;
        }

        static Ellipse CreateEllipse(double width, double height, double desiredCenterX, double desiredCenterY)
        {
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            ellipse.Stroke = Brushes.Black;
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
            return ellipse;
        }


        void DrawAutomat(char info)
        {
            try
            {
                bool isValid = "aeiouAEIOU".Contains(info);
                //Estado inicial, tem uma elipse e um textblc
                Ellipse initialEllipse = CreateEllipse(width: 50, height: 50, desiredCenterX: 20, desiredCenterY: 50);
                TextBlock text = new TextBlock
                {
                    Text = "E0",
                    FontSize = 15,
                    Margin = new Thickness(initialEllipse.Margin.Left + (initialEllipse.Width / 3),
                            initialEllipse.Margin.Top + (initialEllipse.Height / 3), 0, 0)
                };

                //Linha
                ArrowLine line = new ArrowLine();
                line.Stroke = Brushes.Black;
                line.X1 = 1;
                line.X2 = 50;
                line.Margin = new Thickness(initialEllipse.Margin.Left + initialEllipse.Width, initialEllipse.Margin.Top + (initialEllipse.Height / 2), 0, 0);
                line.StrokeThickness = 2;
                TextBlock labelToLine = new TextBlock
                {
                    Text = info.ToString(CultureInfo.InvariantCulture).ToUpper(),
                    FontSize = 25,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(line.Margin.Left, line.Margin.Top, 0, 0),
                    Foreground = isValid ? Brushes.Green : Brushes.Red
                };

                //Estado Final
                Ellipse externalEllipse = CreateEllipse(width: 50, height: 50, desiredCenterX: 20 + line.X2 + 50, desiredCenterY: 50);
                Ellipse internalEllipse = CreateEllipse(width: 35, height: 35, desiredCenterX: 20 + line.X2 + 50, desiredCenterY: 50);
                externalEllipse.Stroke = internalEllipse.Stroke = isValid ? Brushes.Green : Brushes.Red;
                TextBlock textFinal = new TextBlock
                {
                    Text = "E1",
                    FontSize = 15,
                    Margin = new Thickness(internalEllipse.Margin.Left + (internalEllipse.Width / 3),
                            internalEllipse.Margin.Top + (internalEllipse.Height / 3), 0, 0)
                };

                //Adicionando componentes
                Panel.Children.Add(initialEllipse);
                Panel.Children.Add(text);
                Panel.Children.Add(internalEllipse);
                Panel.Children.Add(externalEllipse);
                Panel.Children.Add(textFinal);
                Panel.Children.Add(line);
                Panel.Children.Add(labelToLine);

                //Ajustar a espessura das elipses
                Panel.FindChildren<Ellipse>().ToList().ForEach(ellipse => ellipse.StrokeThickness = 2.2);

            }
            catch (Exception e)
            {
                ShowMessage(e.Message);
            }

        }
        /// <summary>
        /// Metodo responsavel por gerar o automato das math expressions.
        /// </summary>
        private void DrawAutomat()
        {
            for (int index = 0; index < listView1.Items.Count; index++)
            {
                ListViewItem info = (ListViewItem)listView1.Items[index];

                //Estado inicial, tem uma elipse e um textblc

                //a expressão math do 'desiredCenterX' é pura magia... 
                Ellipse initialEllipse = CreateEllipse(width: 50, height: 50, desiredCenterX: 50 + (index * 50) * 2, desiredCenterY: 50);
                TextBlock text = new TextBlock
                {
                    Text = "E" + index,
                    FontSize = 15,
                    Margin = new Thickness(initialEllipse.Margin.Left + (initialEllipse.Width / 3),
                        initialEllipse.Margin.Top + (initialEllipse.Height / 3), 0, 0)
                };

                if (index == 0)
                {
                    //Linha
                    ArrowLine line = new ArrowLine
                    {
                        Stroke = Brushes.SlateGray,
                        X1 = 1,
                        X2 = 20,
                        Margin = new Thickness(0,50, 50, 50),
                        StrokeThickness = 2
                    };
                    Panel.Children.Add(line);
                }
                if (index == listView1.Items.Count - 1)
                {
                    Ellipse finalEllipse = CreateEllipse(width: 40, height:40, desiredCenterX: 50 + (index * 50) * 2, desiredCenterY: 50);
                    TextBlock textFinal = new TextBlock
                    {
                        Text = "E" + index,
                        FontSize = 15,
                        Margin = new Thickness(initialEllipse.Margin.Left + (initialEllipse.Width / 3),
                            initialEllipse.Margin.Top + (initialEllipse.Height / 3), 0, 0)
                    };

                    Panel.Children.Add(finalEllipse);
                    Panel.Children.Add(textFinal);
                }
                else
                {
                    //Linha
                    ArrowLine line = new ArrowLine
                    {
                        Stroke = Brushes.Black,
                        X1 = 1,
                        X2 = 50,
                        Margin = new Thickness(initialEllipse.Margin.Left + initialEllipse.Width,
                            initialEllipse.Margin.Top + (initialEllipse.Height / 2), 0, 0),
                        StrokeThickness = 2
                    };

                    TextBlock labelToLine = new TextBlock
                    {
                        Text = info.Content.ToString().Split(':')[0],
                        FontSize = 25,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(line.Margin.Left, line.Margin.Top, 0, 0),
                        Foreground = info.Foreground
                    };

                    Panel.Children.Add(line);
                    Panel.Children.Add(labelToLine);
                }
                Panel.Children.Add(initialEllipse);
                Panel.Children.Add(text);
            }
        }

        private string VerificaToken(string[] tokens, int index)
        {
            if (index >= 20)
            {
                index--;
            }
            char charToken = Convert.ToChar(tokens[index]);

            if (char.IsDigit(charToken))
            {
                return "<D>";
            }
            if (char.IsLetter(charToken))
            {
                return "<L>";
            }
            if (charToken.Equals('+') || charToken.Equals('-') || charToken.Equals('/') || charToken.Equals('*'))
            {
                return "<OP>";
            }

            return "azul";
        }

        private void MathScanner()
        {

            var tokens = TxtInput.Text.Trim().ToCharArray();
            var t = TxtInput.Text.Trim();
            var teste = new string[t.Length];

            for (int i = 0; i < t.Length; i++)
            {
                teste[i] = t[i].ToString();
            }

            _tok = teste;


            var valida = true;
            var valida1 = false;

            for (var i = 0; i < tokens.Length; i++)
            {

                if (Char.IsLetterOrDigit(tokens[i]))
                {
                    Showlexicalscanner(tokens, i, true);
                }
                if (tokens[i].Equals('('))
                {
                    valida = Verificaparenteses(tokens);
                    Showlexicalscanner(tokens, i, valida);
                }
                if (tokens[i].Equals(')'))
                {
                    valida = Verificaparenteses(tokens);
                    Showlexicalscanner(tokens, i, valida);

                }
                if (tokens[i].Equals('+') || tokens[i].Equals('-') || tokens[i].Equals('/') || tokens[i].Equals('*'))
                {
                    valida1 = VerificaOperacao(tokens, i);

                    Showlexicalscanner(tokens, i, valida1);
                }

                if (tokens[i] != '=') continue;
                valida1 = VerificaOperacao(tokens, i) && VerificaIgualdade(tokens);
                Showlexicalscanner(tokens, i, valida1);
            }

            if (valida1 && valida)
            {
                textAnalise.Text = "Expressão valida";
                textAnalise.Foreground = Brushes.LimeGreen;
                _variavel1 = true;

                _variavel2 = true;
            }
            else
            {
                textAnalise.Text = "Expressão invalida";
                textAnalise.Foreground = Brushes.OrangeRed;
            }
        }
        private static bool VerificaIgualdade(IEnumerable<char> tokens)
        {
            var cont = tokens.Count(token => token.Equals('='));

            return cont == 1;
        }
        private static bool VerificaOperacao(char[] tokens, int index)
        {
            if (index == tokens.Length - 1)
            {
                return false;
            }
            if (Char.IsLetterOrDigit(tokens[index + 1]) || Char.IsLetterOrDigit(tokens[index + 1]) ||
                Char.IsPunctuation(tokens[index + 1]))
            {
                return true;
            }
            if (index == 0)
            {
                return false;
            }

            return Char.IsLetterOrDigit(tokens[index - 1]) || Char.IsLetterOrDigit(tokens[index - 1]) ||
                   Char.IsPunctuation(tokens[index - 1]);

        }
        private static bool Verificaparenteses(char[] tokens)
        {
            var continicial = 0;
            var contfinal = 0;



            for (var i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Equals('('))
                {
                    continicial++;

                }

                if (!tokens[i].Equals(')')) continue;
                contfinal++;

            }
            if (continicial != contfinal)
            {

                return false;
            }

            return true;
        }
        private void Showlexicalscanner(char[] tokens, int i, bool valida)
        {
            if (valida)
            {
                var lvitem = new ListViewItem
                {
                    Content = String.Format(tokens[i] + " :Lexicamente correto"),
                    Foreground = Brushes.Green
                };
                listView1.Items.Add(lvitem);
            }
            else
            {
                var lvitem = new ListViewItem
                {
                    Content = String.Format(tokens[i] + " :Lexicamente Incorreto"),
                    Foreground = Brushes.Red
                };
                listView1.Items.Add(lvitem);
            }
        }

        private void ShowMessage(string msgm)
        {
            this.ShowMessageAsync(Title, msgm);
        }
        private void ListView1_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rbMath.IsChecked.HasValue && rbMath.IsChecked.Value) return;
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;

            string content = ((ListViewItem)e.AddedItems[0]).Content.ToString();

            ClearPanel();
            DrawAutomat(content.Split(':')[0][0]);
        }

        private void ClearPanel()
        {
            Panel.Children.Clear();
        }

        private void BtnClear_OnClick(object sender, RoutedEventArgs e)
        {
            TxtInput.Clear();
            textAnalise.Clear();
            ClearPanel();
            listView1.Items.Clear();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItemOpen_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            bool? result = dialog.ShowDialog();
            string content = string.Empty;
            if ((result.HasValue) && (result.Value))
            {
                content = File.ReadAllText(dialog.FileName);
            }
            TxtInput.Text = content;
            ButtonBase_OnClick(null, null);
        }

        private void MenuItemSave_OnClick(object sender, RoutedEventArgs e)
        {
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.InitialDirectory = mydocpath;
            bool? result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value) return;
            // Get file name.
            string name = dialog.FileName;
            // Write to the file name selected.
            File.WriteAllText(name, TxtInput.Text);

        }

        private void MenuItemAbout_OnClick(object sender, RoutedEventArgs e)
        {
            ShowMessage("Developed by: \nLuís Fernando Moraes \nand \nMarina Silva.");
        }

        private void RbDark_OnChecked(object sender, RoutedEventArgs e)
        {
            var accent = ThemeManager.Accents.First(x => x.Name == "Blue");
            if (rbDark != null && rbDark.IsChecked.HasValue && rbDark.IsChecked.Value)
            {
                //AppTheme theme = new AppTheme("Dark", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/basedark.xaml"));
#pragma warning disable 618
                ThemeManager.ChangeTheme(Application.Current, accent, Theme.Dark);
#pragma warning restore 618
            }
            else if (rbLight != null && rbLight.IsChecked.HasValue && rbLight.IsChecked.Value)
            {
                //AppTheme theme = new AppTheme("Light", new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/baselight.xaml"));
                //ThemeManager.ChangeAppStyle(Application.Current, accent, theme);
#pragma warning disable 618
                ThemeManager.ChangeTheme(Application.Current, accent, Theme.Light);
#pragma warning restore 618
            }

        }
    }
}
