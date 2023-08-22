[DataContract]
public class Message
{
    [DataMember]
    public string Sender { get; set; }
    [DataMember]
    public string Receiver { get; set; }
    [DataMember]
    public DateTime Timestamp { get; set; }
    [DataMember]
    public string Content { get; set; }
    [DataMember]
    public bool IsBroadcast { get; set; }
}
[ServiceContract]
public interface IChatService
{
    [OperationContract]
    void RegisterUser(string username);
    [OperationContract]
    void SendMessage(string senderUsername, string receiverUsername, string message,);
    [OperationContract]
    void SendBroadcastMessage(string senderUsername,string message);
    [OperationContract]
    List<Message> GetMessageHistory(DateTime from, DateTime to)
}
[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
public class ChatService : IChatService
{
    private static Dictionary<string, DateTime> activeUsers = new Dictionary<string, DateTime>();
    private static Live<Message> messageHistory = new List<Message>();

    void RegisterUser(string username)
    {
        if (activeUsers.ContainsKey(username))
        {
            activeUsers[username] = DateTime.Now;
        }
        else
        {
            activeUsers.Add(username, DateTime.Now);
        }
    }
    public void SendMessage(string senderUsername,string receiverUsername,string message){
        var msg=new Message{
            Sender=senderUsername,
            Receiver=receiverUsername,
            Timestamp=DateTime.Now,
            Content=message,
            IsBroadcast=false
        }
        if(receiverUsername=="SVI"){
            SendBroadCastMessage(senderUsername,message)
            return;
        }
        if(activeUsers.ContainsKey(receiverUsername)){
            messageHistory.Add(msg)
        }
    }
    private void SendBroadCastMessage(string senderUsername, string message)
    {
        var msg=new Message{
            Sender=senderUsername,
            Receiver="SVI",
            Timestamp=DateTime.Now,
            Content=message,
            IsBroadcast=true
        }
        messageHistory.Add(msg);
    }
    List<Message> GetMessageHistory(DateTime from, DateTime to){
        return messageHistory.FindAll(msg=>msg.Timestamp>=from && msg.Timestamp<=to);
    }
}
<services>
    <service name="ChatService">
    <endpoint binding="basicHttpBinding" contract="IChatService"/>
    </service>
</services>

static void Main(string[] args)
{
    ChatService client=new ChatService();

    string veljko="Veljko";
    client.RegisterUser(veljko);

    string seljko="Seljko";
    client.RegisterUser(seljko);

    string djura="Djura";
    client.RegisterUser(djura);

    string majmun="Majmun";
    client.RegisterUser(majmun);


    client.SendMessage(veljko,majmun,"Gde si");
    client.SendMessage(seljke,djura,"Pozdrav");

    client.SendMessage(majmun,"SVI","alooooooooooooo");
    client.SendBroadcastMessage(majmun,"alooooooooooooo");

    var messages=client.GetMessageHistory(DateTime.Now,DateTime.Now);

    foreach(var message in messages)
    {
        Console.WriteLine($"Poruka:{message.Content}");
    }

    client.Close();
}