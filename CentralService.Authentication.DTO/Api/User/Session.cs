namespace CentralService.Authentication.DTO.Api.User
{
    public struct Session
    {
        public int SessionKey { get; }
        public int DeviceProfileId { get; }
        public int GameProfileId { get; }
        public string UniqueNickname { get; }
        public SessionStatus CurrentStatus { get; private set; }

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
