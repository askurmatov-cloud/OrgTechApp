namespace OrgTechApp
{
    public class LookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public override string ToString() { return Name; }
    }

    public class StatusItem
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public override string ToString() { return Name; }
    }

    public class EquipmentLookup
    {
        public int Id { get; set; }
        public string Display { get; set; } = string.Empty;
        public override string ToString() { return Display; }
    }
}
