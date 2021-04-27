﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace DCO_Player
{
    /// <summary>
    /// Логика взаимодействия для Log_In.xaml
    /// </summary>
    public partial class Log_In : Page
    {
        string connectionString;

        int id { get; set; }
        string name { get; set; }
        string surname { get; set; }
        string login { get; set; }
        string password { get; set; }
        string createDate { get; set; }
        string imageSrc { get; set; }
        int cash { get; set; }

        Regex RLogin = new Regex(@"(\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,6})");
        Regex RPassword = new Regex(@"(?=.*[0-9])(?=.*[!@#$%^&*])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*]{9,}");

        bool BLogin = false;
        bool BPassword = false;

        public Log_In()
        {
            InitializeComponent();
        }

        private void Log_In_Click(object sender, RoutedEventArgs e)
        {

            try {
                if (RLogin.IsMatch(Login.Text))
                {
                    BLogin = true;
                    login = Login.Text;
                }
                else
                {
                    MessageBox.Show("Поле не должно быть пустым и должно содержать имя почты");
                }

                if (RPassword.IsMatch(CPassword.Password))
                {
                    connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    string sqlExpression = "SELECT Password FROM Users WHERE Login = '" + login + "'";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows) // если есть данные
                        {
                            reader.Read();
                            password = reader.GetValue(0).ToString();
                            reader.Close();
                        }
                    }

                    if (CPassword.Password == password)
                    {
                        BPassword = true;
                    }
                    else
                    {
                        MessageBox.Show("Не верный пароль");
                        password = null;
                    }
                }
                else
                {
                    MessageBox.Show("Поле не должно быть пустым и должно содержать не менее 9 символов включая спецсимволы, буквы латинского алфавита, числа" +
                        "");
                }

                if (BLogin && BPassword)
                {
                    connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    string sqlExpression = "SELECT * FROM Users WHERE Password = '" + password + "' and Login = '" + login + "'";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sqlExpression, connection);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows) // если есть данные
                        {
                            reader.Read();
                            id = (int)reader.GetValue(0);
                            name = reader.GetValue(1).ToString();
                            surname = reader.GetValue(2).ToString();
                            createDate = reader.GetValue(5).ToString();
                            imageSrc = reader.GetValue(6).ToString();
                            cash = (int)reader.GetValue(7);
                            reader.Close();
                        }
                    }

                    Profile.Id_users = id;
                    Profile.name = name;
                    Profile.surname = surname;
                    Profile.createDate = createDate;
                    Profile.imageSrc = imageSrc;
                    Profile.cash = cash;

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Start.Instance.Close();
                }
            }
            catch
            {
                MessageBox.Show("Отсутствует подключение к базе данных,\n проверьте соединение на сервере");

            }
            
        }
    }
}
