[DataContract]
public class Point
{
    [DataMember]
    public double X { get; set; }
    [DataMember]
    public double Y { get; set; }
}
[DataContract]
public class Parcel
{
    [DataMember]
    public List<Point> Polygon { get; set; }
    [DataMember]
    public string OwnerName { get; set; }
}
[ServiceContract]
public interface IParcelService
{
    [OperationContract]
    void RegisterParcel(List<Point> polygon, string ownerName);
    [OperationContract]
    List<Parcel> GetParcelByOwner(string ownerName);
    [OperationContract]
    List<Parcel> GetParcelByMinArea(double minArea);
    [OperationContract]
    List<Parcel> GetAllParcels();
}

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
public class ParcelService : IParcelService
{
    private List<Parcel> parcels = new List<Parcel>();
    void RegisterParcel(List<Point> polygon, string ownerName)
    {
        Parcel newParcel = new Parcel { Polygon = polygon, OwnerName = ownerName };
        parcels.Add(newParcel);
    }

    List<Parcel> GetParcelsByOwner(string ownerName)
    {
        return parcels.FindAll(parcel => parcel.OwnerName == ownerName);
    }
    List<Parcel> GetParcelByMinArea(double minArea)
    {
        return parcels.FindAll(parcel => CalculateArea(parcel.polygon) > minArea);
    }
    List<Parcel> GetAllParcels()
    {
        return parcels;
    }
    
}
public double CalculateArea(List<Point> polygon)
    {
        int numVertices = polygon.Count;
        double area = 0;

        for (int i = 0; i < numVertices; i++)
        {
            int j = (i + 1) % numVertices;
            area += (polygon[j].X + polygon[i].X) * (polygon[j].Y - polygon[i].Y);
        }

        return Math.Abs(area) / 2;
    }

webconfig
<services>
    < service type = "ParcelService"
        < endpoint
            contract = "IParcelService"
            binding = "basicHttpBinding"
        />
    />
</ services >


public static void main(){
    var client=ParcelService();
    var polygon1=new List<Point>{
        new Point{ X=1,Y=2},
        new Point{ X=2,Y=3},
        new Point{ X=5,Y=4},
        new Point{ X=6,Y=2},
        new Point{ X=6,Y=4},
    };
    var polygon2=new List<Point>{
        new Point{ X=5,Y=4},
        new Point{ X=6,Y=2},
        new Point{ X=6,Y=4},
    };
    string ownerName1="Jovo"
    client.RegisterParcel(polygon1,ownerName1);
    client.RegisterParcel(polygon2,ownerName1);

    var polygon3=new List<Point>{
        new Point{ X=1,Y=2},
        new Point{ X=2,Y=3},
        new Point{ X=5,Y=4},
    };
    string ownerName2="Djuro"
    client.RegisterParcel(polygon3,ownerName2);

    var parcels=client.GetParcelsByOwner(ownerName1);

    foreach(var parcel in parcels){
        Console.WriteLine($"Owner: {parcel.ownerName}");
    }

    double minArea=50;
    var minParcels=client.GetParcelByMinArea(minArea);
    foreach(var parcel in parcels){
        Console.WriteLine($"Area:{CalculateArea(parcel.Polygon)}")
    }

    client.Close();
}