[DataContract]
public class Sastojak
{
    [DataMember]
    public string Naziv { get; set; }
    [DataMember]
    public decimal Zapremina { get; set; }
    [DataMember]
    public decimal Gustina { get; set; }
}
[DataContract]
public class Stanje
{
    [DataMember]
    public decimal Zapremina { get; set; }
    [DataMember]
    public decimal Gustina { get; set; }
}
[ServiceContract]
public interface IFabrikaService
{
    [OperationContract]
    void DodajSastojak(Sastojak sastojak);
    [OperationContract]
    void FlasirajZapreminu(decimal zapremina);
    [OperationContract]
    Stanje PrikaziStanje();
}
[ServiceContract(InstanceContextMode = InstanceContextMode.PerSession)]
public class FabrikaService : IFabrikaService
{
    public List<Sastojak> Sastojci = new List<Sastojak>();
    public decimal Zapremina;
    public decimal Gustina;

    void DodajSastojak(Sastojak sastojak)
    {
        Zapremina += sastojak.Zapremina;
        Sastojci.Add(sastojak);
        Console.WriteLine($"Dodao sam:{sastojak.ime}!");
    }
    void FlasirajZapreminu(decimal zapremina)
    {
        if (zapremina >= Zapremina)
        {
            Console.WriteLine("Nema dovoljno!");
        }
        else
        {
            decimal ukupnaMasa=Sastojci.Sum(sbyte=>s.Zapremina*s.Gustina);
            decimal novaZapremina=Zapremina-zapremina;
            if(novaZapremina<=0)
            {
                Zapremina=0;
                Gustina=0;
                Sastojci.Clear();
                Console.WriteLine("Mikser je prazan!");
            }
            else
            {
                Gustina=ukupnaMasa/novaZapremina;
                Zapremina=novaZapremina;
                
                foreach(var sastojak in Sastojci)
                {
                    if (zapremina>=sastojak.Zapremina)
                    {
                        zapremina-=sastojak.Zapremina;
                        Sastojci.Remove(sastojak);
                    }
                    else
                    {
                        sastojak.Zapremina-=zapremina;
                        break;
                    }
                }
                Console.WriteLine($"Flasirano: {zapremina}L");
            }
        }
    }
    public string PrikaziStanje()
    {
        return $"Trenutno stanje - Zapremina: {Zapremina} L, Gustina: {Gustina}";
    }
}
<services>
    <service name="FabrikaService">
        <endpoint binding="basicHttpBinding" contract="IFabrikaService"/>
    </service>
</services>

static void Main(string[] args)
{
    var proxy = FabrikaService();
    proxy.DodajSastojak(new Sastojak
    {
        Naziv="Cokolada",
        Zapremina=54.7,
        Gustina=172
    });
    proxy.DodajSastojak(new Sastojak
    {
        Naziv="Krompir",
        Zapremina=5,
        Gustina=28
    });
    proxy.DodajSastojak(new Sastojak
    {
        Naziv="Teleca Glava",
        Zapremina=56.4,
        Gustina=24
    });
    var stanje=proxy.PrikaziStanje();
    Console.WriteLine(PrikaziTrenutnoStanje(return $"Trenutno stanje - Zapremina: {stanje.Zapremina} L, Gustina: {stanje.Gustina}"));

    proxy.FlasirajZapreminu(56.4);

    var novoStanje=proxy.PrikaziStanje();
    Console.WriteLine(PrikaziTrenutnoStanje(return $"Trenutno stanje - Zapremina: {novoStanje.Zapremina} L, Gustina: {novoStanje.Gustina}"));
}