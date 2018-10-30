using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace ReplicaSet.Status
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void OnConnect(object sender, RoutedEventArgs e)
        {
            var conn = connectionString.Text;
            IMongoClient mongoClient = null;
            try
            {
                mongoClient = new MongoClient(conn);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                var messageQueue = ErrorMessage.MessageQueue;
                Task.Factory.StartNew(() => messageQueue.Enqueue("Error connecting"));
            }

            var adminDatabase = mongoClient.GetDatabase("admin");
            try
            {
                var status = adminDatabase.RunCommand<BsonDocument>("{replSetGetStatus: 1}");
                var result = BsonSerializer.Deserialize<ReplicaSetStatus>(status);
                MembersStatus.ItemsSource = result.Members;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                var messageQueue = ErrorMessage.MessageQueue;
                Task.Factory.StartNew(() => messageQueue.Enqueue("Error getting replica set status"));
            }
        }
    }

    [BsonIgnoreExtraElements]
    public class ReplicaSetStatus
    {
        [BsonElement("members")]
        public IEnumerable<Member> Members { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Member
    {
        public int Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("stateStr")]
        public string StateStr { get; set; }
    }
}
