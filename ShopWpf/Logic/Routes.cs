using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopWpf
{
    public static class Routes
    {
        public static string GetRequest = "GetAll";
        public static string GetByIDRequest = "Get";

        public static string DeleteRequest = "Delete";

        public static string PostRequest = "Post";

        // Developer
        public static string PutLogoRequest = "PutLogo";
        public static string PutNameRequest = "PutName";

        // Game;
        public static string PutAchievementsCountRequest = "PutAchievementsCount";
        public static string PutPriceRequest = "PutPrice";

        // Game stats
        public static string PutGottenAchievementsRequest = "PutGottenAchievements";
        public static string PutHoursPlayedRequest = "PutHoursPlayed";
        public static string PutGameLaunchDateRequest = "PutGameLaunchDate";

        // Review
        public static string PutTextRequest = "PutText";
        public static string PutApprovalRequest = "PutApproval";

        // User
        public static string PutLoginRequest = "PutLogin";
        public static string PutPasswordRequest = "PutPassword";
        public static string PutNicknameRequest = "PutNickname";
        public static string PutEmailRequest = "PutEmail";
        public static string PutGameStatsRequest = "PutGameStats";
        public static string PutMoneyOnAccountRequest = "PutMoneyOnAccount";
        public static string PutAvatarRequest = "PutAvatar";
    }
}
