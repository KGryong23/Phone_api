using System.ComponentModel;

namespace Phone_api.Enums
{
    public enum ModerationStatus
    {
        [Description("Đã duyệt")]
        Approved = 0,
        [Description("Từ chối")]
        Rejected = 1,
    }
}
