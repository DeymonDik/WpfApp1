using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using ClientApplication.Models.Dtos;
using ClientApplication.Models.Enums;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private readonly HttpClient client = new();
        private readonly string baseUrl = "http://localhost:12345/api/people";

        private async Task<string?> SentHttpRequest(string url, MethodEnum method = MethodEnum.GET, string? jsonMsg = null)
        {
            try
            {
                HttpResponseMessage response;
                switch (method)
                {
                    case MethodEnum.GET: { response = await client.GetAsync(url); break; }
                    case MethodEnum.POST:
                        {
                            if (jsonMsg == null) throw new NotImplementedException("msg is null");
                            response = await client.PostAsync(url, new StringContent(jsonMsg, Encoding.UTF8, "application/json"));
                            break;
                        }
                    case MethodEnum.PUT:
                        {
                            if (jsonMsg == null) throw new NotImplementedException("msg is null");
                            response = await client.PutAsync(url, new StringContent(jsonMsg, Encoding.UTF8, "application/json"));
                            break;
                        }
                    case MethodEnum.DELETE: { response = await client.DeleteAsync(url); break; }
                    default: throw new NotImplementedException("method is incorrect");
                }
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    MessageBox.Show("Ошибка (mistake) kogda (when) при получении данных");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        private async void Get_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
            Button1.Content = "Добавить";
            string? response = await SentHttpRequest(baseUrl);
            if (response != null)
            {
                List<PersonDto?>? persons = JsonSerializer.Deserialize<List<PersonDto?>>(response);
                if (persons != null)
                {
                    persons.ForEach(o => listBox1.Items.Add(o));
                }
            }
        }
        private async void Post_Click(object sender, RoutedEventArgs e)
        {
            if (Button1.Content.ToString() == "Сохранить")
            {
                PersonDto? person = listBox1.Items.GetItemAt(listBox1.Items.Count - 1) as PersonDto;
                if (person == null) { }
                string request = JsonSerializer.Serialize(person);
                string? response = await SentHttpRequest(baseUrl, MethodEnum.POST, request);
                //if (response != null) MessageBox.Show(response, "Запрос выполнен");
                Button1.Content = "Добавить";
            }
            else
            {
                if (int.TryParse(textBox1.Text, out int id)) { };
                listBox1.Items.Add(new PersonDto() { Id = id, Name = textBox2.Text });
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
                Button1.Content = "Сохранить";
            }
        }
        private async void Put_Click(object sender, RoutedEventArgs e)
        {
            PersonDto? person = (listBox1.SelectedItem as PersonDto);
            if (person == null) { return; }
            string request = JsonSerializer.Serialize(person);
            string? response = await SentHttpRequest($"{baseUrl}/{person.Id}", MethodEnum.PUT, request);
            //if (response != null) MessageBox.Show(response, "Запрос выполнен");
        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            PersonDto? person = (listBox1.SelectedItem as PersonDto);
            if (person == null) { return; }

            string? response = await SentHttpRequest($"{baseUrl}/{person.Id}", MethodEnum.DELETE);
            //if (response != null) MessageBox.Show(response, "Запрос выполнен");
        }

        private void listBox1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            textBox1.Text = (listBox1.SelectedItem as PersonDto)?.Id.ToString();
            textBox2.Text = (listBox1.SelectedItem as PersonDto)?.Name;
        }

        private void textBox1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            PersonDto? person = listBox1.SelectedItem as PersonDto;
            if (person != null)
            {
                int id = listBox1.Items.IndexOf(person.Id);
                if (int.TryParse(textBox1.Text, out id))
                {
                    person.Id = id;
                }
                listBox1.Items.Refresh();
            }
        }

        private void textBox2_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (listBox1.SelectedItem is PersonDto person)
            {
                person.Name = textBox2.Text;
                listBox1.Items.Refresh();
            }
        }
    }
}