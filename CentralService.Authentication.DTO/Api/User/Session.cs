namespace CentralService.Authentication.DTO.Api.User
{
    public struct Session
    {
        public int SessionKey { get; set; }
        public int DeviceProfileId { get; set; }
        public int GameProfileId { get; set; }
        public string UniqueNickname { get; set; }
        public SessionStatus CurrentStatus { get; set; }

        public Session(int SessionKey, int DeviceProfileId, int GameProfileId, string UniqueNickname, SessionStatus CurrentStatus)
        {
            this.SessionKey = SessionKey;
            this.DeviceProfileId = DeviceProfileId;
            this.GameProfileId = GameProfileId;
            this.UniqueNickname = UniqueNickname;
            this.CurrentStatus = CurrentStatus;
        }
    }
}
