namespace CentralService.Authentication.DTO.Api.User
{
    public struct SessionStatus
    {
        public int Status { get; set; }
        public string StatusString { get; set; }
        public string LocationString { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public int Qm { get; set; }

        public override string ToString() => $"|s|{ Status }|ss|{ StatusString }|ls|{ LocationString }|ip|{ Address }|p|{ Port }|qm|{ Qm }";
    }
}
