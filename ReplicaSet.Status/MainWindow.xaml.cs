using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ReplicaSet.Status
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IMongoClient _mongoClient;
        private ObservableCollection<Counter> _counters;

        public MainWindow()
        {
            InitializeComponent();
            _counters = new ObservableCollection<Counter>();
        }

        private async void OnClear(object sender, RoutedEventArgs e)
        {
            try
            {
                await _mongoClient.DropDatabaseAsync("testrs");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error dropping database");
            }
        }

        private async void OnGetData(object sender, RoutedEventArgs e)
        {
            try
            {
                var testDatabase = _mongoClient.GetDatabase("testrs");
                var collection = testDatabase.GetCollection<Counter>("counter");
                DataSaved.ItemsSource = await collection.AsQueryable().ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error inserting data");
            }
        }

        private void OnInsertData(object sender, RoutedEventArgs e)
        {
            try
            {
                var testDatabase = _mongoClient.GetDatabase("testrs");
                var collection = testDatabase.GetCollection<Counter>("counter");
                var maxCounter = 0;
                if (DataSent.ItemsSource != null)
                {
                    maxCounter = DataSent.ItemsSource.OfType<Counter>().Max(c => c.Count);
                }
                var newCounter = new Counter
                {
                    Count = maxCounter + 1,
                    DateTime = DateTime.Now
                };
                collection.InsertOneAsync(newCounter);
                _counters.Add(newCounter);
                DataSent.ItemsSource = _counters;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error inserting data");
            }
        }

        private void OnConnect(object sender, RoutedEventArgs e)
        {
            var conn = connectionString.Text;
            try
            {
                _mongoClient = new MongoClient(conn);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error connecting");
            }

            var adminDatabase = _mongoClient.GetDatabase("admin");
            try
            {
                var status = adminDatabase.RunCommand<BsonDocument>("{replSetGetStatus: 1}");
                var result = BsonSerializer.Deserialize<ReplicaSetStatus>(status);
                MembersStatus.ItemsSource = result.Members;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Error getting replica set status");
            }
        }
    }
}
